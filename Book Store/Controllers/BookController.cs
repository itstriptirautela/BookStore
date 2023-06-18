using Book_Store.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Book_Store.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]

    [Produces("application/json")]
    //[Authorize]
    public class BookController : ControllerBase
    {
        private BookStoreContext dbcontext;
        public BookController(BookStoreContext bookStoreContext)
        {
            dbcontext = bookStoreContext;
        }

        /// <summary>
        /// Adding New Book, access only to Admin
        /// </summary>
        /// <param name="book">Passing book details from body </param>
        /// <returns>adding book </returns>



        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult AddBook([FromBody] Book book)
        {


            dbcontext.Books.Add(book);
            dbcontext.SaveChanges();
            return StatusCode(StatusCodes.Status201Created);


        }


        /// <summary>
        /// Getting the avaliable book access to all the user
        /// </summary>
        /// <returns>all the books</returns>

        [HttpGet]

        //public IEnumerable<Book> ViewBooks()
        //{
        //    //return dbcontext.Books.Include(c => c.Category).ToList();

         //   var data = from book in dbcontext.Books

        //               select new Book
        //               {
        //                   BookId = book.BookId,
        //                   BookTitle = book.BookTitle,
        //                   Author = book.Author,
        //                   Description = book.Description,
        //                   CategoryId = book.CategoryId,
        //                   Category = new Category()
        //                   {
        //                       CategoryId = book.CategoryId,
        //                       BookCategory = book.Category.BookCategory
        //                   },
        //                   BookPrice = book.BookPrice,
        //                   Publisher = book.Publisher,
        //                   NumberOfCopies = book.NumberOfCopies,
        //                   UserId = book.UserId
        //                   //User = book.User,
        //               };
        //    return (data).ToList();
        //}
        public IEnumerable<Book> ViewBooks()
        {
            return dbcontext.Books.Where(j => j.IsDeleted == false);
        }



        /// <summary>
        /// get book by id
        /// </summary>
        /// <param name="BookId">book id</param>
        /// <returns></returns>


        //public IActionResult ViewBooksById(int BookId)
        //{
        //    var comments = from book in dbcontext.Books.Where(b => b.BookId == BookId && b.IsDeleted == false)
        //                   .Include(b => b.Category)


        //                   select new Book
        //                   {
        //                       BookId = book.BookId,
        //                       BookTitle = book.BookTitle,
        //                       Author = book.Author,
        //                       Description = book.Description,
        //                       CategoryId = book.CategoryId,
        //                       Category = new Category()
        //                       {
        //                           CategoryId = book.CategoryId,
        //                           BookCategory = book.Category.BookCategory
        //                       },

        //                       BookPrice = book.BookPrice,
        //                       Publisher = book.Publisher,
        //                       NumberOfCopies = book.NumberOfCopies,
        //                       UserId = book.UserId

        //                   };
        //    return Ok(comments);
        //}
        [HttpGet]
        public IActionResult ViewBooksById(int Id)
        {
            var bookResult = dbcontext.Books.Where(x => x.BookId == Id && x.IsDeleted == false);

            if (bookResult == null)
            {
                return NotFound("No record found against this Id");
            }
            else
            {
                return Ok(bookResult);
            }
        }
        /// <summary>
        /// deleting book by id 
        /// accessing only to admin
        /// </summary>
        /// <param name="BookId">bookid</param>
        /// <returns>deleting of book from table</returns>

        //[Authorize(Roles = "Admin")]
        [HttpDelete]

        public IActionResult DeleteBook(int BookId)
        {

            var data = dbcontext.Books.FirstOrDefault(x => x.BookId == BookId);

            if (data != null)
            {
                dbcontext.Books.Remove(data);
                dbcontext.SaveChanges();
                return StatusCode(StatusCodes.Status200OK);

            }
            return NotFound("Record not found");


        }
        /// <summary>
        /// here searching a book
        /// paging only 10 record in page
        /// </summary>
        /// <param name="BookTitle"></param>
        /// <param name="sort"></param>
        /// <param name="PageNo"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult SortBook(string sort, int PageNo)
        {
            var data = from book in dbcontext.Books

                       select new
                       {
                           BookId = book.BookId,
                           BookTitle = book.BookTitle,
                           Author = book.Author,
                           Description = book.Description,
                           BookCategory = book.Category,
                           BookPrice = book.BookPrice,
                           Publisher = book.Publisher,
                           NumberOfCopies = book.NumberOfCopies
                       };
            switch (sort)
            {
                case "1":
                    return Ok(data.Skip((PageNo - 1) * 10).Take(10).OrderByDescending(b => b.BookTitle));
                case "0":
                    return Ok(data.Skip((PageNo - 1) * 10).Take(10).OrderBy(b => b.BookTitle));
                default :
                    return Ok(data.Skip((PageNo - 1) * 10).Take(10));
            }

        }
        //[HttpGet]
        //public IActionResult SearchBook(string bookTitle, string author, string bookCategory, string publisher,int price)
        //{
        //    var data = from book in dbcontext.Books
        //               join category in dbcontext.Categories on book.BookId equals category.CategoryId
        //               where book.BookTitle.StartsWith(bookTitle) || book.Author.StartsWith(author) || category.BookCategory.StartsWith(bookCategory) || book.Publisher.StartsWith(publisher) || book.BookPrice <=( price)
        //               select new
        //               {
        //                   BookId = book.BookId,
        //                   BookTitle = book.BookTitle,
        //                   Author = book.Author,
        //                   Description = book.Description,
        //                   BookCategory = category.BookCategory,
        //                   BookPrice = book.BookPrice,
        //                   Publisher = book.Publisher,
        //                   NumberOfCopies = book.NumberOfCopies
        //               };
        //    return Ok(data);


        [HttpGet]
        public ActionResult<IEnumerable<Book>> GetSearchedBooks(string bookTitle, string author, string bookCategory, int? price, string publisher, int sort, int pageno = 1)
        {
            if (sort == -1)
            {

               
                return (List<Book>)dbcontext.Books.Where(j => !j.IsDeleted &&
                 
                  (bookCategory == null || j.Category == bookCategory)&&
                (price == null || j.BookPrice <= price) &&
                (author == null || j.Author == author)).ToList()
                .Where(j => (bookTitle == null || j.BookTitle.StartsWith(bookTitle, StringComparison.OrdinalIgnoreCase))
                && (author == null || j.Author.StartsWith(author, StringComparison.OrdinalIgnoreCase)) &&
                (bookCategory == null || j.Category.StartsWith(bookCategory, StringComparison.OrdinalIgnoreCase))
                  && (publisher == null || j.Publisher.StartsWith(publisher, StringComparison.OrdinalIgnoreCase))).ToList().
                OrderByDescending(p => p.BookId).Skip((pageno - 1) * 6).Take(6).ToList();
            }

            else if (sort == 1)
            {
                return (List<Book>)dbcontext.Books.Where(j => !j.IsDeleted &&
                
                 (bookCategory == null || j.Category == bookCategory)&&
                (price == null || j.BookPrice <= price) &&
                (author == null || j.Author == author)).ToList()
                .Where(j => (bookTitle == null || j.BookTitle.StartsWith(bookTitle, StringComparison.OrdinalIgnoreCase))
                && (author == null || j.Author.StartsWith(author, StringComparison.OrdinalIgnoreCase)) &&
                     (bookCategory == null || j.Category.StartsWith(bookCategory, StringComparison.OrdinalIgnoreCase))
                && (publisher == null || j.Publisher.StartsWith(publisher, StringComparison.OrdinalIgnoreCase))).ToList().
                OrderByDescending(j => j.BookId).Skip((pageno - 1) *6).Take(6).ToList();
            }
            else
            {
                return (List<Book>)dbcontext.Books.Where(j => !j.IsDeleted &&
                
                 (bookCategory == null || j.Category == bookCategory)&&
              (price == null || j.BookPrice <= price) &&
                (author == null || j.Author == author)).ToList()
                .Where(j => (bookTitle == null || j.BookTitle.StartsWith(bookTitle, StringComparison.OrdinalIgnoreCase))
                && (author == null || j.Author.StartsWith(author, StringComparison.OrdinalIgnoreCase)) &&
                     (bookCategory == null || j.Category.StartsWith(bookCategory, StringComparison.OrdinalIgnoreCase))
                && (publisher == null || j.Publisher.StartsWith(publisher, StringComparison.OrdinalIgnoreCase))).ToList().
                 OrderBy(j => j.BookPrice).Skip((pageno - 1) * 6).Take(6).ToList();

            }
        }
        [HttpGet]
        public int NumberOfSearchedProperties(string bookTitle, string author, string bookCategory, int? price, string publisher)
        {
            return dbcontext.Books.Where(j => !j.IsDeleted &&
             (bookTitle == null || j.BookTitle == bookTitle) &&
            (bookCategory == null || j.Category == bookCategory) &&
            (price == null || j.BookPrice <= price) &&
            (author == null || j.Author == author)).ToList()
            .Where(j => (bookTitle == null || j.BookTitle.StartsWith(bookTitle, StringComparison.OrdinalIgnoreCase))
            && (author == null || j.Author.StartsWith(author, StringComparison.OrdinalIgnoreCase))
            && (bookCategory == null || j.Category.StartsWith(bookCategory, StringComparison.OrdinalIgnoreCase))
            && (publisher == null || j.Publisher.StartsWith(publisher, StringComparison.OrdinalIgnoreCase))).ToList().Count();



        }
    }
}



