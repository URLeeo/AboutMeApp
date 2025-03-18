using AboutMeApp.Application.Abstractions.Repositories;
using AboutMeApp.Application.Abstractions.Services;
using AboutMeApp.Application.Dtos.Education;
using AboutMeApp.Application.Dtos.Experience;
using AboutMeApp.Common.Shared;
using AboutMeApp.Domain.Entities;
using AboutMeApp.Persistence.Implementations.Repositories;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace AboutMeApp.Persistence.Implementations.Services;

public class ExperienceService : IExperienceService
{
    private IExperienceRepository _experienceRepository { get; }
    private IMapper _mapper { get; }
    private UserManager<User> _userManager { get; }
    private IValidator<ExperienceCreateDto> _createValidator { get; }
    private IValidator<ExperienceUpdateDto> _updateValidator { get; }
    public ExperienceService(IExperienceRepository experienceRepository, IMapper mapper, UserManager<User> userManager, IValidator<ExperienceCreateDto> createValidator, IValidator<ExperienceUpdateDto> updateValidator)
    {
        _experienceRepository = experienceRepository;
        _mapper = mapper;
        _userManager = userManager;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }
    public async Task<BaseResponse<ExperienceCreateDto>> CreateAsync(ExperienceCreateDto experienceCreateDto)
    {
        var validatonResult = await _createValidator.ValidateAsync(experienceCreateDto);
        if (!validatonResult.IsValid)
        {
            return new BaseResponse<ExperienceCreateDto>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = string.Join(" , ", validatonResult.Errors.Select(e => e.ErrorMessage)),
                Data = null
            };
        }

