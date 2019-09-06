<%@ Page Title="" Language="C#" MasterPageFile="~/Grids.master" AutoEventWireup="true" CodeFile="MDGrid_ProductVersion_Session_Breakout.aspx.cs" Inherits="MDGrid_ProductVersion_Session_Breakout" Theme="Default" %>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" runat="Server">Master Data - Session Breakout Grid</asp:Content>
<asp:Content ID="cpHead" ContentPlaceHolderID="head" runat="Server">
    <style type="text/css">
        .upArrow {
            border: solid green;
            border-width: 0 3px 3px 0;
            display: inline-block;
            padding: 3px;
            transform: rotate(-135deg);
            -webkit-transform: rotate(-135deg);
            float: right;
            margin-right: 3px;
        }

        .downArrow {
            border: solid red;
            border-width: 0 3px 3px 0;
            display: inline-block;
            padding: 3px;
            transform: rotate(45deg);
            -webkit-transform: rotate(45deg);
            float: right;
            margin-right: 3px;
        }
    </style>
</asp:Content>
<asp:Content ID="cpHeaderText" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="Server"></asp:Content>
<asp:Content ID="cpGridSetting" ContentPlaceHolderID="cphGridSettings" runat="Server"></asp:Content>
<asp:Content ID="cpPageContentInfo" ContentPlaceHolderID="cphPageContentInfo" runat="Server">
	<table cellpadding="0" cellspacing="0" style="float: right; padding-right: 2px;">
		<tr>
			<td>
                <img id="imgHelp" src="Images/Icons/help.png" alt="Help" title="Help" width="15" height="15" style="cursor: pointer; vertical-align: middle; padding: 0px 5px 3px 5px;" />
			</td>
		</tr>
	</table>
</asp:Content>
<asp:Content ID="cpBody" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
	<iti_Tools_Sharp:Grid ID="grdMD" runat="server" AllowPaging="false" PageSize="50" FirstPageText="First" LastPageText="Last" PagerMode="NumericFirstLast" AllowResize="true"
		CssClass="grid" BodyCssClass="gridBody" SelectedRowCssClass="selectedRow" HeaderCssClass="gridHeader" PagerCssClass="gridPager" FooterCssClass="gridPager" AlternatingRowColor="#dfdfdf">
	</iti_Tools_Sharp:Grid>

	<script id="jsEvents" type="text/javascript">
		
		function refreshPage() {
			var qs = document.location.href;
            qs = editQueryStringValue(qs, 'RefData', 1);

			document.location.href = 'Loading.aspx?Page=' + qs;
		}

		function imgExport_click() {
		    window.location.href += '&Export=1';
		}

		function imgSort_click() {
			try {
				var sortableColumns = '<%=this.SortableColumns%>';
				while (sortableColumns.indexOf('<BR />') > -1) {
					sortableColumns = sortableColumns.replace("<BR />", ' ');
				}
				while (sortableColumns.indexOf('<BR/>') > -1) {
					sortableColumns = sortableColumns.replace("<BR/>", ' ');
				}
				while (sortableColumns.indexOf('<br />') > -1) {
					sortableColumns = sortableColumns.replace("<br />", ' ');
				}
				while (sortableColumns.indexOf('<br/>') > -1) {
					sortableColumns = sortableColumns.replace("<br/>", ' ');
				}

				while (sortableColumns.indexOf('...') > -1) {
					sortableColumns = sortableColumns.replace('...', '');
				}

				while (sortableColumns.indexOf('<BR>') > -1) {
					sortableColumns = sortableColumns.replace('<BR>', ' ');
				}
				while (sortableColumns.indexOf('<br>') > -1) {
					sortableColumns = sortableColumns.replace('<br>', ' ');
				}

				var sURL = 'SortOptions.aspx?sortColumns=' + escape(sortableColumns) + '&sortOrder=' + '<%=Request.QueryString["sortOrder"]%>';
				var nPopup = popupManager.AddPopupWindow("Sorter", "Sort Grid", sURL, 200, 400, "PopupWindow", this.self);
				if (nPopup) {
					nPopup.Open();
				}
			}
			catch (e) {
			}
		}

		function applySort(sortValue) {
			try {
				var pURL = window.location.href;
				pURL = editQueryStringValue(pURL, 'sortOrder', sortValue);
				pURL = editQueryStringValue(pURL, 'sortChanged', 'true');

				window.location.href = 'Loading.aspx?Page=' + pURL;
			}
			catch (e) {
			}
        }

        function imgHelp_click() {
            var s = '';

            s += '--Closed Tasks--<br />';
            s += '&nbsp;&nbsp;&nbsp;Total Closed: Tasks with the final status of closed within a session.<br />';
            s += '&nbsp;&nbsp;&nbsp;Carry-In: Tasks with the final status of closed within a session which had a final status of open in the previous session.<br />';
            s += '&nbsp;&nbsp;&nbsp;New: Tasks with the final status of closed within a session which were created in that session.<br />';
            s += '&nbsp;&nbsp;&nbsp;Carry-Out: Tasks with the final status of open within a session which were closed at some point in that session.<br />';
            s += '<br />--Open Tasks--<br />';
            s += '&nbsp;&nbsp;&nbsp;Total Open: Tasks with the final status of open within a session.<br />';
            s += '&nbsp;&nbsp;&nbsp;Carry-In: Tasks with the final status of open within a session which had a final status of open in the previous session.<br />';
            s += '&nbsp;&nbsp;&nbsp;New: Tasks with the final status of open within a session which were created in that session.<br />';
            s += '&nbsp;&nbsp;&nbsp;Carry-Out: Tasks with the final status of open within a session.<br />';
            s += '<br />--Resources--<br />';
            s += '&nbsp;&nbsp;&nbsp;All Dev and Biz resources set as Assigned or Primary at some point within a session.<br /><br />';

            MessageBox(s);
        }

        function resizeGrid() {
            setTimeout(function () { <%=this.grdMD.ClientID %>.ResizeGrid(); }, 1);
        }

	</script>

	<script id="jsInit" type="text/javascript">

        $(document).ready(function () {
			$('#imgReport').hide();
			$('#imgExport').click(function () { imgExport_click(); });
			$('#imgRefresh').click(function () { refreshPage(); });
            $('#imgSort').click(function () { imgSort_click(); });
            $('#imgHelp').click(function () { imgHelp_click(); });

            resizeFrame();
		});
	</script>

</asp:Content>
