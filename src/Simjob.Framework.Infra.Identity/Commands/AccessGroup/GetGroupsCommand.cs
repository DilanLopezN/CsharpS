namespace Simjob.Framework.Infra.Identity.Commands.AccessGroup
{
    public class GetGroupsCommand
    {
        public string Id { get; set; }
        public string GroupName { get; set; }

        public int? Cd_empresa { get; set; }
        public int TotalUsersInGroup { get; set; }
    }
}
