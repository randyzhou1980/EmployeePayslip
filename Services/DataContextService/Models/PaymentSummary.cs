using System;

namespace DataContextService.Models
{
    public class PaymentSummary
    {
        public string Id { get; set; }
        public Employee EmployeeInfo { get; set; }
        public decimal AnnualSalary { get; set; }
        public decimal SuperRate { get; set; }
        public DateTime? PaymentStartDate { get; set; }
    }
}
