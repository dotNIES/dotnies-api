using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

public class ApiExceptionFilter : IApiExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is SecurityTokenExpiredException)
        {
            context.Result = new UnauthorizedResult();
            context.ExceptionHandled = true;
        }
        else if (context.Exception is UnauthorizedAccessException)
        {
            context.Result = new UnauthorizedResult();
            context.ExceptionHandled = true;
        }
    }
}