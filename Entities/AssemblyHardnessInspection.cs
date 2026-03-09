using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hardnessapi.Entities;

[Table("coin_qc_assembly_hardness_insp")]
public class AssemblyHardnessInspection : IHardnessInspection
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Rec_No { get; set; }

    [Column("qr_code")]
    public string? QR_Code { get; set; }

    [Column("machine_name")]
    public string? Machine_Name { get; set; }

    [Column("batch_no")]
    public string? Batch_No { get; set; }

    [Column("material_code")]
    public string? Material_Code { get; set; }

    [Column("sample_no")]
    public string? Sample_No { get; set; }

    [Column("inspector_name")]
    public string? Inspector_Name { get; set; }

    [Column("inspection_timestamp")]
    public DateTime? Inspection_Timestamp { get; set; }

    [Column("inspection_lot")]
    public string? Inspection_Lot { get; set; }

    [Column("scale")]
    public string? Scale { get; set; }
    
    [Column("rnghdhrf")]
    public double? RNGHDHRF { get; set; }

    [Column("rnghdhrf_result")]
    public string? RNGHDHRF_Result { get; set; }

    [Column("rnghdhrf_reason")]
    public string? RNGHDHRF_Reason { get; set; }
    
    [Column("inrhdhrf")]
    public double? INRHDHRF { get; set; }

    [Column("inrhdhrf_result")]
    public string? INRHDHRF_Result { get; set; }
    
    [Column("corhdhrb")]
    public double? CORHDHRB { get; set; }

    [Column("corhdhrb_result")]
    public string? CORHDHRB_Result { get; set; }
}
