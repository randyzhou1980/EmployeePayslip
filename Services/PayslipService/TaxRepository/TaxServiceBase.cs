namespace PayslipService.TaxRepository
{
    public abstract class TaxServiceBase: ITaxService
    {
        public abstract decimal MinAnnualSalary { get; }
        public abstract decimal? MaxAnnualSalary { get; }
        public abstract decimal baseTax { get; }
        public abstract decimal TaxRate { get; }

        public virtual decimal CalIncomeTax(decimal annualSalary)
        {
            return baseTax + (annualSalary - (MinAnnualSalary - 1)) * TaxRate;
        }
    }
}
