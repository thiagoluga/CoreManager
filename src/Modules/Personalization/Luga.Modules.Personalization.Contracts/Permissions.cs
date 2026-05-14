namespace Luga.Modules.Personalization.Contracts;

/// <summary>
/// String constants for the permission codes the platform checks for in
/// <c>IPermissionService.HasPermission</c>. Lives in Contracts so any module
/// can reference the codes without pulling Personalization internals.
/// </summary>
/// <remarks>
/// Format: <c>{module}.{resource}.{action}</c>. New codes are added to the
/// owning module's static class, plus reflected in <c>PermissionRegistry</c>
/// for the admin UI to enumerate.
/// </remarks>
public static class Permissions
{
    public static class Customers
    {
        public const string Read = "customers.read";
        public const string Write = "customers.write";
        public const string Delete = "customers.delete";
    }

    public static class Payments
    {
        public const string InvoicesRead = "payments.invoices.read";
        public const string InvoicesWrite = "payments.invoices.write";
        public const string PlansManage = "payments.plans.manage";
    }

    public static class Personalization
    {
        public const string RolesManage = "personalization.roles.manage";
        public const string UsersManage = "personalization.users.manage";
    }

    public static class Jobs
    {
        public const string DashboardAccess = "jobs.dashboard";
    }
}
