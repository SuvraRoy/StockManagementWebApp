using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
/// <summary>
/// Summary description for Class1
/// </summary>
namespace DigitalIcon
{
    public class WebService : System.Web.Services.WebService
    {
        public WebService()
        {

        }

        public static string DataTableToJSON(DataTable Dt)
        {
            string[] StrDc = new string[Dt.Columns.Count];

            string HeadStr = string.Empty;
            for (int i = 0; i < Dt.Columns.Count; i++)
            {
                StrDc[i] = Dt.Columns[i].Caption;
                HeadStr += "\"" + StrDc[i] + "\":\"" + StrDc[i] + i.ToString() + "¾" + "\",";
            }
            HeadStr = HeadStr.Substring(0, HeadStr.Length - 1);
            System.Text.StringBuilder Sb = new System.Text.StringBuilder();
            Sb.Append("{\"status\":\"success\",\"msg\":\"\",\"data\":[");
            for (int i = 0; i < Dt.Rows.Count; i++)
            {

                string TempStr = HeadStr;

                for (int j = 0; j < Dt.Columns.Count; j++)
                {

                    TempStr = TempStr.Replace(Dt.Columns[j] + j.ToString() + "¾", Dt.Rows[i][j].ToString()); // on 23122016 for biometric trim removed
                }
                //Sb.AppendFormat("{{{0}}},",TempStr);

                Sb.Append("{" + TempStr + "},");

            }

            Sb = new System.Text.StringBuilder(Sb.ToString().Substring(0, Sb.ToString().Length - 1));

            if (Sb.ToString().Length > 0)
                Sb.Append("]}");

            // Sb = "{data: " + Sb + "}";

            return StripControlChars(Sb.ToString());

        }
        //To strip control characters:

        //A character that does not represent a printable character but //serves to initiate a particular action.

        public static string StripControlChars(string s)
        {
            //      s=s.Replace("\\","");
            return System.Text.RegularExpressions.Regex.Replace(s, @"[^\x20-\x7F]", "");
        }

        public void ThroughReport(System.Data.DataTable rptdt)
        {
            string ReportData = DataTableToJSON(rptdt);
            string strResult = "success";

            //object ReportData = new { Date = "2016-01-01", Note = "just a Test" };

            object objResult = ReportData; //new { status = strResult,ReportData };
            System.Web.Script.Serialization.JavaScriptSerializer ser = new System.Web.Script.Serialization.JavaScriptSerializer();

            //     string strResponse = ser.Serialize(objResult);
            string strResponse = ReportData; // ser.Serialize(objResult);

            string strCallback = Context.Request.QueryString["callback"];
            strResponse = strCallback + strResponse;
            Context.Response.Clear();
            Context.Response.ContentType = "application/json";
            Context.Response.AddHeader("content-length", strResponse.Length.ToString());
            Context.Response.Flush();
            Context.Response.Write(strResponse);
        }


        public void ThroughError(string strResult, string xmsg)
        {
            object objResult = new { status = strResult, msg = xmsg };
            System.Web.Script.Serialization.JavaScriptSerializer ser = new System.Web.Script.Serialization.JavaScriptSerializer();
            string strResponse = ser.Serialize(objResult);
            string strCallback = Context.Request.QueryString["callback"];
            //    strResponse = strCallback + "(" + strResponse + ")";
            strResponse = strCallback + strResponse;
            Context.Response.Clear();
            Context.Response.ContentType = "application/json";
            Context.Response.AddHeader("content-length", strResponse.Length.ToString());
            Context.Response.Flush();
            Context.Response.Write(strResponse);
        }
        public void ThroughInfo()
        {
            string strResult = "success";
            object objResult = new { status = strResult, msg = "Error Occurred" };
            System.Web.Script.Serialization.JavaScriptSerializer ser = new System.Web.Script.Serialization.JavaScriptSerializer();
            string strResponse = ser.Serialize(objResult);
            string strCallback = Context.Request.QueryString["callback"];
            //    strResponse = strCallback + "(" + strResponse + ")";
            strResponse = strCallback + strResponse;
            Context.Response.Clear();
            Context.Response.ContentType = "application/json";
            Context.Response.AddHeader("content-length", strResponse.Length.ToString());
            Context.Response.Flush();
            Context.Response.Write(strResponse);
        }
    }
}
