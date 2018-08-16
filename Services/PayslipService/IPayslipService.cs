namespace PayslipService
{
    public interface IPayslipService
    {
        string PayslipType { get; }
        void GenerateEmployeePayslip();
    }
}
