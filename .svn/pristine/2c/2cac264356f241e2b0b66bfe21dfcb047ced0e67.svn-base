<%@ Page Language="C#" AutoEventWireup="true" CodeFile="WorkItem_SR_Status.aspx.cs" Inherits="WorkItem_SR_Status" MasterPageFile="~/Grids.master" Theme="Default" %>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" runat="Server">WTS - SR Status</asp:Content>
<asp:Content ID="cpHead" ContentPlaceHolderID="head" runat="Server"></asp:Content>
<asp:Content ID="cpHeaderText" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="Server">SR (<span id="spanRowCount" runat="server">0</span>)</asp:Content>
<asp:Content ID="cpGridSetting" ContentPlaceHolderID="cphGridSettings" runat="Server">
	
</asp:Content>
<asp:Content ID="cpPageContentInfo" ContentPlaceHolderID="cphPageContentInfo" runat="Server">
	<table cellpadding="0" cellspacing="0" style="float: right; padding-right: 2px;">
		<tr>
			
			<td style="padding: 0px; margin: 0px;">
				
			</td>
		</tr>
	</table>
</asp:Content>
<asp:Content ID="cpBody" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
	<iti_Tools_Sharp:Grid ID="grdSR" runat="server" AllowPaging="true" PageSize="30" FirstPageText="First" LastPageText="Last" PagerMode="NumericFirstLast" AllowResize="true"
		CssClass="grid" BodyCssClass="gridBody" SelectedRowCssClass="selectedRow" HeaderCssClass="gridHeader" PagerCssClass="gridPager" FooterCssClass="gridPager" AlternatingRowColor="#dfdfdf">
	</iti_Tools_Sharp:Grid>
	
	<asp:ScriptManager ID="scriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>



	<script id="jsWorkRequest" type="text/javascript">
	    $(document).ready(function () {
	        try {
	        initializeEvents();
	        } catch (e) {

	        }
	    });

	    function initializeEvents() {
	        try {
	            $('#imgExport').click(function (event) { refreshGrid(true); });
	            $('#imgRefresh').click(function () { refreshGrid(false); });
	          
        	} catch (e) {

        	}
	    }


	    function refreshGrid(exportGrid) {
	        try {
	        if (exportGrid === undefined || !exportGrid) {
	            exportGrid = false;
	        }
	        var url = window.location.href;
	     
	        url = editQueryStringValue(url, 'Export', exportGrid);

	        window.location.href = url;
	        } catch (e) {

	        }
	    }
	</script>
</asp:Content>