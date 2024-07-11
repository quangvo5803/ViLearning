using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ViLearning.Models
{
    public class Message
    {
        [Key]
        public int MessageId { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public string MessageText { get; set; }
        public DateTime Timestamp { get; set; }

        //Relation
        [ForeignKey("SenderId")]
        [ValidateNever]
        public ApplicationUser Sender { get; set; }
        [ForeignKey("ReceiverId")]
        [ValidateNever]
        public ApplicationUser Receiver { get; set; }
    }
}
