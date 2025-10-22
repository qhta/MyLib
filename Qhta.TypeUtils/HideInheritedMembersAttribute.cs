using System;

/// <summary>
/// Specifies that class members inherited from base classes should be hidden during GetMembersByInheritance operations.
/// </summary>
/// <remarks>This attribute can be applied to a class to indicate that only the members defined directly within
/// the class should be considered during XML serialization. Inherited members from base classes will be
/// excluded.</remarks>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Class)]
public class HideInheritedMembersAttribute : Attribute
{
}