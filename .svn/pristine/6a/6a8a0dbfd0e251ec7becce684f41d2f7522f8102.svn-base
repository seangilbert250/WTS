<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AOR_Meeting_Instance_Container.aspx.cs" Inherits="AOR_Meeting_Instance_Container" Theme="Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <!-- Copyright (c) 2017 Infinite Technologies, Inc. -->
	<title>AOR Meeting Instance</title>
	<script type="text/javascript" src="Scripts/shell.js"></script>
	<script type="text/javascript" src="Scripts/common.js"></script>
	<script type="text/javascript" src="Scripts/jquery-1.11.2.js"></script>
</head>
<body>
	<form id="form1" runat="server">
		<div id="divFrameContainer">
			<iframe id="frameGrid" src="javascript:'';" frameborder="0" scrolling="no" style="width: 100%; display: none;"></iframe>
			<iframe id="frameEdit" src="javascript:'';" frameborder="0" scrolling="no" style="width: 100%; display: none;"></iframe>
		</div>
	</form>

    <script id="jsVariables" type="text/javascript">
        var _pageUrls;
        var _newItemCreated = false;
        var _expandedAORs = [];
        var _expandedNotes = [];
        var _checkedShowClosedSRs = [];
        var _checkedShowClosedTasks = [];
        var _checkedShowRemoved = [];
        var _checkedShowClosed = [];
    </script>
    
	<script id="jsEvents" type="text/javascript">
		function showFrame(type, blnNewItem, itemID, blnRefresh) {
			$('#frameEdit').hide();
			$('#frameGrid').hide();

			switch (type.toUpperCase()) {
				case 'GRID':
				    showFrameForGrid(blnRefresh);
					break;
				case 'ADD':
				case 'EDIT':
				    showFrameForEdit(blnNewItem, itemID, true);
					break;
			}
		}

		function showFrameForGrid(blnRefresh) {
		    $("#frameEdit").attr("src", "javascript:'';");
		    $('#frameEdit').hide();
			$('#frameGrid').hide();

			if (_newItemCreated) {
			    _newItemCreated = false;
			    blnRefresh = true;
			}

			if (blnRefresh) {
				$('#frameGrid').attr('src', "javascript:'';");

				var nURL = 'Loading.aspx?Page=' + _pageUrls.Maintenance.AORMeetingInstanceGrid + window.location.search;

				$('#frameGrid').attr('src', nURL);
			}

			$('#frameGrid').show();

			resizePage();
		}

		function showFrameForEdit(blnNewItem, itemID, blnRefresh) {
			$('#frameEdit').hide();
			$('#frameGrid').hide();

			if (blnRefresh) {
			    $('#frameEdit').attr('src', "javascript:'';");

			    var nURL = 'Loading.aspx?Page=' + _pageUrls.Maintenance.AORMeetingInstanceEdit + window.location.search + '&NewAORMeetingInstance=' + blnNewItem + '&AORMeetingInstanceID=' + itemID;

				$('#frameEdit').attr('src', nURL);
			}

			$('#frameEdit').show();

			resizePage();
		}

		function resizePage() {
			$('#divFrameContainer iframe').each(function () {
				resizePageElement($(this).attr('id'), 0);
			});
		}
	</script>

    <script id="jsInit" type="text/javascript">
        function initVariables() {
            _pageUrls = new PageURLs();
        }

        function initDisplay() {
            showFrame('Grid', false, 0, true);
            resizePage();
        }

        function initEvents() {
            $(window).resize(resizePage);
        }

        $(document).ready(function () {
            initVariables();
            initDisplay();
            initEvents();
        });
    </script>
</body>
</html>
