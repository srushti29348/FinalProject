using Microsoft.Data.SqlClient;
using Online_Quiz_And_Exam_System.Models;
using Online_Quiz_API.DAL;

namespace Online_Quiz_And_Exam_System.DAL
{
    public class MockDAL
    {
        private readonly DbConnection _db;
        public MockDAL(DbConnection db) => _db = db;

        public List<Question> GetMockQuestions(int moduleId, int mockNumber)
        {
            int offset = (mockNumber - 1) * 40;
            var list = new List<Question>();

            using (SqlConnection con = _db.GetConnection())
            {
                SqlCommand cmd = new SqlCommand(@"
                    SELECT *
                    FROM Questions
                    WHERE ModuleId = @moduleId
                    ORDER BY QuestionId
                    OFFSET @offset ROWS
                    FETCH NEXT 40 ROWS ONLY
                ", con);

                cmd.Parameters.AddWithValue("@moduleId", moduleId);
                cmd.Parameters.AddWithValue("@offset", offset);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    list.Add(new Question
                    {
                        QuestionId = (int)dr["QuestionId"],
                        QuestionText = dr["QuestionText"].ToString(),
                        OptionA = dr["OptionA"].ToString(),
                        OptionB = dr["OptionB"].ToString(),
                        OptionC = dr["OptionC"].ToString(),
                        OptionD = dr["OptionD"].ToString(),
                        CorrectOption = dr["CorrectOption"].ToString()
                    });
                }
            }
            return list;
        }
    }
}
