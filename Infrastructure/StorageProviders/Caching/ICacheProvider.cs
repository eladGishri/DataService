using DataService.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.StorageProviders.Caching
{
    public interface ICacheProvider
    {
        //For Future functionality expansion
        //Task<DataEntity> GetByIdAsync(string key);
        //Task<bool> WriteAsync(DataEntity entity);
        //Task<bool> UpdateAsync(DataEntity entity);
    }
}
