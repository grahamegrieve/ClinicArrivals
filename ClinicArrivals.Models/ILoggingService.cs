namespace ClinicArrivals.Models
{
    public interface ILoggingService
    {
        void Log(int level, string msg);
    }
}