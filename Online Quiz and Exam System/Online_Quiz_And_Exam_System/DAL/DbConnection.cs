using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Online_Quiz_API.DAL
{
    public class DbConnection
    {
        private readonly IConfiguration _configuration;

        public DbConnection(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public SqlConnection GetConnection()
        {
            return new SqlConnection(
                _configuration.GetConnectionString("QuizDB"));
        }
    }

}
