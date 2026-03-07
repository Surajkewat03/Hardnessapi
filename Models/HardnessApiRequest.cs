namespace Hardnessapi.Models;

public class HardnessApiRequest
{
    public string QR_Code { get; set; } = string.Empty;
    public string MIC { get; set; } = string.Empty;
    public string Inspector_Name { get; set; } = string.Empty;
    public string Scale { get; set; } = string.Empty;
    public List<PieceData> Pieces { get; set; } = [];
}
