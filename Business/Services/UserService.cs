﻿using Business.Models;
using Data.Entitites;
using Data.Repositories;
using Domain.Extensions;
using Domain.Models;
using Domain.Responses;
using Microsoft.AspNetCore.Identity;

namespace Business.Services;

public interface IUserService
{
    Task<UserResult> AddUserToRoleAsync(UserEntity user, string roleName);
    Task<string> GetDisplayName(string userId);
    Task<UserResult<User>> GetUserByIdAsync(string id);
    Task<UserResult<IEnumerable<User>>> GetUsersAsync();
    Task<UserResult> UserExistsByEmailAsync(string email);
}

public class UserService(IUserRepository userRepository, UserManager<UserEntity> userManager, RoleManager<IdentityRole> roleManager) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly UserManager<UserEntity> _userManager = userManager;
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;


    public async Task<UserResult<IEnumerable<User>>> GetUsersAsync()
    {
        var repositoryResult = await _userRepository.GetAllAsync();

        // Removed '.Result' as 'repositoryResult' is already the IEnumerable<UserEntity>
        var entities = repositoryResult;
        var users = entities?.OrderBy(x => x.FirstName).Select(entity => entity.MapTo<User>()) ?? Enumerable.Empty<User>();

        return new UserResult<IEnumerable<User>> { Succeeded = true, StatusCode = 200, Result = users };
    }
    public async Task<UserResult<User>> GetUserByIdAsync(string id)
    {
        var entity = await _userRepository.GetAsync(x => x.Id == id);

        if (entity == null)
            return new UserResult<User> { Succeeded = false, StatusCode = 404, Error = $"User with id '{id}' was not found." };

        var user = entity.MapTo<User>();
        return new UserResult<User> { Succeeded = true, StatusCode = 200, Result = user };
    }
    public async Task<UserResult> UserExistsByEmailAsync(string email)
    {
        var exists = await _userRepository.ExistsAsync(x => x.Email == email);
        if (exists)
            return new UserResult { Succeeded = true, StatusCode = 200, Error = "A user with the specified email address exists." };

        return new UserResult { Succeeded = false, StatusCode = 404, Error = "User was not found." };
    }
    public async Task<UserResult> AddUserToRoleAsync(UserEntity user, string roleName)
    {
        if (await _roleManager.RoleExistsAsync(roleName))
        {
            await _userManager.AddToRoleAsync(user, roleName);
            return new UserResult { Succeeded = true, StatusCode = 200 };
        }

        return new UserResult { Succeeded = false };

    }

    public async Task<string> GetDisplayName(string userId)
    {
        if (string.IsNullOrEmpty(userId))
            return "";

        var user = await _userManager.FindByIdAsync(userId);
        return user == null ? "" : $"{user.FirstName} {user.LastName}";
    }
}
