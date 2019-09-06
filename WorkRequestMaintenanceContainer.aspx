﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="WorkRequestMaintenanceContainer.aspx.cs" Inherits="WorkRequestMaintenanceContainer" Theme="Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>Work Request Maintenance</title>
	<script type="text/javascript" src="scripts/shell.js"></script>
	<script type="text/javascript" src="scripts/common.js"></script>
	<script type="text/javascript" src="Scripts/jquery-1.11.2.js"></script>
</head>
<body>
	<form id="form1" runat="server">
		<div id="divFrameContainer">
			<iframe id="frameGrid" src="javascript:'';" frameborder="0" scrolling="no" style="display: none; width: 100%;"></iframe>
			<iframe id="frameEdit" src="javascript:'';" frameborder="0" scrolling="no" style="display: none; width: 100%;"></iframe>
			<iframe id="frameEditChild" src="javascript:'';" frameborder="0" scrolling="no" style="display: none; width: 100%;"></iframe>
		</div>
	</form>

	<script type="text/javascript">
		var _newItemCreated = false;
		var _gridType = 'Request';

		var _pageUrls = new PageURLs();

		function ShowFrame(type, newItem, itemId, sourceItemId, gridType, refresh) {
			$('#frameEditChild').hide();
			$('#frameEdit').hide();
			$('#frameGrid').hide();

			if (!gridType || typeof gridType == "undefined" || gridType == '') {
				gridType = "Request";
			}
			if (refresh == undefined || refresh == null) {
				refresh = true;
			}

			switch (type.toUpperCase()) {
				case 'GRID':
					ShowFrameForGrid(gridType, refresh);
					break;
				case 'ADD':
				case 'EDIT':
				case 'COPY':
					ShowFrameForRequest(newItem, itemId, sourceItemId, true);
					break;
			}
		}

		function ShowFrameForGrid(gridType, refresh) {
			$("#frameEdit").attr("src", "javascript:'';");
			$("#frameEditChild").attr("src", "javascript:'';");
			$('#frameGrid').hide();

			if (refresh == undefined || refresh == null
				|| $('#frameGrid').attr('src') == ''
				|| $('#frameGrid').attr('src') == "javascript:'';") {
				refresh = true;
			}

			if (_newItemCreated == true || _newItemCreated == 1) {
				_newItemCreated = false;
				MessageBox('Please click Get Data.');
			}

			if (refresh) {
				$('#frameGrid').attr('src', "javascript:'';");

				var pageUrl = gridType.toUpperCase().indexOf("HOTLIST") > -1 ? _pageUrls.Maintenance.HotlistGrid : _pageUrls.Maintenance.WorkRequestGrid;
				var search = (window.location.search == '') ? '?' : window.location.search;
				var url = 'Loading.aspx?Page=' + pageUrl
					+ search
					+ '&random=' + new Date().getTime();

				$('#frameGrid').attr('src', url);
			}

			$('#frameGrid').show();

			resizePage();
		}

		function ShowFrameForWorkloadItem(newItem, itemId, sourceItemId, refresh) {
			$('#frameEditChild').hide();
			$('#frameEdit').hide();
			$('#frameGrid').hide();

			$("#frameEdit").attr("src", "javascript:'';");
			$("#frameEditChild").attr("src", "javascript:'';");

			if (refresh == undefined || refresh == null || refresh == '') {
				refresh = true;
			}

			if (refresh) {
				$('#frameEdit').attr('src', '');
				var search = (window.location.search == '') ? '?' : window.location.search;
				var url = 'Loading.aspx?Page=' + _pageUrls.Maintenance.WorkItemEditParent
					+ search
					+ '&newItem=' + newItem
					+ '&WorkItemID=' + itemId
					+ '&sourceItemId=' + sourceItemId
				;

				$('#frameEdit').attr('src', url);
			}

			$('#frameEdit').show();

			resizePage();
		}

		function ShowFrameForWorkRequest(newItem, itemId, sourceItemId, refresh) {
			$('#frameEditChild').hide();
			$('#frameEdit').hide();
			$('#frameGrid').hide();

			$("#frameEdit").attr("src", "javascript:'';");
			$("#frameEditChild").attr("src", "javascript:'';");

			if (refresh == undefined || refresh == null || refresh == '') {
				refresh = true;
			}

			if (refresh) {
				$('#frameEdit').attr("src", "javascript:'';");
				var search = (window.location.search == '') ? '?' : window.location.search;
				var url = 'Loading.aspx?Page=' + _pageUrls.Maintenance.WorkRequestEditParent
					+ search
					+ '&newRequest=' + newItem
					+ '&WorkRequestID=' + itemId
					+ '&sourceWorkRequestID=' + sourceItemId
				;

				$('#frameEdit').attr('src', url);
			}

			$('#frameEdit').show();

			resizePage();
		}

		function ShowFrameForChild(recordType, recordId, recordName) {
			$('#frameEditChild').hide();
			$('#frameEdit').hide();
			$('#frameGrid').hide();

			var url = 'Loading.aspx?Page=';

			$("#frameEditChild").attr("src", "javascript:'';");

            url += _pageUrls.Maintenance.WorkItemEditParent
                + '?itemID=' + recordId;

			$('#frameEditChild').attr('src', url);
			$('#frameEditChild').show();

			resizePage();
		}

		function resizePage() {
			$('#divFrameContainer iframe').each(function () {
				resizePageElement($(this).attr('id'), 0);
			});
		}

		$(document).ready(function () {
			$(window).resize(resizePage);

			_gridType = '<%=this.GridType %>';

			ShowFrame('GRID', false, 0, 0, _gridType, true);

			resizePage();
		});
	</script>
</body>
</html>