using System.Collections.Generic;
using Simjob.Framework.Infra.Identity.Entities;

namespace Simjob.Framework.Infra.Identity.Interfaces
{
    public interface IProfileService
    {
        public void Register(Profile profile);
        public Profile GetProfileById(string id);
        public Profile GetByTenanty(string tenanty);
        public void UpdateProfile(Profile profile, string id);
        public void UpdateProfileLogo(Profile profile, string id);
        public void DeleteProfile(string id);
        List<Profile> GetProfiles();
    }
}

