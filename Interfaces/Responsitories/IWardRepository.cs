using Project_LMS.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project_LMS.Interfaces.Repositories
{
    public interface IWardRepository
    {
        Task<Ward> GetByIdAsync(int id);
    }
}