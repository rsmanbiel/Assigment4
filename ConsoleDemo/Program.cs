using ForumMini.Entities;
using ForumMini.InMemoryRepositories;

Console.WriteLine("ForumMini Demo - Assignment 1: Entities & Repositories");
Console.WriteLine("===================================================\n");

// Create repository instances
var userRepo = new UserInMemoryRepository();
var postRepo = new PostInMemoryRepository();
var commentRepo = new CommentInMemoryRepository();

Console.WriteLine("SEED DATA OVERVIEW");
Console.WriteLine("==================");

// Display all users
Console.WriteLine("\nUSERS:");
var users = userRepo.GetMany().ToList();
foreach (var user in users)
{
    Console.WriteLine($"  ID: {user.Id}, Username: {user.Username}");
}

// Display all posts
Console.WriteLine("\nPOSTS:");
var posts = postRepo.GetMany().ToList();
foreach (var post in posts)
{
    var author = await userRepo.GetSingleAsync(post.UserId);
    Console.WriteLine($"  ID: {post.Id}, Title: '{post.Title}' by {author.Username}");
    Console.WriteLine($"    Content: {post.Body}");
}

// Display all comments
Console.WriteLine("\nCOMMENTS:");
var comments = commentRepo.GetMany().ToList();
foreach (var comment in comments)
{
    var author = await userRepo.GetSingleAsync(comment.UserId);
    var post = await postRepo.GetSingleAsync(comment.PostId);
    Console.WriteLine($"  ID: {comment.Id}, '{comment.Body}' by {author.Username} on post '{post.Title}'");
}

Console.WriteLine("\n\nCRUD OPERATIONS DEMO");
Console.WriteLine("====================");

// Create a new user
Console.WriteLine("\nAdding a new user...");
var newUser = new User { Username = "johndoe", Password = "secret123" };
newUser = await userRepo.AddAsync(newUser);
Console.WriteLine($"Created user: {newUser.Username} (ID: {newUser.Id})");

// Create a new post by the new user
Console.WriteLine("\nAdding a new post...");
var newPost = new Post { Title = "My First Post!", Body = "Hello, this is my first post on the forum!", UserId = newUser.Id };
newPost = await postRepo.AddAsync(newPost);
Console.WriteLine($"Created post: '{newPost.Title}' (ID: {newPost.Id})");

// Add a comment to the new post
Console.WriteLine("\nAdding a comment...");
var newComment = new Comment { Body = "Welcome to the forum!", UserId = 1, PostId = newPost.Id }; // Comment by alice
newComment = await commentRepo.AddAsync(newComment);
Console.WriteLine($"Created comment: '{newComment.Body}' (ID: {newComment.Id})");

// Update the post
Console.WriteLine("\nUpdating the post...");
newPost.Body = "Hello, this is my first post on the forum! (Updated)";
await postRepo.UpdateAsync(newPost);
Console.WriteLine("Updated post content");

// Update the comment
Console.WriteLine("\nUpdating the comment...");
newComment.Body = "Welcome to the forum! Hope you enjoy your stay!";
await commentRepo.UpdateAsync(newComment);
Console.WriteLine("Updated comment");

// Delete the comment
Console.WriteLine("\nDeleting the comment...");
await commentRepo.DeleteAsync(newComment.Id);
Console.WriteLine($"Deleted comment (ID: {newComment.Id})");

// Demonstrate error handling
Console.WriteLine("\nERROR HANDLING DEMO");
Console.WriteLine("===================");
try
{
    await userRepo.GetSingleAsync(999);
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"Caught expected error: {ex.Message}");
}

try
{
    await userRepo.DeleteAsync(999);
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"Caught expected error: {ex.Message}");
}

Console.WriteLine("\n\nDemo completed successfully!");
Console.WriteLine("You can now build upon this foundation for future assignments.");
Console.WriteLine("Next steps: CLI interface, Web API, or EF Core integration.");
