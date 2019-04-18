using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text.RegularExpressions;
using Dictionarys.Models;

namespace Dictionarys.Controllers {
    public class AlbumController : Controller {

        Dictionary<string, List<NumberTeamKey>> firstDictionary = new Dictionary<string, List<NumberTeamKey>>();
        Dictionary<NumberTeamKey, bool> secondDictionary = new Dictionary<NumberTeamKey, bool>();
        StampsList stampList = new StampsList();

        [HttpGet]
        public ActionResult MissingStamps() {
            return View();
        }

        [HttpGet]
        public ActionResult LoadData() {
            return View();
        }

        [HttpPost]
        public ActionResult LoadData(HttpPostedFileBase csvFile, string csvType) {
            ReadFile(csvFile,csvType);
            return View();

        }

        [HttpGet]
        public ActionResult ModInfo() {
            return View();
        }

        private void ReadFile(HttpPostedFileBase csvFile, string csvType) {
            string path = string.Empty;

            if (csvFile != null){
                if (".csv".Equals(Path.GetExtension(csvFile.FileName), StringComparison.OrdinalIgnoreCase)){
                    string fileName = Path.GetFileName(csvFile.FileName);
                    path = Path.Combine(Server.MapPath("~/App_Data/Files"), fileName);
                    csvFile.SaveAs(path); 

                    string file = System.IO.File.ReadAllText(path);
                    foreach (string line in file.Split('\n')) {
                        if (!string.IsNullOrEmpty(line)) {

                            string[] items = Regex.Split(line, ",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

                            if (csvType.Equals("Format")) {
                                try {
                                    NumberTeamKey stampModel = new NumberTeamKey() {
                                        stampNumber = int.Parse(items[0]),
                                        stampTeam = items[1],                                        
                                        isSpecial = items[2].Equals("TRUE\r")
                                    };

                                    if (firstDictionary.ContainsKey(stampModel.stampTeam))
                                    {
                                        List<NumberTeamKey> stamps = firstDictionary[stampModel.stampTeam];
                                        stamps.Add(stampModel);
                                        firstDictionary[stampModel.stampTeam] = stamps;
                                    } else {
                                        firstDictionary.Add(stampModel.stampTeam, new List<NumberTeamKey>() { stampModel });
                                    }
                                }
                                catch (Exception) {

                                }
                            } else {
                                try {
                                    NumberTeamKey numberkey = new NumberTeamKey() { stampNumber = int.Parse(items[0]), stampTeam = items[1], isSpecial = items[2].Equals("TRUE") };
                                    StampsList stamplist = new StampsList() { stampQuantity = int.Parse(items[3]), isAvailable = items[4].Equals("TRUE") };
                                    secondDictionary.Add(numberkey, stampList.isAvailable.Equals("TRUE\r"));

                                }
                                catch (Exception) {

                                }
                            }
                            
                        }
                    }
                }
            }
            
        }
    }
}