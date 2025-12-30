using Domain.Entities.Identity;
using Domain.Events.Verification;
using Domain.ValueObjects.Verification;

namespace Application.Interfaces.Services;

public interface IVerificationService
{
    Task<UserVerification?> FindActiveVerification(ContactInfo contactInfo, VerificationMode mode, long? userId = null);

    Task InsertVerificationAsync(UserVerification verification, CancellationToken cancellation);

    Task UpdateVerificationAsync(UserVerification verification);

    Task DeleteVerificationAsync(long verificationId);

    Task<UserVerification?> FindActiveVerificationById(ContactInfo contactInfo, long verificationId);

    Task<UserVerification?> FindVerificationById(ContactInfo contactInfo, long verificationId);
}