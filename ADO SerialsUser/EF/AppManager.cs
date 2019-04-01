using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using EF.Model;
using EF.ViewModel;


namespace EF
{
    public class AppManager 
    {
        public event EventHandler<EventArgs> LoggedHandler;

        private static DbConnection db;
        private static User activeUser;

        //public static SerialInfo SelectedSerial { get; set; }
        public static SerialView SelectedSerial { get; set; }

        private bool logged;

        public bool Logged
        {
            get { return logged; }
            set
            {
                logged = value;
                if (LoggedHandler != null)
                {
                    LoggedHandler(this, new EventArgs());
                }
            }
        }

        public AppManager()
        {

            logged = false;
        }
        static AppManager()
        {
            db = new DbConnection();
        }

        public void TestConnection(string userName, string pwd)
        {
            try
            {
                db.CreateConnectionString(userName, pwd);
                db.TestConnection();
                activeUser = db.GetUser(userName, pwd);
                Logged = true;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void Register(string userName, string pwd, string eMail)
        {
            if (db.CheckUserName(userName))
            {
                throw new Exception("This username is alredy using");
            }
            if(db.CheckUserEmail(eMail))
            {
                throw new Exception("This email is alredy using");
            }
            db.RegisterUser(userName, pwd, eMail);
        }


        public static List<SerialInfo> GetSerials()
        {
            var Serials = db.GetSerialsFull();
            StringBuilder s = new StringBuilder();
            foreach (var serial in Serials)
            {
                serial.Genres = db.GetSerialGenres(serial.Id);
                foreach (var g in serial.Genres)
                {
                    if (s.Length != 0)
                    {
                        s.Append(", ");
                    }
                    s.Append(g.Name);
                }
                serial.GenresToString = s.ToString();
                s.Clear();

            }
            return Serials.ToList();
        }
        public static List<SerialView> GetTop(int count)
        {
            var Serials = db.GetTop(50);
            return Serials.ToList();

        }
        public static List<SeriesInfo> GetSeries(int id)
        {
            var Series = db.GetSeries(id);
            return Series.ToList();
        }
        public static List<Genre> GetGenres()
        {
            var Genres = db.GetGenres();
            return Genres.ToList();
        }
        public static List<Status> GetStatus()
        {
            var Status = db.GetStatus();
            return Status.ToList();
        }

        public static void SetViewModelSerial(int id)
        {
            SelectedSerial = db.GetSerial(id);
            SelectedSerial.Genres = db.GetSerialGenres(id);
            SelectedSerial.UserMark = db.GetUserMark(activeUser.Id, SelectedSerial.Id);
            SelectedSerial.Watching = db.CheckWatching(activeUser.Id, SelectedSerial.Id);
            SelectedSerial.Watched = db.CheckWatched(activeUser.Id, SelectedSerial.Id);
            if (!SelectedSerial.Watching && !SelectedSerial.Watched) SelectedSerial.DontWatch = true;
            else SelectedSerial.DontWatch = false;
            SelectedSerial.WatchedHandler += WatchedHandler;
            SelectedSerial.WatchingHandler += WatchingHandler;
            SelectedSerial.DontWatchHandler += DontWatchHandler;
            SelectedSerial.UserMarkHandler += MarkHandler;
        }
        public static List<SeriesView> GetViewModelSeries()
        {
            var Series = (from s in db.GetSeries(SelectedSerial.Id)
                          select new SeriesView { Id = s.Id, Name = s.Name, Date = s.Date, Duration = s.Duration, Number = s.Number, Season = s.Season, SerialName = s.Serial_Name}).ToList();
            var Likes = db.GetLikes(activeUser.Id);
            foreach (var s in Series)
            {
                if (Likes.Contains(s.Id))
                {
                    s.Liked = true;
                }
                else
                {
                    s.Liked = false;
                }
            }
            foreach(var s in Series)
            {
                s.LikedHandler += LikedHandler;
            }
            return Series;
        }


        public static List<SeriesInfo> GetUserShedule()
        {
            var Series = db.GetUserShedule(activeUser.Id);
            return Series.ToList();

        }

        public static List<SerialView> GetUserSerials()
        {
            var Serials = db.GetUserSerials(activeUser.Id);
            foreach (var serial in Serials)
            {
                serial.Genres = db.GetSerialGenres(serial.Id);
            }
            return Serials.ToList();
        }

        public static List<SerialView> GetUserViewedSerials()
        {
            var Serials = db.GetUserViewedSerials(activeUser.Id);
            foreach (var serial in Serials)
            {
                serial.Genres = db.GetSerialGenres(serial.Id);
            }
            return Serials.ToList();

        }

        private static void LikedHandler(object sender, SeriesEventArgs e)
        {
            if(e.Liked)
            {
                db.AddLike(activeUser.Id, e.Id);
            }
            else
            {
                db.DeleteLike(activeUser.Id, e.Id);
            }
                

        }
        private static void WatchedHandler(object sender, SerialEventArgs e)
        {
            if (e.status)
            {
                db.SetWatched(activeUser.Id, e.id);
            }
            else
            {
                db.RemoveWatched(activeUser.Id, e.id);
            }
        }
        private static void WatchingHandler(object sender, SerialEventArgs e)
        {
            if (e.status)
            {
                db.SetWatching(activeUser.Id, e.id);
            }
            else
            {
               db.RemoveWatching(activeUser.Id, e.id);
            }
        }
        private static void DontWatchHandler(object sender, SerialEventArgs e)
        {
            if (e.status)
            {
                db.RemoveWatched(activeUser.Id, e.id);
                db.RemoveWatching(activeUser.Id, e.id);
            }
        }
        private static void MarkHandler(object sender, MarkEventArgs e)
        {
            db.SetUserMark(activeUser.Id, e.id, e.mark);
            SelectedSerial.AverageMark = db.GetRating(SelectedSerial.Id);
        }
    }
}
    

