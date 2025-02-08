using Domain.Common.Enumerations;

namespace Domain.Security;

/// <summary>
/// Uygulama permission'ları
/// </summary>
public class Permission(int id, string code, string group, string description)
    : Enumeration(id, code)
{
    #region Users
    public static Permission UsersView = new(1, "Users.View", "Users", "View users");
    public static Permission UsersCreate = new(2, "Users.Create", "Users", "Create users");
    public static Permission UsersEdit = new(3, "Users.Edit", "Users", "Edit users");
    public static Permission UsersDelete = new(4, "Users.Delete", "Users", "Delete users");
    #endregion

    #region Roles
    public static Permission RolesView = new(10, "Roles.View", "Roles", "View roles");
    public static Permission RolesCreate = new(11, "Roles.Create", "Roles", "Create roles");
    public static Permission RolesEdit = new(12, "Roles.Edit", "Roles", "Edit roles");
    public static Permission RolesDelete = new(13, "Roles.Delete", "Roles", "Delete roles");
    #endregion

    #region Products
    public static Permission ProductsView = new(20, "Products.View", "Products", "View products");
    public static Permission ProductsCreate = new(21, "Products.Create", "Products", "Create products");
    public static Permission ProductsEdit = new(22, "Products.Edit", "Products", "Edit products");
    public static Permission ProductsDelete = new(23, "Products.Delete", "Products", "Delete products");
    #endregion

    /// <summary>
    /// Permission'ın kod adı
    /// </summary>
    public string Code { get; } = code;

    /// <summary>
    /// Permission'ın grubu
    /// </summary>
    public string Group { get; } = group;

    /// <summary>
    /// Permission'ın açıklaması
    /// </summary>
    public string Description { get; } = description;

    /// <summary>
    /// Tüm permission'ları gruplar
    /// </summary>
    public static IEnumerable<IGrouping<string, Permission>> GetGroups()
    {
        return GetAll<Permission>().GroupBy(p => p.Group);
    }

    /// <summary>
    /// Verilen grup adına göre permission'ları getirir
    /// </summary>
    public static IEnumerable<Permission> GetByGroup(string group)
    {
        return GetAll<Permission>().Where(p => p.Group == group);
    }

    /// <summary>
    /// Verilen kod adına göre permission'ı getirir
    /// </summary>
    public static Permission? GetByCode(string code)
    {
        return GetAll<Permission>().FirstOrDefault(p => p.Code == code);
    }
}