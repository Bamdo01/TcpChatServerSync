using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySqlConnector;
using NEXTCHATServ.Model;

namespace NEXTCHATServ.Database
{
    public static class UserRepository
    {
        public static bool InsertUser(User user)
        {
            using (MySqlConnection conn = DbManager.GetConnection())
            {
                conn.Open();

                string query = @"INSERT INTO users (userid, password, phone, gender, birthdate)
                         VALUES (@id, @pw, @pn, @gender, @birth)";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", user.UserId);
                cmd.Parameters.AddWithValue("@pw", user.Password); // 보안 위해 실제로는 해시 권장
                cmd.Parameters.AddWithValue("@pn", user.Phone);
                cmd.Parameters.AddWithValue("@gender", user.Gender.ToString()); // 예: "Male", "Female"
                cmd.Parameters.AddWithValue("@birth", user.BirthDate.ToString("yyyy-MM-dd")); // 날짜 포맷 통일

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        //참이면 중복아이디 있는겁니당
        public static bool IsUserIdExists(string userId)
        {
            using (MySqlConnection conn = DbManager.GetConnection())
            {
                conn.Open();

                string query = "SELECT COUNT(*) FROM users WHERE userid = @id";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", userId);

                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0;
            }
        }

        //채팅 저장하는 디비데스
        //public static bool SaveChatMessage(string senderId, string message)
        //{
        //    using (MySqlConnection conn = DbManager.GetConnection())
        //    {
        //        conn.Open();

        //        string query = @"INSERT INTO chat_messages (sender_id, message) 
        //                     VALUES (@sender, @msg)";
        //        MySqlCommand cmd = new MySqlCommand(query, conn);
        //        cmd.Parameters.AddWithValue("@sender", senderId);
        //        cmd.Parameters.AddWithValue("@msg", message);

        //        return cmd.ExecuteNonQuery() > 0;
        //    }
        //}

        //채팅 저장하는 디비데스
        public static bool SaveChatMessage(ChatMessage chatMessage)
        {
            using (MySqlConnection conn = DbManager.GetConnection())
            {
                conn.Open();

                string query = @"INSERT INTO chat_messages (sender_id, message, timestamp)
                         VALUES (@sender, @msg, @time)";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@sender", chatMessage.FromUserId);
                    cmd.Parameters.AddWithValue("@msg", chatMessage.Content);
                    cmd.Parameters.AddWithValue("@time", chatMessage.Timestamp);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        //로그인





    }
}
