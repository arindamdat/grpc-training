using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UserManagement.Privileges;

namespace GrpcService.Services
{
    public class PrivilegeSetGrpcService : PrivilegeSetService.PrivilegeSetServiceBase
    {
        public override async Task GetPrivilegeSets(GetPrivilegeSetsRequest request, IServerStreamWriter<PrivilegeSet> responseStream, ServerCallContext context)
        {
            var dbPrivilegeSets = await GetAllDatabasePrivileges();
            dbPrivilegeSets.ForEach(async (ps) => await responseStream.WriteAsync(ps));
        }
        public override async Task<PrivilegeSetListResponse> GetPrivilegeSetList(GetPrivilegeSetsRequest request, ServerCallContext context)
        {
            var response = new PrivilegeSetListResponse();
            var dbPrivilegeSets = await GetAllDatabasePrivileges();
            response.Items.AddRange(dbPrivilegeSets);
            return response;
        }

        private Task<List<PrivilegeSet>> GetAllDatabasePrivileges()
        {
            var dbPrivilegeSets = new List<PrivilegeSet>();
            var allowId = "440a9616-d1bf-4528-a6e7-6cb40533a195";
            var ps1 = new PrivilegeSet
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Privilege-Set-1"
            };
            ps1.Privileges.Add(new Privilege
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Manage Session",
                AccessLevel = new AccessLevel { Id = allowId, Name = "Allow" }
            });
            var ps2 = new PrivilegeSet
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Privilege-Set-2"
            };
            ps2.Privileges.Add(new Privilege
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Manage Projects",
                AccessLevel = new AccessLevel { Id = allowId, Name = "Allow" }
            });

            dbPrivilegeSets.AddRange(new[] { ps1, ps2 });
            return Task.FromResult(dbPrivilegeSets);
        }
    }
}
