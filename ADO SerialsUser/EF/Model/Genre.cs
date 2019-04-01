using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF.Model
{
    public class Genre
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            Genre objAsPart = obj as Genre;
            if (objAsPart == null) return false;
            else return Equals(objAsPart);
        }
        public bool Equals(Genre other)
        {
            if (other == null) return false;
            return (this.Name.Equals(other.Name));
        }
        public override int GetHashCode()
        {
            return  Id;
        }
    }
}
