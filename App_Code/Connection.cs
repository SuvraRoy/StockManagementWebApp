using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
/// <summary>
/// Summary description for Connection
/// </summary>
public class Connection{
    public static string getConnectionString()
    {
        return ConfigurationManager.ConnectionStrings["StockDB"].ConnectionString;
    }
	    
        public Connection()
	        {
            }

  

}
  public class Utils
    {
         public static bool isValidExtension(string fileName)
    {
        bool isValid = false;
        string[] fileExtension = { ".JPG", ".jpg", ".png", ".jpeg" };
        foreach (string file in fileExtension)
        {
            if (fileName.Contains(file))
            {
                isValid = true;
                break;
            }

        }
        return isValid;
    }

         public static string getImageUrl(Object url)
         {
             string url1 = string.Empty;
             if (string.IsNullOrEmpty(url.ToString()) || url == DBNull.Value)
             {
                 url1 = "../Images/No_image.jpg";
             }
             else
             {
                 url1 = string.Format("../{0}", url);
             }
             return url1;
         }

    }