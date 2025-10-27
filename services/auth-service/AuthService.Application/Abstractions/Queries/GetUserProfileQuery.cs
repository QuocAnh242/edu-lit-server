using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Abstractions.Queries
{
    public class GetUserProfileQuery
    {
        public Guid UserId { get; set; }
    }
}
