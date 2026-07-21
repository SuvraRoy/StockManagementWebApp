using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for RazPaySetting
/// </summary>
public class RazPaySetting
{
    public string Key;
    public string Secret;
	public RazPaySetting()
	{
        Key = "rzp_live_t9wF5hegyVJjEF"; 
        Secret = "sTNIlih8Uoyw0uDCn7o8PmSG"; 
		//
        // Production
        //
	}
}
public class RazPaySettingAdm
{
    public string Key;
    public string Secret;
    public RazPaySettingAdm()
    {
        // test
        Key = "rzp_live_t9wF5hegyVJjEF"; 
        Secret = "sTNIlih8Uoyw0uDCn7o8PmSG"; 
        //
        // Production
        //
    }
}
public class RazPaySettingTest // for testing 
{
    public string Key;
    public string Secret;
    public RazPaySettingTest()
    {
        // test
        Key = "rzp_test_kkYxhh26ErGAIG";
        Secret = "xNMfM6WlapIJ6PCH67UhUxAR"; 
    }
}
public class RazPaySettingAdmTest // for testing admission
{
    public string Key;
    public string Secret;
    public RazPaySettingAdmTest()
    {
        // test
        Key = "rzp_test_kkYxhh26ErGAIG"; // "rzp_test_kkYxhh26ErGAIG";
        Secret = "xNMfM6WlapIJ6PCH67UhUxAR"; // "xNMfM6WlapIJ6PCH67UhUxAR";
    }
}

