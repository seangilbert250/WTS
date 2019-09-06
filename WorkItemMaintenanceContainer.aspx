<%@ Page Language="C#" AutoEventWireup="true" CodeFile="WorkItemMaintenanceContainer.aspx.cs" Inherits="WorkItemMaintenanceContainer" Theme="Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>Workload Maintenance</title>
	<script type="text/javascript" src="scripts/shell.js"></script>
	<script type="text/javascript" src="scripts/common.js"></script>
	<script type="text/javascript" src="Scripts/jquery-1.11.2.js"></script>
</head>
<body>
    <form id="form1" runat="server">
		<div id="divFrameContainer">
			<iframe id="frameGrid" src="javascript:'';" frameborder="0" scrolling="no" style="display: none; width: 100%;"></iframe>
			<iframe id="frameEdit" src="javascript:'';" frameborder="0" scrolling="no" style="display: none; width: 100%;"></iframe>
		</div>
    </form>

	<script type="text/javascript">
	    var _newItemCreated = false;
	    var workItemsList = [];
	    var workItemsListCurrentIndex = undefined;

		var _pageUrls = new PageURLs();

		function ShowFrame(type, newItem, itemId, sourceItemId) {
			$('#frameEdit').hide();
			$('#frameGrid').hide();

			switch (type.toUpperCase()) {
				case 'GRID':
					$("#frameEdit").attr("src", "javascript:'';");
					$('#frameGrid').show();
					$("#frameGrid").attr("src", $('#frameGrid').attr("src"));
					if (_newItemCreated == true || _newItemCreated == 1) {
						_newItemCreated = false;
						MessageBox('Please click Get Data.');
					}

					resizePage();
					break;
				case 'ADD':
				case 'EDIT':
				case 'COPY':
					ShowFrameForWorkloadItem(newItem, itemId, sourceItemId, true);
					break;
			}
		}

		function ShowFrameForWorkRequest(newItem, itemId, sourceItemId, refresh) {
			$('#frameEdit').hide();
			$('#frameGrid').hide();

			$("#frameEdit").attr("src", "javascript:'';");

			if (refresh == undefined || refresh == null) {
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

		function ShowFrameForWorkloadItem(newItem, itemId, sourceItemId, refresh, UseLocal, _selectedStatuses, _selectedAssigned) {
			$('#frameEdit').hide();
			$('#frameGrid').hide();

			$("#frameEdit").attr("src", "javascript:'';");

			if (refresh == undefined || refresh == null || refresh == '') {
				refresh = true;
			}

			if (UseLocal == undefined || UseLocal == null || UseLocal == '') {
			    UseLocal = false;
			}

			if (refresh) {
				$('#frameEdit').attr('src', '');
				var search = (window.location.search == '') ? '?' : window.location.search;
				var url = 'Loading.aspx?Page=' + _pageUrls.Maintenance.WorkItemEditParent
					+ search
					+ '&newItem=' + newItem
					+ '&WorkItemID=' + itemId
					+ '&sourceWorkItemID=' + sourceItemId

                    // 4-14-2017 - Added these
		            + '&SelectedStatuses=' + (_selectedStatuses == undefined ? '' : _selectedStatuses)
		            + '&SelectedAssigned=' + (_selectedAssigned == undefined ? '' : _selectedAssigned)


                    + '&UseLocal=' + UseLocal.toString();
				;

				$('#frameEdit').attr('src', url);
			}

			$('#frameEdit').show();

			resizePage();
		}

		function resizePage() {
			$('#divFrameContainer iframe').each(function () {
				resizePageElement($(this).attr('id'), 0);
			});
		}

		$(document).ready(function () {
			$(window).resize(resizePage);

			$('#frameGrid').attr('src', 'Loading.aspx?Page=' + _pageUrls.Maintenance.WorkItemGrid + window.location.search);
			$('#frameGrid').show();

			resizePage();
		});
	</script>
</body>
</html>
