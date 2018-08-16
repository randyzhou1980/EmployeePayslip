using DataContextService.Models;
using LoggerService;
using System.Collections.Generic;

namespace DataContextService.Abstraction
{
    public abstract class ContextServiceBase: IDataContextService
    {
        #region Constructor
        public ContextServiceBase(ILogService logService)
        {
            LogService = logService;
            PaymentSummaries = new List<PaymentSummary>();
        }
        #endregion

        #region Properties
        protected ILogService LogService { get; private set; }
        public List<PaymentSummary> PaymentSummaries { get; set; }
        #endregion

        public abstract bool IsValidDataContextSettings();
        public abstract IEnumerable<PaymentSummary> GetPaymentSummaries();
        public abstract bool SavePayslips(IEnumerable<PayslipSummary> payslipSummaries);
    }
}
