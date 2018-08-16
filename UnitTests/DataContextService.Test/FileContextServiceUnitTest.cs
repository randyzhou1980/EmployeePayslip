using DataContextService.FileContext;
using DataContextService.Models;
using LoggerService;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using Xunit;

namespace DataContextService.Test
{
    public class FileContextServiceUnitTest
    {
        #region Constructor
        private FileContextService NewContextService()
        {
            var config = Options.Create(new FileContextSetting()
            {
                DataFile = new DataFileSetting() { Directory = "./TestFiles" },
                OutputFile = new DataFileSetting() { Directory = "./TestFiles" }
            });
            return new FileContextService(config, new ConsoleService());
        }
        private FileContextService NewContextService(FileContextSetting setting)
        {
            var config = Options.Create(setting);
            return new FileContextService(config, new ConsoleService());
        }
        #endregion

        [Theory]
        [InlineData(false, true, null, null)]
        [InlineData(false, false, null, "")]
        [InlineData(false, false, "", null)]
        [InlineData(false, false, "", "")]
        [InlineData(false, false, "./TestFiles", "")]
        [InlineData(true, false, "./TestFiles", "./TestFiles")]
        public void IsValidDataContextSettings_Config_ReturnExpectedValue(
            bool expectedResult, bool isNil, string dataFileDirectory, string outputFileDirectory)
        {
            FileContextSetting setting = null;

            if (!isNil)
            {
                setting = new FileContextSetting();

                if(dataFileDirectory != null)
                {
                    setting.DataFile = new DataFileSetting() { Directory = dataFileDirectory };
                }

                if (outputFileDirectory != null)
                {
                    setting.OutputFile = new DataFileSetting() { Directory = outputFileDirectory };
                }
            }


            var context = NewContextService(setting);
            Assert.Equal<bool>(expectedResult, context.IsValidDataContextSettings());
        }

        [Theory]
        [InlineData("./NonTestFiles", null)]
        [InlineData("./TestFiles", "dat")]
        public void GetDataFiles_FileInfoList_ReturnNilValue(
            string dataFileDirectory, string extension)
        {
            FileContextSetting setting = new FileContextSetting()
            {
                DataFile = new DataFileSetting() {Directory = dataFileDirectory, Extension = extension }
            };

            var context = NewContextService(setting);

            Assert.Null(context.GetDataFiles());
        }

        [Theory]
        [InlineData(2, "./TestFiles", "csv")]
        [InlineData(2, "./TestFiles", "")]
        public void GetDataFiles_FileInfoList_ReturnExpectedValue(
            int expectedResult, string dataFileDirectory, string extension)
        {
            FileContextSetting setting = new FileContextSetting()
            {
                DataFile = new DataFileSetting() { Directory = dataFileDirectory, Extension = extension }
            };

            var context = NewContextService(setting);

            Assert.Equal<int>(expectedResult, context.GetDataFiles().Count());
        }

        [Fact]
        public void LineToPaymentSummary_PaymentSummaries_SkipHeader()
        {
            var context = NewContextService();

            context.LineToPaymentSummary("a,b,c,d,e", "a1", true);

            Assert.Empty(context.PaymentSummaries);
        }

        [Theory]
        [InlineData("test01", "David", "Rudd", "", "9", "3/1/2018")]
        [InlineData("test01", "David", "Rudd", "60050", "", "3/1/2018")]
        [InlineData("test01", "David", "Rudd", "60050", "9", "")]
        [InlineData("test01", "David", "Rudd", "60050", "9", "3/1/2018")]
        public void LineToPaymentSummary_PaymentSummaries_ReturnExpectedPaymentSummary(
            string id, string firstName, string lastName, string annualSalary, string superRate, string paymentStartDate)
        {
            var context = NewContextService();

            context.LineToPaymentSummary($"{firstName},{lastName},{annualSalary},{superRate},{paymentStartDate}", id, false);

            var paymentSummary = context.PaymentSummaries[0];

            Assert.NotNull(paymentSummary);
            Assert.Equal(id, paymentSummary.Id);
            Assert.NotNull(paymentSummary.EmployeeInfo);
            Assert.Equal(firstName, paymentSummary.EmployeeInfo.FirstName);
            Assert.Equal(lastName, paymentSummary.EmployeeInfo.LastName);
            Assert.Equal(string.IsNullOrWhiteSpace(annualSalary) ? 0 : decimal.Parse(annualSalary), paymentSummary.AnnualSalary);
            Assert.Equal(string.IsNullOrWhiteSpace(superRate) ? 0 : decimal.Parse(superRate), paymentSummary.SuperRate);
            Assert.Equal(paymentStartDate, paymentSummary.PaymentStartDate.HasValue ? paymentSummary.PaymentStartDate.Value.ToShortDateString() : "");
        }

        [Theory]
        [InlineData("TestFiles", "csv")]
        [InlineData("TestFiles", "dat")]
        [InlineData("ResultTestFiles", "dat")]
        public void GetFileFullName_FileName_ReturnExpectedResult(string filePath, string extension)
        {
            FileContextSetting setting = new FileContextSetting()
            {
                OutputFile = new DataFileSetting() { Directory = $"./{filePath}", Extension = extension }
            };

            var context = NewContextService(setting);

            var filename = context.GetFileFullName("test01");
            Assert.EndsWith($"\\{filePath}\\test01.{extension}", filename);
        }

        [Fact]
        public void PayslipToString_DataRow_ReturnExpectedResult()
        {
            var context = NewContextService();
            var payslip = new PayslipSummary()
            {
                EmployeeInfo = new Employee() { FirstName = "David", LastName = "Rudd" },
                GrossIncome = 10000,
                IncomeTax = 100,
                NetIncome = 9900,
                Super = 100,
                PayPeriod = "01-03-2018 - 31-03-2018"
            };

            Assert.Equal("David Rudd,01-03-2018 - 31-03-2018,10000,100,9900,100", context.PayslipToString(payslip));
        }
    }
}
