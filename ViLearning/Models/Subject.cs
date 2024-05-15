using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ViLearning.Models
{
    public class Subject
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(30)]
        [DisplayName("Nhập tên môn học")]
        public string Name { get; set; }
    }
}
