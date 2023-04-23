using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GPTCodingAssistant.DB;

[Table("ChatMessage")]
[Index("SessionId", Name = "IX_ChatMessage_SessionId")]
public partial class ChatMessage
{
    [Key]
    public long Id { get; set; }

    public int SessionId { get; set; }

    public int Role { get; set; }

    public DateTime CreateTime { get; set; }

    public string Message { get; set; } = null!;

    [ForeignKey("SessionId")]
    [InverseProperty("ChatMessages")]
    public virtual Session Session { get; set; } = null!;
}
