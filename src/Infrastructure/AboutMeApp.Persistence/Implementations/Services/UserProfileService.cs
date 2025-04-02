using AboutMeApp.Application.Abstractions.Repositories;
using AboutMeApp.Application.Abstractions.Services;
using AboutMeApp.Application.Dtos.SocialMedia;
using AboutMeApp.Application.Dtos.UserProfile;
using AboutMeApp.Common.Shared;
using AboutMeApp.Domain.Entities;
using AboutMeApp.Persistence.Implementations.Repositories;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace AboutMeApp.Persistence.Implementations.Services;

public class UserProfileService : IUserProfileService
{
    private IUserProfileRepository _userprofileRepository { get; }
    private IMapper _mapper { get; }
    private UserManager<User> _userManager { get; }
    private IValidator<UserProfileCreateDto> _createValidator { get; }
    private IValidator<UserProfileUpdateDto> _updateValidator { get; }
    public UserProfileService(IUserProfileRepository userprofileRepository, IMapper mapper, UserManager<User> userManager, IValidator<UserProfileCreateDto> createValidator, IValidator<UserProfileUpdateDto> updateValidator)
    {
        _userprofileRepository = userprofileRepository;
        _mapper = mapper;
        _userManager = userManager;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }
    public async Task<BaseResponse<UserProfileCreateDto>> CreateAsync(UserProfileCreateDto userProfileCreateDto)
    {
        var validationResult = await _createValidator.ValidateAsync(userProfileCreateDto);
        if (!validationResult.IsValid)
        {
            return new BaseResponse<UserProfileCreateDto>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = string.Join(" , ", validationResult.Errors.Select(e => e.ErrorMessage)),
                Data = null
            };
        }

