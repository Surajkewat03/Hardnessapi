using Hardnessapi.Data;
using Hardnessapi.Entities;
using Hardnessapi.Models;
using Microsoft.AspNetCore.Mvc;

namespace Hardnessapi.Controllers;

[ApiController]
[Route("api/hardness")]
public class HardnessController : ControllerBase
{
    private readonly HardnessDbContext _db;
    public HardnessController(HardnessDbContext db) { _db = db; }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] HardnessApiRequest request, CancellationToken ct)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.QR_Code) || request.Pieces is null || request.Pieces.Count == 0)
            return BadRequest("Invalid payload");

        var meta = ParseQrMetadata(request.QR_Code);
        if (meta.Stage is null)
            return BadRequest("Stage not found in QR_Code (expected A/ASS/P/FG in segments)");

        switch (meta.Stage)
        {
            case "ASS":
                foreach (var p in request.Pieces)
                {
                    _db.Coin_QC_Assembly_Hardness_Insp.Add(new AssemblyHardnessInspection
                    {
                        QR_Code = request.QR_Code,
                        MIC = request.MIC,
                        Inspector_Name = request.Inspector_Name,
                        Piece_No = p.Piece_No,
                        Hardness_XY = p.Hardness_XY,
                        Machine_Name = meta.MachineName,
                        Batch_No = meta.BatchNo,
                        Material_Code = meta.MaterialCode,
                        Sample_No = meta.SampleNo,
                        Inspection_Timestamp = ParseTimestamp(p.Inspection_Timestamp)
                    });
                }
                break;
            case "A":
                foreach (var p in request.Pieces)
                {
                    _db.Coin_QC_Assembaly_Hardness_Insp.Add(new AssembalyHardnessInspection
                    {
                        QR_Code = request.QR_Code,
                        MIC = request.MIC,
                        Inspector_Name = request.Inspector_Name,
                        Piece_No = p.Piece_No,
                        Hardness_XY = p.Hardness_XY,
                        Machine_Name = meta.MachineName,
                        Batch_No = meta.BatchNo,
                        Material_Code = meta.MaterialCode,
                        Sample_No = meta.SampleNo,
                        Inspection_Timestamp = ParseTimestamp(p.Inspection_Timestamp)
                    });
                }
                break;
            case "P":
                foreach (var p in request.Pieces)
                {
                    _db.Coin_QC_Polishing_Hardness_Insp.Add(new PolishingHardnessInspection
                    {
                        QR_Code = request.QR_Code,
                        MIC = request.MIC,
                        Inspector_Name = request.Inspector_Name,
                        Piece_No = p.Piece_No,
                        Hardness_XY = p.Hardness_XY,
                        Machine_Name = meta.MachineName,
                        Batch_No = meta.BatchNo,
                        Material_Code = meta.MaterialCode,
                        Sample_No = meta.SampleNo,
                        Inspection_Timestamp = ParseTimestamp(p.Inspection_Timestamp)
                    });
                }
                break;
            case "FG":
                foreach (var p in request.Pieces)
                {
                    _db.Coin_QC_FG_Hardness_Insp.Add(new FgHardnessInspection
                    {
                        QR_Code = request.QR_Code,
                        MIC = request.MIC,
                        Inspector_Name = request.Inspector_Name,
                        Piece_No = p.Piece_No,
                        Hardness_XY = p.Hardness_XY,
                        Machine_Name = meta.MachineName,
                        Batch_No = meta.BatchNo,
                        Material_Code = meta.MaterialCode,
                        Sample_No = meta.SampleNo,
                        Inspection_Timestamp = ParseTimestamp(p.Inspection_Timestamp)
                    });
                }
                break;
            default:
                return BadRequest("Unsupported stage");
        }

        await _db.SaveChangesAsync(ct);
        return Ok(new { inserted = request.Pieces.Count, stage = meta.Stage });
    }

    private static DateTime ParseTimestamp(string value)
    {
        if (DateTime.TryParse(value, out var dt)) return dt;
        return DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
    }

    private class QrMetadata
    {
        public string? Stage { get; set; }
        public string? MachineName { get; set; }
        public string? BatchNo { get; set; }
        public string? MaterialCode { get; set; }
        public string? SampleNo { get; set; }
    }

    private static QrMetadata ParseQrMetadata(string qr)
    {
        var meta = new QrMetadata();
        if (string.IsNullOrWhiteSpace(qr)) return meta;
        
        var parts = qr.Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                      .ToArray();
        
        if (parts.Length >= 2)
        {
            // Machine Name (e.g., A/A1 or just A)
            meta.MachineName = parts.Length >= 3 ? $"{parts[1]}/{parts[2]}" : parts[1];
            
            // Extract Stage from parts[1] (machine base)
            var machineBase = parts[1].ToUpperInvariant();
            var lettersOnly = new string(machineBase.TakeWhile(char.IsLetter).ToArray());
            var stageToken = string.IsNullOrEmpty(lettersOnly) ? machineBase : lettersOnly;
            if (stageToken is "A" or "ASS" or "P" or "FG")
                meta.Stage = stageToken;
        }

        if (parts.Length >= 4) meta.BatchNo = parts[3];
        if (parts.Length >= 5) meta.MaterialCode = parts[4];
        if (parts.Length >= 6) meta.SampleNo = parts[5];

        return meta;
    }
}
