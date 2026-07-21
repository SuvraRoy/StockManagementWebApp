using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;

/// <summary>
/// Summary description for BSK
/// </summary>
public class BSK
{
    string skey = ""; //IP16TDW0L5AQB41V6S6J8QLTPLRXBV2W";
    string svector = "";//  "V0ZMZO6WZ45KY2PL";
    string passcode = "SbCsVkT@2962";
    public BSK()
    {
        skey = "KMVO2874KVBQG23B9UXKCRKHCG84N4GX";
        svector = "GMMWJM0V906F0LIW";
       // skey = "IP16TDW0L5AQB41V6S6J8QLTPLRXBV2W"; Test
        //svector = "V0ZMZO6WZ45KY2PL"; //Test

    }
    public string GetPass()
    {
        return passcode;
    }
    public string Encrypt2Base64(string JSonData)
    {
        byte[] bkey = new byte[0];
        byte[] bvector = new byte[0];
        byte[] bdata = new byte[0];
        string sdata = "", err = "ERROR";
        try
        {
            bkey = Encoding.UTF8.GetBytes(skey);
            bvector = Encoding.UTF8.GetBytes(svector);
        }
        catch
        {
            return err;
        }

        try
        {
            bdata = EncryptStringToBytes_Aes(JSonData, bkey, bvector);
        }
        catch
        {
            return err;
        }

        if (bdata == null || bdata.Length <= 0)
            return err;
        try
        {
            sdata = Convert.ToBase64String(bdata);
        }
        catch
        {
            return err;
        }

        return sdata;
    }

    public string Decrypt2PlaneText(string Base64encData)
    {
        byte[] bdata = new byte[0];
        byte[] bkey = new byte[0];
        byte[] bvector = new byte[0];
        string ddata = "";
        try
        {
            bdata = Convert.FromBase64String(Base64encData); // Encoding.ASCII.GetBytes(encdata);
            bkey = Encoding.UTF8.GetBytes(skey);
            bvector = Encoding.UTF8.GetBytes(svector);
        }
        catch(Exception ex)
        {
            return ex.Message;
        }
       
        try
        {
            ddata = DecryptStringFromBytes(bdata, bkey, bvector);
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
        return ddata;
    }
    public BSKReportRequestData GetReportCall(string EncData)
    {
        BSKReportRequestData bskerror = new BSKReportRequestData();
        bskerror.passcode= "ERROR";
        bskerror.fromdate = "Unspecific Error, Please Try Later";
        EncData = EncData.Replace(" ", "+");

        byte[] bdata = new byte[0];
        byte[] bkey = new byte[0];
        byte[] bvector = new byte[0];
        string ddata = "";
        try
        {
            bdata = Convert.FromBase64String(EncData); // Encoding.ASCII.GetBytes(encdata);
            bkey = Encoding.UTF8.GetBytes(skey);
            bvector = Encoding.UTF8.GetBytes(svector);
        }
        catch
        {
            bskerror.fromdate = "Unspecific Error, Please Try Later";
            return bskerror;
        }
        try
        {
            ddata = DecryptStringFromBytes(bdata, bkey, bvector);
        }
        catch
        {
            bskerror.fromdate = "Unspecific Error, Please Try Later";
            return bskerror;
        }

        string findchar = "}";
        if (!ddata.Contains("}"))
        {
            bskerror.fromdate = "Unspecific Error, Please Try Later";
            return bskerror;
        }

        if (ddata.Contains("]"))
            findchar = "]";

        Int32 ActLngth = ddata.IndexOf(findchar);
        if (ActLngth == 0)
        {
            bskerror.fromdate = "Unspecific Error, Please Try Later";
            return bskerror;
        }
        ddata = ddata.Substring(0, ActLngth + 1);
        if (findchar == "}")
        {
            ddata = "[" + ddata + "]";
        }
        JavaScriptSerializer js = new JavaScriptSerializer();
        try
        {
            BSKReportRequestData[] r = js.Deserialize<BSKReportRequestData[]>(ddata);
            //for (int i=0; r.
            if (r.Length == 0)
            {
                return bskerror;
            }
            return r[0];
        }
        catch 
        {
            bskerror.fromdate = "Unspecific Error, Please Try Later";
        }
        return bskerror;
    }

    public BSKLoginData GetLoginCall(string EncData)
    {
        BSKLoginData bskerror = new BSKLoginData();
        bskerror.userid = "ERROR";
        bskerror.citizenname = "Unspecific Error, Please Try Later";
        EncData = EncData.Replace(" ", "+");

        byte[] bdata = new byte[0];
        byte[] bkey = new byte[0];
        byte[] bvector = new byte[0];
        string ddata = "";
        try
        {
            bdata = Convert.FromBase64String(EncData); // Encoding.ASCII.GetBytes(encdata);
            bkey = Encoding.UTF8.GetBytes(skey);
            bvector = Encoding.UTF8.GetBytes(svector);
        }
        catch
        {
            bskerror.citizenname = "Parameter Error";
            return bskerror;
        }
        try
        {
            ddata = DecryptStringFromBytes(bdata, bkey, bvector);
        }
        catch
        {
            bskerror.citizenname = "Invalid Value, Could not be converted";
            return bskerror;
        }

        string findchar = "}";
        if (!ddata.Contains("}"))
        {
            bskerror.citizenname = "Incomplete Call Data, Could not be converted";
            return bskerror;
        }

        if (ddata.Contains("]"))
            findchar = "]";

        Int32 ActLngth = ddata.IndexOf(findchar);
        if (ActLngth == 0)
        {
            bskerror.citizenname = "Incomplete Call Data, Could not be converted";
            return bskerror;
        }
        ddata = ddata.Substring(0, ActLngth + 1);
        if (findchar == "}")
        {
            ddata = "[" + ddata + "]";
        }
        JavaScriptSerializer js = new JavaScriptSerializer();
        try
        {
            BSKLoginData[] r = js.Deserialize<BSKLoginData[]>(ddata);
            //for (int i=0; r.
            if (r.Length == 0)
            {
                bskerror.citizenname = "Data is Corrupted, No Data Found";
                return bskerror;
            }
            return r[0];
        }
        catch (Exception ex)
        {
            bskerror.citizenname = "Data is Corrupted, Could not serialize";
        }
        return bskerror;
    }
    static string DecryptStringFromBytes(byte[] cipherText, byte[] Key, byte[] IV)
    {
       
        // Check arguments.
        if (cipherText == null || cipherText.Length <= 0)
            throw new ArgumentNullException("cipherText");
        if (Key == null || Key.Length <= 0)
            throw new ArgumentNullException("Key");
        if (IV == null || IV.Length <= 0)
            throw new ArgumentNullException("Vector");

        // Declare the string used to hold
        // the decrypted text.
        string plaintext = null;

        // Create an RijndaelManaged object
        // with the specified key and IV.
        try
        {
            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = Key;
                rijAlg.IV = IV;
                rijAlg.Mode = CipherMode.CBC;
                rijAlg.Padding = PaddingMode.Zeros;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }

        }
        catch (Exception ex)
        {
            return ex.Message;
        }
        return plaintext;
    }

