using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace BookStore.Models
{
    //This class to format the columns name of the CSV file
    //and present the data in good shape
    //and add units to that data
    public class BookMap:ClassMap<Book>
    {
        public BookMap()
        {
            Map(r => r.BookID).Name("Book_ID");
            Map(r => r.Title).Name("Book_Title");
            Map(r => r.NumberofPages).Name("#_of_pages").Convert(r => $"{r.Value.NumberofPages} [pages]");
            Map(r => r.CreatedDate).Name("Created_Date").TypeConverterOption.Format("MM/dd/yyyy, hh:mm:ss tt");
            Map(r => r.UpdatedDate).Name("Updated_Date").TypeConverterOption.Format("MM/dd/yyyy, hh:mm:ss tt");
        }
    }






    public class BookMapReading : ClassMap<Book>
    {

        private int myNumberBook (int r)
        {
            string b = string.Empty;
            int val=0;
            string num = (r).ToString();
            

            for (int i = 0; i < num.Length; i++)
            {
                if (Char.IsDigit(num[i]))
                    b += num[i];
            }
            if (b.Length > 0)
                val = int.Parse(b);
            return val;
        }


        public BookMapReading()
        {
            Map(r => r.BookID).Name("Book_ID");
            Map(r => r.Title).Name("Book_Title");
            //Map(r => r.NumberofPages).Name("#_of_pages").Convert(r => { myNumberBook(r.Value.NumberofPages)
            Map(r => r.CreatedDate).Name("Created_Date").TypeConverterOption.Format("MM/dd/yyyy, hh:mm:ss tt");
            Map(r => r.UpdatedDate).Name("Updated_Date").TypeConverterOption.Format("MM/dd/yyyy, hh:mm:ss tt");
        }
    }





    public class Book
    {
        public int BookID { set; get; }
        public string Title { set; get; }
        public int NumberofPages { set; get; }
        public DateTime CreatedDate { set; get; }
        public DateTime UpdatedDate { set; get; }
    }

   

}



