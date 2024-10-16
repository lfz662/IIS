using LFZ_Logic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;

namespace LFZ_Logic.Controllers
{
    public class CareersController : SurfaceController
    {
        // GET: Careers
        public ActionResult Index()
        {
            return View();
        }
        public CareersController()
        {

        }
        public string GetViewPath(string viewName)
        {
            return $"/Views/Partials/Careers/{viewName}.cshtml";
        }

        [HttpGet]
        public ActionResult RenderCareersForm()
        {
            CareersModel model = new CareersModel();

            return PartialView(GetViewPath("careers"), model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitCareersForm(CareersModel model)
        {
            try
            {
                string check = "fail";


                if (ModelState.IsValid)
                {
                    check = saveData(model);
                }


                return Redirect("/careers/?id=" + check);
            }
            catch (Exception exp)
            {
                return Redirect("/careers/?id=" + exp.Message);
            }


        }

        private string saveData(CareersModel model)
        {

            string iRes = "";

            SqlConnection OConn = new SqlConnection(ConfigurationManager.ConnectionStrings["umbracoDbDSN"].ToString());

            SqlCommand OCmd = new SqlCommand();

            try
            {
                OConn.Open();

            }
            catch (Exception Ex)
            {
                iRes = "fail";
            }

            HttpPostedFileBase file = model.UploadFile;
            string fileName = Path.GetFileNameWithoutExtension(file.FileName) +"_"+ DateTime.Now.ToString("ddMMyyhhmmsstt") + Path.GetExtension(file.FileName);
            bool folder_exists = Directory.Exists(Server.MapPath("~/uploads/Careers"));
            if (!folder_exists)
                Directory.CreateDirectory(Server.MapPath("~/uploads/Careers"));
            file.SaveAs(Server.MapPath("~/uploads/Careers/" + fileName ));
           

            OCmd.CommandType = CommandType.StoredProcedure;
            OCmd.Connection = OConn;
            OCmd.CommandText = "Sp_InsertCareers";
            OCmd.CommandTimeout = 30000;


            OCmd.Parameters.AddWithValue("@Name", model.Name);            
            OCmd.Parameters.AddWithValue("@MobileNumber", model.MobileNumber);
            OCmd.Parameters.AddWithValue("@EmailId", model.EmailId);
            OCmd.Parameters.AddWithValue("@Areaofinterest", model.Areaofinterest);
            OCmd.Parameters.AddWithValue("@EducationalBackground", model.EducationalBackground);
            OCmd.Parameters.AddWithValue("@UploadFile", fileName);

            SqlParameter oParamRegId = new SqlParameter("@CrId", SqlDbType.Int);
            oParamRegId.Direction = ParameterDirection.Output;
            OCmd.Parameters.Add(oParamRegId);

            try
            {
                OCmd.ExecuteNonQuery();

            }
            catch (Exception Ex)
            {
                iRes = "fail";
            }
            finally
            {
                OCmd.Dispose();
                OConn.Dispose();
            }

            if (Convert.ToInt32(oParamRegId.Value) > 0)
            {
                string toName = "";
                string fromName = "";
                string sSubject = "";
                string sBodyMessage = "<b>Careers Query :-</b><br /><p> <b>Name :</b> " + model.Name + ":</p><p><b>EmailId : </b>" + model.EmailId + "</p><p><b>Mobile Number :</b> " + model.MobileNumber + "</P><p><b>Area of interest :</b> " + model.Areaofinterest + "</P><p><b>Educational Background :</b> " + model.EducationalBackground + "</P><p><b>Query Posted On : </b>" + DateTime.Now + "</P>";

                string sBody = "Hi,<br/><br/>Thank you for careers apply.<br/><br/><font style=\"font-size:16px\">" + sBodyMessage + "</font><br/>Thanks,<br/>LFZ Team";

                // SendMail(toName, fromName, sSubject, sBody);

                iRes = "succ";
            }
            else
            {
                iRes = "alr";
            }


            return iRes;
        }
       
    }
}