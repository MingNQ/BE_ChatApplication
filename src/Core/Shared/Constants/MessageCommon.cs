using Shared.Enums;
using Shared.Extensions;

namespace Shared.Constants;

public class ActionCommon
{
    public const string Reject = "reject";
    public const string Approve = "approve";
    public const string Delete = "delete";
    public const string Create = "create";
    public const string Edit = "edit";
    public const string View = "view";
}

public class ValidatorCustomRuleCommon
{
    public const string Approve = "Approve";
    public const string Reject = "Reject";
    public const string Edit = "Edit";
}

public class CustomRuleKeyCommon
{
    public const string NotFound = "NotFound";
    public const string Invalid = "Invalid";
    public const string Exists = "Exists";
    public const string Mismatch = "Mismatch";
    public const string Required = "Required";
    public const string Inactive = "Inactive";
    public const string Expired = "Expired";
    public const string AlreadyPaid = "AlreadyPaid";
    public const string InvalidAmount = "InvalidAmount";
    public const string InvalidStatus = "InvalidStatus";
    public const string Overlapping = "Overlapping";
    public const string NoAvailableSlots = "NoAvailableSlots";
    public const string SlotNotAvailable = "SlotNotAvailable";
    public const string VenueNotApplicable = "VenueNotApplicable";
}

public class MessageCommon
{
    public const string InitDataSuccess = "Init data successfully";
    public const string GetDataSuccess = "Get data successfully";
    public const string CreateSuccess = "Create successfully";
    public const string CreateFailed = "Create failed";
    public const string UpdateSuccess = "Update successfully";
    public const string UpdateFailed = "Update failed";
    public const string DeleteSuccess = "Delete successfully";
    public const string DeleteFailed = "Delete failed";
    public const string DataNotFound = "Can't find entity to get";
    public const string CannotUpdateStatus = "Can't update status";
    public const string UserUpdateStatusSuccessful = "Status ser has been updated successfully";
    public const string ArchiveSuccess = "Archive record successfully";
    public const string UnArchiveSuccess = "Unarchive record successfully";
    public const string ImportSuccess = "Import successful";
    public const string ImportFailed = "Import failed";
    public const string UploadSuccess = "Upload successful";
    public const string AssignSuccess = "Assign successfully";
    public const string InvalidOtp = "Invalid OTP. Please try again";

    public static string SetEntityNotFound(string entityName, object id) => "ValidationMessages.Exception.EntityNotFound";

    public static string SetEntityDoesNotPermission(string entityName, string action) => $"You do not have permission to {action} this {entityName}.";

    public static string SetEntityDoesNotEdit(string entityName) => $"Cannot edit {entityName} in the current status.";

    public static string SetEntityExists(string entityName, object id) => $"{entityName} id {id} has been exist.";

    public static string SetSuccessMessage(string entityName, EntityActionEnum action) => $"{action} {entityName.ToSeparatedWords()} successfully";
}