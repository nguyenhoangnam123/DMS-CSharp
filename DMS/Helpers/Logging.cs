using Common;
using Microsoft.Extensions.Configuration;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Helpers
{
    public interface ILogging : IServiceScoped
    {
        Task<bool> CreateAuditLog(object newData, object oldData, string className, [CallerMemberName]string methodName = "");
        Task<bool> CreateSystemLog(Exception ex, string className, [CallerMemberName]string methodName = "");
    }
    public class Logging : ILogging
    {
        private ICurrentContext CurrentContext;
        public Logging(IConfiguration Configuration, ICurrentContext CurrentContext)
        {
            this.CurrentContext = CurrentContext;
        }
        public async Task<bool> CreateAuditLog(object newData, object oldData, string className, [CallerMemberName] string methodName = "")
        {
            return true;
        }
        public async Task<bool> CreateSystemLog(Exception ex, string className, [CallerMemberName] string methodName = "")
        {
            return true;
        }
    }
}
