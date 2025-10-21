using ApiContracts;
using ForumMini.Entities;
using ForumMini.RepositoryContracts;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class PostsController : ControllerBase
{
    private readonly IPostRepository _postRepo;
    private readonly IUserRepository _userRepo;
    private readonly ICommentRepository _commentRepo;

    public PostsController(IPostRepository postRepo, IUserRepository userRepo, ICommentRepository commentRepo)
    {
        _postRepo = postRepo;
        _userRepo = userRepo;
        _commentRepo = commentRepo;
    }

    [HttpPost]
    public async Task<ActionResult<PostDto>> CreatePost([FromBody] CreatePostDto request)
    {
        User user = await _userRepo.GetSingleAsync(request.UserId);

        Post post = new Post
        {
            Title = request.Title,
            Body = request.Body,
            UserId = request.UserId
        };

        Post created = await _postRepo.AddAsync(post);

        PostDto dto = new PostDto
        {
            Id = created.Id,
            Title = created.Title,
            Body = created.Body,
            UserId = created.UserId,
            UserName = user.Username
        };

        return Created($"/posts/{dto.Id}", dto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<PostDto>> UpdatePost(int id, [FromBody] UpdatePostDto request)
    {
        if (id != request.Id)
        {
            throw new ArgumentException("ID mismatch");
        }

        Post existingPost = await _postRepo.GetSingleAsync(id);
        User user = await _userRepo.GetSingleAsync(existingPost.UserId);

        existingPost.Title = request.Title;
        existingPost.Body = request.Body;

        await _postRepo.UpdateAsync(existingPost);

        PostDto dto = new PostDto
        {
            Id = existingPost.Id,
            Title = existingPost.Title,
            Body = existingPost.Body,
            UserId = existingPost.UserId,
            UserName = user.Username
        };

        return Ok(dto);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeletePost(int id)
    {
        await _postRepo.DeleteAsync(id);
        return NoContent();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PostDto>> GetPost(int id, [FromQuery] bool includeComments = false)
    {
        Post post = await _postRepo.GetSingleAsync(id);
        User user = await _userRepo.GetSingleAsync(post.UserId);

        PostDto dto = new PostDto
        {
            Id = post.Id,
            Title = post.Title,
            Body = post.Body,
            UserId = post.UserId,
            UserName = user.Username
        };

        if (includeComments)
        {
            var comments = _commentRepo.GetMany()
                .Where(c => c.PostId == id)
                .ToList();

            dto.Comments = new List<CommentDto>();
            foreach (var comment in comments)
            {
                var commentUser = await _userRepo.GetSingleAsync(comment.UserId);
                dto.Comments.Add(new CommentDto
                {
                    Id = comment.Id,
                    Body = comment.Body,
                    UserId = comment.UserId,
                    UserName = commentUser.Username,
                    PostId = comment.PostId
                });
            }
        }

        return Ok(dto);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PostDto>>> GetPosts(
        [FromQuery] string? title,
        [FromQuery] int? userId,
        [FromQuery] string? userName)
    {
        IQueryable<Post> posts = _postRepo.GetMany();

        if (!string.IsNullOrWhiteSpace(title))
        {
            posts = posts.Where(p => p.Title.Contains(title, StringComparison.OrdinalIgnoreCase));
        }

        if (userId.HasValue)
        {
            posts = posts.Where(p => p.UserId == userId.Value);
        }

        var postList = posts.ToList();
        var postDtos = new List<PostDto>();

        foreach (var post in postList)
        {
            User user = await _userRepo.GetSingleAsync(post.UserId);

            if (!string.IsNullOrWhiteSpace(userName) && 
                !user.Username.Contains(userName, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            postDtos.Add(new PostDto
            {
                Id = post.Id,
                Title = post.Title,
                Body = post.Body,
                UserId = post.UserId,
                UserName = user.Username
            });
        }

        return Ok(postDtos);
    }
}
