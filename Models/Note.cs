using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NotinLite.Models
{
    public class Note
    {
        [Key]
        public int NoteId { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [NotMapped]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; } = string.Empty;

        public string EncryptedContent { get; set; } = string.Empty;

        [DataType(DataType.DateTime)]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Modified Date")]
        public DateTime ModifiedDate { get; set; }


        [Required]
        public int UserId { get; set; } 

        [ForeignKey("UserId")] 
        public virtual User? User { get; set; } 
    }
}