using Client.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Client.Repositories
{
    public interface IReportRepository
    {
        Task<ReportSummary> GetReportData(int timeFilter);
        Task<ReportSummary> GetReportDataByDateRangeAsync(DateTime fromDate, DateTime toDate);
    }
}
