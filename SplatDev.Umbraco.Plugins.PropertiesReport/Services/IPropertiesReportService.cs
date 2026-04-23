using SplatDev.Umbraco.Plugins.PropertiesReport.Models;

namespace SplatDev.Umbraco.Plugins.PropertiesReport.Services;

public interface IPropertiesReportService
{
    PropertiesReportResult GetReport();
    PropertiesReportResult GetByContentType(string alias);
}
