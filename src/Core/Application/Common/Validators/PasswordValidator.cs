using Application.Common.Results;

namespace Application.Common.Validators;

public class PasswordValidator : IPasswordValidator
{
    public ValidationResult Validate(string password)
    {
        var errors = new Dictionary<string, List<string>>();

        if (string.IsNullOrWhiteSpace(password))
            AddError(errors, "password", "Password is required");

        if (password.Length < 8)
            AddError(errors, "password", "Password must be at least 8 characters");

        if (!password.Any(char.IsUpper))
            AddError(errors, "password", "Password must contain at least one uppercase letter");

        if (!password.Any(char.IsLower))
            AddError(errors, "password", "Password must contain at least one lowercase letter");

        if (!password.Any(char.IsDigit))
            AddError(errors, "password", "Password must contain at least one digit");

        if (!password.Any(ch => !char.IsLetterOrDigit(ch)))
            AddError(errors, "password", "Password must contain at least one special character");

        if (IsCommonPassword(password))
            AddError(errors, "password", "This is a commonly used password. Please choose a different one");

        return errors.Any() 
            ? ValidationResult.WithErrors(errors) 
            : ValidationResult.WithErrors(new Dictionary<string, List<string>>());
    }

    private bool IsCommonPassword(string password)
    {
        var commonPasswords = new HashSet<string>
        {
            "Password123!",
            "Admin123!",
            "Test123!",
            "12345678",
        };

        return commonPasswords.Contains(password);
    }
    
    private void AddError(Dictionary<string, List<string>> errors, string key, string message)
    {
        if (!errors.ContainsKey(key))
            errors[key] = new List<string>();
    
        errors[key].Add(message);
    }
}