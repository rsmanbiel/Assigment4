using ForumMini.Entities;
using ForumMini.RepositoryContracts;

namespace ForumMini.InMemoryRepositories;

/// <summary>
/// In-memory implementation of ICommentRepository
/// </summary>
public class CommentInMemoryRepository : ICommentRepository
{
    private readonly List<Comment> _comments = new();

    public CommentInMemoryRepository()
    {
        // Seed data
        _comments.Add(new Comment { Id = 1, Body = "Great first post!", UserId = 2, PostId = 1 });
        _comments.Add(new Comment { Id = 2, Body = "I agree completely.", UserId = 3, PostId = 1 });
        _comments.Add(new Comment { Id = 3, Body = "Thanks for sharing.", UserId = 1, PostId = 2 });
        _comments.Add(new Comment { Id = 4, Body = "Interesting perspective.", UserId = 2, PostId = 3 });
    }

    public Task<Comment> AddAsync(Comment comment)
    {
        comment.Id = _comments.Any()
            ? _comments.Max(c => c.Id) + 1
            : 1;
        _comments.Add(comment);
        return Task.FromResult(comment);
    }

    public Task UpdateAsync(Comment comment)
    {
        Comment? existingComment = _comments.SingleOrDefault(c => c.Id == comment.Id);
        if (existingComment is null)
        {
            throw new InvalidOperationException($"Comment with ID '{comment.Id}' not found");
        }

        _comments.Remove(existingComment);
        _comments.Add(comment);

        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        Comment? commentToRemove = _comments.SingleOrDefault(c => c.Id == id);
        if (commentToRemove is null)
        {
            throw new InvalidOperationException($"Comment with ID '{id}' not found");
        }

        _comments.Remove(commentToRemove);
        return Task.CompletedTask;
    }

    public Task<Comment> GetSingleAsync(int id)
    {
        Comment? comment = _comments.SingleOrDefault(c => c.Id == id);
        if (comment is null)
        {
            throw new InvalidOperationException($"Comment with ID '{id}' not found");
        }
        return Task.FromResult(comment);
    }

    public IQueryable<Comment> GetMany()
    {
        return _comments.AsQueryable();
    }
}