namespace EfCore.Persistence.Configuration;

internal static class SchemaNames
{
    // Tracks user activities, audit logs, change history
    public const string Auditing = nameof(Auditing);

    // Domain for product catalog: products, categories, attributes, etc.
    public const string Catalog = nameof(Catalog);

    // User management: accounts, roles, permissions, authentication
    public const string Identity = nameof(Identity);

    // Third-party integrations: webhooks, external APIs, payment gateways
    public const string External = nameof(External);

    // Shared infrastructure: file uploads, media assets, notifications
    public const string Common = nameof(Common);

    // Global web configuration: site settings, themes, SEO metadata
    public const string Settings = nameof(Settings);

    // Chat and messaging: conversations, messages, attachments
    public const string Chat = nameof(Chat);
}