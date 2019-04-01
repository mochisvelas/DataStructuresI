using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace TuSalud.Models
{
    public class Drugs : IComparable<Drugs>
    {

        public int Uid { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Producer { get; set; }

        public double Price { get; set; }

        public int Stock { get; set; }

        public int CompareTo(Drugs other)
        {
            if (this.Name.CompareTo(other.Name) > 0)
                return -1;
            else if (this.Name.CompareTo(other.Name) < 0)
                return 1;
            else
                return 0;
        }
    }
}