using CsvHelper.Configuration;

namespace AppLicenseCalculator.Models
{
    public class InstallationDetailsMap : ClassMap<InstallationDetails>
    {
        public InstallationDetailsMap()
        {
            Map(m => m.ComputerId).Name("ComputerID");
            Map(m => m.UserId).Name("UserID");
            Map(m => m.ApplicationId).Name("ApplicationID");
            Map(m => m.ComputerType).Name("ComputerType");
            Map(m => m.Comment).Name("Comment");
        }
    }
}
