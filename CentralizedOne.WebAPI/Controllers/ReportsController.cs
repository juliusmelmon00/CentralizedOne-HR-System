using CentralizedOne.Data;
using CentralizedOne.Data.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;
using System;

namespace CentralizedOne.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public ReportsController(ApplicationDbContext db)
        {
            _db = db;
        }
        //----------------------------------------
        //HR/ADMIN + SUPERADMIN: DOCUMENTS SUMMARY
        //----------------------------------------
        [HttpGet("documents-summary")]
        [Authorize(Roles = "HR/Admin,SuperAdmin")]
        public IActionResult GetDocumentSummary()
        {
            var now = DateTime.UtcNow;
            var soon = now.AddDays(30); // EXAMPLE: EXPIRING IN 30 DAYS
            var summary = new DocumentSummaryDto
            {
                TotalDocuments = _db.Documents.Count(),
                Pending = _db.Documents.Count(d => d.Status == "Pending"),
                Approved = _db.Documents.Count(d => d.Status == "Approved"),
                Rejected = _db.Documents.Count(d => d.Status == "Rejected"),
                ExpiringSoon = _db.Documents.Count(d => d.ExpiryDate <= soon && d.ExpiryDate >= now)
            };
            return Ok(summary);
        }
        // -----------------------------------------
        //HR/ADMIN + SUPERADMIN: Appointment Summary
        //------------------------------------------
        [HttpGet("appointment-summary")]
        [Authorize(Roles = "HR/Admin,SuperAdmin")]
        public IActionResult GetAppointmentsSummary()
        {
            var now = DateTime.UtcNow;
            var summary = new AppointmentSummaryDto
            {
                TotalAppointment = _db.Appointments.Count(),
                Pending = _db.Appointments.Count(a => a.Status == "Pending"),
                Approved = _db.Appointments.Count(a => a.Status == "Approved"),
                Rejected = _db.Appointments.Count(a => a.Status == "Rejected"),
                Upcoming = _db.Appointments.Count(a => a.ScheduledAt > now && a.Status == "Approved")
            };
            return Ok(summary);
        }
        //--------------------------------------------------------------
        //HR/ADMIN + SUPERADMIN:Grouped Appointments (Monthly or Daily)
        //--------------------------------------------------------------
        [HttpGet("appointment-grouped")]
        [Authorize(Roles = "HR/Admin,SuperAdmin")]
        public IActionResult GetAppointmentsGrouped([FromQuery] string range = "month")
        {
            //Acceptable values: "month" or "day"
            var query = _db.Appointments.AsEnumerable();

            var grouped = (range.ToLower() == "day")
                ? query.GroupBy(a => a.ScheduledAt.ToString("yyyy-MM-dd"))
                : query.GroupBy(a => a.ScheduledAt.ToString("yyyy-MM"));

            var result = grouped.Select(g => new AppointmentGroupDto
            {
                Period = g.Key,
                TotalAppointments = g.Count(),
                Pending = g.Count(a => a.Status == "Pending"),
                Approved = g.Count(a => a.Status == "Approved"),
                Rejected = g.Count(a => a.Status == "Rejected")
            })
    .OrderBy(r => r.Period)
    .ToList();

            return Ok(result);

        }
        //------------------------------------------------------------
        //HR/Admin + SuperAdmin: Grouped Documents (Monthly or Daily)
        //------------------------------------------------------------
        [HttpGet("documents-grouped")]
        [Authorize(Roles = "HR/Admin,SuperAdmin")]
        public IActionResult GetDocumentsGrouped(
            [FromQuery] string range = "month",
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var query = _db.Documents.AsQueryable();

            // ✅ Apply date range filtering
            if (startDate.HasValue)
                query = query.Where(d => d.UploadedAt >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(d => d.UploadedAt <= endDate.Value);

            // ✅ Group based on user selection
            var grouped = (range.ToLower() == "day")
                ? query.AsEnumerable().GroupBy(d => d.UploadedAt.ToString("yyyy-MM-dd"))
                : query.AsEnumerable().GroupBy(d => d.UploadedAt.ToString("yyyy-MM"));

            var result = grouped.Select(g => new DocumentGroupDto
            {
                Period = g.Key ?? "Unknown",
                TotalDocuments = g.Count(),
                Pending = g.Count(d => d.Status == "Pending"),
                Approved = g.Count(d => d.Status == "Approved"),
                Rejected = g.Count(d => d.Status == "Rejected")
            })
            .OrderBy(r => r.Period)
            .ToList();

            return Ok(result);
        }
        //-----------------------------------------------------------
        //HR/Admin + SuperAdmin: Grouped Appointments (Monthly or Daily)
        //------------------------------------------------------------
        [HttpGet("appointments-grouped")]
        [Authorize(Roles = "HR/Admin,SuperAdmin")]
        public IActionResult GetAppointmentsGrouped(
            [FromQuery] string range = "month",
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var query = _db.Appointments.AsQueryable();

            // ✅ Filter by date range
            if (startDate.HasValue)
                query = query.Where(a => a.CreatedAt >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(a => a.CreatedAt <= endDate.Value);

            // ✅ Grouping logic
            var grouped = (range.ToLower() == "day")
                ? query.AsEnumerable().GroupBy(a => a.CreatedAt.ToString("yyyy-MM-dd"))
                : query.AsEnumerable().GroupBy(a => a.CreatedAt.ToString("yyyy-MM"));

            var result = grouped.Select(g => new AppointmentGroupDto
            {
                Period = g.Key ?? "Unknown",
                TotalAppointments = g.Count(),
                Pending = g.Count(a => a.Status == "Pending"),
                Approved = g.Count(a => a.Status == "Approved"),
                Rejected = g.Count(a => a.Status == "Rejected")
            })
            .OrderBy(r => r.Period)
            .ToList();

            return Ok(result);
        }
        [HttpGet("unified-report")]
        [Authorize(Roles = "HR/Admin,SuperAdmin")]
        public IActionResult GetUnifiedReport(
    [FromQuery] string range = "month",
    [FromQuery] DateTime? startDate = null,
    [FromQuery] DateTime? endDate = null)
        {
            // ✅ Base queries
            var docQuery = _db.Documents.AsQueryable();
            var apptQuery = _db.Appointments.AsQueryable();

            // ✅ Apply date filters
            if (startDate.HasValue)
            {
                docQuery = docQuery.Where(d => d.UploadedAt >= startDate.Value);
                apptQuery = apptQuery.Where(a => a.CreatedAt >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                docQuery = docQuery.Where(d => d.UploadedAt <= endDate.Value);
                apptQuery = apptQuery.Where(a => a.CreatedAt <= endDate.Value);
            }

            // ✅ Grouping logic
            var docGroups = (range.ToLower() == "day")
                ? docQuery.AsEnumerable().GroupBy(d => d.UploadedAt.ToString("yyyy-MM-dd"))
                : docQuery.AsEnumerable().GroupBy(d => d.UploadedAt.ToString("yyyy-MM"));

            var apptGroups = (range.ToLower() == "day")
                ? apptQuery.AsEnumerable().GroupBy(a => a.CreatedAt.ToString("yyyy-MM-dd"))
                : apptQuery.AsEnumerable().GroupBy(a => a.CreatedAt.ToString("yyyy-MM"));

            // ✅ Merge groups by period
            var allPeriods = docGroups.Select(g => g.Key)
                .Union(apptGroups.Select(g => g.Key))
                .Distinct()
                .OrderBy(p => p)
                .ToList();

            var result = new List<UnifiedReportDto>();

            foreach (var period in allPeriods)
            {
                var docGroup = docGroups.FirstOrDefault(g => g.Key == period);
                var apptGroup = apptGroups.FirstOrDefault(g => g.Key == period);

                result.Add(new UnifiedReportDto
                {
                    Period = period,

                    // ✅ Document section
                    TotalDocuments = docGroup?.Count() ?? 0,
                    DocsPending = docGroup?.Count(d => d.Status == "Pending") ?? 0,
                    DocsApproved = docGroup?.Count(d => d.Status == "Approved") ?? 0,
                    DocsRejected = docGroup?.Count(d => d.Status == "Rejected") ?? 0,

                    // ✅ Appointments section
                    TotalAppointments = apptGroup?.Count() ?? 0,
                    ApptsPending = apptGroup?.Count(a => a.Status == "Pending") ?? 0,
                    ApptsApproved = apptGroup?.Count(a => a.Status == "Approved") ?? 0,
                    ApptsRejected = apptGroup?.Count(a => a.Status == "Rejected") ?? 0
                });
            }

            return Ok(result);
        }
        //---------------------------------------------------------
        //GetUserWiseReport
        //-------------------------------------------------------
        [HttpGet("user-breakdown")]
        [Authorize(Roles = "HR/Admin,SuperAdmin")]
        public IActionResult GetUserWiseReport([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            if (!startDate.HasValue && !endDate.HasValue)
            {
                endDate = DateTime.UtcNow;
                startDate = endDate.Value.AddDays(-30);
            }

            var users = _db.Users.ToList();
            var result = new List<UserReportDto>();

            foreach (var user in users)
            {
                var docs = _db.Documents.Where(d => d.UserId == user.Id);
                var appts = _db.Appointments.Where(a => a.UserId == user.Id);

                if (startDate.HasValue)
                    docs = docs.Where(d => d.UploadedAt >= startDate.Value);
                if (endDate.HasValue)
                    docs = docs.Where(d => d.UploadedAt <= endDate.Value);

                if (startDate.HasValue)
                    appts = appts.Where(a => a.CreatedAt >= startDate.Value);
                if (endDate.HasValue)
                    appts = appts.Where(a => a.CreatedAt <= endDate.Value);

                result.Add(new UserReportDto
                {
                    UserId = user.Id,
                    Username = user.Username,
                    DocsPending = docs.Count(d => d.Status == "Pending"),
                    DocsApproved = docs.Count(d => d.Status == "Approved"),
                    DocsRejected = docs.Count(d => d.Status == "Rejected"),

                    ApptsPending = appts.Count(a => a.Status == "Pending"),
                    ApptsApproved = appts.Count(a => a.Status == "Approved"),
                    ApptsRejected = appts.Count(a => a.Status == "Rejected")
                });
            }

            // ✅ Calculate totals first
            int totalDocsPending = result.Sum(r => r.DocsPending);
            int totalDocsApproved = result.Sum(r => r.DocsApproved);
            int totalDocsRejected = result.Sum(r => r.DocsRejected);
            int totalApptsPending = result.Sum(r => r.ApptsPending);
            int totalApptsApproved = result.Sum(r => r.ApptsApproved);
            int totalApptsRejected = result.Sum(r => r.ApptsRejected);

            // ✅ Helper to calculate percent safely
            double Percent(int part, int total) =>
                total == 0 ? 0 : Math.Round((double)part / total * 100, 2);

            // ✅ Apply percentage per user
            foreach (var r in result)
            {
                r.DocsPendingPercent = Percent(r.DocsPending, totalDocsPending);
                r.DocsApprovedPercent = Percent(r.DocsApproved, totalDocsApproved);
                r.DocsRejectedPercent = Percent(r.DocsRejected, totalDocsRejected);

                r.ApptsPendingPercent = Percent(r.ApptsPending, totalApptsPending);
                r.ApptsApprovedPercent = Percent(r.ApptsApproved, totalApptsApproved);
                r.ApptsRejectedPercent = Percent(r.ApptsRejected, totalApptsRejected);
            }

            // ✅ Add TOTAL row
            result.Add(new UserReportDto
            {
                UserId = 0,
                Username = "TOTAL",
                DocsPending = totalDocsPending,
                DocsApproved = totalDocsApproved,
                DocsRejected = totalDocsRejected,
                ApptsPending = totalApptsPending,
                ApptsApproved = totalApptsApproved,
                ApptsRejected = totalApptsRejected,
                DocsPendingPercent = 100,
                DocsApprovedPercent = 100,
                DocsRejectedPercent = 100,
                ApptsPendingPercent = 100,
                ApptsApprovedPercent = 100,
                ApptsRejectedPercent = 100
            });

            return Ok(result);
        }
        //--------------------------------------------------
        //For Front End ready pie chart JSON with percentage
        //---------------------------------------------------
        [HttpGet("chart-summary")]
        [Authorize(Roles = "HR/Admin,SuperAdmin")]
        public IActionResult ChartSummary()
        {
            var docGroups = _db.Documents
                .GroupBy(d => d.Status)
                .Select(g => new { status = g.Key, count = g.Count() })
                .ToList();

            var apptGroups = _db.Appointments
                .GroupBy(a => a.Status)
                .Select(g => new { status = g.Key, count = g.Count() })
                .ToList();

            int totalDocs = docGroups.Sum(d => d.count);
            int totalAppts = apptGroups.Sum(a => a.count);

            var docChart = docGroups.Select(d => new
            {
                d.status,
                d.count,
                percent = totalDocs == 0 ? 0 : Math.Round((double)d.count / totalDocs * 100, 2)
            });

            var apptChart = apptGroups.Select(a => new
            {
                a.status,
                a.count,
                percent = totalAppts == 0 ? 0 : Math.Round((double)a.count / totalAppts * 100, 2)
            });

            return Ok(new
            {
                documents = docChart,
                appointments = apptChart
            });
        }
        //----------------------------
        //Weekly and Montly Trend Data
        //----------------------------

        [HttpGet("trend-summary")]
        [Authorize(Roles = "HR/Admin,SuperAdmin")]
        public IActionResult TrendSummary([FromQuery] string range = "month")
        {
            range = (range ?? "month").ToLowerInvariant();

            // -------------------------------
            // DOCUMENT TREND (Multi-Line Breakdown)
            // -------------------------------
            List<object> docTrend;
            if (range == "week")
            {
                docTrend = _db.Documents
                    .AsEnumerable() // do grouping in-memory; ISOWeek not translatable to SQL
                    .Where(d => d.UploadedAt != default(DateTime))
                    .GroupBy(d => new { Year = d.UploadedAt.Year, Week = System.Globalization.ISOWeek.GetWeekOfYear(d.UploadedAt) })
                    .Select(g => new
                    {
                        Year = g.Key.Year,
                        SortKey = g.Key.Week,
                        period = $"Week {g.Key.Week}, {g.Key.Year}",
                        pending = g.Count(x => x.Status == "Pending"),
                        approved = g.Count(x => x.Status == "Approved"),
                        rejected = g.Count(x => x.Status == "Rejected")
                    })
                    .OrderBy(x => x.Year).ThenBy(x => x.SortKey)
                    .Select(x => new { x.period, x.pending, x.approved, x.rejected })
                    .ToList<object>();
            }
            else // month
            {
                docTrend = _db.Documents
                    .AsEnumerable()
                    .Where(d => d.UploadedAt != default(DateTime))
                    .GroupBy(d => new { Year = d.UploadedAt.Year, Month = d.UploadedAt.Month })
                    .Select(g => new
                    {
                        Year = g.Key.Year,
                        SortKey = g.Key.Month,
                        period = $"{CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(g.Key.Month)} {g.Key.Year}",
                        pending = g.Count(x => x.Status == "Pending"),
                        approved = g.Count(x => x.Status == "Approved"),
                        rejected = g.Count(x => x.Status == "Rejected")
                    })
                    .OrderBy(x => x.Year).ThenBy(x => x.SortKey)
                    .Select(x => new { x.period, x.pending, x.approved, x.rejected })
                    .ToList<object>();
            }

            // -------------------------------
            // APPOINTMENT TREND (Multi-Line Breakdown)
            // -------------------------------
            List<object> apptTrend;
            if (range == "week")
            {
                apptTrend = _db.Appointments
                    .AsEnumerable()
                    .Where(a => a.CreatedAt != default(DateTime))
                    .GroupBy(a => new { Year = a.CreatedAt.Year, Week = System.Globalization.ISOWeek.GetWeekOfYear(a.CreatedAt) })
                    .Select(g => new
                    {
                        Year = g.Key.Year,
                        SortKey = g.Key.Week,
                        period = $"Week {g.Key.Week}, {g.Key.Year}",
                        pending = g.Count(x => x.Status == "Pending"),
                        approved = g.Count(x => x.Status == "Approved"),
                        rejected = g.Count(x => x.Status == "Rejected")
                    })
                    .OrderBy(x => x.Year).ThenBy(x => x.SortKey)
                    .Select(x => new { x.period, x.pending, x.approved, x.rejected })
                    .ToList<object>();
            }
            else // month
            {
                apptTrend = _db.Appointments
                    .AsEnumerable()
                    .Where(a => a.CreatedAt != default(DateTime))
                    .GroupBy(a => new { Year = a.CreatedAt.Year, Month = a.CreatedAt.Month })
                    .Select(g => new
                    {
                        Year = g.Key.Year,
                        SortKey = g.Key.Month,
                        period = $"{CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(g.Key.Month)} {g.Key.Year}",
                        pending = g.Count(x => x.Status == "Pending"),
                        approved = g.Count(x => x.Status == "Approved"),
                        rejected = g.Count(x => x.Status == "Rejected")
                    })
                    .OrderBy(x => x.Year).ThenBy(x => x.SortKey)
                    .Select(x => new { x.period, x.pending, x.approved, x.rejected })
                    .ToList<object>();
            }

            return Ok(new { range, documents = docTrend, appointments = apptTrend });
        }
    }

    }
















