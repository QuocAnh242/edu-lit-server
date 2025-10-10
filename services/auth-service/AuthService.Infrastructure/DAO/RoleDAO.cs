using AuthService.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Infrastructure.DAO
{
    public class RoleDAO
    {
        private readonly AuthDbContext _dbcontext;

        public RoleDAO(AuthDbContext context)
        {
            _dbcontext = context;
        }

    }
}
