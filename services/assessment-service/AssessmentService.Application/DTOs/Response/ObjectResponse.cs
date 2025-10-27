namespace AssessmentService.Application.DTOs.Response
{
    public class ObjectResponse<T>
    {
        public string ErrorCode { get; set; }
        public string Message { get; set; }
        public T? Data { get; set; }
        public static ObjectResponse<T> Response(string errorCode, string message, T? data)
            => new ObjectResponse<T>
            {
                ErrorCode = errorCode,
                Message = message,
                Data = data
            };
        public static ObjectResponse<T> SuccessResponse(T data)
            => new ObjectResponse<T>
            {
                ErrorCode = "200",
                Message = "success",
                Data = data
            };
        public static ObjectResponse<T> FailureResponse(Exception e)
            => new ObjectResponse<T>
            {
                ErrorCode = "500",
                Message = "Internal server error: " + e,
                Data = default
            };
    }
}
