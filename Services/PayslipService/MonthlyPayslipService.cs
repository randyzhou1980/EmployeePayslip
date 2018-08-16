using DataContextService;
using DataContextService.Models;
using LoggerService;
using PayslipService.Abstraction;
using System;
using System.Collections.Generic;

namespace PayslipService
{
    public class MonthlyPayslipService: PayslipServiceBase
    {
        #region Constructor
        public MonthlyPayslipService(IDataContextService contextService, ILogService logService) 
            : base(contextService, logService)
        {

        }
        #endregion

        #region Porperties
        public override string PayslipType => "Monthly";
        #endregion

        public override void GenerateEmployeePayslip()
        {
            try
            {
                // Configuration Validation
                if (!ContextService.IsValidDataContextSettings())
                {
                    return;
                }

                // Get Payment Summary List
                LogService.LogInfo("Load Data...");
                var paymentSummaries = ContextService.GetPaymentSummaries();
                if(paymentSummaries == null)
                {
                    return;
                }

                // Generate Payslip
                LogService.LogInfo("Generate Payslip...");
                var paySlipSummaries = new List<PayslipSummary>();
                foreach (var payment in paymentSummaries)
                {
                    var payslip = CreatePayslipSummary(payment);
                    if (payslip != null)
                    {
                        paySlipSummaries.Add(payslip);
                    }
                }

                if(paySlipSummaries.Count == 0)
                {
                    return;
                }

                // Insert into Data Context
                LogService.LogInfo("Write Result...");
                if (ContextService.SavePayslips(paySlipSummaries))
                {
                    LogService.LogInfo("Payslip Created...");
                }

            }
            catch (Exception ex)
            {
                LogService.LogError(ex);
            }
        }

        public override PayslipSummary CreatePayslipSummary(PaymentSummary payment)
        {
            var payslip = new PayslipSummary()
            {
                Id = $"Payslip_{PayslipType}_{payment.Id}",
                EmployeeInfo = payment.EmployeeInfo,
                PayPeriod = GetPayPeriod(payment.PaymentStartDate),
                GrossIncome = CalGrossIncome(payment.AnnualSalary),
            };

            var incomeTax = CalIncomeTax(payment.AnnualSalary);
            if(incomeTax < 0)
            {
                LogService.LogError($"Cannot find tax formula for employee {payment.EmployeeInfo.FullName} {payment.AnnualSalary}.");
                return null;
            }
            payslip.IncomeTax = incomeTax;
            payslip.NetIncome = CalNetIncome(payslip.GrossIncome, payslip.IncomeTax);
            payslip.Super = CalSuper(payslip.GrossIncome, payment.SuperRate);

            return payslip;
        }

        public override string GetPayPeriod(DateTime? paymentStartDate)
        {
            if (!paymentStartDate.HasValue)
            {
                return string.Empty;
            }

            var lastDay = DateTime.DaysInMonth(paymentStartDate.Value.Year, paymentStartDate.Value.Month);
            return $"{paymentStartDate.Value.ToString("dd-MMMM-yyyy")} - {new DateTime(paymentStartDate.Value.Year, paymentStartDate.Value.Month, lastDay).ToString("dd-MMMM-yyyy")}";
        }
        public override decimal CalGrossIncome(decimal annualSalary)
        {
            return Math.Round(annualSalary / 12, 2);
        }
        public override decimal CalIncomeTax(decimal annualSalary)
        {
            var taxService = TaxServiceFactory.GetTaxService(annualSalary);
            if(taxService == null)
            {
                return -1;
            }

            return Math.Round(taxService.CalIncomeTax(annualSalary) / 12, 2);
        }
    }
}
