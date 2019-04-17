using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text.RegularExpressions;

namespace Dictionarys.Controllers {
    public class AlbumController : Controller {

        Dictionary<string, StampsList>
        

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
                                DrugModel Drug = new DrugModel()
                                {
                                    Uid = int.Parse(items[0]),
                                    Name = items[1],
                                    Description = items[2],
                                    Producer = items[3],
                                    Price = float.Parse(items[4].Trim('$')),
                                    Stock = int.Parse(items[5])
                                };
                                MyTree.Insert(Drug.Name, Drug);
                                succeed = true;
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