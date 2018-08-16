namespace PayslipService.TaxRepository
{
    public class NilTaxService: TaxServiceBase
    {
        public override decimal MinAnnualSalary => 0;
        public override decimal? MaxAnnualSalary => 18200;
        public override decimal baseTax => 0;
        public override decimal TaxRate => 0;

        public override decimal CalIncomeTax(decimal annualSalary)
        {
            return 0;
        }
    }
}
