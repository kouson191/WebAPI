using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcApplication1.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Indexaaa()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase[] fileToUpload)
        {
            if (fileToUpload != null)
            {
                foreach (HttpPostedFileBase file in fileToUpload)
                {
                    string path = System.IO.Path.Combine(Server.MapPath("/upload"), System.IO.Path.GetFileName(file.FileName));
                    file.SaveAs(path);
                }
                ViewBag.Message = "File(s) uploaded successfully";
                
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult BatchUpload()
        {
            bool isSavedSuccessfully = true;
            int count = 0;
            string msg = "";

            string fileName = "";
            string fileExtension = "";
            string filePath = "";
            string fileNewName = "";
            try
            {
                string directoryPath = Server.MapPath("/upload");
                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);

                foreach (string f in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[count];

                    if (file != null && file.ContentLength > 0)
                    {
                        fileName = file.FileName;
                        fileExtension = Path.GetExtension(fileName);
                        //fileNewName = Guid.NewGuid().ToString() + fileExtension;
                        fileNewName = file.FileName;
                        filePath = Path.Combine(directoryPath, fileNewName);
                        file.SaveAs(filePath);

                        count++;
                    }
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                isSavedSuccessfully = false;
            }

            return Json(new
            {
                Result = isSavedSuccessfully,
                Count = count,
                Message = msg
            });
        }

    }
}
