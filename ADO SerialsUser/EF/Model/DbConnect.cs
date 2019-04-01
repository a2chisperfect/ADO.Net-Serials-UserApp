using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using EF.ViewModel;

namespace EF.Model
{
    class DbConnection
    {
        SqlConnection conn;
        SqlConnectionStringBuilder connectionString;
        public DbConnection()
        {
            conn = new SqlConnection();
            connectionString = new SqlConnectionStringBuilder();
        }
        public void CreateConnectionString(string userName, string pwd)
        {
            connectionString.DataSource = ConfigurationManager.AppSettings["dataSource"];
            connectionString.InitialCatalog = ConfigurationManager.AppSettings["initialCatalog"];
            connectionString.IntegratedSecurity = false;
            connectionString.UserID = userName.Trim();
            connectionString.Password = pwd.Trim();
            conn.ConnectionString = connectionString.ConnectionString;
        }
        public void TestConnection()
        {
            try
            {
                conn.Open();
                var comm = conn.CreateCommand();
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = "sp_check_user_role";
                comm.Parameters.AddWithValue("@username", connectionString.UserID);
                if ((bool)comm.ExecuteScalar() ==false)
                    throw new Exception(String.Format("Login failed for user '{0}'.",connectionString.UserID));
            }
            finally
            {
                conn.Close();
            }
        }
        public List<Genre> GetGenres()
        {
            List<Genre> temp;
            try
            {
                conn.Open();
                var comm = conn.CreateCommand();
                comm.CommandText = "SELECT * FROM Genre";
                var reader = comm.ExecuteReader();
                temp = new List<Genre>();
                temp.Add(new Genre() { Id = -1, Name = "Any" });
                while (reader.Read())
                {
                    temp.Add(new Genre() { Id = (int)reader["Id"], Name = reader["Name"].ToString() });
                }
            }
            finally
            {
                conn.Close();

            }
            return temp;
        }
        public List<Status> GetStatus()
        {
            List<Status> temp;
            try
            {
                conn.Open();
                var comm = conn.CreateCommand();
                comm.CommandText = "SELECT * FROM Status";
                var reader = comm.ExecuteReader();
                temp = new List<Status>();
                temp.Add(new Status() { Id = -1, Name = "Any" });
                while (reader.Read())
                {
                    temp.Add(new Status() { Id = (int)reader["Id"], Name = reader["Status"].ToString() });
                }
            }
            finally
            {
                conn.Close();

            }
            return temp;
        }

        public List<Genre> GetSerialGenres(int id)
        {
            List<Genre> temp;
            try
            {
                conn.Open();
                var comm = conn.CreateCommand();
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = "sp_serial_genres";
                comm.Parameters.AddWithValue("@id_serial", id);
                var reader = comm.ExecuteReader();
                temp = new List<Genre>();
                while (reader.Read())
                {
                    temp.Add(new Genre() { Id =(int)reader["Id"], Name = reader["Name"].ToString() });
                }
            }
            finally
            {
                conn.Close();

            }
            return temp;
        }


        public int? GetUserMark(int user_id, int serial_id)
        {
            int? mark;
            try
            {
                conn.Open();
                var comm = conn.CreateCommand();
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = "sp_user_mark";
                comm.Parameters.AddWithValue("@Serial_Id", serial_id);
                comm.Parameters.AddWithValue("@User_Id", user_id);
                mark = (int?)comm.ExecuteScalar();
                
            }
            finally
            {
                conn.Close();

            }
            return mark;
        }

        public void SetUserMark(int user_id, int serial_id, int? mark)
        {
            try
            {
                conn.Open();
                var comm = conn.CreateCommand();
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = "sp_add_mark";
                comm.Parameters.AddWithValue("@SerialId", serial_id);
                comm.Parameters.AddWithValue("@UserId", user_id);
                comm.Parameters.AddWithValue("@Mark", mark);
                comm.ExecuteScalar();
            }
            finally
            {
                conn.Close();

            }
           
        }
        public int? GetRating(int serial_id)
        {
            int? mark = null;
            try
            {
                conn.Open();
                var comm = conn.CreateCommand();
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = "sp_get_serial_rating";
                comm.Parameters.AddWithValue("@SerialId", serial_id);
                mark = (int?)comm.ExecuteScalar();
            }
            finally
            {
                conn.Close();

            }
            return mark;

        }

