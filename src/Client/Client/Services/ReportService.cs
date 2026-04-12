using Client.Models;
using Client.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Client.Services
{
    public class ReportService
    {
        private readonly IReportRepository _repository;

        public ReportService(IReportRepository repository)
        {
            _repository = repository;
        }

        public async Task<ReportSummary> FetchReportMetricsAsync(int timeFilter)
        {
            return await _repository.GetReportData(timeFilter);
        }

        public async Task<ReportSummary> FetchReportMetricsByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            return await _repository.GetReportDataByDateRangeAsync(fromDate, toDate);
        }
    }
}
