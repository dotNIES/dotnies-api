using Microsoft.AspNetCore.Mvc.Filters;

public interface IApiExceptionFilter
{
    void OnException(ExceptionContext context);
}