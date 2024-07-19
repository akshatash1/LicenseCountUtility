namespace AppLicenseCalculator.Models
{
    public class InstallationDetails
    {
        public int ComputerId { get; set; }
        public int UserId { get; set; }
        public int ApplicationId { get; set; }
        public string? ComputerType { get; set; }
        public string? Comment { get; set; }
    }

}
