namespace Application.Common.Constants;

/// <summary>
/// Sistem için temel permission'lar
/// </summary>
public static class SystemPermissions
{
    #region User Management
    public const string UsersView = "Users.View";
    public const string UsersCreate = "Users.Create";
    public const string UsersEdit = "Users.Edit";
    public const string UsersDelete = "Users.Delete";
    public const string UsersAssignRoles = "Users.AssignRoles";
    public const string UsersAssignPermissions = "Users.AssignPermissions";
    #endregion

    #region Role Management
    public const string RolesView = "Roles.View";
    public const string RolesCreate = "Roles.Create";
    public const string RolesEdit = "Roles.Edit";
    public const string RolesDelete = "Roles.Delete";
    public const string RolesAssignPermissions = "Roles.AssignPermissions";
    #endregion

    #region Permission Management
    public const string PermissionsView = "Permissions.View";
    public const string PermissionsCreate = "Permissions.Create";
    public const string PermissionsEdit = "Permissions.Edit";
    public const string PermissionsDelete = "Permissions.Delete";
    #endregion
}