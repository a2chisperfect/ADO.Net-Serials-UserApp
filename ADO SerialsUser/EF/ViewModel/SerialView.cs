using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EF.Model;
using System.ComponentModel;

namespace EF.ViewModel
{
    public class SerialEventArgs : EventArgs
    {
        public readonly bool status;
        public readonly int id;
        public SerialEventArgs(bool Status, int Id)
        {
            status = Status;
            id = Id;
        }
    }

    public class MarkEventArgs: EventArgs
    {
        public readonly int? mark;
        public readonly int id;
        public MarkEventArgs(int? Mark, int Id)
        {
            mark = Mark;
            id = Id;
        }
    }
    public class SerialView : INotifyPropertyChanged
    {
        public EventHandler<MarkEventArgs> UserMarkHandler;
        public EventHandler<SerialEventArgs> WatchedHandler;
        public EventHandler<SerialEventArgs> WatchingHandler;
        public EventHandler<SerialEventArgs> DontWatchHandler;
        public event PropertyChangedEventHandler PropertyChanged;
        public int Id { get; set; }
        public string Name { get; set; }
        public string TVChannel { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public int? Date { get; set; }
        public Int16 Seasons { get; set; }
        public Uri ImagePath { get; set; }

        private double? averageMark;
        public double? AverageMark
        {
            get
            {
                return averageMark;
            }
            set
            {
                averageMark = value;
                OnPropertyChanged("AverageMark");
            }
        }

        public List<Genre> Genres { get; set; }


        private int? userMark;
        public int? UserMark
        {
            get
            {
                return userMark;
            }
            set
            {
                userMark = value;
                OnPropertyChanged("UserMark");
                if (UserMarkHandler != null)
                {
                    UserMarkHandler(this, new MarkEventArgs(userMark, Id));

                }
            }
        }
        private bool watching;
        public bool Watching
        {
            get
            {
                return watching;
            }
            set
            {
                watching = value;
                OnPropertyChanged("Watching");
                if (WatchingHandler != null)
                {
                    WatchingHandler(this, new SerialEventArgs(watching,Id));

                }
            }
        }
        
        private bool watched;
        public bool Watched
        {
            get
            {
                return watched;
            }
            set
            {
                watched = value;
                OnPropertyChanged("Watched");
                if (WatchedHandler != null)
                {
                    WatchedHandler(this, new SerialEventArgs(watched,Id));

                }
            }
        }

        private bool dontWatch;
        public bool DontWatch
        {
            get
            {
                return dontWatch;
            }
            set
            {
                dontWatch = value;
                OnPropertyChanged("DontWatch");
                if (DontWatchHandler != null)
                {
                    DontWatchHandler(this, new SerialEventArgs(dontWatch, Id));

                }
            }
        }
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
