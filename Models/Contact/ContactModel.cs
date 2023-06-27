using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Models.Contact {
    [Table("Contact")]
    public class ContactModel
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(50)]
        [Required(ErrorMessage ="Phải nhập {0}")]
        [Display(Name ="Họ và tên")]
        public string FullName{ get; set; }

        [StringLength(50)]
        [Display(Name ="Số điện thoại")]
        [Phone(ErrorMessage = "{0} không hợp lệ")]
        public string Phone { get; set; }

        [StringLength(100)]
        [EmailAddress(ErrorMessage = "{0} không hợp lệ")]
        [Required(ErrorMessage ="Phải nhập {0}")]
        [Display(Name ="Email")]
        public string Email { get; set; }

        [Display(Name ="Nội dung")]
        public string? Message { get; set;}
        
        public DateTime? DateSent { get; set; }

    }
}