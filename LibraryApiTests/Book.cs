
using System.Collections.Generic;

namespace LibraryApiTests
{
    public class Book
    {
        public string author { get; set; }
        public int id { get; set; }
        public bool isElectronicBook { get; set; }
        public string name { get; set; }
        public int year { get; set; }
    }
    public class BookFromResponseBody
    {
        public Book book { get; set; }
    }
    public class BooksFromResponseBody
    {
        public List<Book> books { get; set; }
    }
    public class ResultFromResponseBody
    {
        public bool result { get; set; }
    }
}
