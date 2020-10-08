using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FullStackJobs.AuthServer.Infrastructure.Data
{
    public interface IUserRepository
    {
        Task InsertEntity(string role, string id, string fullName);
    }
}
