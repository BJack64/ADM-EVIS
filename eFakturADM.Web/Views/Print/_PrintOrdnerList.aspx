
<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<%@ Import Namespace="System.Reflection" %>
<%@ Import Namespace="eFakturADM.Shared.Utility" %>
<%@ Import Namespace="eFakturADM.Logic.Objects" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<script runat="server">
    
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.HttpMethod == "GET")
        {
            ScriptManager scriptManager = ScriptManager.GetCurrent(this.Page);
            scriptManager.RegisterPostBackControl(this.reportContainer);
            var idprint = Session["idprint"] as string;
            if (string.IsNullOrEmpty(idprint))
            {
                //return RedirectToAction("Http404", "Errors");
                Response.Redirect(Url.Action("Http404", "Errors"));
            }
            try
            {
                var ordnerList = Session[idprint] as List<string>;
                if (ordnerList == null)
                {
                    //return RedirectToAction("Http404", "Errors");
                    Response.Redirect(Url.Action("Http404", "Errors"));
                }
                var ds = new List<PrintOrdner>();
                int id = 0;
                foreach (var it in ordnerList)
                {
                    ds.Add(new PrintOrdner()
                    {
                        FakturPajakId = id,
                        FillingIndex = it
                    });

                    id++;

                }

                var reportDataSource = new ReportDataSource("dsPrintPrintOrdners", ds);

                //Session[idprint] = null;
                //Session["idprint"] = null;

                reportContainer.AsyncRendering = false;
                reportContainer.LocalReport.ReportPath = Server.MapPath("~/bin/Reports/PrintOrdner.rdlc");
                reportContainer.LocalReport.DataSources.Clear();
                reportContainer.LocalReport.DataSources.Add(reportDataSource);
                reportContainer.DataBind();

                reportContainer.LocalReport.Refresh();

                

            }
            catch (Exception exception)
            {
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, exception.Message, MethodBase.GetCurrentMethod(), exception);
                throw;
            }
        }
        if (Request.HttpMethod == "POST") // <-- always true
        {
            ScriptManager scriptManager = ScriptManager.GetCurrent(this.Page);
            scriptManager.RegisterPostBackControl(this.reportContainer);
            var idprint = Session["idprint"] as string;
            if (string.IsNullOrEmpty(idprint))
            {
                //return RedirectToAction("Http404", "Errors");
                Response.Redirect(Url.Action("Http404", "Errors"));
            }
            try
            {
                var ordnerList = Session[idprint] as List<string>;
                if (ordnerList == null)
                {
                    //return RedirectToAction("Http404", "Errors");
                    Response.Redirect(Url.Action("Http404", "Errors"));
                }
                var ds = new List<PrintOrdner>();
                int id = 0;
                foreach (var it in ordnerList)
                {
                    ds.Add(new PrintOrdner()
                    {
                        FakturPajakId = id,
                        FillingIndex = it
                    });

                    id++;

                }

                var reportDataSource = new ReportDataSource("dsPrintPrintOrdners", ds);

                //Session[idprint] = null;
                //Session["idprint"] = null;
                string deviceInfo =
                "<DeviceInfo>" +
                "  <OutputFormat>PDF</OutputFormat>" +
                "  <PageWidth>8.27in</PageWidth>" +
                "  <PageHeight>11.69in</PageHeight>" +
                "  <MarginTop>0.3in</MarginTop>" +
                "  <MarginLeft>0in</MarginLeft>" +
                "  <MarginRight>0in</MarginRight>" +
                "  <MarginBottom>0in</MarginBottom>" +
                "</DeviceInfo>";
                
                reportContainer.AsyncRendering = false;
                reportContainer.LocalReport.ReportPath = Server.MapPath("~/bin/Reports/PrintOrdner.rdlc");
                reportContainer.LocalReport.DataSources.Clear();
                reportContainer.LocalReport.DataSources.Add(reportDataSource);
                reportContainer.DataBind();
                reportContainer.LocalReport.Refresh();

                int currentPage = 1;
                try
                {
                    currentPage = Request.Form["reportContainer$ctl05$ctl00$CurrentPage"].ToString() == "" ? 1 : int.Parse(Request.Form["reportContainer$ctl05$ctl00$CurrentPage"].ToString());
                }
                catch
                {

                }

                if (Request.Form["__EVENTTARGET"].ToString().Contains("Next") == true)
                {
                    reportContainer.CurrentPage = currentPage + 1;
                }
                else if (Request.Form["__EVENTTARGET"].ToString().Contains("Prev") == true)
                {
                    reportContainer.CurrentPage = currentPage - 1;
                }
                else if (Request.Form["__EVENTTARGET"].ToString().Contains("First") == true)
                {
                    reportContainer.CurrentPage = 1;
                }
                else if (Request.Form["__EVENTTARGET"].ToString().Contains("Last") == true)
                {
                    reportContainer.PageCountMode = PageCountMode.Actual;
                    reportContainer.CurrentPage = 9999999;
                }
            }
            catch(Exception ex)
            {
                string logKey;
                Logger.WriteLog(out logKey, LogLevel.Error, ex.Message, MethodBase.GetCurrentMethod(), ex);
                throw;
            }
        }

    }

</script>

<form id="form1" runat="server" class="form-horizontal">
    <asp:scriptmanager id="ContentScriptManager" runat="server" enablepagemethods="true"
        enablescriptglobalization="true" enablescriptlocalization="true" />
    <asp:updatepanel id="UpdatePanelContent" runat="server" updatemode="Always">
        </asp:updatepanel>
    <div>
        <rsweb:ReportViewer ID="reportContainer" runat="server" Width="100%" Height="500px" PageCountMode="Actual" ShowRefreshButton="false" ShowFindControls="false" ShowBackButton="false">
        </rsweb:ReportViewer>
    </div>

</form>
