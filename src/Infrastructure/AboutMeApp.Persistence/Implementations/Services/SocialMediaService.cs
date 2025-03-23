using AboutMeApp.Application.Abstractions.Repositories;
using AboutMeApp.Application.Abstractions.Services;
using AboutMeApp.Application.Dtos.Experience;
using AboutMeApp.Application.Dtos.SocialMedia;
using AboutMeApp.Common.Shared;
using AboutMeApp.Domain.Entities;
using AboutMeApp.Persistence.Implementations.Repositories;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace AboutMeApp.Persistence.Implementations.Services;

public class SocialMediaService : ISocialMediaService
{
    private ISocialMediaRepository _socialmediaRepository { get; }
    private IMapper _mapper { get; }
    private UserManager<User> _userManager { get; }
    private IValidator<SocialMediaCreateDto> _createValidator { get; }
    private IValidator<SocialMediaUpdateDto> _updateValidator { get; }
    public SocialMediaService(ISocialMediaRepository socialmediaRepository, IMapper mapper, UserManager<User> userManager, IValidator<SocialMediaCreateDto> createValidator, IValidator<SocialMediaUpdateDto> updateValidator)
    {
        _socialmediaRepository = socialmediaRepository;
        _mapper = mapper;
        _userManager = userManager;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }
    public async Task<BaseResponse<SocialMediaCreateDto>> CreateAsync(SocialMediaCreateDto socialmediaCreateDto)
    {
        var validatonResult = await _createValidator.ValidateAsync(socialmediaCreateDto);
        if (!validatonResult.IsValid)
        {
            return new BaseResponse<SocialMediaCreateDto>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = string.Join(" , ", validatonResult.Errors.Select(e => e.ErrorMessage)),
                Data = null
            };
        }

