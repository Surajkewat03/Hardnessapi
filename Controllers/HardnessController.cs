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
        if (request is null || string.IsNullOrWhiteSpace(request.QR_Code) || request.Pieces is null || request.Pieces.Count == 0)
            return BadRequest("Invalid payload");

        var meta = ParseQrMetadata(request.QR_Code);
        if (meta.Stage is null)
            return BadRequest("Stage not found in QR_Code (expected A/ASS/P/FG in segments)");

        int count = 0;

        foreach (var p in request.Pieces)
        {
            // Find row by Rec_No (mapped from Piece_No)
            int recNo = p.Piece_No;

            switch (meta.Stage)
            {
                case "ASS":
                    var assRow = await _db.Coin_QC_Assembly_Hardness_Insp
                        .FirstOrDefaultAsync(x => x.Rec_No == recNo, ct);
                    if (assRow != null)
                    {
                        UpdateRow(assRow, p, request);
                    }
                    else
                    {
                        var newRow = new AssemblyHardnessInspection();
                        PopulateRow(newRow, p, request, meta);
                        _db.Coin_QC_Assembly_Hardness_Insp.Add(newRow);
                    }
                    count++;
                    break;

                case "A":
                    var annRow = await _db.Coin_QC_Annealing_Hardness_Insp
                        .FirstOrDefaultAsync(x => x.Rec_No == recNo, ct);
                    if (annRow != null)
                    {
                        UpdateRow(annRow, p, request);
                    }
                    else
                    {
                        var newRow = new AnnealingHardnessInspection();
                        PopulateRow(newRow, p, request, meta);
                        _db.Coin_QC_Annealing_Hardness_Insp.Add(newRow);
                    }
                    count++;
                    break;

                case "P":
                    var polRow = await _db.Coin_QC_Polishing_Hardness_Insp
                        .FirstOrDefaultAsync(x => x.Rec_No == recNo, ct);
                    if (polRow != null)
                    {
                        UpdateRow(polRow, p, request);
                    }
                    else
                    {
                        var newRow = new PolishingHardnessInspection();
                        PopulateRow(newRow, p, request, meta);
                        _db.Coin_QC_Polishing_Hardness_Insp.Add(newRow);
                    }
                    count++;
                    break;

                case "FG":
                    var fgRow = await _db.Coin_QC_FG_Hardness_Insp
                        .FirstOrDefaultAsync(x => x.Rec_No == recNo, ct);
                    if (fgRow != null)
                    {
                        UpdateRow(fgRow, p, request);
                    }
                    else
                    {
                        var newRow = new FgHardnessInspection();
                        PopulateRow(newRow, p, request, meta);
                        _db.Coin_QC_FG_Hardness_Insp.Add(newRow);
                    }
                    count++;
                    break;
            }
        }

        await _db.SaveChangesAsync(ct);
        return Ok(new { processed = count, stage = meta.Stage });
    }

    private static void UpdateRow(IHardnessInspection row, PieceData p, HardnessApiRequest request)
    {
        string mic = request.MIC.ToUpperInvariant();
        
        // Match column based on MIC keywords
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

        row.Inspector_Name = request.Inspector_Name;
        row.Scale = request.Scale;
        row.Inspection_Timestamp = ParseTimestamp(p.Inspection_Timestamp);
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
        {
            // PostgreSQL/Npgsql requires UTC for 'timestamp with time zone'
            return DateTime.SpecifyKind(dt, DateTimeKind.Utc);
        }
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
        if (string.IsNullOrWhiteSpace(qr)) return meta;
        
        var parts = qr.Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        
        if (parts.Length >= 2)
        {
            // Machine Name (e.g., if QR is I/ASS/MRV4/..., parts[2] is MRV4)
            // If parts[2] exists, it's the machine name. Otherwise use parts[1].
            meta.MachineName = parts.Length >= 3 ? parts[2] : parts[1];
            
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
