using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TodoItemsController : ControllerBase
{
    private readonly TodoContext todoContext;

    public TodoItemsController(TodoContext context)
    {
        todoContext = context;
    }

    // GET ALL TASKS FROM THE DB
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItems()
    {
        // 1. SORT BY STATUS
        // 2. SORT BY CREATION DATE DESCENDING
        // 3. FINALLY SORT BY DUE DATE DESCENDING
        return await todoContext.TodoItems
            .OrderBy(t => t.TodoItemIsDone)
            .ThenByDescending(t => t.TodoItemCreatedAt)
            .ThenByDescending(t => t.TodoItemDueDate)
            .ToListAsync();
    }

    // ZDOBYWANIE KONKRETNEGO TASKA Z BAZY NA PODSTAWIE ID
    [HttpGet("{id:int}")]
    public async Task<ActionResult<TodoItem>> GetTodoItem(int id)
    {
        // 1. FIND THE ITEM BY ID
        var todoItem = await todoContext.TodoItems
            .FirstOrDefaultAsync(t => t.TodoItemId == id);

        if (todoItem == null)
        {
            // 2. IF THE ITEM IS NNOT FOUND, RETURN EARLY WITH 404 STATUS CODE
            return NotFound();
        }

        return todoItem;
    }

    // ADD A NEW TASK TO THE DB
    [HttpPost]
    public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItem todoItem)
    {
        // 1. IF THE TITLE IS MISSING, INFORM ABOUT IT
        if (string.IsNullOrWhiteSpace(todoItem.TodoItemTitle))
        {
            return BadRequest("Tytuł jest wymagany.");
        }

        todoItem.TodoItemId = 0;
        if (todoItem.TodoItemCreatedAt == default)
        {
            todoItem.TodoItemCreatedAt = DateTime.UtcNow;
        }

        // 2. ADD THE NEW ITEM TO THE DB AND SAVE CHANGES
        todoContext.TodoItems.Add(todoItem);
        await todoContext.SaveChangesAsync();

        // 3. TASK CREATED, RETURN FEEDBACK
        return CreatedAtAction(
            nameof(GetTodoItem),
            new { id = todoItem.TodoItemId },
            todoItem);
    }

    // UPDATE AN EXISTING TASK IN THE DB BASED ON ID
    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutTodoItem(int id, TodoItem todoItem)
    {
        if (id != todoItem.TodoItemId)
        {
            return BadRequest("Podane ID nie odpowiada żadnemu zadaniu w bazie.");
        }

        var existing = await todoContext.TodoItems
            .FirstOrDefaultAsync(t => t.TodoItemId == id);
        if (existing == null)
        {
            return NotFound();
        }

        // 2. UPDATE FIELDS
        existing.TodoItemTitle = todoItem.TodoItemTitle;
        existing.TodoItemDescription = todoItem.TodoItemDescription;
        existing.TodoItemIsDone = todoItem.TodoItemIsDone;
        existing.TodoItemDueDate = todoItem.TodoItemDueDate;

        await todoContext.SaveChangesAsync();

        return NoContent();
    }

    // DELETE A TASK FROM THE DB BASED ON ID
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteTodoItem(int id)
    {
        // 1. FIND THE ITEM BY ID
        var todoItem = await todoContext.TodoItems
            .FirstOrDefaultAsync(t => t.TodoItemId == id);

        if (todoItem == null)
        {
            // 2. IF THE ITEM IS NOT FOUND, RETURN EARLY WITH 404 STATUS CODE
            return NotFound();
        }

        // 3. REMOVE THE ITEM FROM THE DB AND SAVE CHANGES
        todoContext.TodoItems.Remove(todoItem);
        await todoContext.SaveChangesAsync();

        // 4. TASK DELETED, RETURN FEEDBACK
        return NoContent();
    }
}
