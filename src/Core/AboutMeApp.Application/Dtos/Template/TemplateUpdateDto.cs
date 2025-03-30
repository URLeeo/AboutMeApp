﻿namespace AboutMeApp.Application.Dtos.Template;

public record TemplateUpdateDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string PreviewImageUrl { get; set; } = null!;
    public string CssFileUrl { get; set; } = null!;
}
