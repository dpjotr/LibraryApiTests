using Newtonsoft.Json.Linq;
using NUnit.Framework;
using RestSharp;
using System.Collections.Generic;
using System.Text.Json;

namespace LibraryApiTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        private void SetPrecondition()
        {
            // Put your precondition here.
        }

        private void VerifyBook(Book book,
                                bool isEBook,
                                string author,
                                string name,
                                int year,
                                int id)
        {
            Assert.AreEqual(book.isElectronicBook, isEBook, "isElectronicBook");
            Assert.AreEqual(book.author, author, "author");
            Assert.AreEqual(book.name, name, "name");
            Assert.AreEqual(book.year, year, "year");
            Assert.AreEqual(book.id, id, "Id");
        }

        [Test]
        public void TestMethodGetAllBooks()
        {
            RestClient client = new RestClient("http://localhost:5000/api/books");

            var request = new RestRequest();
            RestResponse result = client.GetAsync(request).Result;
            List<Book> booksFromResponseBody =
                JsonSerializer.Deserialize<BooksFromResponseBody>(result.Content).books;
            Assert.Multiple(() =>
            {
                Assert.AreEqual(result.StatusCode, System.Net.HttpStatusCode.OK);
                VerifyBook(booksFromResponseBody[0],
                            false,
                            "Роберт Мартин",
                            "Чистый код",
                            1998,
                            1);
                VerifyBook(booksFromResponseBody[1],
                            true,
                            "Томас Кормен, Чарльз Лейзерсон",
                            "Алгоритмы: построение и анализ",
                            1989,
                            2);
            });
        }

        [Test]
        public void TestMethodGetBookById()
        {
            RestClient client = new RestClient("http://localhost:5000/api/books/1");

            var request = new RestRequest();
            RestResponse result = client.GetAsync(request).Result;
            Book bookFromResponseBody =
                JsonSerializer.Deserialize<BookFromResponseBody>(result.Content).book;
            Assert.Multiple(() =>
            {
                Assert.AreEqual(result.StatusCode, System.Net.HttpStatusCode.OK);
                VerifyBook(bookFromResponseBody,
                            false,
                            "Роберт Мартин",
                            "Чистый код",
                            1998,
                            1);
            });
        }

        [Test]
        public void TestMethodPostWithFullParameters()
        {
            RestClient client = new RestClient("http://localhost:5000/api/books");
            var payload = new JObject
            {
                { "isElectronicBook", true },
                { "author", "Стив Макконел" },
                { "name", "Совершенный код" },
                { "year", 2020 }
            };
            var request = new RestRequest();
            request.AddStringBody(payload.ToString(), DataFormat.Json);
            RestResponse result = client.PostAsync(request).Result;
            Book bookFromResponseBody =
                JsonSerializer.Deserialize<BookFromResponseBody>(result.Content).book;
            Assert.Multiple(() =>
            {
                Assert.AreEqual(result.StatusCode, System.Net.HttpStatusCode.Created);
                VerifyBook(bookFromResponseBody,
                            true,
                            "Стив Макконел",
                            "Совершенный код",
                            2020,
                            3);
            });
        }

        [Test]
        public void TestMethodPostWithDefaultParameters()
        {

            RestClient client = new RestClient("http://localhost:5000/api/books");
            var payload = new JObject
            {
                { "isElectronicBook", true },
                { "author", "Стив Макконел" },
                { "name", "Совершенный код" },
                { "year", 2020 }
            };
            var request = new RestRequest();
            request.AddStringBody(payload.ToString(), DataFormat.Json);
            RestResponse result = client.PostAsync(request).Result;
            Book bookFromResponseBody =
                JsonSerializer.Deserialize<BookFromResponseBody>(result.Content).book;
            Assert.Multiple(() =>
            {
                Assert.AreEqual(result.StatusCode, System.Net.HttpStatusCode.Created);
                VerifyBook(bookFromResponseBody,
                            false,
                            "",
                            "Совершенный код",
                            0,
                            1);
            });
        }

        [Test]
        public void TestMethodPut()
        {
            RestClient client = new RestClient("http://localhost:5000/api/books/1");
            var payload = new JObject
            {
                { "isElectronicBook", true },
                { "author", "Стив Макконел" },
                { "name", "Совершенный код" },
                { "year", 2020 }
            };

            var request = new RestRequest();
            request.AddStringBody(payload.ToString(), DataFormat.Json);
            RestResponse result = client.PutAsync(request).Result;
            Book bookFromResponseBody =
                JsonSerializer.Deserialize<BookFromResponseBody>(result.Content).book;
            Assert.Multiple(() =>
            {
                Assert.AreEqual(result.StatusCode, System.Net.HttpStatusCode.OK);
                VerifyBook(bookFromResponseBody,
                            true,
                            "Стив Макконел",
                            "Совершенный код",
                            2020,
                            1);
            });
        }

        [Test]
        public void TestMethodDelete()
        {
            RestClient client = new RestClient("http://localhost:5000/api/books/2");

            var request = new RestRequest();
            RestResponse result = client.DeleteAsync(request).Result;
            var resultFromResponseBody =
                JsonSerializer.Deserialize<ResultFromResponseBody>(result.Content).result;
            Assert.Multiple(() =>
            {
                Assert.AreEqual(result.StatusCode, System.Net.HttpStatusCode.OK);
                Assert.AreEqual(resultFromResponseBody, true, "Response body");
            });
        }
    }
}