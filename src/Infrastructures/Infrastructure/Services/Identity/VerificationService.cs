using Application.Common.Repositories;
using Application.Common.UnitOfWork;
using Application.Interfaces.Services;
using Domain.Entities.Identity;
using Domain.Events.Verification;
using Domain.ValueObjects.Verification;
using Shared.Constants;

namespace Infrastructure.Services.Identity;

public class VerificationService(IUnitOfWork unitOfWork)
    : IVerificationService
{
    private readonly IWriteRepository<UserVerification> _verificationRepository = unitOfWork.GetRepository<UserVerification>();

    public async Task<UserVerification?> FindActiveVerification(ContactInfo contactInfo, VerificationMode mode, long? userId = null)
    {
        string modeString = mode.ToString();

        if (contactInfo.IsEmail)
        {
            return await _verificationRepository.GetFirstOrDefaultAsync(
                predicate: x => x.Email == contactInfo.Value
                              && x.Mode == modeString
                              && x.Status == UserVerificationStatus.Active
                              && x.CodeExpirationDate.HasValue && x.CodeExpirationDate > DateTime.UtcNow
                              && (userId == null || x.UserId == userId),
                disableTracking: false);
        }

        return await _verificationRepository.GetFirstOrDefaultAsync(
            predicate: x => x.Phone == contactInfo.Value
                          && x.Mode == modeString
                          && x.Status == UserVerificationStatus.Active
                          && x.CodeExpirationDate.HasValue && x.CodeExpirationDate > DateTime.UtcNow
                          && (userId == null || x.UserId == userId),
            disableTracking: false);
    }

    public async Task<UserVerification?> FindActiveVerificationById(ContactInfo contactInfo, long verificationId)
    {
        if (contactInfo.IsEmail)
        {
            return await _verificationRepository.GetFirstOrDefaultAsync(
                predicate: x => x.Id == verificationId
                              && x.Email == contactInfo.Value
                              && x.Status == UserVerificationStatus.Active
                              && x.CodeExpirationDate.HasValue && x.CodeExpirationDate > DateTime.UtcNow,
                disableTracking: false);
        }

        return await _verificationRepository.GetFirstOrDefaultAsync(
            predicate: x => x.Id == verificationId
                          && x.Phone == contactInfo.Value
                          && x.Status == UserVerificationStatus.Active
                          && x.CodeExpirationDate.HasValue && x.CodeExpirationDate > DateTime.UtcNow,
            disableTracking: false);
    }

    public async Task<UserVerification?> FindVerificationById(ContactInfo contactInfo, long verificationId)
    {
        if (contactInfo.IsEmail)
        {
            return await _verificationRepository.GetFirstOrDefaultAsync(
                predicate: x => x.Id == verificationId
                              && x.Email == contactInfo.Value,
                disableTracking: false);
        }

        return await _verificationRepository.GetFirstOrDefaultAsync(
            predicate: x => x.Id == verificationId
                          && x.Phone == contactInfo.Value,
            disableTracking: false);
    }

    public async Task InsertVerificationAsync(UserVerification verification, CancellationToken cancellation)
    {
        await _verificationRepository.InsertAsync(verification, cancellation);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateVerificationAsync(UserVerification verification)
    {
        _verificationRepository.Update(verification);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteVerificationAsync(long verificationId)
    {
        var verification = await _verificationRepository.GetFirstOrDefaultAsync(
            predicate: x => x.Id == verificationId,
            disableTracking: true);

        if (verification != null)
        {
            _verificationRepository.Delete(verification);
            await unitOfWork.SaveChangesAsync();
        }
    }
}