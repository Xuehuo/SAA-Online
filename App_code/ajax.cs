using System;
using System.Web;
using System.Web.SessionState;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

/// <summary>
/// General Ajax processing handler
/// </summary>
public class Ajax : IHttpHandler, IRequiresSessionState
{
    protected struct Return
    {
        public int Flag;
        public object Data;
    }

    protected Return R;
    
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json";
        try
        {
            Process(context);
        }
        catch (Exception ex)
        {
            R.Flag = 3;
            SAAO.Utility.Log(ex);
        }
        context.Response.Write(JsonConvert.SerializeObject(
            value: R,
            formatting: Formatting.Indented,
            settings: new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })
        );
    }

    public virtual void Process(HttpContext context) {}
    
    public bool IsReusable => false;
}