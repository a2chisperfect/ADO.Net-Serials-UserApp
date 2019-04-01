using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF.Model
{
    public class SeriesInfo
    {
        public int Id { get; set; }

        public int Number { get; set; }

        public string Name { get; set; }

        public string Serial_Name { get; set; }

        public DateTime? Date { get; set; }

        public TimeSpan? Duration { get; set; }


        public short Season { get; set; }

    }
}
