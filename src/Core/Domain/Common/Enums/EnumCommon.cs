using System.ComponentModel;

namespace Domain.Common.Enums;

public enum FileStorageStatus
{
    Draft = 1,
    Used = 2
}

public enum FileType
{
    [Description(".jpg,.png,.jpeg")]
    Image = 1
}

public enum EntityStatus
{
    [Description("Inactive")]
    Inactive = 0,

    [Description("Active")]
    Active = 1,
}