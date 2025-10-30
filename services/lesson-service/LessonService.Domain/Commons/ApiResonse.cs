using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LessonService.Domain.Commons
{
    // Non-generic ApiResponse for operations that don't need to return data
    public class ApiResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int? ErrorCode { get; set; }

        public static ApiResponse SuccessResponse(string? message = null)
            => new ApiResponse { Success = true, Message = message };

        public static ApiResponse FailureResponse(string message, int? errorCode = null)
            => new ApiResponse { Success = false, Message = message, ErrorCode = errorCode };
    }

    // Generic ApiResponse for operations that return data
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public T? Data { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int? ErrorCode { get; set; }

        public static ApiResponse<T> SuccessResponse(T data, string? message = null)
            => new ApiResponse<T> { Success = true, Data = data, Message = message};

        public static ApiResponse<T> FailureResponse(string message, int? errorCode = null)
            => new ApiResponse<T> { Success = false, Message = message, ErrorCode = errorCode };
    }
}