using System.Collections.ObjectModel;
using Domain.Authorization.Events;
using Domain.Common.Abstractions;
using Domain.Common.Exceptions;

namespace Domain.Authorization;

/// <summary>
/// Permission entity'si
/// İzinleri temsil eder
/// </summary>
public class Permission : BaseEntity<Guid>, IAggregateRoot<Guid>
{
    public string Name { get; private set; }
    public string SystemName { get; private set; }
    public string? Description { get; private set; }
    public string? Category { get; private set; }
    public bool IsSystemPermission { get; private set; }
    public string? RequiredClaims { get; private set; }
    public bool IsActive { get; private set; } = true;

    private readonly List<PermissionGrant> _grants = new();
    private readonly List<PermissionEndpoint> _endpoints = new();

    public IReadOnlyCollection<PermissionGrant> Grants => _grants.AsReadOnly();
    public IReadOnlyCollection<PermissionEndpoint> Endpoints => _endpoints.AsReadOnly();


    protected Permission() { } // EF Core için

    public static Permission Create(
        string name,
        string systemName,
        string? description = null,
        string? category = null,
        bool isSystemPermission = false)
    {
        var permission = new Permission
        {
            Id = Guid.NewGuid(),
            Name = name,
            SystemName = systemName,
            Description = description,
            Category = category,
            IsSystemPermission = isSystemPermission
        };

        permission.AddDomainEvent(new PermissionCreatedDomainEvent(permission.Id, permission.SystemName));
        return permission;
    }

    public void Update(string name, string? description, string? category)
    {
        if (IsSystemPermission)
            throw new DomainException("System permissions cannot be modified");

        Name = name;
        Description = description;
        Category = category;

        AddDomainEvent(new PermissionUpdatedDomainEvent(Id, SystemName));
    }

    public void SetRequiredClaims(string claims)
    {
        if (IsSystemPermission)
            throw new DomainException("System permissions cannot be modified");

        RequiredClaims = claims;
        AddDomainEvent(new PermissionClaimsUpdatedDomainEvent(Id, claims));
    }
    
    public void AddEndpoint(string controller, string action, string httpMethod, string route)
    {
        var endpoint = PermissionEndpoint.Create(Id, controller, action, httpMethod, route);
        _endpoints.Add(endpoint);
    }

    public void RemoveEndpoint(Guid endpointId)
    {
        var endpoint = _endpoints.FirstOrDefault(e => e.Id == endpointId);
        if (endpoint != null)
            _endpoints.Remove(endpoint);
    }

    public int PendingChanges { get; }
}