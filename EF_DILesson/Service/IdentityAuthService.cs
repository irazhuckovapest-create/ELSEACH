using AutoMapper;
using EF_DILesson.Dto;
using EF_DILesson.Models;
using Microsoft.AspNetCore.Identity;

namespace EF_DILesson.Service
{
    public class IdentityAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public IdentityAuthService(UserManager<User> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<RegistrationResult> RegisterUserAsync(UserDto userDto, string password)
        {
            var user = _mapper.Map<User>(userDto);
            //Створення користувача в Identity
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                return new RegistrationResult
                {
                    Success = true,
                    UserId = user.Id,
                    Errors = Array.Empty<string>()
                };
            }
            else
            {
                return new RegistrationResult
                {
                    Success = false,
                    UserId = null,
                    Errors = result.Errors.Select(x => x.Description).ToArray()
                };
            }
        }

        public async Task<bool> ValidateUserCredentialsAsync(UserLoginDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.Login);
            if (user == null)
            {
                return false;
            }
            return await _userManager.CheckPasswordAsync(user, loginDto.Password);
        }
    }
}