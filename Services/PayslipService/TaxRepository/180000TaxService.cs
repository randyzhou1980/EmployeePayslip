namespace PayslipService.TaxRepository
{
    public class _180000TaxService: TaxServiceBase
    {
        public override decimal MinAnnualSalary => 180001;
        public override decimal? MaxAnnualSalary => null;
        public override decimal baseTax => 54232;
        public override decimal TaxRate => 0.45m;
    }
}
