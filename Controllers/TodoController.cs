using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")] 
    [ApiController]
    public class TodoController : Controller
    {
        private static readonly HttpClient _client = new HttpClient();
        private static readonly string _remoteUrl = "https://DemoUserAuth-BackEndApp.azurewebsites.net";
        private readonly TodoContext _context;

        public TodoController(TodoContext context)
        {
            _context = context;

            if (_context.TodoItems.Count() == 0)
            {
                _context.TodoItems.Add(new TodoItem { Name = "Item1" });
                _context.SaveChanges();
            }
        }

        // GET: api/Todo
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItem()
        {
            //The first line makes a GET /api/Todo call to the back-end API app
            var data = await _client.GetStringAsync($"{_remoteUrl}/api/Todo");
            return JsonConvert.DeserializeObject<List<TodoItem>>(data);
        }

        // GET: api/Todo/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> GetTodoItem(long id)
        {
            //var todoItem = await _context.TodoItems.FindAsync(id);

            //if (todoItem == null)
            //{
            //    return NotFound();
            //}

            //return todoItem;

            var data = await _client.GetStringAsync($"{_remoteUrl}/api/Todo/{id}");
            return Content(data, "application/json");
        }

        // PUT: api/Todo/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(long id, TodoItem todoItem)
        {
            //The first line makes a PUT /api/Todo/{id} call to the back-end API app.
            var res = await _client.PutAsJsonAsync($"{_remoteUrl}/api/Todo/{id}", todoItem);
            return new NoContentResult();
        }

        // POST: api/Todo
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItem todoItem)
        {
            //The first line makes a POST /api/Todo call to the back-end API app
            var response = await _client.PostAsJsonAsync($"{_remoteUrl}/api/Todo", todoItem);
            var data = await response.Content.ReadAsStringAsync();
            return Content(data, "application/json");
        }

        // DELETE: api/Todo/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<TodoItem>> DeleteTodoItem(long id)
        {
            //The first line makes a DELETE /api/Todo/{id} call to the back-end API app.
            var res = await _client.DeleteAsync($"{_remoteUrl}/api/Todo/{id}");
            return new NoContentResult();
        }

        private bool TodoItemExists(long id)
        {
            return _context.TodoItems.Any(e => e.Id == id);
        }
    }
}
