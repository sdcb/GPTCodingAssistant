using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GPTCodingAssistant.DB;

[Table("Session")]
public partial class Session
{
    [Key]
    public int Id { get; set; }

    [Column("IPId")]
    public int Ipid { get; set; }

    [StringLength(200)]
    public string Title { get; set; } = null!;

    public DateTime CreateTime { get; set; }

    public DateTime LastActiveTime { get; set; }

    [InverseProperty("Session")]
    public virtual ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();

    [ForeignKey("Ipid")]
    [InverseProperty("Sessions")]
    public virtual Ip Ip { get; set; } = null!;
}
