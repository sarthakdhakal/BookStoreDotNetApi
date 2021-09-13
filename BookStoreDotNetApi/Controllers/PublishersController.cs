using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookStoreDotNetApi.Models;
using Microsoft.AspNetCore.Authorization;

namespace BookStoreDotNetApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PublishersController : ControllerBase
    {
        private readonly BookStoresDBContext _context;

        public PublishersController(BookStoresDBContext context)
        {
            _context = context;
        }

        // GET: api/Publishers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Publisher>>> GetPublishers()
        {
            return await _context.Publishers.ToListAsync();
        }

        // GET: api/Publishers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Publisher>> GetPublisher(int id)
        {
            var publisher = await _context.Publishers.FindAsync(id);

            if (publisher == null)
            {
                return NotFound();
            }

            return publisher;
        }

        [HttpGet("GetPublishersDetails/{id}")]
        public async Task<ActionResult<Publisher>> GetPublisherDetails(int id)
        {
            //Eager Loading
            // var publishers = await _context.Publishers.Include(pub => pub.Books).ThenInclude(pub => pub.Sales)
            //     .Include(pub => pub.Users).ThenInclude(pub => pub.Job).Where(pub => pub.PubId == id)
            //     .FirstOrDefaultAsync();
            //Explicit Loading
            var publishers = await _context.Publishers.SingleAsync(x => x.PubId == id);
            _context.Entry(publishers).Collection(pub => pub.Users).Query().Where(user =>user.FirstName.Contains("karin")).Load();
            _context.Entry(publishers).Collection(pub=>pub.Books).Query().Include(books=>books.Sales).Load();

            // var users = await _context.Users.SingleAsync(x => x.UserId == "karin.josephs");
            // _context.Entry(users).Reference(user => user.Job).Load();
            
            
            
            if (publishers == null)
            {
                return NotFound();
            }

            return publishers;
        }

        [HttpGet("PostPublishersDetails/")]
        public async Task<ActionResult<Publisher>> PostPublisherDetails()
        {
            try
            {
                var publisher = new Publisher();
                publisher.PublisherName = "Ki2ana";
                publisher.City = "Kan2du";
                publisher.State = "ww";
                publisher.Country = "Nepal";

                Book book1 = new Book();
                book1.Title = "Charlie";
                book1.PublishedDate = DateTime.Now;

                Book book2 = new Book();
                book2.Title = "Seto";
                book2.PublishedDate = DateTime.Now;

                Sale sale1 = new Sale();
                sale1.Quantity = 2;
                sale1.StoreId = "8099";
                sale1.OrderNum = "XYZ";
                sale1.PayTerms = "Net 30";
                sale1.OrderDate = DateTime.Now;

                Sale sale2 = new Sale();
                sale2.Quantity = 2;
                sale2.StoreId = "8099";
                sale2.OrderNum = "XYZ";
                sale2.PayTerms = "Net 30";
                sale2.OrderDate = DateTime.Now;

                book1.Sales.Add(sale1);
                book2.Sales.Add(sale2);
                publisher.Books.Add(book1);
                publisher.Books.Add(book2);
                _context.Publishers.Add(publisher);
                await _context.SaveChangesAsync();
                var publishers = await _context.Publishers.Include(pub => pub.Books).ThenInclude(pub => pub.Sales)
                    .Include(pub => pub.Users).ThenInclude(pub => pub.Job).Where(pub => pub.PubId == publisher.PubId)
                    .FirstOrDefaultAsync();


                if (publishers == null)
                {
                    return NotFound();
                }

                return publishers;
            }
            catch (Exception e)
            {
            }


            return new Publisher();
        }

        // PUT: api/Publishers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPublisher(int id, Publisher publisher)
        {
            if (id != publisher.PubId)
            {
                return BadRequest();
            }

            _context.Entry(publisher).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PublisherExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Publishers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Publisher>> PostPublisher(Publisher publisher)
        {
            _context.Publishers.Add(publisher);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPublisher", new {id = publisher.PubId}, publisher);
        }

        // DELETE: api/Publishers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePublisher(int id)
        {
            var publisher = await _context.Publishers.FindAsync(id);
            if (publisher == null)
            {
                return NotFound();
            }

            _context.Publishers.Remove(publisher);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PublisherExists(int id)
        {
            return _context.Publishers.Any(e => e.PubId == id);
        }
    }
}