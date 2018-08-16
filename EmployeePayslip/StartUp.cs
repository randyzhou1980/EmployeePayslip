using PayslipService;

namespace EmployeePayslip
{
    public class StartUp
    {
        #region Constructor
        private readonly IPayslipService _payslipService;
        public StartUp(IPayslipService payslipService)
        {
            _payslipService = payslipService;
        }
        #endregion

        public void GenerateEmployeePayslip()
        {
            _payslipService.GenerateEmployeePayslip();
        }
    }
}
