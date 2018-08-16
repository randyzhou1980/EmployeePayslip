namespace PayslipService.TaxRepository
{
    public class _37000TaxService: TaxServiceBase
    {
        public override decimal MinAnnualSalary => 37001;
        public override decimal? MaxAnnualSalary => 87000;
        public override decimal baseTax => 3572;
        public override decimal TaxRate => 0.325m;
    }
}
