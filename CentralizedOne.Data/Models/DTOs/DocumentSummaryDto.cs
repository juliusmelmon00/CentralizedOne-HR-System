
namespace CentralizedOne.Data.Models.DTOs
{
    public class DocumentSummaryDto
    {
        public int TotalDocuments { get; set; }
        public int Pending {  get; set; }
        public int Approved { get; set; }
        public int Rejected { get; set; }
        public int ExpiringSoon { get; set; }

    }
    
    
}
