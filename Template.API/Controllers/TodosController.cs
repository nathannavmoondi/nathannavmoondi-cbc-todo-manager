using AutoMapper;

using Microsoft.AspNetCore.Mvc;
using Blazor_Template_API.Contexts.DataAccess;
using Blazor_Template_Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Blazor_Template_API.Controllers
{
    //NATHAN: shoudl have a TodoDTO and Todo Entity model, use automapper to map.  Keeping it simple for time

    [ApiController]
    [Route("[controller]")]
    public class TodosController : ControllerBase
    {
        private readonly ILogger<TodosController> _logger;
        private readonly TodoDbContext _todoDbContext;


        public TodosController(
            ILogger<TodosController> logger,
            TodoDbContext todoDbContext)
        {
            _logger = logger;
            _todoDbContext = todoDbContext;
        }

        [HttpGet("/login")]
        public async Task<ActionResult<bool>> Login(string username, string password)
        {
            if (username.ToUpper() != "ADMIN" && password.ToUpper() != "ADMIN")
            {          
                return BadRequest("Invalid Username or Password");
            }

            return Ok(true);
    
        }

        [HttpGet("/test")]
        public async Task<ActionResult<string>> test()
        {
            
            return Ok("1");

        }


        [HttpGet("/todos")]
        public async Task<ActionResult<List<TodoDTO>>> GetTodosAsync()
        {
            try
            {
                return await _todoDbContext.Todos.ToListAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("/todo/{todoId}")]
        public async Task<ActionResult<TodoDTO>> GetTodoAsync(Guid todoId)
        {
            var todo = await _todoDbContext.FindAsync<TodoDTO>(todoId);
            if (todo == null)
            { return new StatusCodeResult(StatusCodes.Status404NotFound); }

            return todo;
        }

        [HttpDelete("/todo/{todoId}")]
        public async Task<ActionResult> Deletetodo(Guid todoId)
        {
            var todo = await _todoDbContext.Set<TodoDTO>().FindAsync(todoId); //var since might be null and guid can never be null (or Todo?)
            if (todo == null)
            { return new StatusCodeResult(StatusCodes.Status404NotFound); }

            _todoDbContext.Todos.Remove(todo);
            await _todoDbContext.SaveChangesAsync();
            return new StatusCodeResult(StatusCodes.Status200OK);
        }

        [HttpPost("/todo")]
        public async Task<ActionResult<Guid>> AddTodo(TodoDTO todo)
        {
            //NATHAN: Should validate

            bool todoExists = await _todoDbContext.Todos.AnyAsync(x => x.Id == todo.Id);
            if (todoExists)
            { return new StatusCodeResult(StatusCodes.Status409Conflict); }

            todo.Id = Guid.NewGuid();
            await _todoDbContext.Todos.AddAsync(todo);
            await _todoDbContext.SaveChangesAsync();

            Response.StatusCode = StatusCodes.Status201Created;
            return todo.Id;
        }



        [HttpPut("/todo/{todoId}")]
        public async Task<ActionResult> UpdateTodo(Guid todoId, TodoDTO todo)
        {
            var existingtodo = await _todoDbContext.FindAsync<TodoDTO>(todoId); //have to use var since Guid is never null
            if (existingtodo == null)
            { return new StatusCodeResult(StatusCodes.Status404NotFound); }

            try
            {
                _todoDbContext.ChangeTracker.Clear(); //fix error about tracking
                _todoDbContext.Todos.Update(todo);
                _todoDbContext.SaveChanges(); 
            }
            catch (Exception ex)
            {
                throw new Exception("Could not save todo", ex);
            }
            return new StatusCodeResult(StatusCodes.Status200OK);
        }
    }
}
