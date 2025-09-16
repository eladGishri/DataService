using DataService.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.StorageProviders.FileStorage
{
    public interface IFileProvider
    {
        //For Future functionality expansion
        //Task<DataEntity> ReadAsync<T>(string id);
        //Task<bool> WriteAsync(DataEntity entity);
        //Task<bool> UpdateAsync(DataEntity entity);
    }
}
