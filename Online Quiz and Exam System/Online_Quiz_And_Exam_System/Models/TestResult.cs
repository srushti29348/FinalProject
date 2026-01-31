namespace Online_Quiz_And_Exam_System.Models
{
    using System;

    public class TestResult
    {
        public int TestId { get; set; }
        public int UserId { get; set; }
        public int ModuleId { get; set; }
        public int Score { get; set; }
        //public DateTime TestDate { get; set; }
        public int ResultId { get; internal set; }
        public int Attempted { get; set; }
        public int Unattempted { get; set; }
        public string? TestType { get; set; }
        public int? MockNo { get; set; }



    }

}
