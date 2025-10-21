using ForumMini.RepositoryContracts;

namespace CLI.UI;

public class CliApp
{
    private readonly IUserRepository _userRepository;
    private readonly IPostRepository _postRepository;
    private readonly ICommentRepository _commentRepository;
    private bool _isRunning = true;

    public CliApp(IUserRepository userRepository, IPostRepository postRepository, ICommentRepository commentRepository)
    {
        _userRepository = userRepository;
        _postRepository = postRepository;
        _commentRepository = commentRepository;
    }

    public async Task StartAsync()
    {
        Console.WriteLine("\nWelcome to ForumMini CLI!");
        Console.WriteLine("Type 'help' to see available commands.\n");

        while (_isRunning)
        {
            Console.Write("ForumMini> ");
            string? input = Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(input))
                continue;

            await ProcessCommandAsync(input.Trim().ToLower());
        }
    }

    private async Task ProcessCommandAsync(string command)
    {
        try
        {
            switch (command)
            {
                case "help":
                    ShowHelp();
                    break;
                case "1":
                case "create-user":
                    await CreateUserAsync();
                    break;
                case "2":
                case "create-post":
                    await CreatePostAsync();
                    break;
                case "3":
                case "add-comment":
                    await AddCommentAsync();
                    break;
                case "4":
                case "list-posts":
                    await ListPostsAsync();
                    break;
                case "5":
                case "view-post":
                    await ViewPostAsync();
                    break;
                case "6":
                case "list-users":
                    await ListUsersAsync();
                    break;
                case "7":
                case "list-comments":
                    await ListCommentsAsync();
                    break;
                case "clear":
                    Console.Clear();
                    break;
                case "exit":
                case "quit":
                    _isRunning = false;
                    Console.WriteLine("Goodbye!");
                    break;
                default:
                    Console.WriteLine("Unknown command. Type 'help' for available commands.");
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private void ShowHelp()
    {
        Console.WriteLine("\n=== AVAILABLE COMMANDS ===");
        Console.WriteLine("1 or create-user   - Create a new user");
        Console.WriteLine("2 or create-post   - Create a new post");
        Console.WriteLine("3 or add-comment   - Add comment to a post");
        Console.WriteLine("4 or list-posts    - View all posts overview");
        Console.WriteLine("5 or view-post     - View specific post with comments");
        Console.WriteLine("6 or list-users    - View all users");
        Console.WriteLine("7 or list-comments - View all comments");
        Console.WriteLine("clear              - Clear the screen");
        Console.WriteLine("help               - Show this help message");
        Console.WriteLine("exit or quit       - Exit the application");
        Console.WriteLine("==========================\n");
    }

    private async Task CreateUserAsync()
    {
        Console.WriteLine("\n=== CREATE NEW USER ===");
        
        Console.Write("Enter username: ");
        string? username = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(username))
        {
            Console.WriteLine("Username cannot be empty.");
            return;
        }

        // Check if username already exists
        var existingUsers = _userRepository.GetMany().Where(u => u.Username.ToLower() == username.ToLower());
        if (existingUsers.Any())
        {
            Console.WriteLine("Username already exists. Please choose a different username.");
            return;
        }

        Console.Write("Enter password: ");
        string? password = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(password))
        {
            Console.WriteLine("Password cannot be empty.");
            return;
        }

        var newUser = new ForumMini.Entities.User
        {
            Username = username,
            Password = password
        };

        var createdUser = await _userRepository.AddAsync(newUser);
        Console.WriteLine($"User created successfully! ID: {createdUser.Id}, Username: {createdUser.Username}\n");
    }

    private async Task CreatePostAsync()
    {
        Console.WriteLine("\n=== CREATE NEW POST ===");
        
        // Show available users
        var users = _userRepository.GetMany().ToList();
        if (!users.Any())
        {
            Console.WriteLine("No users available. Please create a user first.");
            return;
        }

        Console.WriteLine("Available users:");
        foreach (var user in users)
        {
            Console.WriteLine($"  ID: {user.Id} - {user.Username}");
        }

        Console.Write("Enter user ID: ");
        if (!int.TryParse(Console.ReadLine(), out int userId))
        {
            Console.WriteLine("Invalid user ID.");
            return;
        }

        // Verify user exists
        try
        {
            await _userRepository.GetSingleAsync(userId);
        }
        catch
        {
            Console.WriteLine("User not found.");
            return;
        }

        Console.Write("Enter post title: ");
        string? title = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(title))
        {
            Console.WriteLine("Title cannot be empty.");
            return;
        }

        Console.Write("Enter post body: ");
        string? body = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(body))
        {
            Console.WriteLine("Body cannot be empty.");
            return;
        }

        var newPost = new ForumMini.Entities.Post
        {
            Title = title,
            Body = body,
            UserId = userId
        };

        var createdPost = await _postRepository.AddAsync(newPost);
        Console.WriteLine($"Post created successfully! ID: {createdPost.Id}, Title: '{createdPost.Title}'\n");
    }

    private async Task AddCommentAsync()
    {
        Console.WriteLine("\n=== ADD COMMENT ===");
        
        // Show available posts
        var posts = _postRepository.GetMany().ToList();
        if (!posts.Any())
        {
            Console.WriteLine("No posts available. Please create a post first.");
            return;
        }

        Console.WriteLine("Available posts:");
        foreach (var post in posts)
        {
            var author = await _userRepository.GetSingleAsync(post.UserId);
            Console.WriteLine($"  ID: {post.Id} - '{post.Title}' by {author.Username}");
        }

        Console.Write("Enter post ID: ");
        if (!int.TryParse(Console.ReadLine(), out int postId))
        {
            Console.WriteLine("Invalid post ID.");
            return;
        }

        // Verify post exists
        try
        {
            await _postRepository.GetSingleAsync(postId);
        }
        catch
        {
            Console.WriteLine("Post not found.");
            return;
        }

        // Show available users
        var users = _userRepository.GetMany().ToList();
        Console.WriteLine("\nAvailable users:");
        foreach (var user in users)
        {
            Console.WriteLine($"  ID: {user.Id} - {user.Username}");
        }

        Console.Write("Enter user ID: ");
        if (!int.TryParse(Console.ReadLine(), out int userId))
        {
            Console.WriteLine("Invalid user ID.");
            return;
        }

        // Verify user exists
        try
        {
            await _userRepository.GetSingleAsync(userId);
        }
        catch
        {
            Console.WriteLine("User not found.");
            return;
        }

        Console.Write("Enter comment: ");
        string? body = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(body))
        {
            Console.WriteLine("Comment cannot be empty.");
            return;
        }

        var newComment = new ForumMini.Entities.Comment
        {
            Body = body,
            UserId = userId,
            PostId = postId
        };

        var createdComment = await _commentRepository.AddAsync(newComment);
        Console.WriteLine($"Comment added successfully! ID: {createdComment.Id}\n");
    }

    private async Task ListPostsAsync()
    {
        Console.WriteLine("\n=== POSTS OVERVIEW ===");
        var posts = _postRepository.GetMany().ToList();
        
        if (!posts.Any())
        {
            Console.WriteLine("No posts found.");
            return;
        }

        foreach (var post in posts)
        {
            var author = await _userRepository.GetSingleAsync(post.UserId);
            Console.WriteLine($"[{post.Id}] {post.Title} - by {author.Username}");
        }
        Console.WriteLine();
    }

    private async Task ViewPostAsync()
    {
        Console.WriteLine("\n=== VIEW POST ===");
        
        var posts = _postRepository.GetMany().ToList();
        if (!posts.Any())
        {
            Console.WriteLine("No posts available.");
            return;
        }

        Console.WriteLine("Available posts:");
        foreach (var p in posts)
        {
            Console.WriteLine($"  [{p.Id}] {p.Title}");
        }

        Console.Write("Enter post ID: ");
        if (!int.TryParse(Console.ReadLine(), out int postId))
        {
            Console.WriteLine("Invalid post ID.");
            return;
        }

        try
        {
            var post = await _postRepository.GetSingleAsync(postId);
            var author = await _userRepository.GetSingleAsync(post.UserId);
            
            Console.WriteLine($"\n--- POST #{post.Id} ---");
            Console.WriteLine($"Title: {post.Title}");
            Console.WriteLine($"Author: {author.Username}");
            Console.WriteLine($"Body: {post.Body}");

            // Show comments
            var comments = _commentRepository.GetMany().Where(c => c.PostId == postId).ToList();
            
            if (comments.Any())
            {
                Console.WriteLine("\n--- COMMENTS ---");
                foreach (var comment in comments)
                {
                    var commentAuthor = await _userRepository.GetSingleAsync(comment.UserId);
                    Console.WriteLine($"[{comment.Id}] {commentAuthor.Username}: {comment.Body}");
                }
            }
            else
            {
                Console.WriteLine("\nNo comments on this post.");
            }
        }
        catch
        {
            Console.WriteLine("Post not found.");
        }
        Console.WriteLine();
    }

    private async Task ListUsersAsync()
    {
        Console.WriteLine("\n=== ALL USERS ===");
        var users = _userRepository.GetMany().ToList();
        
        if (!users.Any())
        {
            Console.WriteLine("No users found.");
            return;
        }

        foreach (var user in users)
        {
            Console.WriteLine($"[{user.Id}] {user.Username}");
        }
        Console.WriteLine();
    }

    private async Task ListCommentsAsync()
    {
        Console.WriteLine("\n=== ALL COMMENTS ===");
        var comments = _commentRepository.GetMany().ToList();
        
        if (!comments.Any())
        {
            Console.WriteLine("No comments found.");
            return;
        }

        foreach (var comment in comments)
        {
            var author = await _userRepository.GetSingleAsync(comment.UserId);
            var post = await _postRepository.GetSingleAsync(comment.PostId);
            Console.WriteLine($"[{comment.Id}] {author.Username} on '{post.Title}': {comment.Body}");
        }
        Console.WriteLine();
    }
}