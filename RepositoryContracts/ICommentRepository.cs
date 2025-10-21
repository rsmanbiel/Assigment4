using System.Linq;
using ForumMini.Entities;

namespace ForumMini.RepositoryContracts;

/// <summary>
/// Repository interface for Comment entities
/// </summary>
public interface ICommentRepository
{
    /// <summary>
    /// Adds a new comment asynchronously
    /// </summary>
    /// <param name="comment">The comment to add</param>
    /// <returns>The added comment with assigned ID</returns>
    Task<Comment> AddAsync(Comment comment);

    /// <summary>
    /// Updates an existing comment asynchronously
    /// </summary>
    /// <param name="comment">The comment to update</param>
    Task UpdateAsync(Comment comment);

    /// <summary>
    /// Deletes a comment by ID asynchronously
    /// </summary>
    /// <param name="id">The ID of the comment to delete</param>
    Task DeleteAsync(int id);

    /// <summary>
    /// Gets a single comment by ID asynchronously
    /// </summary>
    /// <param name="id">The ID of the comment to retrieve</param>
    /// <returns>The comment with the specified ID</returns>
    Task<Comment> GetSingleAsync(int id);

    /// <summary>
    /// Gets all comments as a queryable collection
    /// </summary>
    /// <returns>An IQueryable of all comments</returns>
    IQueryable<Comment> GetMany();
}