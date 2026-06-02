using System.ComponentModel.DataAnnotations;

namespace CustomerEnrollment.Api.Models;

public class EncryptedCustomerDto
{
    [Required] public string Name { get; set; } = "";
    [Required] public string Mobile { get; set; } = "";
    [Required] public string Email { get; set; } = "";
    [Required] public string ProofType { get; set; } = "";
    [Required] public string ProofRef { get; set; } = "";
    [Required] public string Address { get; set; } = "";
}

public class CustomerDto
{
    public int      CustomerId   { get; set; }
    public string   Name         { get; set; } = "";
    public string   Mobile       { get; set; } = "";
    public string   Email        { get; set; } = "";
    public string   ProofType    { get; set; } = "";
    public string   ProofRef     { get; set; } = "";
    public string   Address      { get; set; } = "";
    public DateTime CreatedOnUtc { get; set; }
}

public class EnrollResponse
{
    public int CustomerId { get; set; }
}
