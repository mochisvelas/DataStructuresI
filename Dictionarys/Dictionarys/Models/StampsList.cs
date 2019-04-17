using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dictionarys.Models {
    public class StampsList {
        public List<StampModel> missingStamps { get; set; }
        public List<StampModel> availableStamps { get; set; }
        public List<StampModel> exchangeableStamps { get; set; }
    }
}