using BookStore.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BookStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookAPIController : ControllerBase
    {
        // GET: api/<BookAPIController>
        [HttpGet]
        public async Task <ActionResult<List<Book>>> Get()
        {
            BookDB bookDb = new BookDB();
            List<Book> books = bookDb.GetAllBooks();
            return books;
        }

        // GET api/<BookAPIController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> Get(int id)
        {
            BookDB bookDb = new BookDB();
            var book = bookDb.GetBookById(id);
            return book;
        }


    // POST api/<BookAPIController>
    [HttpPost]
        public void AddBooks([FromBody] Book book)
        {
            BookDB bookDb = new BookDB();
            bookDb.BookEditSave(book);
        }

        // PUT api/<BookAPIController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] Book book)
        {
            BookDB bookDb = new BookDB();
            bookDb.BookEditSave(book);
        }

        // DELETE api/<BookAPIController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            BookDB bookDb = new BookDB();
            bookDb.DeleteBook(id);
        }
    }
}