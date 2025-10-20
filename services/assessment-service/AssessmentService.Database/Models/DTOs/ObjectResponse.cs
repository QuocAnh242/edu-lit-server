namespace AssessmentService.Database.Models.DTOs
{
    public class ObjectResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
        public int? ErrorCode { get; set; }

        public static ObjectResponse<T> SuccessResponse(T data, string? message = null)
            => new ObjectResponse<T> { Success = true, Data = data, Message = message };

        public static ObjectResponse<T> FailureResponse(string message, int? errorCode = null)
            => new ObjectResponse<T> { Success = false, Message = message, ErrorCode = errorCode };
    }
}
