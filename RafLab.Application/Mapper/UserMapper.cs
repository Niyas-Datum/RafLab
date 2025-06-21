using RafLab.Application.Dto;
using RafLab.Core.Infrastucture.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RafLab.Application.Mapper
{
    public static class UserMapper
    {
        public static UserDto ToDto(this User user)
        {
            return new UserDto
            {
                Id = user.Id,
                FullName = $"{user.FirstName} {user.LastName}",
                Email = user.Email,
                Avatar = user.Avatar
            };
        }

        public static IEnumerable<UserDto> ToDtoList(this IEnumerable<User> users)
        {
            return users.Select(u => u.ToDto());
        }
    }
}