    static byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
    {

        // Check arguments.
        if (plainText == null || plainText.Length <= 0)
            throw new ArgumentNullException("plainText");
        if (Key == null || Key.Length <= 0)
            throw new ArgumentNullException("Key");
        if (IV == null || IV.Length <= 0)
            throw new ArgumentNullException("IV");
        byte[] encrypted;

        // Create an AesCryptoServiceProvider object
        // with the specified key and IV.
        using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
        {
            aesAlg.Key = Key;
            aesAlg.IV = IV;

            // Create an encryptor to perform the stream transform.
            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            // Create the streams used for encryption.
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        //Write all data to the stream.
                        swEncrypt.Write(plainText);
                    }
                    encrypted = msEncrypt.ToArray();
                }
            }
        }

        // Return the encrypted bytes from the memory stream.
        return encrypted;
    }
}
 public class BSKLoginData
{
    public string userid { get; set; }
    public string ticketno { get; set; }
    public string citizenmobile { get; set; }
    public string citizenemail { get; set; }
    public string citizenname { get; set; }

}
 public class BSKReportData
 {
     public string ticketno { get; set; }
     public string userid { get; set; }
     public string citizenmobile { get; set; }
     public string appno { get; set; }
     public string appsubtime { get; set; }
     public string deptpayrefno { get; set; }
     public string transno { get; set; }
     public string bankrefno { get; set; }
     public int paidamt { get; set; }
     public string applicationstatus { get; set; }
     public string statuscode { get; set; }
     public string message { get; set; }

 }

 public class BSKReportRequestData
 {
     public string passcode { get; set; }
     public string fromdate { get; set; }
     public string todate { get; set; }
     //public string limit { get; set; }
     //public string offset { get; set; }
     public int limit { get; set; }
     public int offset { get; set; }
 }

 public class BSKReportReturnData
 {
     public string status { get; set; } //s/F
     public string statusCode { get; set; } //200/500
     public string statusDesc { get; set; }
     public string encData { get; set; }
 }