        var userExists = await _userManager.FindByIdAsync(experienceCreateDto.UserProfileId.ToString());
        if (userExists == null)
        {
            return new BaseResponse<ExperienceCreateDto>
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = "User profile does not exist.",
                Data = null
            };
        }

        var existedExperience = await _experienceRepository.GetByFilter(
            e => e.UserProfileId == experienceCreateDto.UserProfileId &&
            e.CompanyName.ToLower() == experienceCreateDto.CompanyName.ToLower() &&
            e.Position.ToLower() == experienceCreateDto.Position.ToLower() &&
            !e.IsDeleted,
            isTracking: false);
        if (existedExperience is not null)
        {
            return new BaseResponse<ExperienceCreateDto>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = "This experience record already exists with the same company and position for this start date.",
                Data = null
            };
        }
        var createdExperience = _mapper.Map<Experience>(experienceCreateDto);
        await _experienceRepository.AddAsync(createdExperience);
        await _experienceRepository.SaveChangesAsync();
        return new BaseResponse<ExperienceCreateDto>
        {
            StatusCode = HttpStatusCode.OK,
            Message = "Experience is successfully created.",
            Data = experienceCreateDto
        };
    }

    public async Task<BaseResponse<object>> DeleteAsync(Guid id)
    {
        var experience = await _experienceRepository.GetByIdAsync(id);
        if (experience is null || experience.IsDeleted)
        {
            return new BaseResponse<object>
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = "Experience does not exist.",
                Data = null
            };
        }
        experience.IsDeleted = true;
        experience.ModifiedDate = DateTime.UtcNow;
        await _experienceRepository.SaveChangesAsync();
        return new BaseResponse<object>
        {
            StatusCode = HttpStatusCode.OK,
            Message = "Experience is deleted successfully",
            Data = null
        };
    }

    public async Task<BaseResponse<Pagination<ExperienceGetDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 10, bool isPaginated = false)
    {
        if (pageNumber < 1 || pageSize < 1)
        {
            return new BaseResponse<Pagination<ExperienceGetDto>>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = "Page number and page size should be greater than 0.",
                Data = null
            };
        }

        IQueryable<Experience> query = _experienceRepository.GetAll(
        expression: e => !e.IsDeleted);

        int totalItems = await query.CountAsync();

        if (isPaginated)
        {
            int skip = (pageNumber - 1) * pageSize;
            query = query.Skip(skip).Take(pageSize);
        }

        List<ExperienceGetDto> experienceGetDtos = await query.Select(e => new ExperienceGetDto
        {
            Id = e.Id,
            UserProfileId = e.UserProfileId,
            CompanyName = e.CompanyName,
            Position = e.Position,
            StartDate = e.StartDate,
            EndDate = e.EndDate,
            Description = e.Description
        }).ToListAsync();

        return new BaseResponse<Pagination<ExperienceGetDto>>
        {
            StatusCode = HttpStatusCode.OK,
            Message = "Experiences retrieved successfully.",
            Data = new Pagination<ExperienceGetDto>
            {
                Items = experienceGetDtos,
                TotalCount = totalItems,
                PageIndex = pageNumber,
                PageSize = isPaginated ? pageSize : totalItems,
                TotalPage = (int)Math.Ceiling((double)totalItems / pageSize),
            }
        };
    }

    public async Task<BaseResponse<ExperienceGetDto>> GetByIdAsync(Guid id)
    {
        var experience = await _experienceRepository.GetByFilter(
    expression: e => e.Id == id && !e.IsDeleted);

        if (experience == null)
        {
            return new BaseResponse<ExperienceGetDto>
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = "The experience does not exist or has been deleted.",
                Data = null
            };
        }
        return new BaseResponse<ExperienceGetDto>
        {
            StatusCode = HttpStatusCode.OK,
            Data = _mapper.Map<ExperienceGetDto>(experience)
        };
    }

    public async Task<BaseResponse<Pagination<ExperienceGetDto>>> SearchByNameAsync(string name, int pageNumber = 1, int pageSize = 10, bool isPaginated = false)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return new BaseResponse<Pagination<ExperienceGetDto>>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = "Search name cannot be empty.",
                Data = null
            };
        }

        if (pageNumber < 1 || pageSize < 1)
        {
            return new BaseResponse<Pagination<ExperienceGetDto>>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = "Page number and page size should be greater than 0.",
                Data = null
            };
        }

        IQueryable<Experience> query = _experienceRepository.GetAll(
            expression: e => !e.IsDeleted &&
                 (e.CompanyName.ToLower().Contains(name.ToLower()) ||
                  e.Position.ToLower().Contains(name.ToLower())));

        int totalItems = await query.CountAsync();

        if (totalItems == 0)
        {
            return new BaseResponse<Pagination<ExperienceGetDto>>
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = "No experiences found for the given name.",
                Data = null
            };
        }

        if (isPaginated)
        {
            int skip = (pageNumber - 1) * pageSize;
            query = query.Skip(skip).Take(pageSize);
        }

        List<ExperienceGetDto> experienceGetDtos = await query.Select(e => new ExperienceGetDto
        {
            Id = e.Id,
            UserProfileId = e.UserProfileId,
            CompanyName = e.CompanyName,
            Position = e.Position,
            StartDate = e.StartDate,
            EndDate = e.EndDate,
            Description = e.Description
        }).ToListAsync();

        return new BaseResponse<Pagination<ExperienceGetDto>>
        {
            StatusCode = HttpStatusCode.OK,
            Message = "Experiences retrieved successfully.",
            Data = new Pagination<ExperienceGetDto>
            {
                Items = experienceGetDtos,
                TotalCount = totalItems,
                PageIndex = pageNumber,
                PageSize = isPaginated ? pageSize : totalItems,
                TotalPage = (int)Math.Ceiling((double)totalItems / pageSize),
            }
        };
    }

    public async Task<BaseResponse<ExperienceUpdateDto>> UpdateAsync(Guid id, ExperienceUpdateDto experienceUpdateDto)
    {
        if (id != experienceUpdateDto.Id)
        {
            return new BaseResponse<ExperienceUpdateDto>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = "The provided ID does not match the experience ID.",
                Data = null
            };
        }

        var validationResult = await _updateValidator.ValidateAsync(experienceUpdateDto);
        if (!validationResult.IsValid)
        {
            return new BaseResponse<ExperienceUpdateDto>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)),
                Data = null
            };
        }

        var experience = await _experienceRepository.GetByIdAsync(id);
        if (experience is null || experience.IsDeleted)
        {
            return new BaseResponse<ExperienceUpdateDto>
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = "The experience does not exist or has been deleted.",
                Data = null
            };
        }

        var existedExperience = await _experienceRepository.GetByFilter(
            expression: e => e.CompanyName.ToLower() == experienceUpdateDto.CompanyName.ToLower()
                             && e.Id != id
                             && !e.IsDeleted,
            isTracking: false);

        if (existedExperience is not null)
        {
            return new BaseResponse<ExperienceUpdateDto>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = $"An experience with the company name '{experienceUpdateDto.CompanyName}' already exists.",
                Data = null
            };
        }

        experience.CompanyName = experienceUpdateDto.CompanyName ?? experience.CompanyName;
        experience.Position = experienceUpdateDto.Position ?? experience.Position;
        experience.Description = experienceUpdateDto.Description ?? experience.Description;
        experience.StartDate = experienceUpdateDto.StartDate;
        experience.EndDate = experienceUpdateDto.EndDate ?? experience.EndDate;
        experience.ModifiedDate = DateTime.UtcNow;

        await _experienceRepository.SaveChangesAsync();

        return new BaseResponse<ExperienceUpdateDto>
        {
            StatusCode = HttpStatusCode.OK,
            Message = "The experience is successfully updated.",
            Data = experienceUpdateDto
        };

    }
}

