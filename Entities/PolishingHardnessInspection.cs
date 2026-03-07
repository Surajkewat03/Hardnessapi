namespace Hardnessapi.Entities;

public class PolishingHardnessInspection
{
    public string QR_Code { get; set; } = string.Empty;
    public string MIC { get; set; } = string.Empty;
    public string Inspector_Name { get; set; } = string.Empty;
    public int Piece_No { get; set; }
    public double Hardness_XY { get; set; }
    public string? Machine_Name { get; set; }
    public string? Batch_No { get; set; }
    public string? Material_Code { get; set; }
    public string? Sample_No { get; set; }
    public DateTime Inspection_Timestamp { get; set; }
}
