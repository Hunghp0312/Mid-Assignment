namespace Common.Parameters;

public class BookFilterParams
{
    public string? Query { get; set; }
    public bool? Available { get; set; }
    public double? Rating { get; set; }
    public Guid? CategoryId { get; set; }
}
