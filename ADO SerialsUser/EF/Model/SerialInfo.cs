using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF.Model
{
    public class SerialInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string TVChannel { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public int? Date { get; set; }
        public Int16 Seasons { get; set; }
        public int? AverageMark { get; set; }
        public Uri ImagePath { get; set; }
        public List<Genre> Genres { get; set; }
        public string GenresToString { get; set; }
    }
}
