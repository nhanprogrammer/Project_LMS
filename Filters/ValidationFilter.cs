using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Project_LMS.Exceptions;
using Project_LMS.DTOs.Response;
namespace Project_LMS.Filters
{
    public class ValidationFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState
                    .Where(ms => ms.Value != null && ms.Value.Errors.Count > 0)
                    .Select(ms => new ValidationError
                    {
                        Field = ms.Key,
                        Error = ms.Value.Errors.FirstOrDefault()?.ErrorMessage ?? "Lỗi không xác định"
                    })
                    .ToList();

                var response = new ApiResponse<List<ValidationError>>(1, "Dữ liệu không hợp lệ.", errors);
                context.Result = new BadRequestObjectResult(response);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}