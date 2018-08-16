namespace PayslipService.TaxRepository
{
    public class _18200TaxService: TaxServiceBase
    {
        public override decimal MinAnnualSalary => 18201;
        public override decimal? MaxAnnualSalary => 37000;
        public override decimal baseTax => 0;
        public override decimal TaxRate => 0.19m;
    }
}
