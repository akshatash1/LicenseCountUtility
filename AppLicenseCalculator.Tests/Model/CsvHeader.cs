namespace AppLicenseCalculator.Tests.Model
{
    /// <summary>
    /// Class used to create Header of CSV file (Unit test only)
    /// </summary>
    public class CsvHeader
    {
        public int ComputerID { get; set; }
        public int UserID { get; set; }
        public int ApplicationID { get; set; }
        public string ComputerType { get; set; }
        public string Comment { get; set; }
    }
}
