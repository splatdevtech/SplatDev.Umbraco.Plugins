using UmbracoCms.Plugins.PropertiesReport.Models;

namespace UmbracoCms.Plugins.PropertiesReport.Services;

public interface IPropertiesReportService
{
    PropertiesReportResult GetReport();
    PropertiesReportResult GetByContentType(string alias);
}
