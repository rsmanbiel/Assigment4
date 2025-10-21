namespace ApiContracts;

public class UpdatePostDto
{
    public required int Id { get; set; }
    public required string Title { get; set; }
    public required string Body { get; set; }
}
