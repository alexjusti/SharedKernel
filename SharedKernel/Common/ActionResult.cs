using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;

namespace SharedKernel.Common
{
    public class ActionResult<TResult>
    {
        public bool Success { get; set; }

        public TResult Result { get; set; }

        public IEnumerable<ApplicationError> ApplicationErrors { get; set; }

        public IEnumerable<ValidationError> ValidationErrors { get; set; }

        public static ActionResult<TResult> SuccessResult(TResult result)
        {
            return new ActionResult<TResult>
            {
                Success = true,
                Result = result
            };
        }

        public static ActionResult<TResult> ApplicationFailureResult(IEnumerable<ApplicationError> applicationErrors)
        {
            return new ActionResult<TResult>
            {
                Success = false,
                ApplicationErrors = applicationErrors
            };
        }

        public static ActionResult<TResult> ValidationFailureResult(ValidationResult validationResult)
        {
            return new ActionResult<TResult>
            {
                Success = false,
                ValidationErrors = validationResult.Errors.Select(error => new ValidationError
                {
                    Property = error.PropertyName,
                    Message = error.ErrorMessage
                })
            };
        }
    }
}