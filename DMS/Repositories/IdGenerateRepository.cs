using DMS.Enums;
using DMS.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Repositories
{
    public interface IIdGenerateRepository
    {
        Task<long> GetCounter();
        Task<List<long>> ListCounter(long countElement);
    }
    public class IdGenerateRepository : IIdGenerateRepository
    {
        private DataContext DataContext;
        public IdGenerateRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        public async Task<long> GetCounter()
        {
            var Counter = await DataContext.IdGenerate
                .Where(x => x.IdGenerateTypeId == IdGenerateTypeEnum.STORE.Id)
                .Where(x => x.Used)
                .MaxAsync(x => (long?)x.Counter) ?? 0;

            //chưa có cái nào đc sử dụng
            if (Counter == 0)
            {
                await DataContext.IdGenerate
                .Where(x => x.IdGenerateTypeId == IdGenerateTypeEnum.STORE.Id)
                .Where(x => x.Counter == 1)
                .UpdateFromQueryAsync(x => new IdGenerateDAO
                {
                    Used = true
                });
                return 1;
            }
            else
            {
                await DataContext.IdGenerate
                .Where(x => x.IdGenerateTypeId == IdGenerateTypeEnum.STORE.Id)
                .Where(x => x.Counter == Counter + 1)
                .UpdateFromQueryAsync(x => new IdGenerateDAO
                {
                    Used = true
                });
                return Counter + 1;
            }
        }

        public async Task<List<long>> ListCounter(long countElement)
        {
            var Counter = await DataContext.IdGenerate
                .Where(x => x.IdGenerateTypeId == IdGenerateTypeEnum.STORE.Id)
                .Where(x => x.Used)
                .MaxAsync(x => (long?)x.Counter) ?? 0;

            List<long> Counters = new List<long>();

            await DataContext.IdGenerate
            .Where(x => x.IdGenerateTypeId == IdGenerateTypeEnum.STORE.Id)
            .Where(x => Counter + 1 <= x.Counter && x.Counter <= countElement)
            .UpdateFromQueryAsync(x => new IdGenerateDAO
            {
                Used = true
            });

            for (long i = Counter + 1; i <= Counter + countElement; i++)
            {
                Counters.Add(i);
            }

            return Counters;
        }
    }
}
