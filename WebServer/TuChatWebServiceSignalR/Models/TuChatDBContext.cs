using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TuChatWebServiceSignalR.Models
{
    public class TuChatDBContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<UserToken> UserTokens { get; set; }

        public TuChatDBContext(DbContextOptions<TuChatDBContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public TuChatDBContext() { }
    }
}
