using AboutMeApp.Application.Abstractions.Repositories;
using AboutMeApp.Application.Abstractions.Services;
using AboutMeApp.Application.Dtos.Certificate;
using AboutMeApp.Common.Shared;
using AboutMeApp.Domain.Entities;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace AboutMeApp.Persistence.Implementations.Services;

public class CertificateService : ICertificateService
{
    private ICertificateRepository _certificateRepository { get; }
    private IMapper _mapper { get; }

    private UserManager<User> _userManager { get; }
    private IValidator<CertificateCreateDto> _createValidator { get; }
    private IValidator<CertificateUpdateDto> _updateValidator { get; }

    public CertificateService(ICertificateRepository certificateRepository, IMapper mapper, IValidator<CertificateCreateDto> createValidator, IValidator<CertificateUpdateDto> updateValidator, UserManager<User> userManager)
    {
        _certificateRepository = certificateRepository;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _userManager = userManager;

    }
    public async Task<BaseResponse<CertificateCreateDto>> CreateAsync(CertificateCreateDto certificateCreateDto)
    {
        var validationResult = await _createValidator.ValidateAsync(certificateCreateDto);
        if (!validationResult.IsValid)
        {
            return new BaseResponse<CertificateCreateDto>()
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)),
                Data = null
            };
        }

        var userExists = await _userManager.FindByIdAsync(certificateCreateDto.UserId.ToString());
        if (userExists == null)
        {
            return new BaseResponse<CertificateCreateDto>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = "User does not exist.",
                Data = null
            };
        }

        var existedCertificate = await _certificateRepository.GetByFilter(
            c => c.Title == certificateCreateDto.Title && !c.IsDeleted,
            isTracking: false);

        if (existedCertificate != null)
        {
            return new BaseResponse<CertificateCreateDto>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = "Certificate is already exist.",
                Data = null
            };
        }
        var createdCertificate = _mapper.Map<Certificate>(certificateCreateDto);
        await _certificateRepository.AddAsync(createdCertificate);
        await _certificateRepository.SaveChangesAsync();
        return new BaseResponse<CertificateCreateDto>
        {
            StatusCode = HttpStatusCode.OK,
            Message = "Certificate is successfully created.",
            Data = certificateCreateDto
        };
    }

    public async Task<BaseResponse<object>> DeleteAsync(Guid id)
    {
        var certificate = await _certificateRepository.GetByIdAsync(id);
        if (certificate is null || certificate.IsDeleted)
        {
            return new BaseResponse<object>
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = "Certificate does not exist.",
                Data = null
            };
        }
        certificate.IsDeleted = true;
        certificate.ModifiedDate = DateTime.UtcNow;
        await _certificateRepository.SaveChangesAsync();
        return new BaseResponse<object>
        {
            StatusCode = HttpStatusCode.OK,
            Message = "Certificate is deleted successfully",
            Data = null
        };
    }

    public async Task<BaseResponse<Pagination<CertificateGetDto>>> GetAllAsync(int pageNumber = 1, int pageSize = 10, bool isPaginated = false)
    {
        if (pageNumber < 1 || pageSize < 1)
        {
            return new BaseResponse<Pagination<CertificateGetDto>>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = "Page number and page size should be greater than 0.",
                Data = null
            };
        }

        IQueryable<Certificate> query = _certificateRepository.GetAll(
            expression: c => !c.IsDeleted,
            includes: new[] { "User" });

        int totalItems = await query.CountAsync();

        if (isPaginated)
        {
            int skip = (pageNumber - 1) * pageSize;
            query = query.Skip(skip).Take(pageSize);
        }

        List<CertificateGetDto> certificateGetDtos = await query.Select(c => new CertificateGetDto
        {
            Id = c.Id,
            UserId = c.User.Id,
            Title = c.Title,
            Issuer = c.Issuer,
            IssueDate = c.IssueDate,
            ExpiryDate = c.ExpiryDate,
            CertificateUrl = c.CertificateUrl
        }).ToListAsync();

        return new BaseResponse<Pagination<CertificateGetDto>>
        {
            StatusCode = HttpStatusCode.OK,
            Message = "Certificates retrieved successfully.",
            Data = new Pagination<CertificateGetDto>
            {
                Items = certificateGetDtos,
                TotalCount = totalItems,
                PageIndex = pageNumber,
                PageSize = isPaginated ? pageSize : totalItems,
                TotalPage = (int)Math.Ceiling((double)totalItems / pageSize),
            }
        };
    }

    public async Task<BaseResponse<CertificateGetDto>> GetByIdAsync(Guid id)
    {
        var certificate = await _certificateRepository.GetByFilter(
            expression: c => c.Id == id && !c.IsDeleted,
            includes: new[] { "User" });

        if (certificate == null)
        {
            return new BaseResponse<CertificateGetDto>
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = "The certificate does not exist or has been deleted.",
                Data = null
            };
        }
        return new BaseResponse<CertificateGetDto>
        {
            StatusCode = HttpStatusCode.OK,
            Data = _mapper.Map<CertificateGetDto>(certificate)
        };
    }

    public async Task<BaseResponse<Pagination<CertificateGetDto>>> SearchByNameAsync(string name, int pageNumber = 1, int pageSize = 10, bool isPaginated = false)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return new BaseResponse<Pagination<CertificateGetDto>>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = "Search name cannot be empty.",
                Data = null
            };
        }

        if (pageNumber < 1 || pageSize < 1)
        {
            return new BaseResponse<Pagination<CertificateGetDto>>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = "Page number and page size should be greater than 0.",
                Data = null
            };
        }

        IQueryable<Certificate> query = _certificateRepository.GetAll(
            expression: c => !c.IsDeleted && EF.Functions.Like(c.Title, $"%{name}%"),
            includes: new[] { "User" });

        int totalItems = await query.CountAsync();

        if (totalItems == 0)
        {
            return new BaseResponse<Pagination<CertificateGetDto>>
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = "No certificates found for the given name.",
                Data = null
            };
        }

        if (isPaginated)
        {
            int skip = (pageNumber - 1) * pageSize;
            query = query.Skip(skip).Take(pageSize);
        }

        List<CertificateGetDto> certificateGetDtos = await query.Select(c => new CertificateGetDto
        {
            Id = c.Id,
            UserId = c.User.Id,
            Title = c.Title,
            Issuer = c.Issuer,
            IssueDate = c.IssueDate,
            ExpiryDate = c.ExpiryDate,
            CertificateUrl = c.CertificateUrl
        }).ToListAsync();

        return new BaseResponse<Pagination<CertificateGetDto>>
        {
            StatusCode = HttpStatusCode.OK,
            Message = "Certificates retrieved successfully.",
            Data = new Pagination<CertificateGetDto>
            {
                Items = certificateGetDtos,
                TotalCount = totalItems,
                PageIndex = pageNumber,
                PageSize = isPaginated ? pageSize : totalItems,
                TotalPage = (int)Math.Ceiling((double)totalItems / pageSize),
            }
        };
    }



    public async Task<BaseResponse<CertificateUpdateDto>> UpdateAsync(Guid id, CertificateUpdateDto certificateUpdateDto)
    {
        if (id != certificateUpdateDto.Id)
        {
            return new BaseResponse<CertificateUpdateDto>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = "The provided ID does not match the certificate ID.",
                Data = null
            };
        }

        var validationResult = await _updateValidator.ValidateAsync(certificateUpdateDto);
        if (!validationResult.IsValid)
        {
            return new BaseResponse<CertificateUpdateDto>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)),
                Data = null
            };
        }

        var certificate = await _certificateRepository.GetByIdAsync(id);
        if (certificate is null || certificate.IsDeleted)
        {
            return new BaseResponse<CertificateUpdateDto>
            {
                StatusCode = HttpStatusCode.NotFound,
                Message = "The certificate does not exist or has been deleted.",
                Data = null
            };
        }

        var existedCertificate = await _certificateRepository.GetByFilter(
            expression: c => c.Title.ToLower() == certificateUpdateDto.Title.ToLower()
                             && c.Id != id
                             && !c.IsDeleted,
            isTracking: false);

        if (existedCertificate is not null)
        {
            return new BaseResponse<CertificateUpdateDto>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = $"A certificate with the title '{certificateUpdateDto.Title}' already exists.",
                Data = null
            };
        }

        certificate.Title = certificateUpdateDto.Title ?? certificate.Title;
        certificate.Issuer = certificateUpdateDto.Issuer ?? certificate.Issuer;
        certificate.IssueDate = certificateUpdateDto.IssueDate ?? certificate.IssueDate;
        certificate.ExpiryDate = certificateUpdateDto.ExpiryDate ?? certificate.ExpiryDate;
        certificate.CertificateUrl = certificateUpdateDto.CertificateUrl ?? certificate.CertificateUrl;
        certificate.ModifiedDate = DateTime.UtcNow;

        await _certificateRepository.SaveChangesAsync();

        return new BaseResponse<CertificateUpdateDto>
        {
            StatusCode = HttpStatusCode.OK,
            Message = "The certificate is successfully updated...",
            Data = certificateUpdateDto
        };
    }
}