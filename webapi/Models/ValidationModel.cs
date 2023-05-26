namespace webapi.Models
{
    public class ValidationModel
    {
        public ValidationModel()
        {

        }
        public ValidationModel(string file)
        {
            CSVFile = file;
        }
        public int ID { get; set; }
        public string CSVFile { get; set; }
        public int NoOfEmptyRows { get; set; } = 0;
        public int NoOfDuplicateRows { get; set; } = 0;
        public int NoOfInvalidRows { get; set; } = 0;   
        public int TotalRecords { get; set; }
        public bool isFileEmpty { get; set; }  
        public DateTime DateOfUpload { get; set; } = DateTime.Now;
    }
}
