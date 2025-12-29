namespace TodoApi.Models;

public class TodoItem
{
    public int TodoItemId { get; set; }
    public string TodoItemTitle { get; set; } = string.Empty;
    public string TodoItemDescription { get; set; } = string.Empty;
    public bool TodoItemIsDone { get; set; }
    public DateTime TodoItemCreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? TodoItemDueDate { get; set; }
}
