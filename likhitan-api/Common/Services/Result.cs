using System.Net;

namespace likhitan.Common.Services
{
    public class Result<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }

        public Result(bool isSuccess, string message, HttpStatusCode statusCode, T? data = default, List<string>? errors = null)
        {
            IsSuccess = isSuccess;
            Message = message;
            StatusCode = statusCode;
            Data = data;
            Errors = errors ?? [];
        }

        // Static factory methods for convenience
        public static Result<T> Success(T data, string message = "Request successful", HttpStatusCode httpStatusCode = HttpStatusCode.OK) =>
            new Result<T>(true, message, httpStatusCode, data);

        public static Result<T> Fail(string message = "Fail", List<string>? errors = null, HttpStatusCode httpStatusCode = HttpStatusCode.FailedDependency) =>
            new(true, message, httpStatusCode, default, errors);

        public static Result<T> Created(T data, string message = "Created", HttpStatusCode httpStatusCode = HttpStatusCode.Created) =>
            new(true, message, httpStatusCode, data);

        public static Result<T> NoContent(string message = "No Content", HttpStatusCode httpStatusCode = HttpStatusCode.NoContent) =>
            new(true, message, httpStatusCode);

        public static Result<T> BadRequest(string message = "Bad Request", List<string>? errors = null, HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest) =>
           new(false, message, httpStatusCode, default, errors);

        public static Result<T> Unauthorized(string message = "Unauthorized", HttpStatusCode httpStatusCode = HttpStatusCode.Unauthorized) =>
            new(false, message, httpStatusCode);

        public static Result<T> Forbidden(string message = "Forbidden", HttpStatusCode httpStatusCode = HttpStatusCode.Forbidden) =>
            new(false, message, httpStatusCode);

        public static Result<T> NotFound(string message = "Not Found", HttpStatusCode httpStatusCode = HttpStatusCode.NotFound) =>
            new(false, message, httpStatusCode);

        public static Result<T> InternalServerError(string message = "Internal Server Error", List<string>? errors = null, HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError) =>
            new(false, message, httpStatusCode);
    }
}
