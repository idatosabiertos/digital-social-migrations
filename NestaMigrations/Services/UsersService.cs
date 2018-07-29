using NestaMigrations.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NestaMigrations.Services
{
    public class UsersService
    {
        NestaContext context;

        public UsersService(NestaContext context)
        {
            this.context = context;
        }
        
    }
}
