using DataContextService.Abstraction;
using DataContextService.Models;
using LoggerService;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataContextService.FileContext
{
    public class FileContextService: ContextServiceBase
    {
        #region Constructor
        private const string _resultHeader = "Full Name,Pay Period,Gross Income,Income Tax,Net Income,Super";
        private readonly FileContextSetting _settings;
        public FileContextService(IOptions<FileContextSetting> config, ILogService logService)
            :base(logService)
        {
            _settings = config.Value;
        }
        #endregion

        // Validation
        public override bool IsValidDataContextSettings()
        {
            if (_settings == null || _settings.DataFile == null || _settings.OutputFile == null)
            {
                LogService.LogError("Cofiguration settings of data files are missing.");
                return false;
            }

            // Data File Settings Validation
            if (string.IsNullOrWhiteSpace(_settings.DataFile.Directory))
            {
                LogService.LogError("Cofiguration of data file directory cannot be blank.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(_settings.OutputFile.Directory))
            {
                LogService.LogError("Cofiguration of output file directory cannot be blank.");
                return false;
            }

            return true;
        }

        // Read Data
        public override IEnumerable<PaymentSummary> GetPaymentSummaries()
        {
            var dataFiles = GetDataFiles();
            if(dataFiles == null)
            {
                return null;
            }

            Parallel.ForEach(dataFiles, (dataFile) =>
            {
                ProcessDataFile(dataFile);
            });

            if (PaymentSummaries.Count == 0)
            {
                LogService.LogInfo("No data found.");
                return null;
            }

            return PaymentSummaries.OrderBy(p => p.Id);
        }

        public IEnumerable<FileInfo> GetDataFiles()
        {
            var directory = new DirectoryInfo(_settings.DataFile.Directory);
            
            // Check Directory for Data Files
            if (!directory.Exists)
            {
                LogService.LogError($"Cannot find data files folder {_settings.DataFile.Directory}");
                return null;
            }

            // Check File Extension
            var extension = string.IsNullOrWhiteSpace(_settings.DataFile.Extension) ? "*" : _settings.DataFile.Extension;

            // Get Data Files
            var dataFiles = directory.GetFiles($"*.{extension}");
            if (dataFiles.Length == 0)
            {
                LogService.LogError($"No data files under folder {_settings.DataFile.Directory}");
                return null;
            }

            return dataFiles;
        }

        private void ProcessDataFile(FileInfo dataFile)
        {
            StreamReader reader = null;

            try
            {
                reader = new StreamReader(dataFile.FullName);

                bool isFirstLine = true;
                string line = string.Empty;
                var paymentId = dataFile.Name.Remove(dataFile.Name.LastIndexOf(dataFile.Extension));

                while ((line = reader.ReadLine()) != null)
                {
                    LineToPaymentSummary(line, paymentId, isFirstLine);

                    isFirstLine = false;
                }
            }
            catch(Exception ex)
            {
                LogService.LogError(ex);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
        }

        public void LineToPaymentSummary(string dataRow, string paymentId, bool isFirstLine)
        {
            decimal value = 0;
            var columns = dataRow.Split(',');

            // Skip Header
            if (isFirstLine)
            {
                if(!columns.Any(field => decimal.TryParse(field, out value)))
                {
                    return;
                }
            }

            var paymentSummary = new PaymentSummary()
            {
                Id = paymentId,
                EmployeeInfo = new Employee() { FirstName = columns[0], LastName = columns[1] }
            };

            if(decimal.TryParse(columns[2], out value))
            {
                paymentSummary.AnnualSalary = value;
            }

            if (decimal.TryParse(columns[3], out value))
            {
                paymentSummary.SuperRate = value;
            }

            DateTime dateTime;
            if (DateTime.TryParse(columns[4], out dateTime))
            {
                paymentSummary.PaymentStartDate = dateTime;
            }

            PaymentSummaries.Add(paymentSummary);
        }

        // Write Data
        public override bool SavePayslips(IEnumerable<PayslipSummary> payslipSummaries)
        {
            StreamWriter writer = null;
            string currentId = string.Empty;

            try
            {
                foreach(var payslip in payslipSummaries)
                {
                    if(currentId != payslip.Id)
                    {
                        currentId = payslip.Id;

                        if(writer != null)
                        {
                            writer.Close();
                        }

                        writer = new StreamWriter(GetFileFullName(payslip.Id), false, Encoding.UTF8);
                        writer.WriteLine(_resultHeader);
                    }

                    writer.WriteLine(PayslipToString(payslip));
                }

                return true;
            }
            catch(Exception ex)
            {
                LogService.LogError(ex);
                return false;
            }
            finally
            {
                if(writer != null)
                {
                    writer.Close();
                }
            }
        }

        public string GetFileFullName(string payslipId)
        {
            var directoryInfo = new DirectoryInfo(_settings.OutputFile.Directory);
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }

            var extension = string.IsNullOrWhiteSpace(_settings.OutputFile.Extension) ? "csv" : _settings.OutputFile.Extension;

            return $"{directoryInfo.FullName}\\{payslipId}.{extension}";
        }

        public string PayslipToString(PayslipSummary payslip)
        {
            return $"{payslip.EmployeeInfo.FullName},{payslip.PayPeriod},{payslip.GrossIncome},{payslip.IncomeTax},{payslip.NetIncome},{payslip.Super}";
        }
    }
}
