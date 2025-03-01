using AboutMeApp.Domain.Entities.Common;

namespace AboutMeApp.Domain.Entities;

public class Template : BaseEntity
{
    public string Name { get; set; } = null!;
    public string PreviewImageUrl { get; set; } = null!;
    public string CssFileUrl { get; set; } = null!;
}

