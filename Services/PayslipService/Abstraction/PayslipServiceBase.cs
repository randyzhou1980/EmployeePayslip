using DataContextService;
using DataContextService.Models;
using LoggerService;
using PayslipService.TaxRepository;
using System;

namespace PayslipService.Abstraction
{
    public abstract class PayslipServiceBase: IPayslipService
    {
        #region Constructor
        public PayslipServiceBase(IDataContextService contextService, ILogService logService)
        {
            ContextService = contextService;
            LogService = logService;
            TaxServiceFactory = new TaxServiceFactory();
        }
        #endregion

        #region Properties
        protected IDataContextService ContextService { get; private set; }
        protected ILogService LogService { get; private set; }
        public abstract string PayslipType { get; }
        protected TaxServiceFactory TaxServiceFactory { get; private set; }
        #endregion

        public abstract void GenerateEmployeePayslip();
        public abstract PayslipSummary CreatePayslipSummary(PaymentSummary payment);
        public abstract string GetPayPeriod(DateTime? paymentStartDate);
        public abstract decimal CalGrossIncome(decimal annualSalary);
        public abstract decimal CalIncomeTax(decimal annualSalary);
        public decimal CalNetIncome(decimal grossIncome, decimal incomeTax)
        {
            return grossIncome - incomeTax;
        }
        public decimal CalSuper(decimal grossIncome, decimal superRate)
        {
            return Math.Round(grossIncome * superRate / 100, 2);
        }
    }
}
