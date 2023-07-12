using System;

namespace DinamoErpQuestion.Models
{
    public class ProductionOperationsRecord
    {
        public int RecordNo { get; set; }
        public DateTime Start { get; set; }
        public DateTime Finish { get; set; }
        public TimeSpan TotalTime { get; set; }
        public string Status { get; set; }
        public string ReasonForStoppingTheOperation { get; set; }
    }
}
