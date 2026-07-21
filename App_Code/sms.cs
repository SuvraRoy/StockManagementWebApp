using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using System.Net;
using System.IO;

public class sms
{
    // Fields
    private string DltID, Msg, Mobiles;
    private string Variable1 = "", Variable2 = "";
    public string Campaign_CANDREGDOTP = "CANDREGDOTP";
    public string Campaign_PASSRESETOTP = "PASSRESETOTP";
    public string Campaign_PASSRESETOTPSCVT = "PASSRESETOTPSCVT";
    
    public string Campaign_VERIGYDOC = "VERIFYDOC";
  
    public string Campaign_HOMEREGNOTP = "HOMEREGNOTP";
    public string Campaign_HOMECONFOTP = "HOMECONFOTP";
    public string Campaign_HOMEAPPROTP = "HOMEAPPROTP";

    public string Campaign_APPDOCRESET = "APPDOCRESET";

    public string Campaign = "";
    public sms(string xMobiles, string xMsg, string xCampaign)
    {
        DltID = GetDLTID(xCampaign); Mobiles = xMobiles; Msg = xMsg; Campaign = xCampaign;
    }
    public sms(string xMobiles, string xMsg, string V1, String V2)
    {
        DltID = Mobiles = xMobiles; Msg = xMsg; Variable1 = V1; Variable2 = V2;
    }
    public sms(string xMobiles, string xMsg)
    {
        Mobiles = xMobiles; Msg = xMsg;
    }
    protected string GetDLTID(string SMSType)
    {
        // kono kaje lagche na
        if (SMSType == "CANDREGDOTP") return "1507164602481553471";
        if (SMSType == "PASSRESETOTP") return "1507161892085216315";
        if (SMSType == "PASSRESETOTPSCVT") return "1507166192187123010";
        if (SMSType == "VERIFYDOC") return "1507164603475568125";
        if (SMSType == "HOMEREGNOTP") return "1507162073930389691";
        if (SMSType == "HOMECONFOTP") return "1507162073934843036";
        if (SMSType == "HOMEAPPROTP") return "1507162073940879568";
        if (SMSType == "APPDOCRESET") return "1507162097176983420";
        return "";
    }
    public string SendSMS()
    {
        DltID = GetDLTID(Campaign);
        if (DltID == "") return "ERROR-INVALIDDLTID";
 //       string url = "https://pbssd.org/sapi/?x=" + this.Mobiles + "&y=" + this.Msg + "&z=" + this.Campaign;
//        string url = "sapi/?x=" + this.Mobiles + "&y=" + this.Msg + "&z=" + this.Campaign;
        // same code hase been incorporated in sapi folder too, not useful anymore

    //    return apicall(url);

        // new upadte as on 13/06/2022

        return SendSMS(this.Mobiles,this.Msg,this.Campaign);
    }
    private string SendSMS(string Mobile, string Msg, string MsgType)
    {
        //1507166192187123010
        string tempID = "1507164602481553471";
        if (MsgType == "CANDREGDOTP")
        {
            // it is directly run from the regd page
            tempID = "1507164602481553471";
        }
        else if (MsgType == "PASSRESETOTP")
            tempID = "1507161892085216315";
        else if (MsgType == "PASSRESETOTPSCVT")
            tempID = "1507166192187123010";
        else if (MsgType == "VERIFYDOC")
            tempID = "1507164603475568125";
        else if (MsgType == "HOMEREGNOTP")
            tempID = "1507162073930389691";
        else if (MsgType == "HOMECONFOTP")
            tempID = "1507162073934843036";
        else if (MsgType == "HOMEAPPROTP")
            tempID = "1507162073940879568";
        else if (MsgType == "APPDOCRESET")
            tempID = "1507162097176983420";
        else
            return "Invalid Type";

        string apistring = "http://www.oursms.in/api/v1/send-message?app_key=WkWGdjwEhAz9W1RIumYMbXKfH&app_secret=bLmnYDqyEZa11oC&dlt_template_id=" + tempID + "&campaign=" + Campaign + "&mobile_numbers=" + Mobiles + "&message_type=0&route_type=0&schedule_date=&is_flash=0";

        if (Variable1 != "")
        {
            apistring = "http://www.oursms.in/api/v1/send-message?app_key=WkWGdjwEhAz9W1RIumYMbXKfH&app_secret=bLmnYDqyEZa11oC&dlt_template_id=" + tempID + "&campaign=" + Campaign + "&mobile_numbers=" + Mobiles + "&message_type=0&route_type=0&var_enabled=1&v1=" + Variable1;
            if (Variable2 != "")
                apistring = "http://www.oursms.in/api/v1/send-message?app_key=WkWGdjwEhAz9W1RIumYMbXKfH&app_secret=bLmnYDqyEZa11oC&dlt_template_id=" + tempID + "&campaign=" + Campaign + "&mobile_numbers=" + Mobiles + "&message_type=0&route_type=0&var_enabled=1&v1=" + Variable1 + "&v2=" + Variable2;
        }
        return apicall(apistring);
    }
        
    public string apicall(string url)
    {
        HttpWebRequest httpreq = (HttpWebRequest)WebRequest.Create(url);

        try
        {
            HttpWebResponse httpres = (HttpWebResponse)httpreq.GetResponse();
            StreamReader sr = new StreamReader(httpres.GetResponseStream());
            string results = sr.ReadToEnd();
            sr.Close();
            return results;

        }
        catch(Exception ex)
        {
            return "ERROR! " + ex.Message;
        }
    }

}