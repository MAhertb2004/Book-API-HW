using BookStore.Models;
using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Globalization;
using System.Linq;
using System.IO;
using System.IO.Pipes;
using Microsoft.AspNetCore.Http.Metadata;

namespace BookStore.Controllers
{
    public class BookController : Controller
    {

        public IActionResult ExportToCSV()
        {
            HttpClient client = new HttpClient();
            var responseTask = client.GetAsync("https://localhost:7068/api/BookAPI");
            responseTask.Wait();
            if (responseTask.IsCompleted)
            {
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var MessageTask = result.Content.ReadAsStringAsync();
                    var str = MessageTask.Result;
                    var books = JsonConvert.DeserializeObject<List<Book>>(MessageTask.Result);

                    if (books.Count > 0)
                    {
                        using (var writer = new StreamWriter("books.csv"))
                        {
                            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                            {
                                csv.Context.RegisterClassMap<BookMap>();
                                csv.WriteRecords(books);
                            }
                        }
                    }
                }
            }
            return RedirectToAction("ReadBooks");
        }







        [HttpPost]
        public async Task<IActionResult> ImportFromCSV()
        {
            try
            {
                if (Request.Form.Files[0] != null)
                {
                    var importedData = Request.Form.Files[0];
                    var fullPath = Path.GetFullPath(importedData.FileName);
                    using (var streamReader = new StreamReader(@fullPath))
                    {
                        using (var csv = new CsvReader(streamReader, CultureInfo.InvariantCulture))
                        {
                            //csv.Context.RegisterClassMap<BookMapReading>();
                            var record = csv.GetRecords<Book>().ToList();
                            for (int i = 0; i < record.Count; i++)
                            {
                                record[i].BookID = 0;
                                SaveCreateAndUpdateBook(record[i]);
                            }
                        }
                    }
                }
            }
            catch
            {
                TempData["FileErrorMessage"] = "No Fiel was uploaded";
                return RedirectToAction("ReadBooks");
            }
            
                return RedirectToAction("ReadBooks");
        }


        public IActionResult CreateAndUpdateBook(Book book)
        {
            ViewBag.errorMessage = TempData["errorMessage"];

            if ((ViewBag.errorMessage == null) && (book.BookID != 0))
            {
                HttpClient client = new HttpClient();
                var responseTask = client.GetAsync("https://localhost:7068/api/BookAPI/"+book.BookID);
                responseTask.Wait();
                if (responseTask.IsCompleted)
                {
                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var MessageTask = result.Content.ReadAsStringAsync();
                        var str = MessageTask.Result;
                        var bookToUpdate = JsonConvert.DeserializeObject<Book>(MessageTask.Result);

                        ViewBag.Book = bookToUpdate;
                    }
                }
            }
            else
                ViewBag.Book = book;

            return View();
        }

        public IActionResult SaveCreateAndUpdateBook(Book book)
        {
            List<string> errorMessages = new List<string>();
            BookDB bookDb = new BookDB();

            if (book.Title == null)
                errorMessages.Add("Title is required");
            if (book.NumberofPages == 0)
                errorMessages.Add("Number of pages is required");

            if (errorMessages.Count > 0)
            {
                TempData["errorMessage"] = errorMessages;
                //here we are redirecting to the above action and pass the mistaken object to send it back to the HTML page to be edited
                return RedirectToAction("CreateAndUpdateBook", book);
            }

            if (book.BookID == 0)
            {
                book.CreatedDate = DateTime.Now;
                //call the API create action
                HttpClient client = new HttpClient();
                var responseTask = client.PostAsJsonAsync("https://localhost:7068/api/BookAPI/", book);
                responseTask.Wait();
            }
            else
            {
                book.UpdatedDate = DateTime.Now;
                //call the API update action
                HttpClient client = new HttpClient();
                var responseTask = client.PutAsJsonAsync("https://localhost:7068/api/BookAPI/"+book.BookID, book);
                responseTask.Wait();
            }

            return RedirectToAction("ReadBooks");
        }

        //public static string RandomString(int length)
        //{
        //    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        //    string[] firstName = { "Learning", "Developing", "Discovering", "Improving" };
        //    string[] lastName = { "IT", "AI", "Mathematic", ".Net" };

        //    Random random = new Random();
        //    return new string(Enumerable.Repeat(chars, length)
        //        .Select(s => s[random.Next(s.Length)]).ToArray());
        //}


        public IActionResult ReadBooks()
        {
            ViewBag.FileErrorMessage = TempData["FileErrorMessage"];


            ////Generating random Titles from these matrixes
            //string[] firstName = { "Learning", "Developing", "Discovering", "Improving" };
            //string[] lastName = { "IT", "AI", "Mathematic", ".Net" };
            //List<int> listNumbers = new List<int>();

            //for (int i = 0; i < 10; i++)
            //{
            //    var book = new Book();
            //    Random random = new Random();
            //    var titleFirstWord = firstName[random.Next(firstName.Length)];
            //    var titleSecondWord = lastName[random.Next(lastName.Length)];
            //    book.Title = titleFirstWord + " " + titleSecondWord;

            //    //Generating random numbers without repeating [duplicate]
            //    int number;
            //    do
            //    {
            //        number = random.Next(1001);
            //        number = (number / 50) *50;
            //    } while (listNumbers.Contains(number) || number==0);
            //    listNumbers.Add(number);
            //    book.NumberofPages = number;
            //    SaveCreateAndUpdateBook(book);
            //}

            HttpClient client = new HttpClient();
            var responseTask = client.GetAsync("https://localhost:7068/api/BookAPI");
            responseTask.Wait();
            if (responseTask.IsCompleted)
            {
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var MessageTask = result.Content.ReadAsStringAsync();
                    var str = MessageTask.Result;
                    var books = JsonConvert.DeserializeObject<List<Book>>(MessageTask.Result);

                    ////Deleting Books greater than 10
                    //List<int> listDeletedNumbers = new List<int>();
                    //do
                    //{
                    //    Random random = new Random();
                    //    //Generating random numbers without repeating [duplicate]
                    //    int number;
                    //    do
                    //    {
                    //        number = random.Next(books.Count);
                    //    } while (listDeletedNumbers.Contains(number));
                    //    listDeletedNumbers.Add(number);
                    //    DeleteBook(number);

                    //} while (books.Count > 10);



                    ////Updating the book titles and number of pages
                    //List<int> listUpdatedNumbers = new List<int>();

                    //for (int j = 0; j < books.Count; j++)
                    //{
                    //    var book = new Book();
                    //    Random random = new Random();
                    //    var titleFirstWord = firstName[random.Next(firstName.Length)];
                    //    var titleSecondWord = lastName[random.Next(lastName.Length)];
                    //    books[j].Title = titleFirstWord + " " + titleSecondWord;

                    //    //Generating random numbers without repeating [duplicate]
                    //    int number;
                    //    do
                    //    {
                    //        number = random.Next(1001);
                    //        number = (number / 50) * 50;
                    //    } while (listUpdatedNumbers.Contains(number) || number == 0);
                    //    listUpdatedNumbers.Add(number);
                    //    books[j].NumberofPages = number;
                    //    SaveCreateAndUpdateBook(books[j]);
                    //}

                    ViewBag.Book = books;
                }
            }
            return View();
        }

        public IActionResult DeleteBook(int BookID)
        {
            HttpClient client = new HttpClient();
            var responseTask = client.DeleteAsync("https://localhost:7068/api/BookAPI/"+BookID);
            responseTask.Wait();
         
            return RedirectToAction("ReadBooks");
        }
    }
}