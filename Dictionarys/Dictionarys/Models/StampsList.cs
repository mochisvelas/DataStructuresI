using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dictionarys.Models {
    public class StampsList {
        public List<StampModel> missingStamps;
        public List<StampModel> availableStamps { get; set; }
        public List<StampModel> exchangeableStamps { get; set; }

        public void InsertList(StampModel stamp, int type) {

            if (type == 0) {
                missingStamps.Add(stamp);
            }
            else if (type == 1) {
                availableStamps.Add(stamp);
            }
            else {
                exchangeableStamps.Add(stamp);
            }
        }
        
    }
}