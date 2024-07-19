using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ViLearning.Models
{
    public class Conversation
    {
        [Key]
        public int ConversationId { get; set; }
        public string User1Id { get; set; }
        public string User2Id { get; set; }
        public int? LastMessageId { get; set; }

        public ICollection<Message> Messages { get; set; }

        // Relation
        [ForeignKey("User1Id")]
        [ValidateNever]
        public ApplicationUser User1 { get; set; }
        [ForeignKey("User2Id")]
        [ValidateNever]
        public ApplicationUser User2 { get; set; }

        [ForeignKey("LastMessageId")]
        [ValidateNever]
        public Message LastMessage { get; set; }
    }

}
