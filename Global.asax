<%@ Application Language="C#" %>

<script runat="server">
    void Application_Start(object sender, EventArgs e)
    {
        SAAO.Organization SAA = new SAAO.Organization(DateTime.Now);
        Application["org"] = SAA;
    }
</script>
