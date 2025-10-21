using System.Net;

namespace AssessmentService.Service.Errors
{
    public class ApiError
    {
        public string ErrorCode { get; set; }
        public int ErrorStatus { get; set; }
        public string Message { get; set; }
        public ApiError(string errorCode, int errorStatus, string message)
        {
            ErrorCode = errorCode;
            ErrorStatus = errorStatus;
            Message = message;
        }
        private static readonly Dictionary<int, ApiError> _errorInfos = new()
        {
            {400, new ApiError("HB40001", 400, "Missing/invalid input") },
            {401, new ApiError("HB40101", 401, "Token missing/invalid") },
            {403, new ApiError("HB40301", 403, "Permission denied") },
            {404, new ApiError("HB40401", 404, "Resource not found") },
            {500, new ApiError("HB50001", 500, "Internal server error") }
        };

        public static ApiError GetErrorInfo(int errorCode)
        {
            return _errorInfos.TryGetValue(errorCode, out var errorInfo)
            ? errorInfo // exception null with auto return new error 500 below
            : new ApiError("HB50001", 500, "Internal server error");
        }
    }
}
