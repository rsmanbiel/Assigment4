using System.Linq;
using ForumMini.Entities;

namespace ForumMini.RepositoryContracts;

/// <summary>
/// Repository interface for Post entities
/// </summary>
public interface IPostRepository
{
    /// <summary>
    /// Adds a new post asynchronously
    /// </summary>
    /// <param name="post">The post to add</param>
    /// <returns>The added post with assigned ID</returns>
    Task<Post> AddAsync(Post post);

    /// <summary>
    /// Updates an existing post asynchronously
    /// </summary>
    /// <param name="post">The post to update</param>
    Task UpdateAsync(Post post);

    /// <summary>
    /// Deletes a post by ID asynchronously
    /// </summary>
    /// <param name="id">The ID of the post to delete</param>
    Task DeleteAsync(int id);

    /// <summary>
    /// Gets a single post by ID asynchronously
    /// </summary>
    /// <param name="id">The ID of the post to retrieve</param>
    /// <returns>The post with the specified ID</returns>
    Task<Post> GetSingleAsync(int id);

    /// <summary>
    /// Gets all posts as a queryable collection
    /// </summary>
    /// <returns>An IQueryable of all posts</returns>
    IQueryable<Post> GetMany();
}
