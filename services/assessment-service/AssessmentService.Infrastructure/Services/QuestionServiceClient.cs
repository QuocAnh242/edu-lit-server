using AssessmentService.Application.DTOs;
using AssessmentService.Application.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text.Json;

namespace AssessmentService.Infrastructure.Services
{
    public class QuestionServiceClient : IQuestionServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly JsonSerializerOptions _jsonOptions;

        public QuestionServiceClient(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var baseUrl = _configuration["QuestionService:BaseUrl"] ?? "http://localhost:8003";
            _httpClient.BaseAddress = new Uri(baseUrl);
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        private HttpRequestMessage CreateRequestWithAuth(HttpMethod method, string uri)
        {
            var request = new HttpRequestMessage(method, uri);
            
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                var authHeader = httpContext.Request.Headers["Authorization"].FirstOrDefault();
                if (!string.IsNullOrEmpty(authHeader))
                {
                    request.Headers.Authorization = AuthenticationHeaderValue.Parse(authHeader);
                }
            }
            
            return request;
        }

        public async Task<QuestionDto?> GetQuestionByIdAsync(Guid questionId, CancellationToken cancellationToken = default)
        {
            try
            {
                var request = CreateRequestWithAuth(HttpMethod.Get, $"/api/v1/question/{questionId}");
                var response = await _httpClient.SendAsync(request, cancellationToken);
                
                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                var apiResponse = JsonSerializer.Deserialize<QuestionServiceResponse<QuestionDto>>(content, _jsonOptions);

                if (apiResponse?.Success == true && apiResponse.Data != null)
                {
                    return apiResponse.Data;
                }

                return null;
            }
            catch (Exception)
            {
                // Log error if needed
                return null;
            }
        }

        public async Task<List<QuestionOptionDto>> GetQuestionOptionsByQuestionIdAsync(Guid questionId, CancellationToken cancellationToken = default)
        {
            try
            {
                var request = CreateRequestWithAuth(HttpMethod.Get, $"/api/v1/questionoption/question/{questionId}");
                var response = await _httpClient.SendAsync(request, cancellationToken);
                
                if (!response.IsSuccessStatusCode)
                {
                    return new List<QuestionOptionDto>();
                }

                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                
                // Question Service might return List<QuestionOptionDto> directly or wrapped in ApiResponse
                // Try to deserialize as ApiResponse first, if fails try direct list
                try
                {
                    var apiResponse = JsonSerializer.Deserialize<QuestionServiceResponse<List<QuestionOptionDto>>>(content, _jsonOptions);
                    if (apiResponse?.Success == true && apiResponse.Data != null)
                    {
                        return apiResponse.Data;
                    }
                }
                catch
                {
                    // Try direct list deserialization
                    var directList = JsonSerializer.Deserialize<List<QuestionOptionDto>>(content, _jsonOptions);
                    if (directList != null)
                    {
                        return directList;
                    }
                }

                return new List<QuestionOptionDto>();
            }
            catch (Exception)
            {
                // Log error if needed
                return new List<QuestionOptionDto>();
            }
        }

        public async Task<bool> ValidateQuestionIdsAsync(List<Guid> questionIds, CancellationToken cancellationToken = default)
        {
            if (questionIds == null || !questionIds.Any())
            {
                return false;
            }

            // Validate each question ID by calling the API
            var validationTasks = questionIds.Select(async questionId =>
            {
                var question = await GetQuestionByIdAsync(questionId, cancellationToken);
                return question != null;
            });

            var results = await Task.WhenAll(validationTasks);
            return results.All(r => r);
        }
    }
}

