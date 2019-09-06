<%@ Page Language="C#" AutoEventWireup="true" CodeFile="WorkItemGrid_Tasks.aspx.cs" Inherits="WorkItemGrid_Tasks" Theme="Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Work Item Tasks</title>
	<script type="text/javascript" src="Scripts/shell.js"></script>
	<script type="text/javascript" src="Scripts/common.js"></script>
	<script type="text/javascript" src="Scripts/jquery-1.11.2.js"></script>
</head>
<body>
    <form id="form1" runat="server">
		<div id="divContents">
			<asp:GridView runat="server" ID="gridTask" Style="width: 100%; border: none;" GridLines="None" AllowPaging="false" AllowSorting="false" PageSize="10" CellPadding="0" CellSpacing="0"
				CssClass="grid" RowStyle-CssClass="gridBody" HeaderStyle-CssClass="gridHeader" SelectedRowStyle-CssClass="gridBody" FooterStyle-CssClass="gridPager" PagerStyle-CssClass="gridPager">
			</asp:GridView>
		</div>

		<script type="text/javascript">
			var _pageUrls;
			var _workItemId = 0;

			function refreshPage() {
				document.location.href = 'Loading.aspx?Page=' + document.location.href;
			}

			function resizeFrame() {
				if (parent.resizeFrames) {
					parent.resizeFrames();
				}
			}
		</script>

		<script id="jsEvents" type="text/javascript">

			function lbEditTask_click(recordId) {
				var title = '', url = '';
				var h = 700, w = 850;

				title = 'Task - [' + recordId + ']';
				url = _pageUrls.Maintenance.TaskEdit
					+ '?TaskID=' + recordId;

				//open in a popup
				var openPopup = popupManager.AddPopupWindow('Task', title, 'Loading.aspx?Page=' + url, h, w, 'PopupWindow', this);
				if (openPopup) {
					openPopup.Open();
				}
			}

		</script>

	</form>

	<script id="jsInit" type="text/javascript">

		$(document).ready(function () {
			_pageUrls = new PageURLs();

			$(window).resize(resizeFrame);

			resizeFrame();
		});
	</script>
</body>
</html>