        public bool CheckWatching(int user_id, int serial_id)
        {
            bool status;
            try
            {
                conn.Open();
                var comm = conn.CreateCommand();
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = "sp_check_watchinig";
                comm.Parameters.AddWithValue("@SerialId", serial_id);
                comm.Parameters.AddWithValue("@UserId", user_id);
                status = (bool)comm.ExecuteScalar();

            }
            finally
            {
                conn.Close();

            }
            return status;
        }
        public bool CheckWatched(int user_id, int serial_id)
        {
            bool status;
            try
            {
                conn.Open();
                var comm = conn.CreateCommand();
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = "sp_check_viewded";
                comm.Parameters.AddWithValue("@SerialId", serial_id);
                comm.Parameters.AddWithValue("@UserId", user_id);
                status = (bool)comm.ExecuteScalar();

            }
            finally
            {
                conn.Close();

            }
            return status;
        }

        public void SetWatching(int user_id, int serial_id)
        {
            try
            {
                conn.Open();
                var comm = conn.CreateCommand();
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = "sp_add_watchingserial";
                comm.Parameters.AddWithValue("@SerialId", serial_id);
                comm.Parameters.AddWithValue("@UserId", user_id);
                comm.ExecuteScalar();

            }
            finally
            {
                conn.Close();

            }
        }
        public void SetWatched(int user_id, int serial_id)
        {
            try
            {
                conn.Open();
                var comm = conn.CreateCommand();
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = "sp_add_viewedserial";
                comm.Parameters.AddWithValue("@SerialId", serial_id);
                comm.Parameters.AddWithValue("@UserId", user_id);
                comm.ExecuteScalar();
            }
            finally
            {
                conn.Close();

            }

        }
        public void RemoveWatching(int user_id, int serial_id)
        {
            try
            {
                conn.Open();
                var comm = conn.CreateCommand();
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = "sp_remove_watching_serial";
                comm.Parameters.AddWithValue("@SerialId", serial_id);
                comm.Parameters.AddWithValue("@UserId", user_id);
                comm.ExecuteScalar();

            }
            finally
            {
                conn.Close();

            }
        }
        public void RemoveWatched(int user_id, int serial_id)
        {
            try
            {
                conn.Open();
                var comm = conn.CreateCommand();
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = "sp_remove_viewed_serial";
                comm.Parameters.AddWithValue("@SerialId", serial_id);
                comm.Parameters.AddWithValue("@UserId", user_id);
                comm.ExecuteScalar();
            }
            finally
            {
                conn.Close();
            }
        }

        public List<SerialInfo> GetSerialsFull()
        {
            List<SerialInfo> temp;
            try
            {
                conn.Open();
                var comm = conn.CreateCommand();
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = "sp_serial_info";
                var reader = comm.ExecuteReader();
                temp = new List<SerialInfo>();
                while (reader.Read())
                {
                    temp.Add(new SerialInfo()
                    {
                        Id = (int)reader["Id"],
                        Name = reader["Name"].ToString(),
                        TVChannel = reader["TVChannel"].ToString(),
                        Description = reader["Description"].ToString(),
                        Status = reader["Status"].ToString(),
                        ImagePath = String.IsNullOrEmpty(reader["Path"].ToString()) ? null : new Uri(reader["Path"].ToString(), UriKind.Absolute),
                        Seasons = (Int16)reader["Seasons"],
                        Date = (int)reader["Year"],
                        AverageMark = String.IsNullOrEmpty(reader["Average mark"].ToString()) ? null : (int?)reader["Average mark"]
                    });
                }

            }
            finally
            {
                conn.Close();

            }
            return temp;
        }
        public List<SeriesInfo> GetSeries(int id)
        {
            List<SeriesInfo> temp;
            try
            {
                conn.Open();
                var comm = conn.CreateCommand();
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = "sp_series";
                comm.Parameters.AddWithValue("@serial_id", id);
                var reader = comm.ExecuteReader();
                temp = new List<SeriesInfo>();
                while (reader.Read())
                {
                    temp.Add(new SeriesInfo()
                    {
                        Id = (int)reader["Id"],
                        Name = reader["SeriesName"].ToString(),
                        Number = (int)reader["Number"],
                        Serial_Name = reader["SerialName"].ToString(),
                        Season = (short)reader["Season"],
                        Date = (DateTime?)reader.GetDateTime(2),
                        Duration = (TimeSpan?)reader.GetTimeSpan(3)
                    });
                }
            }
            finally
            {
                conn.Close();

            }
            return temp;
        }
        public List<SerialView> GetTop(int count)
        {
            List<SerialView> temp;
            try
            {
                conn.Open();
                var comm = conn.CreateCommand();
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = "sp_rating";
                comm.Parameters.AddWithValue("@count", count);
                var reader = comm.ExecuteReader();
                temp = new List<SerialView>();
                while (reader.Read())
                {
                    temp.Add(new SerialView()
                    {
                        Id = (int)reader["Id"],
                        Name = reader["Name"].ToString(),
                        TVChannel = reader["TVChannel"].ToString(),
                        Description = reader["Description"].ToString(),
                        Status = reader["Status"].ToString(),
                        ImagePath = String.IsNullOrEmpty(reader["Path"].ToString()) ? null : new Uri(reader["Path"].ToString(), UriKind.Absolute),
                        Seasons = (Int16)reader["Seasons"],
                        Date = (int)reader["Year"],
                        AverageMark = String.IsNullOrEmpty(reader["Average mark"].ToString()) ? null : (int?)reader["Average mark"],
                        //UserMark = String.IsNullOrEmpty(reader["Mark"].ToString()) ? null : (int?)reader["Mark"]
                    });
                }
            }
            finally
            {
                conn.Close();

            }
            return temp;
        }

