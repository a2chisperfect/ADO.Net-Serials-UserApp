using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
namespace EF.ViewModel
{
    public class SeriesEventArgs : EventArgs
    {
        public readonly int Id;
        public readonly bool Liked; 
        public SeriesEventArgs(int id, bool liked)
        {
            Id = id;
            Liked = liked;
        }
    }
    public class SeriesView : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public EventHandler<SeriesEventArgs> LikedHandler;
        public int Id { get; set; }

        public int Number { get; set; }

        public string Name { get; set; }

        public DateTime? Date { get; set; }

        public TimeSpan? Duration { get; set; }

        public string SerialName { get; set; }

        public short Season { get; set; }

        private bool liked;

        public bool Liked
        {
           get
            {
                return liked;
            }
            set
            {
                //if(DateTime.Now > Date)
                //{
                    liked = value;
                    OnPropertyChanged("Liked");
                    if (LikedHandler != null)
                    {
                        LikedHandler(this, new SeriesEventArgs(Id, liked));

                    }
                //}
                //else
                //{
                //    liked = liked;
                //}
                
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
