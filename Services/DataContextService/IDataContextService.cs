using DataContextService.Models;
using System.Collections.Generic;

namespace DataContextService
{
    public interface IDataContextService
    {
        bool IsValidDataContextSettings();
        IEnumerable<PaymentSummary> GetPaymentSummaries();
        bool SavePayslips(IEnumerable<PayslipSummary> payslipSummaries);
    }
}
