using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TuSalud.Models
{
    public class Orders
    {

        public string Name { get; set; }

        public string Address { get; set; }

        public int NIT { get; set; }

        public List<Drugs> Drugs { get; set; }

        public double Total { get; set; }

    }
}