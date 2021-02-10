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
            return new Result
            {
                IsSuccess = true
            };
        }

        public static Result ValidationFailure(ValidationError error)
        {
            return new Result
            {
                IsSuccess = false,
                ValidationErrors = new[] { error }
            };
        }

        public static Result FromValidationResult(ValidationResult validationResult)
        {
            return new Result
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
            return new Result
            {
                IsSuccess = false,
                InputErrors = inputErrors
            };
        }

        public static Result ApplicationFailure(IEnumerable<Error> applicationErrors)
        {
            return new Result
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

        public static implicit operator Result<T>(T response)
        {
            return Ok(response);
        }

        private static Result<T> Ok(T response)
        {
            return new Result<T>
            {
                IsSuccess = true,
                Response = response
            };
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