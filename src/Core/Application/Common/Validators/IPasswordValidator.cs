using Application.Common.Results;

namespace Application.Common.Validators;

public interface IPasswordValidator
{
    ValidationResult Validate(string password);
}