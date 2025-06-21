using Microsoft.EntityFrameworkCore;
using RafLab.Core.Infrastucture.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RafLab.Core.Infrastucture
{
    public class ReqresUserDbContext :DbContext
    {
        public ReqresUserDbContext(DbContextOptions<ReqresUserDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; } = null!;

    }
}
