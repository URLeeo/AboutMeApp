using AboutMeApp.Application.Abstractions.Repositories;
using AboutMeApp.Application.Abstractions.Services;
using AboutMeApp.Application.Dtos.SocialMedia;
using AboutMeApp.Application.Dtos.Template;
using AboutMeApp.Common.Shared;
using AboutMeApp.Domain.Entities;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace AboutMeApp.Persistence.Implementations.Services;

public class TemplateService : ITemplateService
{
    private ITemplateRepository _templateRepository { get; }
    private IMapper _mapper { get; }
    private IValidator<TemplateCreateDto> _createValidator { get; }
    private IValidator<TemplateUpdateDto> _updateValidator { get; }
    public TemplateService(ITemplateRepository templateRepository, IMapper mapper,IValidator<TemplateCreateDto> createValidator, IValidator<TemplateUpdateDto> updateValidator)
    {
        _templateRepository = templateRepository;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }
    public async Task<BaseResponse<TemplateCreateDto>> CreateAsync(TemplateCreateDto templateCreateDto)
    {
        var validationResult = await _createValidator.ValidateAsync(templateCreateDto);
        if (!validationResult.IsValid)
        {
            return new BaseResponse<TemplateCreateDto>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = string.Join(" , ", validationResult.Errors.Select(e => e.ErrorMessage)),
                Data = null
            };
        }

        var existingTemplate = await _templateRepository.GetByFilter(
            t => t.Name.ToLower() == templateCreateDto.Name.ToLower() &&
                 t.PreviewImageUrl.ToLower() == templateCreateDto.PreviewImageUrl.ToLower() &&
                 t.CssFileUrl.ToLower() == templateCreateDto.CssFileUrl.ToLower() &&
                 !t.IsDeleted,
            isTracking: false);

