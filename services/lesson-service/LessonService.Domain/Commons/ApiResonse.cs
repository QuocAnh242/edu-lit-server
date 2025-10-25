using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LessonService.Domain.Commons
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
        public int? StatusCode { get; set; }

        public static ApiResponse<T> SuccessResponse(T data, string? message = null, int? statusCode = null)
            => new ApiResponse<T> { Success = true, Data = data, Message = message, StatusCode = statusCode};

        public static ApiResponse<T> FailureResponse(string message, int? statusCode = null)
            => new ApiResponse<T> { Success = false, Message = message, StatusCode = statusCode };
    }
}