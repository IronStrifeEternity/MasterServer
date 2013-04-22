using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace IronStrife.Matchmaking
{
    public static class StrifeDB
    {
        const string connectionString = "server=isedbinstance.chxcq0xz0ul7.us-east-1.rds.amazonaws.com; database=ISEdatabase; uid=ISE; pwd=yoloswag420";

        public static int GetSkillLevel(int userId, int defaultValue = 100)
        {
            string query = "SELECT P.skillRating FROM PlayerStats P WHERE P.user_id = " + userId;

            MySqlConnection dbConn = new MySqlConnection(connectionString);
            try
            {
                dbConn.Open();
                MySqlCommand command = new MySqlCommand(query, dbConn);

                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    var val = reader.GetInt32("skillRating");
                    return val;
                }
                else
                {
                    return defaultValue;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
                return defaultValue;
            }
        }
    }
}
