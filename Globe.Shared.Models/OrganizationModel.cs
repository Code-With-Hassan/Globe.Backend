using System.ComponentModel.DataAnnotations;

namespace Globe.Shared.Models
{
    /// <summary>
    /// The organization Model.
    /// </summary>
    public class OrganizationModel
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = "";

        /// <summary>
        /// Gets or sets the address id.
        /// </summary>
        public long? AddressId { get; set; }

        /// <summary>
        /// Gets or sets the phone.
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        [MaxLength(50)]
        public string Email { get; set; }

        /// <summary>
        /// IsActive
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// ContactIds
        /// </summary>
        public List<long> ContactIds { get; set; }
    }
}
