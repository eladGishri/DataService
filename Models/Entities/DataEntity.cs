using System;
using System.ComponentModel.DataAnnotations;

namespace DataService.Models.Entities
{
    /// <summary>
    /// Represents a basic data entity with an identifier, value, and creation timestamp.
    /// </summary>
    public class DataEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier for the data entity.
        /// </summary>
        [Key]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the value associated with the data entity.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the data entity was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}