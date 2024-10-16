using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco.Web.Mvc;
using Newtonsoft.Json.Linq;
using Umbraco.Core.Persistence;
using Umbraco.Core;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Globalization;
using System.Net;
using System.IO;
using System.Security.Cryptography;
using System.Net.Mail;

namespace LFZ_Logic.Controllers
{
    public class ApplyNowController : SurfaceController
    {
        // GET: ContactUs
        public ActionResult Index()
        {
            return View();
        }
        public readonly UmbracoDatabase _database;
        public ApplyNowController()
        {
            _database = ApplicationContext.Current.DatabaseContext.Database;
        }
        [HttpPost]
        public string SubmitApplyNowForm(string name, string emailId, string mobile, string enquiryFor, string message)
        {
            string sRes = "";
            
            try
            {

                sRes = saveData(name, emailId, mobile, enquiryFor, message);
                return sRes;   
            }
            catch (Exception exp)
            {
                return "fail";
            }
        }

        private string saveData(string Name, string EmailId, string MobileNumber, string EnquiryFor, string Message)
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
            OCmd.CommandText = "Sp_InsertApplyNow";
            OCmd.CommandTimeout = 30000;

            OCmd.Parameters.AddWithValue("@Name", Name);
            OCmd.Parameters.AddWithValue("@EmailId", EmailId);
            OCmd.Parameters.AddWithValue("@MobileNumber", MobileNumber);
            OCmd.Parameters.AddWithValue("@EnquiryFor", EnquiryFor);
            OCmd.Parameters.AddWithValue("@Message", Message);

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
                string sSubject = "Apply Now";
                string sBodyMessage = "<b>Apply Now :-</b><br /><p> <b>Name :</b> " + Name + ":</p><p><b>EmailId : </b>" + EmailId + "</p><p><b>Mobile Number :</b> " + MobileNumber + "</P><p><b>EnquiryFor :</b> " + EnquiryFor + "</P><p><b>Message :</b> " + Message + "</P><p><b>Query Posted On : </b>" + DateTime.Now + "</P>";

                string sBody = "Hi,<br/><br/>Thank you for apply now for the Lagos Free Zone.<br/><br/><font style=\"font-size:16px\">" + sBodyMessage + "</font><br/>Thanks,<br/>LFZ Team";

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