using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;

namespace SharedKernel.Shared
{
    public class Result
    {
        public bool IsSuccess { get; set; }

        public IEnumerable<ValidationError> ValidationErrors { get; set; }

        public IEnumerable<Error> InputErrors { get; set; }

        public IEnumerable<Error> ApplicationErrors { get; set; }

        public static Result Ok()
        {
            return new()
            {
                IsSuccess = true
            };
        }

        private static Result ValidationFailure(ValidationError error)
        {
            return new()
            {
                IsSuccess = false,
                ValidationErrors = new[] { error }
            };
        }

        public static Result FromValidationResult(ValidationResult validationResult)
        {
            return new()
            {
                IsSuccess = false,
                ValidationErrors = validationResult.Errors.Select(f => new ValidationError
                {
                    Property = f.PropertyName,
                    Message = f.ErrorMessage
                })
            };
        }

        public static Result InputFailure(IEnumerable<Error> inputErrors)
        {
            return new()
            {
                IsSuccess = false,
                InputErrors = inputErrors
            };
        }

        public static Result InputFailure(Error inputError)
        {
            return InputFailure(new[] {inputError});
        }

        public static Result ApplicationFailure(IEnumerable<Error> applicationErrors)
        {
            return new()
            {
                IsSuccess = false,
                ApplicationErrors = applicationErrors
            };
        }

        public static Result ApplicationFailure(Error applicationError)
        {
            return ApplicationFailure(new[] { applicationError });
        }
    }

    public class Result<T> : Result
    {
        public T Response { get; set; }

        public static Result<T> Ok(T response)
        {
            return new()
            {
                IsSuccess = true,
                Response = response
            };
        }

        public new static Result<T> FromValidationResult(ValidationResult validationResult)
        {
            return new()
            {
                IsSuccess = false,
                ValidationErrors = validationResult.Errors.Select(f => new ValidationError
                {
                    Property = f.PropertyName,
                    Message = f.ErrorMessage
                })
            };
        }

        public new static Result<T> InputFailure(IEnumerable<Error> inputErrors)
        {
            return new()
            {
                IsSuccess = false,
                InputErrors = inputErrors
            };
        }

        public new static Result<T> InputFailure(Error inputError)
        {
            return InputFailure(new[] { inputError });
        }

        public new static Result<T> ApplicationFailure(IEnumerable<Error> applicationErrors)
        {
            return new()
            {
                IsSuccess = false,
                ApplicationErrors = applicationErrors
            };
        }

        public new static Result<T> ApplicationFailure(Error applicationError)
        {
            return ApplicationFailure(new[] { applicationError });
        }
    }

    public class ValidationError
    {
        public string Property { get; set; }

        public string Message { get; set; }
    }

    public class Error
    {
        public string Code { get; set; }

        public string Message { get; set; }
    }
}