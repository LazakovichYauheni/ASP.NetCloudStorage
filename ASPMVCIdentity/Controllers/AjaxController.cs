using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.Data.SqlClient;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace ASPMVCIdentity.Controllers
{
    public class AjaxController : Controller
    {
        private IHostingEnvironment env { get; }
        public AjaxController(IHostingEnvironment env) => this.env = env;

        [HttpPost]
        [ValidateAntiForgeryToken]
        public string UploadFile()      //Method to upload files
        {
            int i;
            for (i = 0; i < Request.Form.Files.Count; i++)
            {
                if (Request.Form.Files[i].Length > 0)
                {  
                    try
                    {
                        string file = env.WebRootPath + "/Files/" + Request.Form.Files[i].FileName;
                        using (FileStream fs = new FileStream(file, FileMode.Create, FileAccess.Write))
                        {
                            const int bufsize = 500000000;
                            byte[] buffer = new byte[bufsize];
                            using (Stream stream = Request.Form.Files[i].OpenReadStream())
                            {
                                int b = stream.Read(buffer, 0, (int)bufsize);
                                int written = b;
                                while (b > 0)
                                {
                                    fs.Write(buffer, 0, b);
                                    b = stream.Read(buffer, 0, (int)bufsize);
                                    written += b;
                                }
                            }
                        }
                        #region Connect to SQL
                        var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                        IConfigurationRoot configuration = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json")
                            .Build();
                        string constr = configuration.GetConnectionString("DefaultConnection");
                        using (SqlConnection conn = new SqlConnection(constr))
                        {
                            string sql = "INSERT INTO UploadFiles (Name,Path,FileId) VALUES (@Name, @Path, @Id)";
                            using (SqlCommand cmd = new SqlCommand(sql, conn))
                            {
                                cmd.Parameters.AddWithValue("@Name", Request.Form.Files[i].FileName);
                                cmd.Parameters.AddWithValue("@Path", file);
                                cmd.Parameters.AddWithValue("@Id", userId);
                                conn.Open();
                                cmd.ExecuteNonQuery();
                                conn.Close();
                            }
                        }
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        return ex.Message.ToString();
                    }
                    
                }
            }
            if (i == 1)
                return i.ToString() + "file copied.";
            return i.ToString() + "files copied.";
        }
    }
}