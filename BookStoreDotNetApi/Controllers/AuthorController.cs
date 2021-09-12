using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStoreDotNetApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
// using 
namespace BookStoreDotNetApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthorController : ControllerBase
    {

        [HttpGet]
        public IEnumerable<Author> Get()
        {
            using (var context = new BookStoresDBContext())
            {
                // return context.Authors.ToList();
                Author author =context.Authors.Where(x => x.FirstName == "Ram").First();
                // author.FirstName = "Ram";
                // author.LastName = "Shyam";
                context.Authors.Remove(author);
                context.SaveChanges();
                return context.Authors.Where(x => x.FirstName == "Ram").ToList();
            }
        }
    }
}