        if (existingTemplate is not null)
        {
            return new BaseResponse<TemplateCreateDto>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = "This Template already exists with the same Name, Preview Image, and CSS file.",
                Data = null
            };
        }

        var createdTemplate = _mapper.Map<Template>(templateCreateDto);
        await _templateRepository.AddAsync(createdTemplate);
        await _templateRepository.SaveChangesAsync();

        return new BaseResponse<TemplateCreateDto>
        {
            StatusCode = HttpStatusCode.OK,
            Message = "Template is successfully created.",
            Data = templateCreateDto
        };
    }

    public async Task<BaseResponse<object>> DeleteAsync(Guid id)
    {
        var template = await _templateRepository.GetByIdAsync(id);
        if (template is null || template.IsDeleted)
        {
            return new BaseResponse<object>
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = "Template does not exist.",
                Data = null
            };
        }

        template.IsDeleted = true;
        template.ModifiedDate = DateTime.UtcNow;
        await _templateRepository.SaveChangesAsync();

        return new BaseResponse<object>
        {
            StatusCode = HttpStatusCode.OK,
            Message = "Template is deleted successfully",
            Data = null
        };
    }


    public async Task<BaseResponse<Pagination<TemplateGetDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 10, bool isPaginated = false)
    {
        if (pageNumber < 1 || pageSize < 1)
        {
            return new BaseResponse<Pagination<TemplateGetDto>>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = "Page number and page size should be greater than 0.",
                Data = null
            };
        }

        IQueryable<Template> query = _templateRepository.GetAll(
            expression: t => !t.IsDeleted);

        int totalItems = await query.CountAsync();

        if (isPaginated)
        {
            int skip = (pageNumber - 1) * pageSize;
            query = query.Skip(skip).Take(pageSize);
        }

        List<TemplateGetDto> templateGetDtos = await query.Select(t => new TemplateGetDto
        {
            Id = t.Id,
            Name = t.Name,
            PreviewImageUrl = t.PreviewImageUrl,
            CssFileUrl = t.CssFileUrl
        }).ToListAsync();

        return new BaseResponse<Pagination<TemplateGetDto>>
        {
            StatusCode = HttpStatusCode.OK,
            Message = "Templates retrieved successfully.",
            Data = new Pagination<TemplateGetDto>
            {
                Items = templateGetDtos,
                TotalCount = totalItems,
                PageIndex = pageNumber,
                PageSize = isPaginated ? pageSize : totalItems,
                TotalPage = (int)Math.Ceiling((double)totalItems / pageSize),
            }
        };
    }

    public async Task<BaseResponse<TemplateGetDto>> GetByIdAsync(Guid id)
    {
        var template = await _templateRepository.GetByFilter(
            expression: t => t.Id == id && !t.IsDeleted);

        if (template == null)
        {
            return new BaseResponse<TemplateGetDto>
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = "The template does not exist or has been deleted.",
                Data = null
            };
        }

        return new BaseResponse<TemplateGetDto>
        {
            StatusCode = HttpStatusCode.OK,
            Data = _mapper.Map<TemplateGetDto>(template)
        };
    }


    public async Task<BaseResponse<Pagination<TemplateGetDto>>> SearchByNameAsync(string name, int pageNumber = 1, int pageSize = 10, bool isPaginated = false)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return new BaseResponse<Pagination<TemplateGetDto>>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = "Search name cannot be empty.",
                Data = null
            };
        }

        if (pageNumber < 1 || pageSize < 1)
        {
            return new BaseResponse<Pagination<TemplateGetDto>>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = "Page number and page size should be greater than 0.",
                Data = null
            };
        }

        IQueryable<Template> query = _templateRepository.GetAll(
            expression: t => !t.IsDeleted &&
                 t.Name.ToLower().Contains(name.ToLower()));

        int totalItems = await query.CountAsync();

        if (totalItems == 0)
        {
            return new BaseResponse<Pagination<TemplateGetDto>>
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = "No templates found for the given name.",
                Data = null
            };
        }

        if (isPaginated)
        {
            int skip = (pageNumber - 1) * pageSize;
            query = query.Skip(skip).Take(pageSize);
        }

        List<TemplateGetDto> templateGetDtos = await query.Select(t => new TemplateGetDto
        {
            Id = t.Id,
            Name = t.Name,
            PreviewImageUrl = t.PreviewImageUrl,
            CssFileUrl = t.CssFileUrl
        }).ToListAsync();

        return new BaseResponse<Pagination<TemplateGetDto>>
        {
            StatusCode = HttpStatusCode.OK,
            Message = "Templates retrieved successfully.",
            Data = new Pagination<TemplateGetDto>
            {
                Items = templateGetDtos,
                TotalCount = totalItems,
                PageIndex = pageNumber,
                PageSize = isPaginated ? pageSize : totalItems,
                TotalPage = (int)Math.Ceiling((double)totalItems / pageSize),
            }
        };
    }

    public async Task<BaseResponse<TemplateUpdateDto>> UpdateAsync(Guid id, TemplateUpdateDto templateUpdateDto)
    {
        if (id != templateUpdateDto.Id)
        {
            return new BaseResponse<TemplateUpdateDto>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = "The provided ID does not match the template ID.",
                Data = null
            };
        }

        var validationResult = await _updateValidator.ValidateAsync(templateUpdateDto);
        if (!validationResult.IsValid)
        {
            return new BaseResponse<TemplateUpdateDto>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)),
                Data = null
            };
        }

        var template = await _templateRepository.GetByIdAsync(id);
        if (template is null || template.IsDeleted)
        {
            return new BaseResponse<TemplateUpdateDto>
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = "The template does not exist or has been deleted.",
                Data = null
            };
        }

        var existingTemplate = await _templateRepository.GetByFilter(
            expression: t => t.Name.ToLower() == templateUpdateDto.Name.ToLower()
                             && t.Id != id
                             && !t.IsDeleted,
            isTracking: false);

        if (existingTemplate is not null)
        {
            return new BaseResponse<TemplateUpdateDto>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = $"A template with the name '{templateUpdateDto.Name}' already exists.",
                Data = null
            };
        }

        template.Name = templateUpdateDto.Name ?? template.Name;
        template.PreviewImageUrl = templateUpdateDto.PreviewImageUrl ?? template.PreviewImageUrl;
        template.CssFileUrl = templateUpdateDto.CssFileUrl ?? template.CssFileUrl;
        template.ModifiedDate = DateTime.UtcNow;

        await _templateRepository.SaveChangesAsync();

        return new BaseResponse<TemplateUpdateDto>
        {
            StatusCode = HttpStatusCode.OK,
            Message = "The template has been successfully updated.",
            Data = templateUpdateDto
        };
    }
}
