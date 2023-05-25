using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HELMo_bilite.Models
{
    public class Admin
    {
        [Key]
        [ForeignKey("UserId")]
        public string UserId { get; set; }
    }
}
