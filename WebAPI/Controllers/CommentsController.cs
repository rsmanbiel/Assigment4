using ApiContracts;
using ForumMini.Entities;
using ForumMini.RepositoryContracts;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class CommentsController : ControllerBase
{
    private readonly ICommentRepository _commentRepo;
    private readonly IUserRepository _userRepo;
    private readonly IPostRepository _postRepo;

    public CommentsController(ICommentRepository commentRepo, IUserRepository userRepo, IPostRepository postRepo)
    {
        _commentRepo = commentRepo;
        _userRepo = userRepo;
        _postRepo = postRepo;
    }

    [HttpPost]
    public async Task<ActionResult<CommentDto>> CreateComment([FromBody] CreateCommentDto request)
    {
        User user = await _userRepo.GetSingleAsync(request.UserId);
        await _postRepo.GetSingleAsync(request.PostId);

        Comment comment = new Comment
        {
            Body = request.Body,
            UserId = request.UserId,
            PostId = request.PostId
        };

        Comment created = await _commentRepo.AddAsync(comment);

        CommentDto dto = new CommentDto
        {
            Id = created.Id,
            Body = created.Body,
            UserId = created.UserId,
            UserName = user.Username,
            PostId = created.PostId
        };

        return Created($"/comments/{dto.Id}", dto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<CommentDto>> UpdateComment(int id, [FromBody] UpdateCommentDto request)
    {
        if (id != request.Id)
        {
            throw new ArgumentException("ID mismatch");
        }

        Comment existingComment = await _commentRepo.GetSingleAsync(id);
        User user = await _userRepo.GetSingleAsync(existingComment.UserId);

        existingComment.Body = request.Body;

        await _commentRepo.UpdateAsync(existingComment);

        CommentDto dto = new CommentDto
        {
            Id = existingComment.Id,
            Body = existingComment.Body,
            UserId = existingComment.UserId,
            UserName = user.Username,
            PostId = existingComment.PostId
        };

        return Ok(dto);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteComment(int id)
    {
        await _commentRepo.DeleteAsync(id);
        return NoContent();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CommentDto>> GetComment(int id)
    {
        Comment comment = await _commentRepo.GetSingleAsync(id);
        User user = await _userRepo.GetSingleAsync(comment.UserId);

        CommentDto dto = new CommentDto
        {
            Id = comment.Id,
            Body = comment.Body,
            UserId = comment.UserId,
            UserName = user.Username,
            PostId = comment.PostId
        };

        return Ok(dto);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CommentDto>>> GetComments(
        [FromQuery] int? userId,
        [FromQuery] string? userName,
        [FromQuery] int? postId)
    {
        IQueryable<Comment> comments = _commentRepo.GetMany();

        if (userId.HasValue)
        {
            comments = comments.Where(c => c.UserId == userId.Value);
        }

        if (postId.HasValue)
        {
            comments = comments.Where(c => c.PostId == postId.Value);
        }

        var commentList = comments.ToList();
        var commentDtos = new List<CommentDto>();

        foreach (var comment in commentList)
        {
            User user = await _userRepo.GetSingleAsync(comment.UserId);

            if (!string.IsNullOrWhiteSpace(userName) && 
                !user.Username.Contains(userName, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            commentDtos.Add(new CommentDto
            {
                Id = comment.Id,
                Body = comment.Body,
                UserId = comment.UserId,
                UserName = user.Username,
                PostId = comment.PostId
            });
        }

        return Ok(commentDtos);
    }
}
