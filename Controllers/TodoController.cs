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
        private readonly TodoContext _context;

        //added 5/04//22
        private static readonly HttpClient _client = new HttpClient();
        private static readonly string _remoteUrl = "https://DemoUserAuth-BackEndApp.azurewebsites.net";
       
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
            //chito - The first line makes a GET /api/Todo call to the back-end API app.
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

            //chito - the first line makes a GET /api/Todo/{id} call to the back-end API app.
            var data = await _client.GetStringAsync($"{_remoteUrl}/api/Todo/{id}");
            return Content(data, "application/json");
        }

        // PUT: api/Todo/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(long id, TodoItem todoItem)
        {
            //chito - The first line makes a PUT /api/Todo/{id} call to the back-end API app
            var res = await _client.PutAsJsonAsync($"{_remoteUrl}/api/Todo/{id}", todoItem);
            return new NoContentResult();

            //if (id != todoItem.Id)
            //{
            //    return BadRequest();
            //}

            //_context.Entry(todoItem).State = EntityState.Modified;

            //try
            //{
            //    await _context.SaveChangesAsync();
            //}
            //catch (DbUpdateConcurrencyException)
            //{
            //    if (!TodoItemExists(id))
            //    {
            //        return NotFound();
            //    }
            //    else
            //    {
            //        throw;
            //    }
            //}

            //return NoContent();
        }

        // POST: api/Todo
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItem todoItem)
        {
            //_context.TodoItems.Add(todoItem);
            //await _context.SaveChangesAsync();

            //return CreatedAtAction("GetTodoItem", new { id = todoItem.Id }, todoItem);

            //chito - The first line makes a POST /api/Todo call to the back-end API app. 
            var response = await _client.PostAsJsonAsync($"{_remoteUrl}/api/Todo", todoItem);
            var data = await response.Content.ReadAsStringAsync();
            return Content(data, "application/json");
        }

        // DELETE: api/Todo/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<TodoItem>> DeleteTodoItem(long id)
        {
            //chito - The first line makes a DELETE /api/Todo/{id} call to the back-end API app.
            var res = await _client.DeleteAsync($"{_remoteUrl}/api/Todo/{id}");
            return new NoContentResult();

            //var todoItem = await _context.TodoItems.FindAsync(id);
            //if (todoItem == null)
            //{
            //    return NotFound();
            //}

            //_context.TodoItems.Remove(todoItem);
            //await _context.SaveChangesAsync();

            //return todoItem;
        }

        private bool TodoItemExists(long id)
        {
            return _context.TodoItems.Any(e => e.Id == id);
        }

        /// <summary>
        /// Added Chito
        /// This code adds the standard HTTP header Authorization: Bearer <access-token>
        /// to all remote API calls. In the ASP.NET Core MVC request execution pipeline,
        /// OnActionExecuting executes just before the respective action does,
        /// so each of your outgoing API call now presents the access token.
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", Request.Headers["X-MS-TOKEN-AAD-ACCESS-TOKEN"]);
        }
    }
}