        var userExists = await _userManager.FindByIdAsync(userProfileCreateDto.UserId.ToString());
        if (userExists == null)
        {
            return new BaseResponse<UserProfileCreateDto>
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = "User does not exist.",
                Data = null
            };
        }

        var existingProfile = await _userprofileRepository.GetByFilter(
            up => up.UserId == userProfileCreateDto.UserId && !up.IsDeleted,
            isTracking: false);

        if (existingProfile is not null)
        {
            return new BaseResponse<UserProfileCreateDto>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = "A profile for this user already exists.",
                Data = null
            };
        }

        var createdProfile = _mapper.Map<UserProfile>(userProfileCreateDto);
        await _userprofileRepository.AddAsync(createdProfile);
        await _userprofileRepository.SaveChangesAsync();

        return new BaseResponse<UserProfileCreateDto>
        {
            StatusCode = HttpStatusCode.OK,
            Message = "User profile successfully created.",
            Data = userProfileCreateDto
        };
    }


    public async Task<BaseResponse<object>> DeleteAsync(Guid id)
    {
        var userProfile = await _userprofileRepository.GetByIdAsync(id);
        if (userProfile is null || userProfile.IsDeleted)
        {
            return new BaseResponse<object>
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = "User profile does not exist.",
                Data = null
            };
        }

        userProfile.IsDeleted = true;
        userProfile.ModifiedDate = DateTime.UtcNow;
        await _userprofileRepository.SaveChangesAsync();

        return new BaseResponse<object>
        {
            StatusCode = HttpStatusCode.OK,
            Message = "User profile is deleted successfully.",
            Data = null
        };
    }


    public async Task<BaseResponse<Pagination<UserProfileGetDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 10, bool isPaginated = false)
    {
        if (pageNumber < 1 || pageSize < 1)
        {
            return new BaseResponse<Pagination<UserProfileGetDto>>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = "Page number and page size should be greater than 0.",
                Data = null
            };
        }

        IQueryable<UserProfile> query = _userprofileRepository.GetAll(
            expression: up => !up.IsDeleted);

        int totalItems = await query.CountAsync();

        if (isPaginated)
        {
            int skip = (pageNumber - 1) * pageSize;
            query = query.Skip(skip).Take(pageSize);
        }

        List<UserProfileGetDto> userProfileGetDtos = await query.Select(up => new UserProfileGetDto
        {
            ID = up.Id,
            UserId = up.UserId,
            Bio = up.Bio,
            ProfileImageUrl = up.ProfileImageUrl,
            WebsiteUrl = up.WebsiteUrl,
            PhoneNumber = up.PhoneNumber,
            Location = up.Location,
            TemplateId = up.TemplateId
        }).ToListAsync();

        return new BaseResponse<Pagination<UserProfileGetDto>>
        {
            StatusCode = HttpStatusCode.OK,
            Message = "User profiles retrieved successfully.",
            Data = new Pagination<UserProfileGetDto>
            {
                Items = userProfileGetDtos,
                TotalCount = totalItems,
                PageIndex = pageNumber,
                PageSize = isPaginated ? pageSize : totalItems,
                TotalPage = (int)Math.Ceiling((double)totalItems / pageSize),
            }
        };
    }


    public async Task<BaseResponse<UserProfileGetDto>> GetByIdAsync(Guid id)
    {
        var userProfile = await _userprofileRepository.GetByFilter(
            expression: up => up.Id == id && !up.IsDeleted);

        if (userProfile == null)
        {
            return new BaseResponse<UserProfileGetDto>
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = "The user profile does not exist or has been deleted.",
                Data = null
            };
        }

        return new BaseResponse<UserProfileGetDto>
        {
            StatusCode = HttpStatusCode.OK,
            Data = _mapper.Map<UserProfileGetDto>(userProfile)
        };
    }


    public async Task<BaseResponse<Pagination<UserProfileGetDto>>> SearchByNameAsync(string name, int pageNumber = 1, int pageSize = 10, bool isPaginated = false)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return new BaseResponse<Pagination<UserProfileGetDto>>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = "Search name cannot be empty.",
                Data = null
            };
        }

        if (pageNumber < 1 || pageSize < 1)
        {
            return new BaseResponse<Pagination<UserProfileGetDto>>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = "Page number and page size should be greater than 0.",
                Data = null
            };
        }

        IQueryable<UserProfile> query = _userprofileRepository.GetAll(
            expression: up => !up.IsDeleted &&
                (up.User.Name.ToLower().Contains(name.ToLower()) ||
                 up.User.Surname.ToLower().Contains(name.ToLower())),
            includes: new[] { "User" }
        );

        int totalItems = await query.CountAsync();

        if (totalItems == 0)
        {
            return new BaseResponse<Pagination<UserProfileGetDto>>
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = "No user profiles found for the given name.",
                Data = null
            };
        }

        if (isPaginated)
        {
            int skip = (pageNumber - 1) * pageSize;
            query = query.Skip(skip).Take(pageSize);
        }

        List<UserProfileGetDto> userProfileGetDtos = await query.Select(up => new UserProfileGetDto
        {
            ID = up.Id,
            UserId = up.UserId,
            Bio = up.Bio,
            ProfileImageUrl = up.ProfileImageUrl,
            WebsiteUrl = up.WebsiteUrl,
            PhoneNumber = up.PhoneNumber,
            Location = up.Location,
            TemplateId = up.TemplateId
        }).ToListAsync();

        return new BaseResponse<Pagination<UserProfileGetDto>>
        {
            StatusCode = HttpStatusCode.OK,
            Message = "User profiles retrieved successfully.",
            Data = new Pagination<UserProfileGetDto>
            {
                Items = userProfileGetDtos,
                TotalCount = totalItems,
                PageIndex = pageNumber,
                PageSize = isPaginated ? pageSize : totalItems,
                TotalPage = (int)Math.Ceiling((double)totalItems / pageSize),
            }
        };
    }

    public async Task<BaseResponse<UserProfileUpdateDto>> UpdateAsync(Guid id, UserProfileUpdateDto userProfileUpdateDto)
    {
        if (id != userProfileUpdateDto.Id)
        {
            return new BaseResponse<UserProfileUpdateDto>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = "The provided ID does not match the user profile ID.",
                Data = null
            };
        }

        var validationResult = await _updateValidator.ValidateAsync(userProfileUpdateDto);
        if (!validationResult.IsValid)
        {
            return new BaseResponse<UserProfileUpdateDto>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)),
                Data = null
            };
        }

        var userProfile = await _userprofileRepository.GetByIdAsync(id);
        if (userProfile is null || userProfile.IsDeleted)
        {
            return new BaseResponse<UserProfileUpdateDto>
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = "The user profile does not exist or has been deleted.",
                Data = null
            };
        }

        userProfile.Bio = userProfileUpdateDto.Bio ?? userProfile.Bio;
        userProfile.ProfileImageUrl = userProfileUpdateDto.ProfileImageUrl ?? userProfile.ProfileImageUrl;
        userProfile.WebsiteUrl = userProfileUpdateDto.WebsiteUrl ?? userProfile.WebsiteUrl;
        userProfile.PhoneNumber = userProfileUpdateDto.PhoneNumber ?? userProfile.PhoneNumber;
        userProfile.Location = userProfileUpdateDto.Location ?? userProfile.Location;
        userProfile.TemplateId = userProfileUpdateDto.TemplateId ?? userProfile.TemplateId;
        userProfile.ModifiedDate = DateTime.UtcNow;

        await _userprofileRepository.SaveChangesAsync();

        return new BaseResponse<UserProfileUpdateDto>
        {
            StatusCode = HttpStatusCode.OK,
            Message = "The user profile has been successfully updated.",
            Data = userProfileUpdateDto
        };
    }
}

