using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Asp_MVC.Models
{
    [Table("Advertisements")]
    public class Advertisement
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        public long UserId { get; set; } 

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [Column("title")]
        public string Title { get; set; } = string.Empty;

        [Column("description")]
        public string Description { get; set; } = string.Empty;

        [Column("price")]
        public string Price { get; set; } = string.Empty;

        [Column("duration")]
        public string Duration { get; set; } = string.Empty;

        [Column("created_at")]
        public string CreatedAt { get; set; } = string.Empty;
    }
}
