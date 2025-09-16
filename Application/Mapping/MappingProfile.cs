using AutoMapper;
using DataService.Application.DTOs;
using DataService.Models.Entities;

namespace DataService.Application.Mapping
{
    /// <summary>
    /// AutoMapper profile configuration that defines mapping relationships between domain entities and data transfer objects.
    /// This profile enables bidirectional mapping between <see cref="DataEntity"/> and <see cref="DataDto"/>.
    /// </summary>
    public class MappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MappingProfile"/> class and configures the mapping rules.
        /// Sets up bidirectional mapping between <see cref="DataEntity"/> and <see cref="DataDto"/> objects.
        /// </summary>
        /// <remarks>
        /// The ReverseMap() call creates mappings in both directions:
        /// - DataEntity to DataDto (for outbound API responses)
        /// - DataDto to DataEntity (for inbound API requests)
        /// </remarks>
        public MappingProfile()
        {
            CreateMap<DataEntity, DataDto>().ReverseMap();
        }
    }
}