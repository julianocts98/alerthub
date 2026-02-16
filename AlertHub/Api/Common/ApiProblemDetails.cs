using Microsoft.AspNetCore.Mvc;

namespace AlertHub.Api.Common;

public static class ApiProblemDetails
{
    public static ObjectResult Build(int statusCode, string title, string detail)
    {
        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail
        };

        return new ObjectResult(problem)
        {
            StatusCode = statusCode
        };
    }
}
