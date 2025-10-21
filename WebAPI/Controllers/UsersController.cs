using ApiContracts;
using ForumMini.Entities;
using ForumMini.RepositoryContracts;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Exceptions;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepo;

    public UsersController(IUserRepository userRepo)
    {
        _userRepo = userRepo;
    }

    [HttpPost]
    public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserDto request)
    {
        await VerifyUserNameIsAvailableAsync(request.UserName);

        User user = new User
        {
            Username = request.UserName,
            Password = request.Password
        };

        User created = await _userRepo.AddAsync(user);

        UserDto dto = new UserDto
        {
            Id = created.Id,
            UserName = created.Username
        };

        return Created($"/users/{dto.Id}", dto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<UserDto>> UpdateUser(int id, [FromBody] UpdateUserDto request)
    {
        if (id != request.Id)
        {
            throw new ArgumentException("ID mismatch");
        }

        User existingUser = await _userRepo.GetSingleAsync(id);

        if (existingUser.Username != request.UserName)
        {
            await VerifyUserNameIsAvailableAsync(request.UserName);
        }

        existingUser.Username = request.UserName;
        existingUser.Password = request.Password;

        await _userRepo.UpdateAsync(existingUser);

        UserDto dto = new UserDto
        {
            Id = existingUser.Id,
            UserName = existingUser.Username
        };

        return Ok(dto);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteUser(int id)
    {
        await _userRepo.DeleteAsync(id);
        return NoContent();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUser(int id)
    {
        User user = await _userRepo.GetSingleAsync(id);

        UserDto dto = new UserDto
        {
            Id = user.Id,
            UserName = user.Username
        };

        return Ok(dto);
    }

    [HttpGet]
    public ActionResult<IEnumerable<UserDto>> GetUsers([FromQuery] string? username)
    {
        IQueryable<User> users = _userRepo.GetMany();

        if (!string.IsNullOrWhiteSpace(username))
        {
            users = users.Where(u => u.Username.Contains(username, StringComparison.OrdinalIgnoreCase));
        }

        var userDtos = users.Select(u => new UserDto
        {
            Id = u.Id,
            UserName = u.Username
        }).ToList();

        return Ok(userDtos);
    }

    private async Task VerifyUserNameIsAvailableAsync(string userName)
    {
        var existingUser = _userRepo.GetMany()
            .FirstOrDefault(u => u.Username.ToLower() == userName.ToLower());

        if (existingUser != null)
        {
            throw new InvalidOperationException($"Username '{userName}' is already taken");
        }
    }
}
