using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PedidosBarrio.Domain.Entities;

[Index("Category", Name = "IX_Logs_Category")]
[Index("EmpresaID", Name = "IX_Logs_EmpresaID")]
[Index("Level", Name = "IX_Logs_Level")]
[Index("Logger", Name = "IX_Logs_Logger")]
[Index("RequestId", Name = "IX_Logs_RequestId")]
[Index("Timestamp", Name = "IX_Logs_Timestamp", AllDescending = true)]
public partial class Log
{
    [Key]
    [Column("LogID")]
    public long LogID { get; set; }

    public DateTime? Timestamp { get; set; }

    [StringLength(10)]
    public string Level { get; set; } = null!;

    [StringLength(255)]
    public string? Logger { get; set; }

    public string Message { get; set; } = null!;

    public string? Exception { get; set; }

    [Column(TypeName = "jsonb")]
    public string? Properties { get; set; }

    [StringLength(50)]
    public string? MachineName { get; set; }

    public int? ProcessId { get; set; }

    public int? ThreadId { get; set; }

    [StringLength(50)]
    public string? UserId { get; set; }

    [StringLength(50)]
    public string? RequestId { get; set; }

    [StringLength(500)]
    public string? RequestPath { get; set; }

    [StringLength(10)]
    public string? HttpMethod { get; set; }

    public int? StatusCode { get; set; }

    public int? Duration { get; set; }

    [Column("EmpresaID")]
    public Guid? EmpresaID { get; set; }

    [StringLength(50)]
    public string? Category { get; set; }
}


