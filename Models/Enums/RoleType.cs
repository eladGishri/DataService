using System;

namespace DataService.Models.Enums
{
    /// <summary>
    /// Defines the types of user roles within the system.
    /// </summary>
    public enum RoleType
    {
        /// <summary>
        /// Represents a user with administrative privileges.
        /// </summary>
        Admin,

        /// <summary>
        /// Represents a standard user with limited access.
        /// </summary>
        User
    }
}