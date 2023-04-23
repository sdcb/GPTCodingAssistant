using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GPTCodingAssistant.DB;

[Table("IP")]
public partial class Ip
{
    [Key]
    public int Id { get; set; }

    [Column("IP")]
    [StringLength(40)]
    [Unicode(false)]
    public string Ip1 { get; set; } = null!;

    [InverseProperty("Ip")]
    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();
}
