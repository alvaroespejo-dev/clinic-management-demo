using AEspejo.Clinic.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace AEspejo.Clinic.API.Controllers;

/// <summary>Translates Application's <see cref="Result"/>/<see cref="Result{T}"/> into HTTP responses.</summary>
public static class ApiResults
{
    public static IActionResult ToActionResult<T>(this Result<T> result) => result.Status switch
    {
        ResultStatus.Ok => new OkObjectResult(result.Value),
        ResultStatus.NotFound => new NotFoundObjectResult(Problem(404, result.Errors)),
        ResultStatus.Invalid => new BadRequestObjectResult(Problem(400, result.Errors)),
        ResultStatus.Conflict => new ConflictObjectResult(Problem(409, result.Errors)),
        ResultStatus.Unauthorized => new UnauthorizedObjectResult(Problem(401, result.Errors)),
        _ => new StatusCodeResult(500)
    };

    public static IActionResult ToCreatedResult<T>(this Result<T> result) =>
        result.Status == ResultStatus.Ok
            ? new ObjectResult(result.Value) { StatusCode = 201 }
            : result.ToActionResult();

    public static IActionResult ToActionResult(this Result result) => result.Status switch
    {
        ResultStatus.Ok => new NoContentResult(),
        ResultStatus.NotFound => new NotFoundObjectResult(Problem(404, result.Errors)),
        ResultStatus.Invalid => new BadRequestObjectResult(Problem(400, result.Errors)),
        ResultStatus.Conflict => new ConflictObjectResult(Problem(409, result.Errors)),
        _ => new StatusCodeResult(500)
    };

    private static object Problem(int status, IReadOnlyList<string> errors) =>
        new { status, errors };
}
