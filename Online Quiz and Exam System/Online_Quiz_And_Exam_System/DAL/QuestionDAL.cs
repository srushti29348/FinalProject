using Microsoft.Data.SqlClient;
using Online_Quiz_And_Exam_System.Models;
using System.Collections.Generic;

namespace Online_Quiz_API.DAL
{
    public class QuestionDAL
    {
        private readonly DbConnection _db;

        public QuestionDAL(DbConnection db)
        {
            _db = db;
        }

        // =========================
        // GET 40 RANDOM QUESTIONS
        // =========================
        public List<Question> GetRandomQuestions(int moduleId)
        {
            List<Question> list = new();

            using SqlConnection con = _db.GetConnection();
            SqlCommand cmd = new SqlCommand(
                @"SELECT TOP 40 * 
                  FROM Questions 
                  WHERE ModuleId = @mid 
                  ORDER BY NEWID()", con);

            cmd.Parameters.AddWithValue("@mid", moduleId);

            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                list.Add(new Question
                {
                    QuestionId = (int)dr["QuestionId"],
                    ModuleId = (int)dr["ModuleId"],
                    QuestionText = dr["QuestionText"].ToString(),
                    OptionA = dr["OptionA"].ToString(),
                    OptionB = dr["OptionB"].ToString(),
                    OptionC = dr["OptionC"].ToString(),
                    OptionD = dr["OptionD"].ToString(),
                    CorrectOption = dr["CorrectOption"].ToString()
                });
            }
            return list;
        }



        public List<Question> GetDemoQuestions(int moduleId)
        {
            List<Question> list = new();

            using SqlConnection con = _db.GetConnection();
            SqlCommand cmd = new SqlCommand(
                @"SELECT TOP 20 *
          FROM Questions
          WHERE ModuleId = @mid
          ORDER BY QuestionId ASC", con);   // ✅ FIXED


            cmd.Parameters.AddWithValue("@mid", moduleId);

            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                list.Add(new Question
                {
                    QuestionId = (int)dr["QuestionId"],
                    ModuleId = (int)dr["ModuleId"],
                    QuestionText = dr["QuestionText"].ToString(),
                    OptionA = dr["OptionA"].ToString(),
                    OptionB = dr["OptionB"].ToString(),
                    OptionC = dr["OptionC"].ToString(),
                    OptionD = dr["OptionD"].ToString(),
                    CorrectOption = dr["CorrectOption"].ToString()
                });
            }

            return list;
        }




    }
}
