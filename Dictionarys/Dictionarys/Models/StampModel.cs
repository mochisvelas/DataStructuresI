using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dictionarys.Models {
    public class StampModel {
        public int stampNumber { get; set; }
        public string stampTeam { get; set; }
        public bool isSpecial { get; set; }
        public int stampQuantity { get; set; }
    }
}