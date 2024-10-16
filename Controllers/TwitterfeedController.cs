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

namespace LFZ_Logic.Controllers
{
    public class TwitterfeedController : SurfaceController
    {
        // GET: Twitterfeed
        public ActionResult Index()
        {
            return View();
        }
        public readonly UmbracoDatabase _database;

        public TwitterfeedController()
        {
            _database = ApplicationContext.Current.DatabaseContext.Database;
        }
        [HttpGet]
        //[OutputCache(Duration=600, VaryByParam="none")]
        public JsonResult GetTwitterFeeds()
        {
            string sTiwtterFeeds = "";

            DataTable oDt = CheckInDatabaseTW();

            DateTime d1 = Convert.ToDateTime(System.DateTime.Now.ToString());

            DateTime d2 = Convert.ToDateTime(oDt.Rows[0]["Addedon"].ToString());

            int diffMinutes = (d1 - d2).Minutes;

            int diffHours = (d1 - d2).Hours;

            int diffDays = (d1 - d2).Days;

            diffHours = diffHours + (diffDays * 24);

            diffMinutes = diffMinutes + (diffHours * 60);

            if (diffMinutes > 60)
            {
                int j = 0;

                sTiwtterFeeds = GetTwitterFeedsList();

                JArray jsonDatTw = JArray.Parse("[" + sTiwtterFeeds.ToString() + "]");

                if (jsonDatTw.Count() > 0)
                {
                    for (int k = 0; k < jsonDatTw.Count(); k++)
                    {
                        InsertDatabaseTW(jsonDatTw[k]["id"].ToString(), jsonDatTw[k]["text"].ToString(), jsonDatTw[k]["image"].ToString(), jsonDatTw[k]["url"].ToString().Trim(), jsonDatTw[k]["time"].ToString().Trim(), "Twitter", j);
                        j++;
                    }
                }
                else
                {
                    DataSet oDsFBTW = new DataSet();

                    oDsFBTW = GetDatabaseTWFeeds();

                    if (oDsFBTW.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < oDsFBTW.Tables[0].Rows.Count; i++)
                        {
                            if (i == 0)
                            {
                                sTiwtterFeeds = "{\"text\":\"" + oDsFBTW.Tables[0].Rows[i]["text"].ToString() + "\",\"image\":\"" + oDsFBTW.Tables[0].Rows[i]["image"].ToString() + "\",\"url\":\"" + oDsFBTW.Tables[0].Rows[i]["url"].ToString() + "\",\"time\":\"" + oDsFBTW.Tables[0].Rows[i]["time"].ToString() + "\"}";
                            }
                            else
                            {
                                sTiwtterFeeds = sTiwtterFeeds + ",{\"text\":\"" + oDsFBTW.Tables[0].Rows[i]["text"].ToString() + "\",\"image\":\"" + oDsFBTW.Tables[0].Rows[i]["image"].ToString() + "\",\"url\":\"" + oDsFBTW.Tables[0].Rows[i]["url"].ToString() + "\",\"time\":\"" + oDsFBTW.Tables[0].Rows[i]["time"].ToString() + "\"}";
                            }
                        }
                    }
                }
            }
            else
            {
                DataSet oDsFBTW = new DataSet();

                oDsFBTW = GetDatabaseTWFeeds();

                if (oDsFBTW.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < oDsFBTW.Tables[0].Rows.Count; i++)
                    {
                        if (i == 0)
                        {
                            sTiwtterFeeds = "{\"text\":\"" + oDsFBTW.Tables[0].Rows[i]["text"].ToString() + "\",\"image\":\"" + oDsFBTW.Tables[0].Rows[i]["image"].ToString() + "\",\"url\":\"" + oDsFBTW.Tables[0].Rows[i]["url"].ToString() + "\",\"time\":\"" + oDsFBTW.Tables[0].Rows[i]["time"].ToString() + "\"}";
                        }
                        else
                        {
                            sTiwtterFeeds = sTiwtterFeeds + ",{\"text\":\"" + oDsFBTW.Tables[0].Rows[i]["text"].ToString() + "\",\"image\":\"" + oDsFBTW.Tables[0].Rows[i]["image"].ToString() + "\",\"url\":\"" + oDsFBTW.Tables[0].Rows[i]["url"].ToString() + "\",\"time\":\"" + oDsFBTW.Tables[0].Rows[i]["time"].ToString() + "\"}";
                        }
                    }
                }
            }

            StringBuilder sFilnalJson = new StringBuilder();

            sFilnalJson.Append("[" + sTiwtterFeeds + "]");

