using ForumMini.Entities;
using ForumMini.FileRepositories;
using ForumMini.RepositoryContracts;
using CLI.UI;

Console.WriteLine("ForumMini CLI Application");
Console.WriteLine("========================");

IUserRepository userRepo = new UserFileRepository();
IPostRepository postRepo = new PostFileRepository();
ICommentRepository commentRepo = new CommentFileRepository();

var cliApp = new CliApp(userRepo, postRepo, commentRepo);
await cliApp.StartAsync();