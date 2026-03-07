using System.ComponentModel.DataAnnotations;

namespace Hardnessapi.Entities;

public class FgHardnessInspection : IHardnessInspection
{
    [Key]
    public int Rec_No { get; set; }
    public string? QR_Code { get; set; }
    public string? Machine_Name { get; set; }
    public string? Batch_No { get; set; }
    public string? Material_Code { get; set; }
    public string? Sample_No { get; set; }
    public string? Inspector_Name { get; set; }
    public DateTime? Inspection_Timestamp { get; set; }
    public string? Inspection_Lot { get; set; }
    public string? Scale { get; set; }
    
    public double? RNGHDHRF { get; set; }
    public string? RNGHDHRF_Result { get; set; }
    public string? RNGHDHRF_Reason { get; set; }
    
    public double? INRHDHRF { get; set; }
    public string? INRHDHRF_Result { get; set; }
    
    public double? CORHDHRB { get; set; }
    public string? CORHDHRB_Result { get; set; }
}
