namespace Hardnessapi.Entities;

public interface IHardnessInspection
{
    int Rec_No { get; set; }
    string? QR_Code { get; set; }
    string? Machine_Name { get; set; }
    string? Batch_No { get; set; }
    string? Material_Code { get; set; }
    string? Sample_No { get; set; }
    string? Inspector_Name { get; set; }
    DateTime? Inspection_Timestamp { get; set; }
    string? Inspection_Lot { get; set; }
    string? Scale { get; set; }
    
    // RNG Hardness (was RINGHDHV)
    double? RNGHDHRF { get; set; }
    string? RNGHDHRF_Result { get; set; }
    string? RNGHDHRF_Reason { get; set; }
    
    // INR Hardness
    double? INRHDHRF { get; set; }
    string? INRHDHRF_Result { get; set; }
    
    // COR Hardness
    double? CORHDHRB { get; set; }
    string? CORHDHRB_Result { get; set; }
}
