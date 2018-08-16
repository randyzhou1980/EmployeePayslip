using System.Collections.Generic;
using System.Linq;

namespace PayslipService.TaxRepository
{
    public class TaxServiceFactory
    {
        private List<ITaxService> _taxServiceRepo;

        public ITaxService GetTaxService(decimal annualSalary)
        {
            if(_taxServiceRepo == null)
            {
                // Lazy Loading Services
                LoadTaxServices();
            }

            return _taxServiceRepo
                .Where(t => t.MinAnnualSalary <= annualSalary && (!t.MaxAnnualSalary.HasValue || t.MaxAnnualSalary.Value >= annualSalary))
                .FirstOrDefault();
        }

        private void LoadTaxServices()
        {
            _taxServiceRepo = new List<ITaxService>();

            _taxServiceRepo.Add(new NilTaxService());
            _taxServiceRepo.Add(new _18200TaxService());
            _taxServiceRepo.Add(new _37000TaxService());
            _taxServiceRepo.Add(new _87000TaxService());
            _taxServiceRepo.Add(new _180000TaxService());
        }
    }
}
