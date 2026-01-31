using Microsoft.Data.SqlClient;
using Online_Quiz_And_Exam_System.Models;

namespace Online_Quiz_API.DAL
{
    public class UserDAL
    {
        private readonly DbConnection _db;

        public UserDAL(DbConnection db)
        {
            _db = db;
        }

        // =========================
        // LOGIN
        // =========================
        public User ValidateUser(string email, string password)
        {
            using SqlConnection con = _db.GetConnection();

            SqlCommand cmd = new SqlCommand(
                "SELECT * FROM Users WHERE Email=@e AND Password=@p", con);

            cmd.Parameters.AddWithValue("@e", email);
            cmd.Parameters.AddWithValue("@p", password);

            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                return new User
                {
                    UserId = (int)dr["UserId"],
                    FullName = dr["FullName"].ToString(),
                    Email = dr["Email"].ToString(),
                    MobileNumber = dr["MobileNumber"].ToString()
                };
            }
            return null;
        }

        // =========================
        // REGISTER
        // =========================
        public void Register(User user)
        {
            using SqlConnection con = _db.GetConnection();

            SqlCommand cmd = new SqlCommand(
                @"INSERT INTO Users (FullName, Email, Password, MobileNumber)
                  VALUES (@n, @e, @p, @m)", con);

            cmd.Parameters.AddWithValue("@n", user.FullName);
            cmd.Parameters.AddWithValue("@e", user.Email);
            cmd.Parameters.AddWithValue("@p", user.Password);
            cmd.Parameters.AddWithValue("@m", user.MobileNumber);

            con.Open();
            cmd.ExecuteNonQuery();
        }

        public User GetUserByEmail(string email)
        {
            using SqlConnection con = _db.GetConnection();
            SqlCommand cmd = new SqlCommand(
                "SELECT * FROM Users WHERE Email=@e", con);

            cmd.Parameters.AddWithValue("@e", email);
            con.Open();

            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                return new User
                {
                    UserId = (int)dr["UserId"],
                    FullName = dr["FullName"].ToString(),
                    Email = dr["Email"].ToString()
                };
            }
            return null;
        }

        public void RegisterGoogleUser(User user)
        {
            using SqlConnection con = _db.GetConnection();
            SqlCommand cmd = new SqlCommand(
                "INSERT INTO Users (FullName, Email, Password, CreatedAt) VALUES (@n,@e,NULL,GETDATE())", con);

            cmd.Parameters.AddWithValue("@n", user.FullName);
            cmd.Parameters.AddWithValue("@e", user.Email);

            con.Open();
            cmd.ExecuteNonQuery();
        }
    }
}
