using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Online_Quiz_And_Exam_System.Models;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;


namespace Online_Quiz_API.DAL
{
    public class ResultDAL
    {
        private readonly DbConnection _db;

        public ResultDAL(DbConnection db)
        {
            _db = db;
        }

        // ================= SAVE RESULT =================
        public void Save(TestResult result)
        {
            using SqlConnection con = _db.GetConnection();
            using SqlCommand cmd = new SqlCommand();

            cmd.Connection = con;
            cmd.CommandText =
                @"INSERT INTO TestResults
          (UserId, ModuleId, Score, Attempted, Unattempted, TestType, MockNo)
          VALUES
          (@uid, @mid, @score, @attempted, @unattempted, @type, @mockNo)";

            cmd.Parameters.Add("@uid", SqlDbType.Int).Value = result.UserId;
            cmd.Parameters.Add("@mid", SqlDbType.Int).Value = result.ModuleId;
            cmd.Parameters.Add("@score", SqlDbType.Int).Value = result.Score;
            cmd.Parameters.Add("@attempted", SqlDbType.Int).Value = result.Attempted;
            cmd.Parameters.Add("@unattempted", SqlDbType.Int).Value = result.Unattempted;
            cmd.Parameters.Add("@type", SqlDbType.NVarChar, 20).Value = result.TestType;

            // 🔑 THIS LINE PREVENTS THE ERROR
            cmd.Parameters.Add("@mockNo", SqlDbType.Int).Value =
                result.TestType == "Mock"
                    ? result.MockNo
                    : DBNull.Value;

            con.Open();
            cmd.ExecuteNonQuery();
        }



        // ================= ATTEMPT SUMMARY =================
        public List<object> GetAttemptSummary(int userId)
        {
            List<object> list = new();

            using var conn = _db.GetConnection();
            var cmd = new SqlCommand(@"
                SELECT m.ModuleName, COUNT(*) AS Attempts
                FROM TestResults t
                JOIN Modules m ON t.ModuleId = m.ModuleId
                WHERE t.UserId = @uid
                GROUP BY m.ModuleName", conn);

            cmd.Parameters.AddWithValue("@uid", userId);
            conn.Open();

            using var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                list.Add(new
                {
                    moduleName = dr["ModuleName"].ToString(),
                    attempts = (int)dr["Attempts"]
                });
            }

            return list;
        }

        // ================= LATEST RESULT (FIXED & SAFE) =================
        public object GetLatestResult(int userId)
        {
            using var conn = _db.GetConnection();

            var cmd = new SqlCommand(@"
                SELECT TOP 1
                    m.ModuleName,
                    r.Score,
                    r.Attempted,
                    r.Unattempted,

                    (SELECT COUNT(*) FROM TestResults WHERE UserId = @uid) AS TotalTests,
                    (SELECT COUNT(*) FROM TestResults WHERE UserId = @uid AND TestType = 'Practice') AS PracticeTests,
                    (SELECT COUNT(*) FROM TestResults WHERE UserId = @uid AND TestType = 'Mock') AS MockTests,
                    (SELECT ISNULL(MAX(Score), 0) FROM TestResults WHERE UserId = @uid) AS BestScore

                FROM TestResults r
                JOIN Modules m ON r.ModuleId = m.ModuleId
                WHERE r.UserId = @uid
                ORDER BY r.ResultId DESC
            ", conn);

            cmd.Parameters.AddWithValue("@uid", userId);

            conn.Open();
            using var dr = cmd.ExecuteReader();

            if (!dr.Read())
                return null;   // 🔑 new user case

            return new
            {
                moduleName = dr["ModuleName"],
                score = dr["Score"],
                attempted = dr["Attempted"],
                unattempted = dr["Unattempted"],
                totalTests = dr["TotalTests"],
                practiceTests = dr["PracticeTests"],
                mockTests = dr["MockTests"],
                bestScore = dr["BestScore"]
            };
        }




        public bool HasAttemptedMock(int userId, int moduleId, int mockNo)
        {
            using SqlConnection con = _db.GetConnection();
            SqlCommand cmd = new SqlCommand(
                @"SELECT COUNT(*) 
          FROM TestResults
          WHERE UserId = @uid
            AND ModuleId = @mid
            AND TestType = 'Mock'
            AND MockNo = @mockNo", con);

            cmd.Parameters.AddWithValue("@uid", userId);
            cmd.Parameters.AddWithValue("@mid", moduleId);
            cmd.Parameters.AddWithValue("@mockNo", mockNo);

            con.Open();
            int count = (int)cmd.ExecuteScalar();
            return count > 0;
        }

    }
}
