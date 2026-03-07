namespace Hardnessapi.Models;

public class PieceData
{
    public int Piece_No { get; set; }
    public double Hardness_XY { get; set; }
    public string Inspection_Timestamp { get; set; } = string.Empty;
}
