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

        Dictionary<string, StampsList> firstDictionary;
        Dictionary<NumberTeamKey, bool> secondDictionary;
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
        public ActionResult LoadData(HttpPostedFileBase csvFile) {
            ReadFile(csvFile);
            return View();

        }

        [HttpGet]
        public ActionResult ModInfo() {
            return View();
        }

        private void ReadFile(HttpPostedFileBase csvFile) {
            string path = string.Empty;

            if (csvFile != null){
                if (".csv".Equals(Path.GetExtension(csvFile.FileName), StringComparison.OrdinalIgnoreCase)){
                    string fileName = Path.GetFileName(csvFile.FileName);
                    path = Path.Combine(Server.MapPath("~/App_Data/Files"), fileName);
                    csvFile.SaveAs(path);

                    string file = System.IO.File.ReadAllText(path);
                    foreach (string line in file.Split('\n'))
                    {
                        if (!string.IsNullOrEmpty(line))
                        {
                            string[] items = Regex.Split(line, ",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

                            try
                            {
                                StampModel stampModel = new StampModel()
                                {
                                    stampNumber = int.Parse(items[0]),
                                    stampTeam = items[1],
                                    stampQuantity = int.Parse(items[2])
                                    //Producer = items[3],
                                    //Price = float.Parse(items[4].Trim('$')),
                                    //Stock = int.Parse(items[5])
                                };

                                if (stampModel.stampQuantity == 0)
                                {
                                    stampList.InsertList(stampModel, 0);
                                }
                                else if (stampModel.stampQuantity == 1)
                                {

                                }
                                else
                                {
                                    
                                }                            
                                
                            }
                            catch (Exception)
                            {
                                MyTree.WipeOut();
                                return false;
                            }
                        }
                    }
                }
            }
            return succeed;
        }
    }
}