using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ASPMVCIdentity.Entities;
using ASPMVCIdentity.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ASPMVCIdentity.Controllers
{
    
    public class HomeController : Controller
    {
        public usersdbContext _db;
        private IHostingEnvironment _hostingEnvironment;
        public AspNetUsers _users;
        public UploadFiles upload;
        public SharedFiles shared;
        public HomeController(IHostingEnvironment hostingEnvironment, usersdbContext db)
        {
            _hostingEnvironment = hostingEnvironment;
            _db = db;
        }

        public ActionResult Index()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var model = _db.UploadFiles.Where(f => f.FileId == userId).ToList();
                return View(model);
            }
            catch (System.NullReferenceException)
            {

            }
            return View();
        }

        [HttpGet]
        [HttpPost]
        public ActionResult Share(List<UploadFiles> files)
        {
            var checkedFiles = files.Where(f => f.isChecked == true).ToList();
            #region Connect to SharedFiles
            IConfigurationRoot configuration = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json")
                            .Build();
            string constr = configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection conn = new SqlConnection(constr))
            {
                foreach (var check in checkedFiles)
                {
                    string file = _hostingEnvironment.WebRootPath + "/Files/" + checkedFiles;
                    string sql = "INSERT INTO SharedFiles (FileName,Path,UserName) VALUES (@Name, @Path,@UserName)";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Name", check.Name);
                        cmd.Parameters.AddWithValue("@Path", file);
                        cmd.Parameters.AddWithValue("@UserName", User.Identity.Name);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                }
            }
            #endregion
            var Files = _db.SharedFiles.Select(f => f).ToList();
            return View(Files);

        }

        public async Task<IActionResult> Delete(int? id)
        {
            var file = await _db.UploadFiles.FindAsync(id);
            _db.UploadFiles.Remove(file);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteSharedFiles(int? id)
        {
            var sharedFiles = await _db.SharedFiles.FindAsync(id);
            _db.SharedFiles.Remove(sharedFiles);
            await _db.SaveChangesAsync();
            return RedirectToAction("Share");
        }

        [HttpGet]
        public ActionResult Friends()
        {

            var mod = _db.AspNetUsers.Select(x => x).ToList();
            return View(mod);
        }

        [HttpPost]
        public ActionResult Friends(List<AspNetUsers> users)
        {
            var us = users.Where(x => x.isFriend == true).ToList();
            return View(us);
        }
        public FileResult DownloadFile(string fileName)
        {
            string contentType = System.Net.Mime.MediaTypeNames.Application.Octet;
            var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "Files", fileName);
            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, contentType, fileName);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
