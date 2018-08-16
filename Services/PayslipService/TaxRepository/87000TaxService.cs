namespace PayslipService.TaxRepository
{
    public class _87000TaxService: TaxServiceBase
    {
        public override decimal MinAnnualSalary => 87001;
        public override decimal? MaxAnnualSalary => 180000;
        public override decimal baseTax => 19822;
        public override decimal TaxRate => 0.37m;
    }
}
