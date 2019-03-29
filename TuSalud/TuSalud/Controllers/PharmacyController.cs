using TuSalud.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Structures;
using System.Linq;


namespace TuSalud.Controllers
{
    public class PharmacyController : Controller
    {

        private static BTree<string, Drugs> Btree;

        [HttpGet]
        public ActionResult LoadBTree()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LoadBTree(HttpPostedFileBase csvFile, int order)
        {

            Btree = new BTree<string, Drugs>(order);
            ReadFile(csvFile);

            return View();

        }
        

        [HttpGet]
        public ActionResult Orders()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Refill()
        {
            return View();
        }

        [HttpGet]
        public ActionResult DisplayBTree()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DisplayBTree(string name)
        {
            PrintMed(name);
            return View();
        }

        private bool ReadFile(HttpPostedFileBase csvFile)
        {
            bool succeed = false;
            string path = string.Empty;
            // Check if the fileUpload is not empty
            if (csvFile != null)
            {
                // Check if the file is a csv
                if (".csv".Equals(Path.GetExtension(csvFile.FileName), StringComparison.OrdinalIgnoreCase))
                {
                    // Store the file
                    string fileName = Path.GetFileName(csvFile.FileName);
                    path = Path.Combine(Server.MapPath("~/App_Data/Files"), fileName);
                    csvFile.SaveAs(path);
                    // Read the file and split each element
                    string file = System.IO.File.ReadAllText(path);
                    foreach (string line in file.Split('\n'))
                    {
                        if (!string.IsNullOrEmpty(line))
                        {
                            string[] items = Regex.Split(line, ",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
                            // Create a new object and insert it into the tree
                            try
                            {
                                Drugs Drug = new Drugs()
                                {
                                    Uid = int.Parse(items[0]),
                                    Name = items[1],
                                    Description = items[2],
                                    Producer = items[3],
                                    Price = float.Parse(items[4].Trim('$')),
                                    Stock = int.Parse(items[5])
                                };
                                Btree.Insert(Drug.Name, Drug);
                                succeed = true;
                            }
                            catch (Exception)
                            {
                                Btree.WipeOut();
                                return false;
                            }
                        }
                    }
                }
            }
            return succeed;
        }

        public JsonResult PrintMed(string name)
        {
            Drugs med = Btree.Search(name);
            return Json(new { name = med.Name, description = med.Description, production = med.Producer, price = med.Price, stock = med.Stock }, JsonRequestBehavior.AllowGet);
        }
    }
}