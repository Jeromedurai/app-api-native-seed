namespace Tenant.Query.Model.Email
{
    /// <summary>
    /// Toggle payload for the activate/deactivate endpoints.
    /// </summary>
    public class SetActiveRequest
    {
        public bool Active { get; set; }
    }
}
