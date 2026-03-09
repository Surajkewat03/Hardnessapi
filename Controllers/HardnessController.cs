using Hardnessapi.Data;
using Hardnessapi.Entities;
using Hardnessapi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hardnessapi.Controllers;

[ApiController]
[Route("api/hardness")]
public class HardnessController(HardnessDbContext db) : ControllerBase
{
    private readonly HardnessDbContext _db = db;

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] HardnessApiRequest request, CancellationToken ct)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.QR_Code) || request.Pieces == null)
            return BadRequest("Invalid payload");

        var meta = ParseQrMetadata(request.QR_Code);

        if (meta.Stage == null)
            return BadRequest("Stage not found in QR");

        int count = 0;

        foreach (var p in request.Pieces)
        {
            IHardnessInspection row;

            switch (meta.Stage)
            {
                case "ASS":
                    row = new AssemblyHardnessInspection();
                    _db.Coin_QC_Assembly_Hardness_Insp.Add((AssemblyHardnessInspection)row);
                    break;

                case "A":
                    row = new AnnealingHardnessInspection();
                    _db.Coin_QC_Annealing_Hardness_Insp.Add((AnnealingHardnessInspection)row);
                    break;

                case "P":
                    row = new PolishingHardnessInspection();
                    _db.Coin_QC_Polishing_Hardness_Insp.Add((PolishingHardnessInspection)row);
                    break;

                case "FG":
                    row = new FgHardnessInspection();
                    _db.Coin_QC_FG_Hardness_Insp.Add((FgHardnessInspection)row);
                    break;

                default:
                    return BadRequest("Invalid stage");
            }

            PopulateRow(row, p, request, meta);
            count++;
        }

        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.ToString());
        }

        return Ok(new { saved = count, stage = meta.Stage });
    }

    private static void PopulateRow(IHardnessInspection row, PieceData p, HardnessApiRequest request, QrMetadata meta)
    {
        row.QR_Code = request.QR_Code;
        row.Machine_Name = meta.MachineName;
        row.Batch_No = meta.BatchNo;
        row.Material_Code = meta.MaterialCode;
        row.Sample_No = meta.SampleNo ?? p.Piece_No.ToString();
        row.Inspector_Name = request.Inspector_Name;
        row.Scale = request.Scale;
        row.Inspection_Timestamp = ParseTimestamp(p.Inspection_Timestamp);

        string mic = request.MIC.ToUpperInvariant();

        if (mic.Contains("RING") || mic.Contains("RNG"))
        {
            row.RNGHDHRF = p.Hardness_XY;
            row.RNGHDHRF_Reason = request.Scale;
        }
        else if (mic.Contains("INR"))
        {
            row.INRHDHRF = p.Hardness_XY;
            row.INRHDHRF_Result = request.Scale;
        }
        else if (mic.Contains("COR"))
        {
            row.CORHDHRB = p.Hardness_XY;
            row.CORHDHRB_Result = request.Scale;
        }
    }

    private static DateTime ParseTimestamp(string value)
    {
        if (DateTime.TryParse(value, out var dt))
            return DateTime.SpecifyKind(dt, DateTimeKind.Utc);

        return DateTime.UtcNow;
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

        var parts = qr.Split('/', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length >= 2)
        {
            var stage = parts[1].ToUpper();

            if (stage is "A" or "ASS" or "P" or "FG")
                meta.Stage = stage;

            meta.MachineName = parts.Length >= 3 ? parts[2] : null;
        }

        if (parts.Length >= 4) meta.BatchNo = parts[3];
        if (parts.Length >= 5) meta.MaterialCode = parts[4];
        if (parts.Length >= 6) meta.SampleNo = parts[5];

        return meta;
    }
}