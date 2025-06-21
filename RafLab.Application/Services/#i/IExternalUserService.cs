using RafLab.Application.Dto;
using RafLab.Core.Infrastucture.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RafLab.Application.Services._i
{
    public interface IExternalUserService
    {
        Task<UserDto?> GetUserByIdAsync(int userId);
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<IEnumerable<UserDto>> GetCacheUser();

    }
}
