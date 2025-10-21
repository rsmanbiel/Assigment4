using ForumMini.Entities;
using ForumMini.RepositoryContracts;

namespace ForumMini.InMemoryRepositories;

/// <summary>
/// In-memory implementation of IUserRepository
/// </summary>
public class UserInMemoryRepository : IUserRepository
{
    private readonly List<User> _users = new();

    public UserInMemoryRepository()
    {
        // Seed data
        _users.Add(new User { Id = 1, Username = "alice", Password = "password123" });
        _users.Add(new User { Id = 2, Username = "bob", Password = "password123" });
        _users.Add(new User { Id = 3, Username = "charlie", Password = "password123" });
    }

    public Task<User> AddAsync(User user)
    {
        user.Id = _users.Any()
            ? _users.Max(u => u.Id) + 1
            : 1;
        _users.Add(user);
        return Task.FromResult(user);
    }

    public Task UpdateAsync(User user)
    {
        User? existingUser = _users.SingleOrDefault(u => u.Id == user.Id);
        if (existingUser is null)
        {
            throw new InvalidOperationException($"User with ID '{user.Id}' not found");
        }

        _users.Remove(existingUser);
        _users.Add(user);

        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        User? userToRemove = _users.SingleOrDefault(u => u.Id == id);
        if (userToRemove is null)
        {
            throw new InvalidOperationException($"User with ID '{id}' not found");
        }

        _users.Remove(userToRemove);
        return Task.CompletedTask;
    }

    public Task<User> GetSingleAsync(int id)
    {
        User? user = _users.SingleOrDefault(u => u.Id == id);
        if (user is null)
        {
            throw new InvalidOperationException($"User with ID '{id}' not found");
        }
        return Task.FromResult(user);
    }

    public IQueryable<User> GetMany()
    {
        return _users.AsQueryable();
    }
}
