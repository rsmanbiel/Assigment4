using System.Linq;
using ForumMini.Entities;

namespace ForumMini.RepositoryContracts;

/// <summary>
/// Repository interface for User entities
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Adds a new user asynchronously
    /// </summary>
    /// <param name="user">The user to add</param>
    /// <returns>The added user with assigned ID</returns>
    Task<User> AddAsync(User user);

    /// <summary>
    /// Updates an existing user asynchronously
    /// </summary>
    /// <param name="user">The user to update</param>
    Task UpdateAsync(User user);

    /// <summary>
    /// Deletes a user by ID asynchronously
    /// </summary>
    /// <param name="id">The ID of the user to delete</param>
    Task DeleteAsync(int id);

    /// <summary>
    /// Gets a single user by ID asynchronously
    /// </summary>
    /// <param name="id">The ID of the user to retrieve</param>
    /// <returns>The user with the specified ID</returns>
    Task<User> GetSingleAsync(int id);

    /// <summary>
    /// Gets all users as a queryable collection
    /// </summary>
    /// <returns>An IQueryable of all users</returns>
    IQueryable<User> GetMany();
}
