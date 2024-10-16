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
using System.Net.Mail;

namespace LFZ_Logic.Controllers
{
    public class ContactUsController : SurfaceController
    {
        // GET: ContactUs
        public ActionResult Index()
        {
            return View();
        }
        public ContactUsController()
        {

        }
        public string GetViewPath(string viewName)
        {
            return $"/Views/Partials/ContactUs/{viewName}.cshtml";
        }

        [HttpGet]
        public ActionResult RenderContactUsForm()
        {
            ContactUsModel model = new ContactUsModel();

            return PartialView(GetViewPath("contact-us"), model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitContactUsForm(ContactUsModel model)
        {
            try
            {
                string check = "fail";


                if (ModelState.IsValid)
                {
                    check = saveData(model);
                }


                return Redirect("/contact-us/?id=" + check);
            }
            catch (Exception exp)
            {
                return Redirect("/contact-us/?id=" + exp.Message);
            }


        }

        private string saveData(ContactUsModel model)
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


            OCmd.CommandType = CommandType.StoredProcedure;
            OCmd.Connection = OConn;
            OCmd.CommandText = "Sp_InsertContactUs";
            OCmd.CommandTimeout = 30000;


            OCmd.Parameters.AddWithValue("@Name", model.Name);
            OCmd.Parameters.AddWithValue("@EmailId", model.EmailId);
            OCmd.Parameters.AddWithValue("@MobileNumber", model.MobileNumber);
            OCmd.Parameters.AddWithValue("@EnquiryFor", model.EnquiryFor);
            OCmd.Parameters.AddWithValue("@Message", model.Message);

            SqlParameter oParamRegId = new SqlParameter("@CId", SqlDbType.Int);
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
                string sSubject = "General Query";
                string sBodyMessage = "<b>General Query :-</b><br /><p> <b>Name :</b> " + model.Name + ":</p><p><b>EmailId : </b>" + model.EmailId + "</p><p><b>Mobile Number :</b> " + model.MobileNumber + "</P><p><b>EnquiryFor :</b> " + model.EnquiryFor + "</P><p><b>Message :</b> " + model.Message + "</P><p><b>Query Posted On : </b>" + DateTime.Now + "</P>";

                string sBody = "Hi,<br/><br/>Thank you for contact us for the Lagos Free Zone.<br/><br/><font style=\"font-size:16px\">" + sBodyMessage + "</font><br/>Thanks,<br/>LFZ Team";

               // SendMail(toName, fromName, sSubject, sBody);

                iRes = "succ";
            }
            else
            {
                iRes = "alr";
            }


            return iRes;
        }
        public void SendMail(string toName, string fromName, string subject, string body)
        {
            try
            {

                MailMessage msg = new MailMessage(fromName, toName, subject, body);
               
                msg.IsBodyHtml = true;
                string smtpname = "";
                SmtpClient mysmtp = new SmtpClient(smtpname);
                mysmtp.Credentials = new System.Net.NetworkCredential("", "");
                mysmtp.Port = 25;
                mysmtp.EnableSsl = true;
                mysmtp.Send(msg);
            }
            catch (Exception ex)
            {
                Console.Write("SendMail : error " + ex.Message.ToString());
            }

        }
    }
}