        var userExists = await _userManager.FindByIdAsync(socialmediaCreateDto.UserProfileId.ToString());
        if (userExists == null)
        {
            return new BaseResponse<SocialMediaCreateDto>
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = "User profile does not exist.",
                Data = null
            };
        }

        var existedSocialMedia = await _socialmediaRepository.GetByFilter(
            sm => sm.UserProfileId == socialmediaCreateDto.UserProfileId &&
            sm.Platform.ToLower() == socialmediaCreateDto.Platform.ToLower() &&
            sm.Url.ToLower() == socialmediaCreateDto.Url.ToLower() &&
            !sm.IsDeleted,
            isTracking: false);
        if (existedSocialMedia is not null)
        {
            return new BaseResponse<SocialMediaCreateDto>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = "This Social Media record already exists with the same Platform and Url.",
                Data = null
            };
        }
        var createdSocialMedia = _mapper.Map<SocialMedia>(socialmediaCreateDto);
        await _socialmediaRepository.AddAsync(createdSocialMedia);
        await _socialmediaRepository.SaveChangesAsync();
        return new BaseResponse<SocialMediaCreateDto>
        {
            StatusCode = HttpStatusCode.OK,
            Message = "Social Media is successfully created.",
            Data = socialmediaCreateDto
        };
    }

    public async Task<BaseResponse<object>> DeleteAsync(Guid id)
    {
        var socialMedia = await _socialmediaRepository.GetByIdAsync(id);
        if (socialMedia is null || socialMedia.IsDeleted)
        {
            return new BaseResponse<object>
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = "Social Media does not exist.",
                Data = null
            };
        }
        socialMedia.IsDeleted = true;
        socialMedia.ModifiedDate = DateTime.UtcNow;
        await _socialmediaRepository.SaveChangesAsync();
        return new BaseResponse<object>
        {
            StatusCode = HttpStatusCode.OK,
            Message = "Social Media is deleted successfully",
            Data = null
        };
    }

    public async Task<BaseResponse<Pagination<SocialMediaGetDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 10, bool isPaginated = false)
    {
        if (pageNumber < 1 || pageSize < 1)
        {
            return new BaseResponse<Pagination<SocialMediaGetDto>>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = "Page number and page size should be greater than 0.",
                Data = null
            };
        }

        IQueryable<SocialMedia> query = _socialmediaRepository.GetAll(
            expression: sm => !sm.IsDeleted);

        int totalItems = await query.CountAsync();

        if (isPaginated)
        {
            int skip = (pageNumber - 1) * pageSize;
            query = query.Skip(skip).Take(pageSize);
        }

        List<SocialMediaGetDto> socialMediaGetDtos = await query.Select(sm => new SocialMediaGetDto
        {
            Id = sm.Id,
            UserProfileId = sm.UserProfileId,
            Platform = sm.Platform,
            Url = sm.Url
        }).ToListAsync();

        return new BaseResponse<Pagination<SocialMediaGetDto>>
        {
            StatusCode = HttpStatusCode.OK,
            Message = "Social media records retrieved successfully.",
            Data = new Pagination<SocialMediaGetDto>
            {
                Items = socialMediaGetDtos,
                TotalCount = totalItems,
                PageIndex = pageNumber,
                PageSize = isPaginated ? pageSize : totalItems,
                TotalPage = (int)Math.Ceiling((double)totalItems / pageSize),
            }
        };
    }


    public async Task<BaseResponse<SocialMediaGetDto>> GetByIdAsync(Guid id)
    {
        var socialMedia = await _socialmediaRepository.GetByFilter(
            expression: sm => sm.Id == id && !sm.IsDeleted);

        if (socialMedia == null)
        {
            return new BaseResponse<SocialMediaGetDto>
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = "The social media record does not exist or has been deleted.",
                Data = null
            };
        }

        return new BaseResponse<SocialMediaGetDto>
        {
            StatusCode = HttpStatusCode.OK,
            Data = _mapper.Map<SocialMediaGetDto>(socialMedia)
        };
    }


    public async Task<BaseResponse<Pagination<SocialMediaGetDto>>> SearchByNameAsync(string name, int pageNumber = 1, int pageSize = 10, bool isPaginated = false)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return new BaseResponse<Pagination<SocialMediaGetDto>>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = "Search name cannot be empty.",
                Data = null
            };
        }

        if (pageNumber < 1 || pageSize < 1)
        {
            return new BaseResponse<Pagination<SocialMediaGetDto>>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = "Page number and page size should be greater than 0.",
                Data = null
            };
        }

        IQueryable<SocialMedia> query = _socialmediaRepository.GetAll(
            expression: sm => !sm.IsDeleted &&
                 (sm.Platform.ToLower().Contains(name.ToLower()) ||
                  sm.Url.ToLower().Contains(name.ToLower())));

        int totalItems = await query.CountAsync();

        if (totalItems == 0)
        {
            return new BaseResponse<Pagination<SocialMediaGetDto>>
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = "No social media links found for the given name.",
                Data = null
            };
        }

        if (isPaginated)
        {
            int skip = (pageNumber - 1) * pageSize;
            query = query.Skip(skip).Take(pageSize);
        }

        List<SocialMediaGetDto> socialMediaGetDtos = await query.Select(sm => new SocialMediaGetDto
        {
            Id = sm.Id,
            UserProfileId = sm.UserProfileId,
            Platform = sm.Platform,
            Url = sm.Url
        }).ToListAsync();

        return new BaseResponse<Pagination<SocialMediaGetDto>>
        {
            StatusCode = HttpStatusCode.OK,
            Message = "Social media links retrieved successfully.",
            Data = new Pagination<SocialMediaGetDto>
            {
                Items = socialMediaGetDtos,
                TotalCount = totalItems,
                PageIndex = pageNumber,
                PageSize = isPaginated ? pageSize : totalItems,
                TotalPage = (int)Math.Ceiling((double)totalItems / pageSize),
            }
        };
    }


    public async Task<BaseResponse<SocialMediaUpdateDto>> UpdateAsync(Guid id, SocialMediaUpdateDto socialMediaUpdateDto)
    {
        if (id != socialMediaUpdateDto.Id)
        {
            return new BaseResponse<SocialMediaUpdateDto>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = "The provided ID does not match the social media ID.",
                Data = null
            };
        }

        var validationResult = await _updateValidator.ValidateAsync(socialMediaUpdateDto);
        if (!validationResult.IsValid)
        {
            return new BaseResponse<SocialMediaUpdateDto>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)),
                Data = null
            };
        }

        var socialMedia = await _socialmediaRepository.GetByIdAsync(id);
        if (socialMedia is null || socialMedia.IsDeleted)
        {
            return new BaseResponse<SocialMediaUpdateDto>
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = "The social media entry does not exist or has been deleted.",
                Data = null
            };
        }

        var existedSocialMedia = await _socialmediaRepository.GetByFilter(
            expression: sm => sm.Platform.ToLower() == socialMediaUpdateDto.Platform.ToLower()
                             && sm.UserProfileId == socialMediaUpdateDto.UserProfileId
                             && sm.Id != id
                             && !sm.IsDeleted,
            isTracking: false);

        if (existedSocialMedia is not null)
        {
            return new BaseResponse<SocialMediaUpdateDto>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = $"A social media entry with the platform '{socialMediaUpdateDto.Platform}' already exists for this user.",
                Data = null
            };
        }

        socialMedia.Platform = socialMediaUpdateDto.Platform ?? socialMedia.Platform;
        socialMedia.Url = socialMediaUpdateDto.Url ?? socialMedia.Url;
        socialMedia.ModifiedDate = DateTime.UtcNow;

        await _socialmediaRepository.SaveChangesAsync();

        return new BaseResponse<SocialMediaUpdateDto>
        {
            StatusCode = HttpStatusCode.OK,
            Message = "The social media entry has been successfully updated.",
            Data = socialMediaUpdateDto
        };
    }

}
