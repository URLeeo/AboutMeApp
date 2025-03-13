using AboutMeApp.Application.Abstractions.Repositories;
using AboutMeApp.Application.Abstractions.Services;
using AboutMeApp.Application.Dtos.Certificate;
using AboutMeApp.Application.Dtos.Education;
using AboutMeApp.Common.Shared;
using AboutMeApp.Domain.Entities;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace AboutMeApp.Persistence.Implementations.Services;

public class EducationService : IEducationService
{
    private IEducationRepository _educationRepository { get; }
    private IMapper _mapper { get; }
    private UserManager<User> _userManager { get; }
    private IValidator<EducationCreateDto> _createValidator { get; }
    private IValidator<EducationUpdateDto> _updateValidator { get; }
    public EducationService(IEducationRepository educationRepository, IMapper mapper, UserManager<User> userManager, IValidator<EducationCreateDto> createValidator, IValidator<EducationUpdateDto> updateValidator)
    {
        _educationRepository = educationRepository;
        _mapper = mapper;
        _userManager = userManager;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<BaseResponse<EducationCreateDto>> CreateAsync(EducationCreateDto educationCreateDto)
    {
        var validatonResult = await _createValidator.ValidateAsync(educationCreateDto);
        if (!validatonResult.IsValid)
        {
            return new BaseResponse<EducationCreateDto>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = string.Join(" , ", validatonResult.Errors.Select(e => e.ErrorMessage)),
                Data = null
            };
        }

        var userExists = await _userManager.FindByIdAsync(educationCreateDto.UserProfileId.ToString());
        if (userExists == null)
        {
            return new BaseResponse<EducationCreateDto>
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = "User profile does not exist.",
                Data = null
            };
        }

