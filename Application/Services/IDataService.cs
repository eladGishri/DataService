using DataService.Application.DTOs;
using DataService.Models.Entities;

namespace DataService.Application.Services
{
    public interface IDataService
    {
        Task<DataDto> GetByIdAsync(string id);
        Task<string> SaveAsync(string data);
        Task<string> UpdateAsync(string id, string data);
    }
}
