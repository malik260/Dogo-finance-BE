using System.Text.Json.Serialization;

namespace DogoFinance.Integration.Models.YouVerify
{
    public class BvnVerificationRequest
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("isSubjectConsent")]
        public bool IsSubjectConsent { get; set; } = true;

        [JsonPropertyName("premiumBVN")]
        public bool PremiumBvn { get; set; } = false;

        [JsonPropertyName("shouldRetrivedNin")]
        public bool ShouldRetrieveNin { get; set; } = false;

        [JsonPropertyName("validations")]
        public IdentityValidations? Validations { get; set; }

        [JsonPropertyName("metadata")]
        public Dictionary<string, string>? Metadata { get; set; }
    }

    public class NinVerificationRequest
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("isSubjectConsent")]
        public bool IsSubjectConsent { get; set; } = true;

        [JsonPropertyName("premiumNin")]
        public bool PremiumNin { get; set; } = false;

        [JsonPropertyName("validations")]
        public IdentityValidations? Validations { get; set; }

        [JsonPropertyName("metadata")]
        public Dictionary<string, string>? Metadata { get; set; }
    }

    public class IdentityValidations
    {
        [JsonPropertyName("data")]
        public IdentityDataValidation? Data { get; set; }
    }

    public class IdentityDataValidation
    {
        [JsonPropertyName("firstName")]
        public string? FirstName { get; set; }

        [JsonPropertyName("lastName")]
        public string? LastName { get; set; }

        [JsonPropertyName("dateOfBirth")]
        public string? DateOfBirth { get; set; }
    }

    public class YouVerifyResponse<T>
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("statusCode")]
        public int StatusCode { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("data")]
        public T? Data { get; set; }
    }

    public class IdentityVerificationData
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("idNumber")]
        public string? IdNumber { get; set; }

        [JsonPropertyName("firstName")]
        public string? FirstName { get; set; }

        [JsonPropertyName("middleName")]
        public string? MiddleName { get; set; }

        [JsonPropertyName("lastName")]
        public string? LastName { get; set; }

        [JsonPropertyName("image")]
        public string? Image { get; set; }

        [JsonPropertyName("signature")]
        public string? Signature { get; set; }

        [JsonPropertyName("mobile")]
        public string? Mobile { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("dateOfBirth")]
        public string? DateOfBirth { get; set; }

        [JsonPropertyName("gender")]
        public string? Gender { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("nin")]
        public string? Nin { get; set; }

        [JsonPropertyName("birthState")]
        public string? BirthState { get; set; }

        [JsonPropertyName("birthLGA")]
        public string? BirthLga { get; set; }

        [JsonPropertyName("birthCountry")]
        public string? BirthCountry { get; set; }

        [JsonPropertyName("religion")]
        public string? Religion { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("dataValidation")]
        public bool DataValidation { get; set; }

        [JsonPropertyName("selfieValidation")]
        public bool SelfieValidation { get; set; }

        [JsonPropertyName("allValidationPassed")]
        public bool AllValidationPassed { get; set; }

        [JsonPropertyName("validations")]
        public ValidationsResponse? Validations { get; set; }

        [JsonPropertyName("address")]
        public IdentityAddress? Address { get; set; }
    }

    public class ValidationsResponse
    {
        [JsonPropertyName("data")]
        public DataValidationResult? Data { get; set; }

        [JsonPropertyName("validationMessages")]
        public string? ValidationMessages { get; set; }
    }

    public class DataValidationResult
    {
        [JsonPropertyName("firstName")]
        public ValidationField? FirstName { get; set; }

        [JsonPropertyName("lastName")]
        public ValidationField? LastName { get; set; }

        [JsonPropertyName("middleName")]
        public ValidationField? MiddleName { get; set; }

        [JsonPropertyName("dateOfBirth")]
        public ValidationField? DateOfBirth { get; set; }

        [JsonPropertyName("gender")]
        public ValidationField? Gender { get; set; }
    }

    public class ValidationField
    {
        [JsonPropertyName("validated")]
        public bool Validated { get; set; }

        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }

    public class IdentityAddress
    {
        [JsonPropertyName("town")]
        public string? Town { get; set; }

        [JsonPropertyName("lga")]
        public string? Lga { get; set; }

        [JsonPropertyName("state")]
        public string? State { get; set; }

        [JsonPropertyName("addressLine")]
        public string? AddressLine { get; set; }
    }
}
