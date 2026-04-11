using Client.Models;
using Client.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Services
{
    public class ReportService
    {
        private readonly IReportRepository _repository;

        public ReportService()
        {
            _repository = new MockReportRepository();
        }

        public ReportSummary FetchReportMetrics(int timeFilter)
        {
            return _repository.GetReportData(timeFilter);
        }
    }
}
