using Newtonsoft.Json.Linq;
using NUnit.Framework;
using RestSharp;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace LibraryApiTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {            
        }

        // SetPrecondition() method should be http methods independent
        // So it is done here only to provide test running after some changes,
        // not like real Precondition.
        // Important: it does not work with empty library.
        private void SetPrecondition(string condition = "")
        {
            RestClient client = new RestClient("http://localhost:5000/api/books");

            var request = new RestRequest();
            RestResponse result = client.GetAsync(request).Result;
            List<Book> booksFromResponseBody =
                JsonSerializer.Deserialize<BooksFromResponseBody>(result.Content).books;
            
            foreach (var bookId in booksFromResponseBody.Select(x => x.id))
            {
                // Because of bug with POST to emty book library.
                if (bookId == 1 && condition != "Empty library") continue;

                client = new RestClient($"http://localhost:5000/api/books/{bookId}");
                request = new RestRequest();
                result = client.Delete(request);
            }
            if (condition == "Empty library") return;

            client = new RestClient("http://localhost:5000/api/books/1");
            var payload = new JObject
            {
                { "isElectronicBook", false },
                { "author", "Роберт Мартин" },
                { "name", "Чистый код" },
                { "year", 1998 }
            };
            request = new RestRequest();
            request.AddStringBody(payload.ToString(), DataFormat.Json);
            result = client.PutAsync(request).Result;

            client = new RestClient("http://localhost:5000/api/books");
            payload = new JObject
            {
                { "isElectronicBook", true },
                { "author", "Томас Кормен, Чарльз Лейзерсон" },
                { "name", "Алгоритмы: построение и анализ" },
                { "year", 1989 }
            };
            request = new RestRequest();
            request.AddStringBody(payload.ToString(), DataFormat.Json);
            result = client.PostAsync(request).Result;
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
            SetPrecondition();
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
            SetPrecondition();
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
            SetPrecondition();
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
                Assert.AreEqual(result.StatusCode,System.Net.HttpStatusCode.Created);
                VerifyBook(bookFromResponseBody,
                            true,
                            "Стив Макконел",
                            "Совершенный код",
                            2020,
                            3);
            });
        }

        // Achtung!!!
        // After running this test everything will become broken,
        // no any test will work correctly.
        // And restart of docker container will be needed to fix it.
        
        [Test]
        public void TestMethodPostWithFullParametersToEmptyLibrary()
        {
            SetPrecondition("Empty library");
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
            SetPrecondition();
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
            SetPrecondition();
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
            SetPrecondition();
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