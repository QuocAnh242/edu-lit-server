using AuthService.Application.Abstractions.Messaging;
using AuthService.Application.Abstractions.Messaging.Dispatcher.Interfaces;
using AuthService.Application.DTOs;
using AuthService.Application.DTOs.Request;
using AuthService.Application.DTOs.Response;
using AuthService.Application.Enums;
using AuthService.Application.Exceptions;
using AuthService.Application.Services.Auth.Commands;
using AuthService.Infrastructure.Messaging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AuthService.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController : ControllerBase
    {
        // Command Dispatcher (Commands)
        private readonly ICommandDispatcher _commands;
        //// RabbitMQ Publisher
        private readonly IMessageBusPublisher _messageBusPublisher;
        private readonly IOutbox _outbox;

        public AuthController(
            ICommandDispatcher commands, 
            IMessageBusPublisher messageBusPublisher, 
            IOutbox outbox)
        {
            _commands = commands;
            _messageBusPublisher = messageBusPublisher;
            _outbox = outbox;
        }

        // api/v1/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
        {
            // check model state validations
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<UserDto>.FailureResponse("Invalid input, please try again", 400));
            // Handle Login Process
            try
            {
                // create Login Command from Request
                var cmd = new LoginCommand
                {
                    Username = request.Username,
                    Password = request.Password
                };

                // Send Login Command to Handler
                var response = await _commands.Send<LoginCommand, UserDto>(cmd, ct);

                // Fail Scenario: Publish Login Failed Event
                if (!response.Success)
                {
                    var failPayload = JsonSerializer.Serialize(new
                    {
                        Username = request.Username,
                        Reason = response.Message,
                        errorCode = response.ErrorCode,
                        occurredAtUtc = DateTime.UtcNow
                    });

                    // Publish Event to Message Bus
                    //await _messageBusPublisher.PublishAsync("auth.user.login.failed", failPayload, ct);
                    await _outbox.EnqueueAsync("auth.user.login.failed", failPayload, ct);
                    
                    return Unauthorized(response);
                }

                // Success Scenario: Publish Login Successful Event
                var successPayload = JsonSerializer.Serialize(new
                {
                    user = response.Data,
                    occurredAtUtc = DateTime.UtcNow
                });

                // Publish Event to Message Bus
                // await _messageBusPublisher.PublishAsync("auth.user.login.successful", successPayload, ct);
                await _outbox.EnqueueAsync("auth.user.login.successful", successPayload, ct);

                // Return success response
                return Ok(response);
            }
            catch (AuthException ex)
            {
                var failPayload = JsonSerializer.Serialize(new
                {
                    Username = request.Username,
                    Reason = ex.Message,
                    errorCode = ex.ErrorCode,
                    occurredAtUtc = DateTime.UtcNow
                });

                // Publish Event to Message Bus
                //await _messageBusPublisher.PublishAsync("auth.user.login.failed", failPayload, ct);
                await _outbox.EnqueueAsync("auth.user.login.failed", failPayload, ct);

                // Return failure response
                return Unauthorized(ApiResponse<UserDto>.FailureResponse(ex.Message, (int)ex.ErrorCode));
            }
            catch (Exception ex)
            {
                var errorPayload = JsonSerializer.Serialize(new
                {
                    Username = request.Username,
                    error= "Unexpected error occurred",
                    detail = ex.Message,
                    occurredAtUtc = DateTime.UtcNow
                });

                // Publish Event to Message Bus
                //await _messageBusPublisher.PublishAsync("auth.user.login.failed", errorPayload, ct);
                await _outbox.EnqueueAsync("auth.user.login.failed", errorPayload, ct);

                // return error response
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<UserDto>.FailureResponse("An unexpected error occurred", 500));
            }
        }

        // api/v1/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
        {
            // check model state validations
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<UserDto>.FailureResponse("Invalid input", 400));
            try
            {
                // Mapping from Request into Command
                var cmd = new RegisterCommand
                {
                    Username = request.Username,
                    Email = request.Email,
                    Password = request.Password,
                    FullName = request.FullName
                };

                // Send Register Command to Handler
                var response = await _commands.Send<RegisterCommand, UserDto>(cmd, ct);

                // Fail Scenario: Publish Registration Failed Event
                if (!response.Success || response.Data == null)
                {
                    // Prepare Payload
                    var failedPayload = JsonSerializer.Serialize(new
                    {
                        Username = request.Username,
                        Email = request.Email,
                        Reason = response.Message,
                        errorCode = response.ErrorCode,
                        occurredAtUtc = DateTime.UtcNow
                    });
                    // Publish Event to Message Bus
                    await _outbox.EnqueueAsync("auth.user.registration.failed", failedPayload, ct);
                    
                    // Return Bad Request
                    return BadRequest(response);
                }

                // Note: Event "auth.user.created" is already published by RegisterHandler
                // No need to publish duplicate event here

                // Return Success Response
                return Ok(response);
            }
            catch (AuthException ex)
            {
                // Prepare Payload
                var failPayload = JsonSerializer.Serialize(new
                {
                    Username = request.Username,
                    Email = request.Email,
                    Reason = ex.Message,
                    errorCode = ex.ErrorCode,
                    occurredAtUtc = DateTime.UtcNow
                });
                // Publish Event to Message Bus
                // await _messageBusPublisher.PublishAsync("auth.user.registration.failed", failPayload, ct);
                await _outbox.EnqueueAsync("auth.user.registration.failed", failPayload, ct);

                // Map some common auth error codes
                var status = ex.ErrorCode == AuthErrorCode.UserAlreadyExists
                   ? StatusCodes.Status409Conflict
                   : StatusCodes.Status400BadRequest;

                return StatusCode(status, ApiResponse<UserDto>.FailureResponse(ex.Message, (int)ex.ErrorCode));
            }
            // Catch all unhandled exceptions
            catch (Exception ex)
            {
                var errorPayload = JsonSerializer.Serialize(new
                {
                    Username = request.Username,
                    Email = request.Email,
                    error = "Unhandled error occurred during registration",
                    detail = ex.Message,
                    occurredAtUtc = DateTime.UtcNow
                });

                // Publish Event to Message Bus
                //await _messageBusPublisher.PublishAsync("auth.user.registration.failed", errorPayload, ct);
                await _outbox.EnqueueAsync("auth.user.registration.failed", errorPayload, ct);

                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<UserDto>.FailureResponse("An unexpected error occurred", 500));

            }
        }

        // api/v1/auth/google-login
        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<UserDto>.FailureResponse("Invalid input", 400));

            try
            {
                var cmd = new GoogleLoginCommand
                {
                    IdToken = request.IdToken
                };

                var response = await _commands.Send<GoogleLoginCommand, UserDto>(cmd, ct);

                if (!response.Success)
                {
                    var failPayload = JsonSerializer.Serialize(new
                    {
                        Reason = response.Message,
                        errorCode = response.ErrorCode,
                        occurredAtUtc = DateTime.UtcNow
                    });

                    await _outbox.EnqueueAsync("auth.user.google.login.failed", failPayload, ct);
                    return Unauthorized(response);
                }

                var successPayload = JsonSerializer.Serialize(new
                {
                    user = response.Data,
                    occurredAtUtc = DateTime.UtcNow
                });

                await _outbox.EnqueueAsync("auth.user.google.login.successful", successPayload, ct);
                return Ok(response);
            }
            catch (Exception ex)
            {
                var errorPayload = JsonSerializer.Serialize(new
                {
                    error = "Unexpected error occurred during Google login",
                    detail = ex.Message,
                    occurredAtUtc = DateTime.UtcNow
                });

                await _outbox.EnqueueAsync("auth.user.google.login.failed", errorPayload, ct);
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<UserDto>.FailureResponse("An unexpected error occurred", 500));
            }
        }

        // api/v1/auth/refresh-token
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<UserDto>.FailureResponse("Invalid input", 400));

            try
            {
                var cmd = new RefreshTokenCommand
                {
                    RefreshToken = request.RefreshToken
                };

                var response = await _commands.Send<RefreshTokenCommand, UserDto>(cmd, ct);

                if (!response.Success)
                {
                    return Unauthorized(response);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<UserDto>.FailureResponse("An unexpected error occurred", 500));
            }
        }

        // api/v1/auth/forget-password
        [HttpPost("forget-password")]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordRequest request, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<bool>.FailureResponse("Invalid input", 400));

            try
            {
                var cmd = new ForgetPasswordRequestCommand
                {
                    Email = request.Email
                };

                var response = await _commands.Send<ForgetPasswordRequestCommand, bool>(cmd, ct);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<bool>.FailureResponse("An unexpected error occurred", 500));
            }
        }

        // api/v1/auth/reset-password
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<bool>.FailureResponse("Invalid input", 400));

            try
            {
                var cmd = new ResetPasswordCommand
                {
                    Email = request.Email,
                    OtpCode = request.OtpCode,
                    NewPassword = request.NewPassword
                };

                var response = await _commands.Send<ResetPasswordCommand, bool>(cmd, ct);
                
                if (!response.Success)
                    return BadRequest(response);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<bool>.FailureResponse("An unexpected error occurred", 500));
            }
        }

        // api/v1/auth/change-password
        [HttpPost("change-password")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<bool>.FailureResponse("Invalid input", 400));

            try
            {
                // Get user ID from JWT claims
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)
                                  ?? User.FindFirst("sub")
                                  ?? User.Claims.FirstOrDefault(c => c.Type == "user_id");

                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
                {
                    return Unauthorized(ApiResponse<bool>.FailureResponse("Invalid or missing user token", 401));
                }

                var cmd = new ChangePasswordCommand
                {
                    UserId = userId,
                    OldPassword = request.OldPassword,
                    NewPassword = request.NewPassword
                };

                var response = await _commands.Send<ChangePasswordCommand, bool>(cmd, ct);
                
                if (!response.Success)
                    return BadRequest(response);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<bool>.FailureResponse("An unexpected error occurred", 500));
            }
        }

        // api/v1/auth/profile (PUT - Edit Profile)
        [HttpPut("profile")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<UserDto>.FailureResponse("Invalid input", 400));

            try
            {
                // Get user ID from JWT claims
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)
                                  ?? User.FindFirst("sub")
                                  ?? User.Claims.FirstOrDefault(c => c.Type == "user_id");

                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
                {
                    return Unauthorized(ApiResponse<UserDto>.FailureResponse("Invalid or missing user token", 401));
                }

                var cmd = new UpdateProfileCommand
                {
                    UserId = userId,
                    FullName = request.FullName
                };

                var response = await _commands.Send<UpdateProfileCommand, UserDto>(cmd, ct);
                
                if (!response.Success)
                    return BadRequest(response);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<UserDto>.FailureResponse("An unexpected error occurred", 500));
            }
        }
    }
}
