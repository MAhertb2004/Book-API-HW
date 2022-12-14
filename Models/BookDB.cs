using Dapper;
using System.Data.SqlClient;
using static System.Reflection.Metadata.BlobBuilder;

namespace BookStore.Models
{
    public class BookDB
    {
        private string connectionString = ("server = Administrator; database = Book; user id = reader; password = pass123;");
        public List<Book> GetAllBooks()
        {
            List<Book> books = new List<Book>();
            string sql = "select * from Book";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                books = conn.Query<Book>(sql).ToList();
            }
            return books;
        }

        public Book GetBookById(int BookID)
        {
            var book = new Book();
            string sql =" select * from Book WHERE BookID =@BookID";
           
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                book = conn.QueryFirst<Book>(sql, new { BookID });
            }

            return book;
        }

        public void DeleteBook(int BookID)
        {
            string sql = "DELETE FROM [Book] WHERE [BookID] > @BookID";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Execute(sql, new { BookID });
            }

        }



        public void BookEditSave(Book book)
        {
            string sql;
            if (book.BookID == 0)
            {
                sql = "INSERT INTO [Book] " +
                        "(Title,NumberofPages,CreatedDate)" +

                      "VALUES" +
                        "(@Title, @NumberofPages, @CreatedDate);";
            }
            else
            {
                sql = "UPDATE Book SET " +
                        "Title = @Title, NumberofPages = @NumberofPages, UpdatedDate = @UpdatedDate " +
                      "WHERE BookID = @BookID;";

            }
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Execute(sql, book);
            }
        }



    }
}