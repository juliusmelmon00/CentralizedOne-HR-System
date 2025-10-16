public class UserReportDto
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;

    public int DocsPending { get; set; }
    public int DocsApproved { get; set; }
    public int DocsRejected { get; set; }
    public int ApptsPending { get; set; }
    public int ApptsApproved { get; set; }
    public int ApptsRejected { get; set; }

    // ✅ NEW: Percentage fields
    public double DocsPendingPercent { get; set; }
    public double DocsApprovedPercent { get; set; }
    public double DocsRejectedPercent { get; set; }
    public double ApptsPendingPercent { get; set; }
    public double ApptsApprovedPercent { get; set; }
    public double ApptsRejectedPercent { get; set; }
}
