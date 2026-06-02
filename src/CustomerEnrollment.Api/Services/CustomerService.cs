using CustomerEnrollment.Api.Data;
using CustomerEnrollment.Api.Models;
using CustomerEnrollment.Api.Security;

namespace CustomerEnrollment.Api.Services;

public interface ICustomerService
{
    Task<int> EnrollAsync(EncryptedCustomerDto encrypted, CancellationToken ct);
    Task<CustomerDto?> GetAsync(int customerId, CancellationToken ct);
}

public class CustomerService : ICustomerService
{
    private readonly IClientPayloadCryptor _cryptor;
    private readonly ICustomerRepository _repo;

    public CustomerService(IClientPayloadCryptor cryptor, ICustomerRepository repo)
    {
        _cryptor = cryptor;
        _repo = repo;
    }

    public async Task<int> EnrollAsync(EncryptedCustomerDto e, CancellationToken ct)
    {
        var customer = new CustomerDto
        {
            Name = _cryptor.Decrypt(e.Name),
            Mobile = _cryptor.Decrypt(e.Mobile),
            Email = _cryptor.Decrypt(e.Email),
            ProofType = _cryptor.Decrypt(e.ProofType),
            ProofRef = _cryptor.Decrypt(e.ProofRef),
            Address = _cryptor.Decrypt(e.Address)
        };

        if (string.IsNullOrWhiteSpace(customer.Name)
            || string.IsNullOrWhiteSpace(customer.Mobile)
            || string.IsNullOrWhiteSpace(customer.Email))
        {
            throw new ArgumentException("Name, mobile and email are required.");
        }

        return await _repo.InsertAsync(customer, ct);
    }

    public Task<CustomerDto?> GetAsync(int customerId, CancellationToken ct)
    {
        return _repo.GetByIdAsync(customerId, ct);
    }
}
