using Client.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Repositories
{
    public interface IReportRepository
    {
        ReportSummary GetReportData(int timeFilter);
    }
}
