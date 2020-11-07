using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;

namespace SharedKernel.Common
{
    public class ActionResult
    {
        public bool Success { get; set; }

        public IEnumerable<ApplicationError> ApplicationErrors { get; set; }

        public IEnumerable<ValidationError> ValidationErrors { get; set; }

        public static ActionResult SuccessResult()
        {
            return new ActionResult
            {
                Success = true
            };
        }

        public static ActionResult ApplicationFailureResult(IEnumerable<ApplicationError> applicationErrors)
        {
            return new ActionResult
            {
                Success = false,
                ApplicationErrors = applicationErrors
            };
        }

        public static ActionResult ValidationFailureResult(ValidationResult validationResult)
        {
            return new ActionResult
            {
                Success = false,
                ValidationErrors = validationResult.Errors.Select(error => new ValidationError
                {
                    Property = error.PropertyName,
                    Message = error.ErrorMessage
                })
            };
        }

        public static ActionResult ApplicationFailureResult(ApplicationError applicationError)
        {
            return ApplicationFailureResult(new[] { applicationError });
        }
    }

    public class ActionResult<TResult>
        : ActionResult
    {
        public TResult Result { get; set; }

        public static ActionResult<TResult> SuccessResult(TResult result)
        {
            return new ActionResult<TResult>
            {
                Success = true,
                Result = result
            };
        }
    }
}