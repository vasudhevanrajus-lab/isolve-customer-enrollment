using System.ComponentModel.DataAnnotations;

namespace CustomerEnrollment.Web.Models;

public class CustomerInputModel
{
    [Required, StringLength(200)]
    public string Name { get; set; } = "";

    [Required, RegularExpression(@"^\d{10}$", ErrorMessage = "Mobile must be a 10-digit number.")]
    public string Mobile { get; set; } = "";

    [Required, EmailAddress, StringLength(200)]
    public string Email { get; set; } = "";

    [Required]
    public string ProofType { get; set; } = "Aadhaar";

    [Required, StringLength(100)]
    public string ProofRef { get; set; } = "";

    [Required, StringLength(500)]
    public string Address { get; set; } = "";
}

public class CustomerView
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

public class EnrollResultViewModel
{
    public int CustomerId { get; set; }
    public string Message { get; set; } = "";
}