        public List<int> GetLikes(int user_id)
        {
            List<int> temp = new List<int>(); ;
            try
            {
                conn.Open();
                var comm = conn.CreateCommand();
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = "sp_my_likes";
                comm.Parameters.AddWithValue("@User_Id", user_id);
                var reader = comm.ExecuteReader();

                while (reader.Read())
                {
                    temp.Add((int)reader["Id_Series"]);
                }

            }
            finally
            {
                conn.Close();

            }
            return temp;
        }
        public void AddLike(int user_id, int series_id)
        {
            
            try
            {
                conn.Open();
                var comm = conn.CreateCommand();
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = "sp_add_like";
                comm.Parameters.AddWithValue("@User_Id", user_id);
                comm.Parameters.AddWithValue("@Series_Id", series_id);
                comm.ExecuteNonQuery();
            }
            finally
            {
                conn.Close();

            }
            
        }
        public void DeleteLike(int user_id, int series_id)
        {

            try
            {
                conn.Open();
                var comm = conn.CreateCommand();
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = "sp_remove_like";
                comm.Parameters.AddWithValue("@User_Id", user_id);
                comm.Parameters.AddWithValue("@Series_Id", series_id);
                comm.ExecuteNonQuery();
            }
            finally
            {
                conn.Close();

            }

        }

        public List<SerialView> GetUserSerials(int id)
        {
            List<SerialView> temp = new List<SerialView>();
            try
            {
                conn.Open();
                var comm = conn.CreateCommand();
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = "sp_my_serials";
                comm.Parameters.AddWithValue("@UserId", id);
                var reader = comm.ExecuteReader();

                while (reader.Read())
                {
                    temp.Add(new SerialView()
                    {
                        Id = (int)reader["Id"],
                        Name = reader["Name"].ToString(),
                        TVChannel = reader["TVChannel"].ToString(),
                        Description = reader["Description"].ToString(),
                        Status = reader["Status"].ToString(),
                        ImagePath = String.IsNullOrEmpty(reader["Path"].ToString()) ? null : new Uri(reader["Path"].ToString(), UriKind.Absolute),
                        Seasons = (Int16)reader["Seasons"],
                        Date = (int)reader["Year"],
                        AverageMark = String.IsNullOrEmpty(reader["Average mark"].ToString()) ? null : (int?)reader["Average mark"],
                        UserMark = String.IsNullOrEmpty(reader["Mark"].ToString()) ? null : (int?)reader["Mark"]
                    });
                }
            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }
            finally
            {
                conn.Close();
            }
            return temp;
        }
        public SerialView GetSerial(int id)
        {
           SerialView temp = null;
            try
            {
                conn.Open();
                var comm = conn.CreateCommand();
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = "sp_get_serial";
                comm.Parameters.AddWithValue("@SerialId", id);
                var reader = comm.ExecuteReader();

                while (reader.Read())
                {
                    temp =  new SerialView()
                    {
                        Id = (int)reader["Id"],
                        Name = reader["Name"].ToString(),
                        TVChannel = reader["TVChannel"].ToString(),
                        Description = reader["Description"].ToString(),
                        Status = reader["Status"].ToString(),
                        ImagePath = String.IsNullOrEmpty(reader["Path"].ToString()) ? null : new Uri(reader["Path"].ToString(), UriKind.Absolute),
                        Seasons = (Int16)reader["Seasons"],
                        Date = (int)reader["Year"],
                        AverageMark = String.IsNullOrEmpty(reader["Average mark"].ToString()) ? null : (int?)reader["Average mark"],
                        //UserMark = String.IsNullOrEmpty(reader["Mark"].ToString()) ? null : (int?)reader["Mark"]
                    };
                }
            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }
            finally
            {
                conn.Close();
            }
            return temp;
        }
        public List<SerialView> GetUserViewedSerials(int id)
        {
            List<SerialView> temp = new List<SerialView>();
            try
            {
                conn.Open();
                var comm = conn.CreateCommand();
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = "sp_viewed_serials";
                comm.Parameters.AddWithValue("@UserId", id);
                var reader = comm.ExecuteReader();

                while (reader.Read())
                {
                    temp.Add(new SerialView()
                    {
                        Id = (int)reader["Id"],
                        Name = reader["Name"].ToString(),
                        TVChannel = reader["TVChannel"].ToString(),
                        Description = reader["Description"].ToString(),
                        Status = reader["Status"].ToString(),
                        ImagePath = String.IsNullOrEmpty(reader["Path"].ToString()) ? null : new Uri(reader["Path"].ToString(), UriKind.Absolute),
                        Seasons = (Int16)reader["Seasons"],
                        Date = (int)reader["Year"],
                        AverageMark = String.IsNullOrEmpty(reader["Average mark"].ToString()) ? null : (int?)reader["Average mark"],
                        UserMark = String.IsNullOrEmpty(reader["Mark"].ToString()) ? null : (int?)reader["Mark"]
                    });
                }
            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }
            finally
            {
                conn.Close();
            }
            return temp;
        }

