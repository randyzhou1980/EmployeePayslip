namespace PayslipService.TaxRepository
{
    public interface ITaxService
    {
        decimal MinAnnualSalary { get; }
        decimal? MaxAnnualSalary { get; }

        decimal CalIncomeTax(decimal annualSalary);
    }
}
