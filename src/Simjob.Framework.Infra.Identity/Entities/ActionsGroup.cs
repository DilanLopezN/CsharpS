namespace Simjob.Framework.Infra.Identity.Entities
{
    public class ActionsGroup
    {
        public ActionsGroup(string actionName, string actionId, bool permission)
        {
            ActionName = actionName;
            Permissions = permission;
            ActionId = actionId;
        }

        public string ActionName { get; set; }
        public bool Permissions { get; set; } // true or false
        public string ActionId { get; set; }

    }
}
