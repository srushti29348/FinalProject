namespace Online_Quiz_And_Exam_System.Models
{
    public class Question
    {
        public int QuestionId { get; set; }
        public int ModuleId { get; set; }

        public string? QuestionText { get; set; }
        public string? OptionA { get; set; }
        public string? OptionB { get; set; }
        public string? OptionC { get; set; }
        public string? OptionD { get; set; }

        public string? CorrectOption { get; set; }
    }
}