        public List<SeriesInfo> GetUserShedule(int id)
        {
            List<SeriesInfo> temp;
            try
            {
                conn.Open();
                var comm = conn.CreateCommand();
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = "sp_my_series";
                comm.Parameters.AddWithValue("@UserId", id);
                var reader = comm.ExecuteReader();
                temp = new List<SeriesInfo>();
                while (reader.Read())
                {
                    temp.Add(new SeriesInfo()
                    {
                        Id = (int)reader["Id"],
                        Name = reader["SeriesName"].ToString(),
                        Number = (int)reader["Number"],
                        Serial_Name = reader["SerialName"].ToString(),
                        Season = (short)reader["Season"],
                        Date = (DateTime?)reader.GetDateTime(2),
                        Duration = (TimeSpan?)reader.GetTimeSpan(3)
                    });
                }
            }
            finally
            {
                conn.Close();

            }
            return temp;
        }


        public void RegisterUser(string userName, string pwd, string eMail)
        {
            connectionString.DataSource = ConfigurationManager.AppSettings["dataSource"];
            connectionString.InitialCatalog = ConfigurationManager.AppSettings["initialCatalog"];
            connectionString.IntegratedSecurity = false;
            connectionString.UserID = "admin_db";
            connectionString.Password = "1337";
            conn.ConnectionString = connectionString.ConnectionString;

            try
            {
               
                conn.Open();
                var comm = conn.CreateCommand();
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = "sp_add_user";
                List<SqlParameter> list = new List<SqlParameter>();
                list.Add(new SqlParameter("@Name", userName));
                list.Add(new SqlParameter("@Password", pwd));
                list.Add(new SqlParameter("@Mail", eMail));
                comm.Parameters.AddRange(list.ToArray<SqlParameter>());

                int i = comm.ExecuteNonQuery();
            }
            finally
            {
                conn.Close();
            }


        }

        public bool CheckUserName (string userName)
        {
            connectionString.DataSource = ConfigurationManager.AppSettings["dataSource"];
            connectionString.InitialCatalog = ConfigurationManager.AppSettings["initialCatalog"];
            connectionString.IntegratedSecurity = false;
            connectionString.UserID = "admin_db";
            connectionString.Password = "1337";
            conn.ConnectionString = connectionString.ConnectionString;
            try
            {
                conn.Open();
                var comm = conn.CreateCommand();
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = "sp_check_user_name";
                comm.Parameters.AddWithValue("@UserName", userName);
                bool tmp = (bool)comm.ExecuteScalar();
                return tmp;
            }
            finally
            {
                conn.Close();
            }
          
        }
        public bool CheckUserEmail(string eMail)
        {
            connectionString.DataSource = ConfigurationManager.AppSettings["dataSource"];
            connectionString.InitialCatalog = ConfigurationManager.AppSettings["initialCatalog"];
            connectionString.IntegratedSecurity = false;
            connectionString.UserID = "admin_db";
            connectionString.Password = "1337";
            conn.ConnectionString = connectionString.ConnectionString;
            try
            {
                conn.Open();
                var comm = conn.CreateCommand();
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = "sp_check_user_mail";
                comm.Parameters.AddWithValue("@UserMail", eMail);
                bool tmp = (bool)comm.ExecuteScalar();
                return tmp;
            }
            finally
            {
                conn.Close();
            }

        }
        public User GetUser(string User, string Pwd)
        {
            User tmp = new User();
            try
            {
                conn.Open();
                var comm = conn.CreateCommand();
                comm.CommandText = "Select * from Users where Name =@User";
                comm.Parameters.AddWithValue("@User", User);
                comm.Parameters.AddWithValue("@Pwd", Pwd);

                var reader = comm.ExecuteReader();
                while (reader.Read())
                {
                    tmp.Id = (int)reader["id"]; tmp.Name = reader["Name"].ToString(); tmp.Email = reader["Email"].ToString(); tmp.Password = reader["Password"].ToString();
                }
            }
            finally
            {
                conn.Close();
            }
            return tmp;
        }
    }
}
