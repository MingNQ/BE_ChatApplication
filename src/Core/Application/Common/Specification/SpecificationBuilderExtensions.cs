using Application.Common.Exceptions;
using Application.Common.Models;
using Ardalis.Specification;
using Domain.Extensions;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;

namespace Application.Common.Specification;

[ExcludeFromCodeCoverage]
public static class SpecificationBuilderExtensions
{
    public static ISpecificationBuilder<T> SearchBy<T, TSearch, TFilter>(
        this ISpecificationBuilder<T> query,
        IAdvancedFilter<TSearch, TFilter> filter)
        where TSearch : ISearch
        where TFilter : IFilter<TFilter>
    {
        return query
            .AdvancedSearch(filter.AdvancedSearch)
            .AdvancedFilter(filter.AdvancedFilter);
    }

    public static ISpecificationBuilder<T> PaginateBy<T>(this ISpecificationBuilder<T> query, IPagination filter)
    {
        return query
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize);
    }

    public static IOrderedSpecificationBuilder<T> OrderBy<T>(this ISpecificationBuilder<T> query, IOrderBy filter)
    {
        if (filter is ICustomOrderBy<T> customOrderBy)
        {
            return query.OrderBy(filter.OrderBy, customOrderBy.CustomOrderBy);
        }

        return query.OrderBy(filter.OrderBy);
    }

    public static IOrderedSpecificationBuilder<T> OrderBy<T>(
        this ISpecificationBuilder<T> query,
        IOrderBy filter,
        IEnumerable<string> orderByFields)
    {
        string[] orderByFieldsArr = (filter.OrderBy ?? []).Concat(orderByFields).ToArray();
        if (filter is ICustomOrderBy<T> customOrderBy)
        {
            return query.OrderBy(orderByFieldsArr, customOrderBy.CustomOrderBy);
        }

        return query.OrderBy(orderByFieldsArr);
    }

    private static IOrderedSpecificationBuilder<T> AdvancedSearch<T>(
        this ISpecificationBuilder<T> specificationBuilder,
        ISearch? search)
    {
        if (string.IsNullOrEmpty(search?.Keyword))
        {
            return (IOrderedSpecificationBuilder<T>)specificationBuilder;
        }

        if (search.Fields.Any())
        {
            // Escape SQL wildcards
            search.Keyword = search.Keyword
                .Replace("[]", "[[]]")
                .Replace("_", "[_]")
                .Replace("%", "[%]");

            // search selected fields (can contain deeper nested fields)
            foreach (string field in search.Fields)
            {
                var paramExpr = Expression.Parameter(typeof(T));

                var propertyExpr = GetPropertyExpression(field, paramExpr);

                if ((Nullable.GetUnderlyingType(propertyExpr.Type) ?? propertyExpr.Type) is { IsEnum: true })
                {
                    specificationBuilder.AddSearchEnumPropertyByKeyword(propertyExpr, paramExpr, search.Keyword);
                }
                else
                {
                    specificationBuilder.AddSearchPropertyByKeyword(propertyExpr, paramExpr, search.Keyword);
                }
            }
        }
        else
        {
            // search all fields (only first level)
            foreach (var property in typeof(T).GetProperties()
                         .Where(prop => (Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType) is
                         { IsEnum: false } propertyType
                                        && Type.GetTypeCode(propertyType) != TypeCode.Object))
            {
                var paramExpr = Expression.Parameter(typeof(T));
                var propertyExpr = Expression.Property(paramExpr, property);

                specificationBuilder.AddSearchPropertyByKeyword(propertyExpr, paramExpr, search.Keyword);
            }
        }

        return (IOrderedSpecificationBuilder<T>)specificationBuilder;
    }

    private static void AddSearchPropertyByKeyword<T>(
        this ISpecificationBuilder<T> specificationBuilder,
        Expression propertyExpr,
        ParameterExpression paramExpr,
        string keyword,
        string operatorSearch = FilterOperator.Contains)
    {
        if (propertyExpr is not MemberExpression { Member: PropertyInfo property })
        {
            throw new ArgumentException("propertyExpr must be a property expression.", nameof(propertyExpr));
        }

        var toLower = typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes)!;

        Expression left = Expression.Call(propertyExpr, toLower);

        Expression right = Expression.Constant(keyword.ToLower());

        Expression body = operatorSearch switch
        {
            FilterOperator.Contains =>
                Expression.Call(left, nameof(string.Contains), null, right),

            FilterOperator.StartsWith =>
                Expression.Call(left, nameof(string.StartsWith), null, right),

            FilterOperator.EndsWith =>
                Expression.Call(left, nameof(string.EndsWith), null, right),

            _ => throw new CustomException("operatorSearch is not valid.")
        };

        var lambda = Expression.Lambda<Func<T, bool>>(body, paramExpr);

        specificationBuilder.Where(lambda);
    }

    private static void AddSearchEnumPropertyByKeyword<T>(
        this ISpecificationBuilder<T> specificationBuilder,
        Expression propertyExpr,
        ParameterExpression paramExpr,
        string keyword)
    {
        if (propertyExpr is not MemberExpression { Member: PropertyInfo })
        {
            throw new ArgumentException("propertyExpr must be a property expression.", nameof(propertyExpr));
        }

        var enumType = Nullable.GetUnderlyingType(propertyExpr.Type) ?? propertyExpr.Type;
        var matchingEnumValues = Enum.GetValues(enumType)
            .Cast<Enum>()
            .Where(e =>
                e.GetDescription()
                 .Contains(keyword, StringComparison.CurrentCultureIgnoreCase))
            .Select(e => Convert.ToInt32(e))
            .ToList();

        if (matchingEnumValues.Count == 0)
        {
            return;
        }

        // x => matchingEnumValues.Contains((int)x.Property)
        var convertedProperty = Expression.Convert(propertyExpr, typeof(int));

        var containsMethod = typeof(List<int>)
            .GetMethod(nameof(List<int>.Contains), new[] { typeof(int) })!;

        var valuesExpression = Expression.Constant(matchingEnumValues);

        var body = Expression.Call(valuesExpression, containsMethod, convertedProperty);

        var lambda = Expression.Lambda<Func<T, bool>>(body, paramExpr);

        specificationBuilder.Where(lambda);
    }

    private static IOrderedSpecificationBuilder<T> AdvancedFilter<T, TFilter>(
        this ISpecificationBuilder<T> specificationBuilder,
        IFilter<TFilter>? filter)
        where TFilter : IFilter<TFilter>
    {
        if (filter is null)
        {
            return (IOrderedSpecificationBuilder<T>)specificationBuilder;
        }

        var parameter = Expression.Parameter(typeof(T));

        Expression binaryExpressionFilter;

        if (!string.IsNullOrEmpty(filter.Logic))
        {
            if (filter.Filters is null)
            {
                throw new CustomException("The Filters attribute is required when declaring a logic");
            }

            binaryExpressionFilter = CreateFilterExpression(filter.Logic, filter.Filters, parameter);
        }
        else
        {
            var filterValid = GetValidFilter(filter);
            binaryExpressionFilter =
                CreateFilterExpression(filterValid.Field!, filterValid.Operator!, filterValid.Value, parameter);
        }

        var lambda = Expression.Lambda<Func<T, bool>>(
            binaryExpressionFilter, parameter);

        specificationBuilder.Where(lambda);

        return (IOrderedSpecificationBuilder<T>)specificationBuilder;
    }

    private static Expression CreateFilterExpression<TFilter>(
        string logic,
        IEnumerable<TFilter> filters,
        ParameterExpression parameter)
        where TFilter : IFilter<TFilter>
    {
        Expression? filterExpression = default;

        foreach (var filter in filters)
        {
            Expression bExpressionFilter;

            if (!string.IsNullOrEmpty(filter.Logic))
            {
                if (filter.Filters is null)
                {
                    throw new CustomException("The Filters attribute is required when declaring a logic");
                }

                bExpressionFilter = CreateFilterExpression(filter.Logic, filter.Filters, parameter);
            }
            else
            {
                var filterValid = GetValidFilter(filter);
                bExpressionFilter = CreateFilterExpression(filterValid.Field!, filterValid.Operator!, filterValid.Value,
                    parameter);
            }

            filterExpression = filterExpression is null
                ? bExpressionFilter
                : CombineFilter(logic, filterExpression, bExpressionFilter);
        }

        return filterExpression!;
    }

    private static Expression CreateFilterExpression(
        string field,
        string filterOperator,
        object? value,
        ParameterExpression parameter)
    {
        var propertyExpression = GetPropertyExpression(field, parameter);
        var valueExpression = GetValueExpression(field, value, propertyExpression);
        return CreateFilterExpression(propertyExpression, valueExpression, filterOperator);
    }

    private static Expression CreateFilterExpression(
        Expression memberExpression,
        Expression constantExpression,
        string filterOperator)
    {
        if (memberExpression.Type == typeof(string))
        {
            constantExpression = Expression.Call(constantExpression, "ToLower", null);
            memberExpression = Expression.Call(memberExpression, "ToLower", null);
        }

        return filterOperator switch
        {
            FilterOperator.Eq => Expression.Equal(memberExpression, constantExpression),
            FilterOperator.Neq => Expression.NotEqual(memberExpression, constantExpression),
            FilterOperator.Lt => Expression.LessThan(memberExpression, constantExpression),
            FilterOperator.Lte => Expression.LessThanOrEqual(memberExpression, constantExpression),
            FilterOperator.Gt => Expression.GreaterThan(memberExpression, constantExpression),
            FilterOperator.Gte => Expression.GreaterThanOrEqual(memberExpression, constantExpression),
            FilterOperator.Contains => Expression.Call(memberExpression, "Contains", null, constantExpression),
            FilterOperator.StartsWith => Expression.Call(memberExpression, "StartsWith", null, constantExpression),
            FilterOperator.EndsWith => Expression.Call(memberExpression, "EndsWith", null, constantExpression),
            FilterOperator.NotContains =>
                CreateFilterExpressionForDoesNotContain(memberExpression, constantExpression),
            _ => throw new CustomException("Filter Operator is not valid.")
        };
    }

    private static Expression CreateFilterExpressionForDoesNotContain(
        Expression memberExpression,
        Expression constantExpression)
    {
        var notContainsMethod = typeof(StringExtensions).GetMethod("NotContains", [typeof(string), typeof(string)])!;

        // static method doesn't need an instance => first parameter is null
        return Expression.Call(null, notContainsMethod, memberExpression, constantExpression);
    }

    private static Expression CombineFilter(
        string filterOperator,
        Expression bExpressionBase,
        Expression bExpression) => filterOperator switch
        {
            FilterLogic.And => Expression.And(bExpressionBase, bExpression),
            FilterLogic.Or => Expression.Or(bExpressionBase, bExpression),
            FilterLogic.Xor => Expression.ExclusiveOr(bExpressionBase, bExpression),
            _ => throw new ArgumentException("FilterLogic is not valid.")
        };

    private static MemberExpression GetPropertyExpression(
        string propertyName,
        ParameterExpression parameter)
    {
        Expression propertyExpression = parameter;
        foreach (string member in propertyName.Split('.'))
        {
            // check if property is collection or not
            var propertyType = Nullable.GetUnderlyingType(propertyExpression.Type) ?? propertyExpression.Type;
            bool isCollection = propertyType.GetInterfaces().Any(x =>
                x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>));

            if (isCollection)
            {
                var elementType = propertyType.GetGenericArguments()[0];
                var elementTypeExpression = Expression.Parameter(elementType);
                Expression propertyExpression1 = elementTypeExpression;
                propertyExpression = Expression.PropertyOrField(propertyExpression1, member);
            }

            propertyExpression = Expression.PropertyOrField(propertyExpression, member);
        }

        return (MemberExpression)propertyExpression;
    }

    private static string GetStringFromJsonElement(object value)
        => ((JsonElement)value).GetString()!;

    private static bool GetBooleanFromJsonElement(object value)
        => ((JsonElement)value).GetBoolean()!;

    private static int GetIntFromJsonElement(object value)
        => ((JsonElement)value).GetInt32();

    private static short GetShortFromJsonElement(object value)
        => ((JsonElement)value).GetInt16();

    private static long GetLongFromJsonElement(object value)
        => ((JsonElement)value).GetInt64();

    private static ConstantExpression GetValueExpression(string field, object? value,
        MemberExpression propertyExpression)
    {
        var propertyType = propertyExpression.Type;
        var propertyUnderlyingType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;
        if (value == null)
        {
            return Expression.Constant(null, propertyType);
        }

        if (propertyUnderlyingType.IsEnum)
        {
            int enumValue = GetIntFromJsonElement(value);

            if (!Enum.IsDefined(propertyUnderlyingType, enumValue))
            {
                throw new CustomException($"Value {value} is not valid for {field}");
            }

            object valueParsed = Enum.ToObject(propertyType, enumValue);
            return Expression.Constant(valueParsed, propertyType);
        }

        if (propertyUnderlyingType == typeof(Guid))
        {
            string stringGuid = GetStringFromJsonElement(value);

            if (!Guid.TryParse(stringGuid, out var valueParsed))
            {
                throw new CustomException($"Value {value} is not valid for {field}");
            }

            return Expression.Constant(valueParsed, propertyType);
        }

        if (propertyUnderlyingType == typeof(bool))
        {
            bool valueParsed = GetBooleanFromJsonElement(value);
            return Expression.Constant(valueParsed, propertyType);
        }

        if (propertyUnderlyingType == typeof(int))
        {
            int valueParsed = GetIntFromJsonElement(value);
            return Expression.Constant(valueParsed, propertyType);
        }

        if (propertyUnderlyingType == typeof(short))
        {
            short valueParsed = GetShortFromJsonElement(value);
            return Expression.Constant(valueParsed, propertyType);
        }

        if (propertyUnderlyingType == typeof(long))
        {
            long valueParsed = GetLongFromJsonElement(value);
            return Expression.Constant(valueParsed, propertyType);
        }

        if (propertyUnderlyingType != typeof(DateTimeOffset) && propertyUnderlyingType != typeof(DateTimeOffset?))
        {
            return Expression.Constant(ChangeType(((JsonElement)value).GetString(), propertyType), propertyType);
        }

        string text = GetStringFromJsonElement(value);
        return Expression.Constant(ChangeType(text, propertyType), propertyType);
    }

    private static dynamic? ChangeType(object? value, Type conversion)
    {
        var t = conversion;

        if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            if (value == null)
            {
                return null;
            }

            t = Nullable.GetUnderlyingType(t);
        }

        if (t == typeof(DateTimeOffset))
        {
            var culture = CultureInfo.CurrentCulture;
            return value switch
            {
                string stringValue => DateTimeOffset.Parse(stringValue, culture),
                DateTime dateTimeValue => new DateTimeOffset(dateTimeValue),
                _ => throw new InvalidCastException($"Cannot convert {value} to {t}")
            };
        }

        return Convert.ChangeType(value, t!);
    }

    private static IFilter<TFilter> GetValidFilter<TFilter>(IFilter<TFilter> filter)
        where TFilter : IFilter<TFilter>
    {
        if (string.IsNullOrEmpty(filter.Field))
        {
            throw new CustomException("The field attribute is required when declaring a filter");
        }

        if (string.IsNullOrEmpty(filter.Operator))
        {
            throw new CustomException("The Operator attribute is required when declaring a filter");
        }

        return filter;
    }

    private static IOrderedSpecificationBuilder<T> OrderBy<T>(
        this ISpecificationBuilder<T> specificationBuilder,
        string[]? orderByFields) => specificationBuilder.OrderBy(orderByFields, null);

    private static IOrderedSpecificationBuilder<T> OrderBy<T>(
        this ISpecificationBuilder<T> specificationBuilder,
        string[]? orderByFields,
        Dictionary<string, Expression<Func<T, object?>>>? customOrderBy)
    {
        if (orderByFields is null)
        {
            return (IOrderedSpecificationBuilder<T>)specificationBuilder;
        }

        IOrderedSpecificationBuilder<T>? ordered = null;

        foreach (var field in ParseOrderBy(orderByFields))
        {
            Expression<Func<T, object?>> keySelector;

            if (customOrderBy == null || !customOrderBy.TryGetValue(field.Key, out var orderByFunc))
            {
                var paramExpr = Expression.Parameter(typeof(T));

                var propertyExpr = field.Key.Split('.')
                    .Aggregate<string, Expression>(paramExpr, Expression.PropertyOrField);

                keySelector = Expression.Lambda<Func<T, object?>>(
                    Expression.Convert(propertyExpr, typeof(object)),
                    paramExpr);
            }
            else
            {
                keySelector = orderByFunc;
            }

            ordered = field.Value switch
            {
                OrderTypeEnum.OrderBy =>
                    ordered == null
                        ? specificationBuilder.OrderBy(keySelector)
                        : ordered.ThenBy(keySelector),

                OrderTypeEnum.OrderByDescending =>
                    ordered == null
                        ? specificationBuilder.OrderByDescending(keySelector)
                        : ordered.ThenByDescending(keySelector),

                OrderTypeEnum.ThenBy =>
                    ordered!.ThenBy(keySelector),

                OrderTypeEnum.ThenByDescending =>
                    ordered!.ThenByDescending(keySelector),

                _ => ordered
            };
        }

        return ordered ?? (IOrderedSpecificationBuilder<T>)specificationBuilder;
    }

    public static ISpecificationBuilder<T> CopyFrom<T>(
        this ISpecificationBuilder<T> specificationBuilder,
        ISpecification<T> specification, CopyFromMode? mode = null)
        where T : class
    {
        if (mode?.HasFlag(CopyFromMode.OrderExpressions) != false)
        {
            IOrderedSpecificationBuilder<T>? ordered = null;

            foreach (var order in specification.OrderExpressions)
            {
                ordered = order.OrderType switch
                {
                    OrderTypeEnum.OrderBy =>
                        ordered == null
                            ? specificationBuilder.OrderBy(order.KeySelector)
                            : ordered.ThenBy(order.KeySelector),

                    OrderTypeEnum.OrderByDescending =>
                        ordered == null
                            ? specificationBuilder.OrderByDescending(order.KeySelector)
                            : ordered.ThenByDescending(order.KeySelector),

                    OrderTypeEnum.ThenBy =>
                        ordered!.ThenBy(order.KeySelector),

                    OrderTypeEnum.ThenByDescending =>
                        ordered!.ThenByDescending(order.KeySelector),

                    _ => ordered
                };
            }
        }

        if (mode?.HasFlag(CopyFromMode.IncludeExpressions) != false)
        {
            foreach (var include in specification.IncludeExpressions)
            {
                specificationBuilder.Include((Expression<Func<T, object>>)include.LambdaExpression);
            }
        }

        if (mode?.HasFlag(CopyFromMode.WhereExpressions) != false)
        {
            foreach (var where in specification.WhereExpressions)
            {
                specificationBuilder.Where(where.Filter);
            }
        }

        if (mode?.HasFlag(CopyFromMode.SearchCriteria) != false)
        {
            foreach (var search in specification.SearchCriterias)
            {
                specificationBuilder.Search(search.Selector, search.SearchTerm);
            }
        }

        return specificationBuilder;
    }

    private static Dictionary<string, OrderTypeEnum> ParseOrderBy(string[] orderByFields) =>
        new(orderByFields.Select((orderByField, index) =>
        {
            string[] fieldParts = orderByField.Split(' ');
            string field = fieldParts[0];
            bool descending = fieldParts.Length > 1 &&
                              fieldParts[1].StartsWith("Desc", StringComparison.OrdinalIgnoreCase);
            var orderBy = index == 0
                ? descending
                    ? OrderTypeEnum.OrderByDescending
                    : OrderTypeEnum.OrderBy
                : descending
                    ? OrderTypeEnum.ThenByDescending
                    : OrderTypeEnum.ThenBy;

            return new KeyValuePair<string, OrderTypeEnum>(field, orderBy);
        }));
}

[Flags]
public enum CopyFromMode
{
    OrderExpressions = 1,
    IncludeExpressions = 2,
    WhereExpressions = 4,
    SearchCriteria = 8
}