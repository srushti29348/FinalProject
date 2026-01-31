using Microsoft.Data.SqlClient;
using Online_Quiz_And_Exam_System.Models;
using System.Collections.Generic;

namespace Online_Quiz_API.DAL
{
    public class ModuleDAL
    {
        private readonly DbConnection _db;

        public ModuleDAL(DbConnection db)
        {
            _db = db;
        }

        // =========================
        // GET ALL MODULES
        // =========================
        public List<Module> GetAllModules()
        {
            List<Module> list = new();

            using SqlConnection con = _db.GetConnection();
            SqlCommand cmd = new SqlCommand(
                "SELECT * FROM Modules", con);

            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                list.Add(new Module
                {
                    ModuleId = (int)dr["ModuleId"],
                    ModuleName = dr["ModuleName"].ToString()
                });
            }
            return list;
        }
    }
}
