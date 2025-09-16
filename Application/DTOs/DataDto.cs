namespace DataService.Application.DTOs
{
    /// <summary>
    /// Data Transfer Object representing a data entity for API responses and client communication.
    /// This DTO contains the essential properties of a data item including its identifier, content, and creation timestamp.
    /// </summary>
    public class DataDto
    {
        /// <summary>
        /// Gets or sets the unique identifier of the data entity.
        /// This identifier is used to reference and retrieve the specific data item from storage.
        /// </summary>
        /// <value>A string representing the unique ID of the data entity, typically a GUID.</value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the actual data content stored in the entity.
        /// This contains the business data that was saved and retrieved from the storage system.
        /// </summary>
        /// <value>A string containing the data content.</value>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the timestamp when the data entity was created or last updated.
        /// This property tracks when the data was initially saved or most recently modified in the system.
        /// </summary>
        /// <value>A <see cref="DateTime"/> representing the creation or last update time.</value>
        public DateTime CreatedAt { get; set; }
    }
}