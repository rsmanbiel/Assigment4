using ForumMini.Entities;
using ForumMini.RepositoryContracts;

namespace ForumMini.InMemoryRepositories;

/// <summary>
/// In-memory implementation of IPostRepository
/// </summary>
public class PostInMemoryRepository : IPostRepository
{
    private readonly List<Post> _posts = new();

    public PostInMemoryRepository()
    {
        // Seed data
        _posts.Add(new Post { Id = 1, Title = "Welcome to the forum!", Body = "This is our first post.", UserId = 1 });
        _posts.Add(new Post { Id = 2, Title = "Second post", Body = "Just testing things out.", UserId = 2 });
        _posts.Add(new Post { Id = 3, Title = "Another post", Body = "Content is king.", UserId = 1 });
    }

    public Task<Post> AddAsync(Post post)
    {
        post.Id = _posts.Any()
            ? _posts.Max(p => p.Id) + 1
            : 1;
        _posts.Add(post);
        return Task.FromResult(post);
    }

    public Task UpdateAsync(Post post)
    {
        Post? existingPost = _posts.SingleOrDefault(p => p.Id == post.Id);
        if (existingPost is null)
        {
            throw new InvalidOperationException($"Post with ID '{post.Id}' not found");
        }

        _posts.Remove(existingPost);
        _posts.Add(post);

        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        Post? postToRemove = _posts.SingleOrDefault(p => p.Id == id);
        if (postToRemove is null)
        {
            throw new InvalidOperationException($"Post with ID '{id}' not found");
        }

        _posts.Remove(postToRemove);
        return Task.CompletedTask;
    }

    public Task<Post> GetSingleAsync(int id)
    {
        Post? post = _posts.SingleOrDefault(p => p.Id == id);
        if (post is null)
        {
            throw new InvalidOperationException($"Post with ID '{id}' not found");
        }
        return Task.FromResult(post);
    }

    public IQueryable<Post> GetMany()
    {
        return _posts.AsQueryable();
    }
}
