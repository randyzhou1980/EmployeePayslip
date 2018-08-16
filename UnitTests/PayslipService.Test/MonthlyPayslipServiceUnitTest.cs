using DataContextService.FileContext;
using DataContextService.Models;
using LoggerService;
using Microsoft.Extensions.Options;
using System;
using Xunit;

namespace PayslipService.Test
{
    public class MonthlyPayslipServiceUnitTest
    {
        #region Constructor
        private readonly MonthlyPayslipService _payslipService;
        public MonthlyPayslipServiceUnitTest()
        {
            var config = Options.Create(new FileContextSetting()
            {
                DataFile = new DataFileSetting() { Directory = "./TestFiles" },
                OutputFile = new DataFileSetting() { Directory = "./TestFiles" }
            });

            var logService = new ConsoleService();
            var contextService = new FileContextService(config, logService);
            _payslipService = new MonthlyPayslipService(contextService, logService);
        }
        #endregion

        [Theory]
        [InlineData("David", "Rudd", "3/1/2018", 18200, 10, "01-March-2018 - 31-March-2018", 1516.67, 0, 1516.67, 151.67)]
        [InlineData("David", "Rudd", "3/1/2018", 37000, 10, "01-March-2018 - 31-March-2018", 3083.33, 297.67, 2785.66, 308.33)]
        [InlineData("David", "Rudd", "3/1/2018", 87000, 10, "01-March-2018 - 31-March-2018", 7250.00, 1651.83, 5598.17, 725.00)]
        [InlineData("David", "Rudd", "3/1/2018", 180000, 10, "01-March-2018 - 31-March-2018", 15000.00, 4519.33, 10480.67, 1500.00)]
        [InlineData("David", "Rudd", "3/1/2018", 200000, 10, "01-March-2018 - 31-March-2018", 16666.67, 5269.33, 11397.34, 1666.67)]
        public void CreatePayslipSummary_Payslip_ReturnExpectedResult(
            string firstName, string lastName, string paymentStartDate, decimal annualSalary, decimal superRate,
            string expectedPayPeriod, decimal expectedGrossIncome, decimal expectedIncomeTax, decimal expectedNetIncome, decimal expectedSuper)
        {
            var paymentSummary = new PaymentSummary()
            {
                EmployeeInfo = new Employee() { FirstName = firstName, LastName = lastName },
                PaymentStartDate = DateTime.Parse(paymentStartDate),
                AnnualSalary = annualSalary,
                SuperRate = superRate
            };

            var payslip = _payslipService.CreatePayslipSummary(paymentSummary);

            Assert.NotNull(payslip);
            Assert.Equal(firstName, payslip.EmployeeInfo.FirstName);
            Assert.Equal(lastName, payslip.EmployeeInfo.LastName);
            Assert.Equal(expectedPayPeriod, payslip.PayPeriod);
            Assert.Equal(expectedGrossIncome, payslip.GrossIncome);
            Assert.Equal(expectedIncomeTax, payslip.IncomeTax);
            Assert.Equal(expectedNetIncome, payslip.NetIncome);
            Assert.Equal(expectedSuper, payslip.Super);
        }
    }
}