            var jsonResult = Json(sFilnalJson.ToString(), JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        private string GetTwitterFeedsList()
        {

            string q = "LFZTolaram";

            string resource_url = "https://api.twitter.com/1.1/statuses/user_timeline.json";

            var oauth_token = "324759702-xQsYN05G7wRvDmtltA1Ekv3uQPOirENlZ9BTeDyu";

            var oauth_token_secret = "2D1wtQeY29tnlUKoGxfkl1LkSZ2OKr1WMaQQLSN2DG6Wm";

            var oauth_consumer_key = "oXuAFE9kTOUiisN4SFl6Q";

            var oauth_consumer_secret = "MQIcgHN8SRXbr8LrFcuyPBGIwwdFrgrm1oxiD8fSs";

            // oauth implementation details

            var oauth_version = "1.0";

            var oauth_signature_method = "HMAC-SHA1";

            // unique request details

            var oauth_nonce = Convert.ToBase64String(

                new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString()));

            var timeSpan = DateTime.UtcNow

                - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

            var oauth_timestamp = Convert.ToInt64(timeSpan.TotalSeconds).ToString();

            // create oauth signature

            var baseFormat = "oauth_consumer_key={0}&oauth_nonce={1}&oauth_signature_method={2}" +

                            "&oauth_timestamp={3}&oauth_token={4}&oauth_version={5}&screen_name={6}";

            var count = 10;
            var baseString = string.Format(baseFormat,

                                        oauth_consumer_key,

                                        oauth_nonce,

                                        oauth_signature_method,

                                        oauth_timestamp,

                                        oauth_token,

                                        oauth_version,

                                        Uri.EscapeDataString(q)
                                        );

            baseString = string.Concat("GET&", Uri.EscapeDataString(resource_url), "&", Uri.EscapeDataString(baseString));
                       
            var compositeKey = string.Concat(Uri.EscapeDataString(oauth_consumer_secret),

                                    "&", Uri.EscapeDataString(oauth_token_secret));
                       
            string oauth_signature;

            using (HMACSHA1 hasher = new HMACSHA1(ASCIIEncoding.ASCII.GetBytes(compositeKey)))
            {

                oauth_signature = Convert.ToBase64String(

                    hasher.ComputeHash(ASCIIEncoding.ASCII.GetBytes(baseString)));

            }
            // create the request header

            var headerFormat = "OAuth oauth_nonce=\"{0}\", oauth_signature_method=\"{1}\", " +

                               "oauth_timestamp=\"{2}\", oauth_consumer_key=\"{3}\", " +

                               "oauth_token=\"{4}\", oauth_signature=\"{5}\", " +

                               "oauth_version=\"{6}\"";

            var authHeader = string.Format(headerFormat,

                                    Uri.EscapeDataString(oauth_nonce),

                                    Uri.EscapeDataString(oauth_signature_method),

                                    Uri.EscapeDataString(oauth_timestamp),

                                    Uri.EscapeDataString(oauth_consumer_key),

                                    Uri.EscapeDataString(oauth_token),

                                    Uri.EscapeDataString(oauth_signature),

                                    Uri.EscapeDataString(oauth_version)

                            );

            ServicePointManager.Expect100Continue = false;

            // make the request

            var postBody = "screen_name=" + Uri.EscapeDataString(q);//

            resource_url += "?" + postBody;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(resource_url);

            request.Headers.Add("Authorization", authHeader);

            request.Method = "GET";

            request.ContentType = "application/x-www-form-urlencoded";

            var response = (HttpWebResponse)request.GetResponse();

            var reader = new StreamReader(response.GetResponseStream());


            var objText = reader.ReadToEnd();

            JArray jsonDat = JArray.Parse(objText);
            StringBuilder sJson = new StringBuilder();

            int ss = 0;
            for (int x = 0; x < jsonDat.Count(); x++)
            {
                string sIn_reply_to_screen_name = "";
                string sCId = "";

                if (jsonDat[x]["in_reply_to_status_id"] != null)
                {
                    if (jsonDat[x]["in_reply_to_status_id"].ToString().Trim() != "")
                    {
                        sIn_reply_to_screen_name = jsonDat[x]["in_reply_to_status_id"].ToString();
                    }
                }

                if (sIn_reply_to_screen_name.Trim() == "null" || sIn_reply_to_screen_name.Trim() == "")
                {
                    //html += "Id : " + jsonDat[x]["id"].ToString();
                    string sText = jsonDat[x]["text"].ToString().Replace("\"", "");//.Replace("’", "").Replace("'", "");
                    sCId = jsonDat[x]["id"].ToString();
                    string complete = "";
                    string test = "";
                    string slink = "";
                    sText = sText.Replace("\\n", " ").Replace("\"", "'");
                    string[] strArry = sText.Split(' ');
                    foreach (string str in strArry)
                    {
                        if (str.Contains("http"))
                        {
                            // slink =  str ;
                            complete += test + " ";
                        }
                        else
                        {

                            complete += str + " ";
                        }
                    }

                    sText = complete;

                    slink = "https://twitter.com/LFZTolaram/status/" + jsonDat[x]["id"].ToString().Replace("\"", "");

                    string sCreateDate = jsonDat[x]["created_at"].ToString().Replace("\"", "");
                    
                    ss = ss + 1;
                    DateTime dt = DateTime.ParseExact(sCreateDate,
                                          "ddd MMM dd HH:mm:ss zzz yyyy",
                                          CultureInfo.InvariantCulture,
                                          DateTimeStyles.AdjustToUniversal);
                    dt = dt.AddHours(5.61);//5.30

                    DateTime sDateTime = DateTime.Now;
                    var utcTime = "";


                    //if (sDateTime.Date == dt.Date)
                    //{

                    //    TimeSpan duration = sDateTime - dt;

                    //    utcTime = duration.Hours.ToString() + " h";
                    //    if (utcTime == "0 h")
                    //        utcTime = duration.Minutes.ToString() + " m";

                    //    if (utcTime == "0 m")
                    //        utcTime = duration.Seconds.ToString() + " s";
                    //}
                    //else
                    //{
                    utcTime = dt.ToString("dd MMM yyyy");
                    //}


                    var objText1 = "[" + jsonDat[x]["user"].ToString().Replace("\r", "").Replace("\n", "") + "]";

                    JArray jsonDat1 = JArray.Parse(objText1);


                    for (int i = 0; i < jsonDat1.Count(); i++)
                    {

                        if (ss == 1)
                        {
                            sJson.Append("{").Append("\"id\":").Append("\"").Append(sCId.Trim()).Append("\"").Append(",\"text\":").Append("\"").Append(sText.Trim().Replace("\n", "").Replace("\r", "")).Append("\"").Append(",\"image\":").Append("\"").Append(jsonDat1[i]["profile_image_url_https"].ToString().Replace("\"", "")).Append("\"").Append(",\"url\":").Append("\"").Append(slink).Append("\"").Append(",\"time\":");
                            sJson.Append("\"").Append(utcTime).Append("\"");

                            sJson.Append("}");
                        }
                        else
                        {
                            sJson.Append(",{").Append("\"id\":").Append("\"").Append(sCId.Trim()).Append("\"").Append(",\"text\":").Append("\"").Append(sText.Trim().Replace("\n", "").Replace("\r", "")).Append("\"").Append(",\"image\":").Append("\"").Append(jsonDat1[i]["profile_image_url_https"].ToString().Replace("\"", "")).Append("\"").Append(",\"url\":").Append("\"").Append(slink).Append("\"").Append(",\"time\":");
                            sJson.Append("\"").Append(utcTime).Append("\"");
                            sJson.Append("}");
                        }

                    }

                    //if (ss == 5)
                    //  break;


                }
            }
            return sJson.ToString();

        }
        protected DataTable CheckInDatabaseTW()
        {
            SqlConnection OConn = new SqlConnection(ConfigurationManager.ConnectionStrings["umbracoDbDSN"].ToString());
            SqlCommand OCmd = new SqlCommand();
            DataTable oDt = new DataTable();

            try
            {

                OConn.Open();
                OCmd.CommandType = CommandType.StoredProcedure;
                OCmd.Connection = OConn;
                OCmd.CommandText = "SP_CheckTwitterDataInSocialMedia";
                SqlDataAdapter dap = new SqlDataAdapter(OCmd);
                dap.Fill(oDt);

            }
            catch (Exception exp)
            {

            }
            finally
            {
                OCmd.Dispose();
                OConn.Dispose();
            }
            return oDt;
        }
        protected DataSet GetDatabaseTWFeeds()
        {
            SqlConnection OConn = new SqlConnection(ConfigurationManager.ConnectionStrings["umbracoDbDSN"].ToString());
            SqlCommand OCmd = new SqlCommand();
            DataSet oDs = new DataSet();

            try
            {

                OConn.Open();
                OCmd.CommandType = CommandType.StoredProcedure;
                OCmd.Connection = OConn;
                OCmd.CommandText = "SP_GetTwitterDataInSocialMedia";
                SqlDataAdapter dap = new SqlDataAdapter(OCmd);
                dap.Fill(oDs);

            }
            catch (Exception exp)
            {

            }
            finally
            {
                OCmd.Dispose();
                OConn.Dispose();
            }
            return oDs;
        }
        protected void InsertDatabaseTW(string sCId, string sMessage, string sImage, string sUrl, string sTime, string sType, int sCount)
        {

            try
            {
                SqlConnection OConn = new SqlConnection(ConfigurationManager.ConnectionStrings["umbracoDbDSN"].ToString());
                SqlCommand OCmd = new SqlCommand();

                OConn.Open();
                OCmd.CommandType = CommandType.StoredProcedure;
                OCmd.Connection = OConn;
                OCmd.CommandText = "SP_InsertTwitterDataInSocialMedia";
                OCmd.Parameters.AddWithValue("@CId", sCId.Replace("\"", ""));
                OCmd.Parameters.AddWithValue("@text", sMessage.Replace("\"", ""));
                OCmd.Parameters.AddWithValue("@image", sImage.Replace("\"", ""));
                OCmd.Parameters.AddWithValue("@url", sUrl.Replace("\"", ""));
                OCmd.Parameters.AddWithValue("@time", sTime.Replace("\"", ""));
                OCmd.Parameters.AddWithValue("@type", sType);
                OCmd.Parameters.AddWithValue("@sCount", sCount);

                try
                {
                    OCmd.ExecuteNonQuery();

                }
                catch (Exception exp)
                {
                    //lblError.Text = exp.Message;

                }
                finally
                {
                    OCmd.Dispose();
                    OConn.Dispose();
                }
            }
            catch (Exception exp)
            {

            }
        }
    }
}