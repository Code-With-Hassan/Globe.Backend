using Globe.Core.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Globe.Shared.Entities
{
    /// <summary>
    /// The address entity.
    /// </summary>
    public class AddressEntity : BaseEntity
    {
        /// <summary>
        /// Gets or sets the street name.
        /// </summary>
        [Required]
        public string StreetName { get; set; }

        /// <summary>
        /// Gets or sets the street number.
        /// </summary>
        [Required]
        public string StreetNumber { get; set; }

        /// <summary>
        /// Gets or sets the post code.
        /// </summary>
        public string? PostCode { get; set; }

        /// <summary>
        /// Gets or sets the city id.
        /// </summary>
        [Required]
        public long CityId { get; set; }

        /// <summary>
        /// Gets or sets the state / province / region.
        /// </summary>
        public string? StateProvinceRegion { get; set; }
    }
}
