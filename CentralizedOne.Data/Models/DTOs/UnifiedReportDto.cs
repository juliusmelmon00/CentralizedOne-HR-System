public class UnifiedReportDto
{
    public string Period { get; set; } = string.Empty;

    // ✅ Documents Summary
    public int TotalDocuments { get; set; }
    public int DocsPending { get; set; }
    public int DocsApproved { get; set; }
    public int DocsRejected { get; set; }

    // ✅ Appointments Summary
    public int TotalAppointments { get; set; }
    public int ApptsPending { get; set; }
    public int ApptsApproved { get; set; }
    public int ApptsRejected { get; set; }
}
