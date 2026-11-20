using AutoMapper;
using EF_DILesson.Contracts;
using EF_DILesson.Dto;
using EF_DILesson.Service;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace EF_DILesson.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IdentityAuthService _identityAuthService;
        private readonly IMapper mapper;
        private readonly IValidator<IdentityLoginRequest> _loginValidator;
        private readonly IValidator<IdentityRegisterRequest> _registerValidator;

        public AuthController(IdentityAuthService identityAuthService, IMapper mapper,
            IValidator<IdentityLoginRequest> loginValidator,
            IValidator<IdentityRegisterRequest> registerValidator)
        {
            _identityAuthService = identityAuthService;
            this.mapper = mapper;
            _loginValidator = loginValidator;
            _registerValidator = registerValidator;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] IdentityRegisterRequest data)
        {
            var validationResult = _registerValidator.Validate(data);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var userDto = mapper.Map<UserDto>(data);
            var result = await _identityAuthService.RegisterUserAsync(userDto, data.Password);

            if (!result.Success) return BadRequest(result.Errors);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<ActionResult<bool>> Login([FromBody] IdentityLoginRequest data)
        {
            var validationResult = _loginValidator.Validate(data);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var loginDto = mapper.Map<UserLoginDto>(data);
            var isValid = await _identityAuthService.ValidateUserCredentialsAsync(loginDto);
            return Ok(isValid);
        }
    }
}