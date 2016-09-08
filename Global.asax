<%@ Application Language="C#" %>

<script runat="server">
    void Application_Start(object sender, EventArgs e)
    {
        // Store current organization structure when application starts
        Application.Add("org", new SAAO.Organization(DateTime.Now));
    }
</script>
