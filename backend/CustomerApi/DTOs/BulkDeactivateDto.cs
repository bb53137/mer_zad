namespace CustomerApi.DTOs;

public class BulkDeactivateDto
{
    public List<int> Ids { get; set; } = new();
}