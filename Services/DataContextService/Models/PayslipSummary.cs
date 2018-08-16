namespace DataContextService.Models
{
    public class PayslipSummary
    {
        public string Id { get; set; }
        public Employee EmployeeInfo { get; set; }
        public string PayPeriod { get; set; }
        public decimal GrossIncome { get; set; }
        public decimal IncomeTax { get; set; }
        public decimal NetIncome { get; set; }
        public decimal Super { get; set; }
    }
}
