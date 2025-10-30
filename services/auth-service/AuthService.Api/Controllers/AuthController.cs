using AuthService.Application.Abstractions.Messaging;
using AuthService.Application.Abstractions.Messaging.Dispatcher.Interfaces;
using AuthService.Application.DTOs;
using AuthService.Application.DTOs.Request;
using AuthService.Application.DTOs.Response;
using AuthService.Application.Enums;
using AuthService.Application.Exceptions;
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

        public AuthController(ICommandDispatcher commands, IMessageBusPublisher messageBusPublisher, IOutbox outbox)
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
                    //await _messageBusPublisher.PublishAsync("auth.user.registration.failed", failedPayload, ct);
                    await _outbox.EnqueueAsync("auth.user.registration.failed", failedPayload, ct);
                    
                    // Return Bad Request
                    return BadRequest(response);
                }

                var successPayload = JsonSerializer.Serialize(new
                {
                    user = response.Data,
                    occurredAtUtc = DateTime.UtcNow
                });

                // Publish Event to Message Bus
                //await _messageBusPublisher.PublishAsync("auth.user.registration.successful", successPayload, ct);
                await _outbox.EnqueueAsync("auth.user.registration.successful", successPayload, ct);

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

                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<UserDto>.FailureResponse("An Unexpected error occurred", 500));

            }
        }
    }
}
