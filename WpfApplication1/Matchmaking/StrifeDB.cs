using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace IronStrife.Matchmaking
{
    class StrifeDB
    {
        const string connectionString = "server=isedbinstance.chxcq0xz0ul7.us-east-1.rds.amazonaws.com; database=ISEdatabase; uid=ISE; pwd=yoloswag420";

        public static int GetSkillLevel(int userId)
        {
            string query = "SELECT P.skillRating FROM PlayerStats P WHERE P.user_id = " + userId;

            MySqlConnection dbConn = new MySqlConnection(connectionString);
            dbConn.Open();
            MySqlCommand command = new MySqlCommand(query, dbConn);

            var reader = command.ExecuteReader();
            reader.Read();
            var val = reader.GetInt32("skillRating");
            return val;
        }
    }
}