        var existedEducation = await _educationRepository.GetByFilter(
            e => e.UserProfileId == educationCreateDto.UserProfileId &&
            e.SchoolName.ToLower() == educationCreateDto.SchoolName.ToLower() &&
            e.Degree.ToLower() == educationCreateDto.Degree.ToLower() &&
            e.FieldOfStudy.ToLower() == educationCreateDto.FieldOfStudy.ToLower() &&
            !e.IsDeleted,
            isTracking: false);
        if (existedEducation is not null)
        {
            return new BaseResponse<EducationCreateDto>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = "This education record already exists with the same school, degree, and field of study for this start date.",
                Data = null
            };
        }
        var createdEducation = _mapper.Map<Education>(educationCreateDto);
        await _educationRepository.AddAsync(createdEducation);
        await _educationRepository.SaveChangesAsync();
        return new BaseResponse<EducationCreateDto>
        {
            StatusCode = HttpStatusCode.OK,
            Message = "Education is successfully created.",
            Data = educationCreateDto
        };
    }

    public async Task<BaseResponse<object>> DeleteAsync(Guid id)
    {
        var education = await _educationRepository.GetByIdAsync(id);
        if (education is null || education.IsDeleted)
        {
            return new BaseResponse<object>
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = "Education does not exist.",
                Data = null
            };
        }
        education.IsDeleted = true;
        education.ModifiedDate = DateTime.UtcNow;
        await _educationRepository.SaveChangesAsync();
        return new BaseResponse<object>
        {
            StatusCode = HttpStatusCode.OK,
            Message = "Education is deleted successfully",
            Data = null
        };
    }

    public async Task<BaseResponse<Pagination<EducationGetDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 10, bool isPaginated = false)
    {
        if (pageNumber < 1 || pageSize < 1)
        {
            return new BaseResponse<Pagination<EducationGetDto>>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = "Page number and page size should be greater than 0.",
                Data = null
            };
        }

        IQueryable<Education> query = _educationRepository.GetAll(
        expression: e => !e.IsDeleted);

        int totalItems = await query.CountAsync();

        if (isPaginated)
        {
            int skip = (pageNumber - 1) * pageSize;
            query = query.Skip(skip).Take(pageSize);
        }

        List<EducationGetDto> educationGetDtos = await query.Select(e => new EducationGetDto
        {
            Id = e.Id,
            UserProfileId = e.UserProfileId,
            SchoolName = e.SchoolName,
            Degree = e.Degree,
            FieldOfStudy = e.FieldOfStudy,
            StartDate = e.StartDate,
            EndDate = e.EndDate,
        }).ToListAsync();

        return new BaseResponse<Pagination<EducationGetDto>>
        {
            StatusCode = HttpStatusCode.OK,
            Message = "Educations retrieved successfully.",
            Data = new Pagination<EducationGetDto>
            {
                Items = educationGetDtos,
                TotalCount = totalItems,
                PageIndex = pageNumber,
                PageSize = isPaginated ? pageSize : totalItems,
                TotalPage = (int)Math.Ceiling((double)totalItems / pageSize),
            }
        };
    }

    public async Task<BaseResponse<EducationGetDto>> GetByIdAsync(Guid id)
    {
        var education = await _educationRepository.GetByFilter(
            expression: e => e.Id == id && !e.IsDeleted);

        if (education == null)
        {
            return new BaseResponse<EducationGetDto>
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = "The education does not exist or has been deleted.",
                Data = null
            };
        }
        return new BaseResponse<EducationGetDto>
        {
            StatusCode = HttpStatusCode.OK,
            Data = _mapper.Map<EducationGetDto>(education)
        };
    }

    public async Task<BaseResponse<Pagination<EducationGetDto>>> SearchByNameAsync(string name, int pageNumber = 1, int pageSize = 10, bool isPaginated = false)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return new BaseResponse<Pagination<EducationGetDto>>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = "Search name cannot be empty.",
                Data = null
            };
        }

        if (pageNumber < 1 || pageSize < 1)
        {
            return new BaseResponse<Pagination<EducationGetDto>>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = "Page number and page size should be greater than 0.",
                Data = null
            };
        }

        IQueryable<Education> query = _educationRepository.GetAll(
            expression: e => !e.IsDeleted &&
                 (e.SchoolName.ToLower().Contains(name.ToLower()) ||
                  e.Degree.ToLower().Contains(name.ToLower()) ||
                  e.FieldOfStudy.ToLower().Contains(name.ToLower())));

        int totalItems = await query.CountAsync();

        if (totalItems == 0)
        {
            return new BaseResponse<Pagination<EducationGetDto>>
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = "No educations found for the given name.",
                Data = null
            };
        }

        if (isPaginated)
        {
            int skip = (pageNumber - 1) * pageSize;
            query = query.Skip(skip).Take(pageSize);
        }

        List<EducationGetDto> educationGetDtos = await query.Select(e => new EducationGetDto
        {
            Id = e.Id,
            UserProfileId = e.UserProfileId,
            SchoolName = e.SchoolName,
            Degree = e.Degree,
            FieldOfStudy = e.FieldOfStudy,
            StartDate = e.StartDate,
            EndDate = e.EndDate,
        }).ToListAsync();

        return new BaseResponse<Pagination<EducationGetDto>>
        {
            StatusCode = HttpStatusCode.OK,
            Message = "Educations retrieved successfully.",
            Data = new Pagination<EducationGetDto>
            {
                Items = educationGetDtos,
                TotalCount = totalItems,
                PageIndex = pageNumber,
                PageSize = isPaginated ? pageSize : totalItems,
                TotalPage = (int)Math.Ceiling((double)totalItems / pageSize),
            }
        };
    }

    public async Task<BaseResponse<EducationUpdateDto>> UpdateAsync(Guid id, EducationUpdateDto educationUpdateDto)
    {
        if (id != educationUpdateDto.Id)
        {
            return new BaseResponse<EducationUpdateDto>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = "The provided ID does not match the education ID.",
                Data = null
            };
        }

        var validationResult = await _updateValidator.ValidateAsync(educationUpdateDto);
        if (!validationResult.IsValid)
        {
            return new BaseResponse<EducationUpdateDto>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)),
                Data = null
            };
        }

        var education = await _educationRepository.GetByIdAsync(id);
        if (education is null || education.IsDeleted)
        {
            return new BaseResponse<EducationUpdateDto>
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = "The education does not exist or has been deleted.",
                Data = null
            };
        }

        var existedEducation = await _educationRepository.GetByFilter(
            expression: e => e.SchoolName.ToLower() == educationUpdateDto.SchoolName.ToLower()
                             && e.Id != id
                             && !e.IsDeleted,
            isTracking: false);

        if (existedEducation is not null)
        {
            return new BaseResponse<EducationUpdateDto>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = $"A education with the school name '{educationUpdateDto.SchoolName}' already exists.",
                Data = null
            };
        }

        education.SchoolName = educationUpdateDto.SchoolName ?? education.SchoolName;
        education.Degree = educationUpdateDto.Degree ?? education.Degree;
        education.FieldOfStudy = educationUpdateDto.FieldOfStudy ?? education.FieldOfStudy;
        education.StartDate = educationUpdateDto.StartDate;
        education.EndDate = educationUpdateDto.EndDate ?? education.EndDate;
        education.ModifiedDate = DateTime.UtcNow;

        await _educationRepository.SaveChangesAsync();

        return new BaseResponse<EducationUpdateDto>
        {
            StatusCode = HttpStatusCode.OK,
            Message = "The education is successfully updated.",
            Data = educationUpdateDto
        };
    }
}
