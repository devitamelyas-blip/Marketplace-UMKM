using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UmkmJatim.Application.Common.Exceptions;
using UmkmJatim.Domain.Exceptions;
using DomainValidationException = UmkmJatim.Domain.Exceptions.ValidationException;

namespace UmkmJatim.API.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Terjadi unhandled exception: {Message}", exception.Message);

        var (statusCode, title, errors) = exception switch
        {
            ValidationException appValidationEx => (
                StatusCodes.Status422UnprocessableEntity,
                "Kesalahan Validasi Data",
                appValidationEx.Errors),

            DomainValidationException domainValidationEx => (
                StatusCodes.Status422UnprocessableEntity,
                "Kesalahan Validasi Aturan Bisnis",
                domainValidationEx.Errors),

            NotFoundException notFoundEx => (
                StatusCodes.Status404NotFound,
                "Sumber Daya Tidak Ditemukan",
                new Dictionary<string, string[]> { { "Detail", new[] { notFoundEx.Message } } }),

            UnauthorizedException unauthorizedEx => (
                StatusCodes.Status401Unauthorized,
                "Tidak Memiliki Otorisasi",
                new Dictionary<string, string[]> { { "Detail", new[] { unauthorizedEx.Message } } }),

            ForbiddenException forbiddenEx => (
                StatusCodes.Status403Forbidden,
                "Akses Ditolak",
                new Dictionary<string, string[]> { { "Detail", new[] { forbiddenEx.Message } } }),

            ConflictException conflictEx => (
                StatusCodes.Status409Conflict,
                "Konflik Data",
                new Dictionary<string, string[]> { { "Detail", new[] { conflictEx.Message } } }),

            DbUpdateConcurrencyException => (
                StatusCodes.Status409Conflict,
                "Konflik Konkurensi Data",
                new Dictionary<string, string[]> { { "Detail", new[] { "Data telah diubah oleh pengguna lain. Silakan periksa kembali dan coba lagi." } } }),

            _ => (
                StatusCodes.Status500InternalServerError,
                "Terjadi Kesalahan Server",
                new Dictionary<string, string[]> { { "Detail", new[] { "Terjadi kesalahan yang tidak terduga pada server kami." } } })
        };

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Type = $"https://httpstatuses.com/{statusCode}",
            Instance = httpContext.Request.Path
        };

        if (errors.Any())
        {
            problemDetails.Extensions["errors"] = errors;
        }

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
