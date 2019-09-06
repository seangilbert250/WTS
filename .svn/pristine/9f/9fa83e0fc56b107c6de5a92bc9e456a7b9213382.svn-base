<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AOR_Maintenance_Container.aspx.cs" Inherits="AOR_Maintenance_Container" Theme="Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <!-- Copyright (c) 2017 Infinite Technologies, Inc. -->
	<title>AOR Maintenance</title>
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
        var _CRMetrics = false;
        var MenuType = '';
    </script>
    
	<script id="jsEvents" type="text/javascript">
		function showFrame(type, blnNewItem, itemID, gridType, blnRefresh) {
			$('#frameEdit').hide();
			$('#frameGrid').hide();

			switch (type.toUpperCase()) {
				case 'GRID':
				    showFrameForGrid(gridType, blnRefresh);
					break;
				case 'ADD':
				case 'EDIT':
				    showFrameForEdit(gridType, blnNewItem, itemID, true);
					break;
			}
		}

		function showFrameForGrid(gridType, blnRefresh) {
		    $("#frameEdit").attr("src", "javascript:'';");
		    $('#frameEdit').hide();
			$('#frameGrid').hide();

			if (_newItemCreated) {
			    _newItemCreated = false;
			    blnRefresh = true;
			}

			if (blnRefresh && gridType != null && gridType.toUpperCase) { // some pages are passing in gridType incorrectly, so this is a stopgap to prevent an error
				$('#frameGrid').attr('src', "javascript:'';");

				var nURL = '';

				switch (gridType.toUpperCase()) {
				    case 'AOR':
				        nURL = _pageUrls.Maintenance.AORGrid;
				        break;
				    case 'AORMEETING':
				        nURL = _pageUrls.Maintenance.AORMeetingGrid;
				        break;
				    case 'CR':
				        nURL = _pageUrls.Maintenance.AORCRGrid;
                        break;
                    case 'DEPLOYMENTS':
                        nURL = _pageUrls.Maintenance.AORScheduledDeliverables;
                        break;
                    case 'RELEASEASSESSMENT':
                        nURL = _pageUrls.Maintenance.AORReleaseAssessment;
                        break;
				    default:
				        nURL = _pageUrls.Maintenance.AORGrid;
				        break;
				}

				nURL = 'Loading.aspx?Page=' + nURL + window.location.search;
                MenuType = '<%=this.MenuType %>';
                if (MenuType.toUpperCase() == 'AOR') nURL = nURL + '&View=Meeting';

				$('#frameGrid').attr('src', nURL);
			}

			$('#frameGrid').show();

			resizePage();
		}

		function showFrameForEdit(type, blnNewItem, itemID, blnRefresh, item2ID) {
			$('#frameEdit').hide();
			$('#frameGrid').hide();

			if (blnRefresh) {
			    $('#frameEdit').attr('src', "javascript:'';");

			    var nURL = '';
			    
			    switch (type.toUpperCase()) {
			        case 'AOR':
			            nURL = _pageUrls.Maintenance.AORTabs + window.location.search + '&NewAOR=' + blnNewItem + '&AORID=' + itemID + '&AORReleaseID=' + item2ID;
			            break;
			        case 'AORMEETING':
			            nURL = _pageUrls.Maintenance.AORMeetingTabs + window.location.search + '&NewAORMeeting=' + blnNewItem + '&AORMeetingID=' + itemID;
                        break;
                    case 'AORMEETINGINSTANCE':
                        nURL = _pageUrls.Maintenance.AORMeetingInstanceEdit + window.location.search + '&AORMeetingID=' + itemID + '&NewAORMeetingInstance=' + blnNewItem + '&AORMeetingInstanceID=' + item2ID;
                        break;
			        case 'CR':
			            nURL = _pageUrls.Maintenance.AORCRTabs + window.location.search + '&NewCR=' + blnNewItem + '&CRID=' + itemID;
                        break;
                    case 'SCHEDULEDDELIVERABLES':
                        nURL = _pageUrls.Maintenance.AORCRTabs + window.location.search + '&NewCR=' + blnNewItem + '&CRID=' + itemID;
                        break;
			        case 'TASK':
			            nURL = _pageUrls.Maintenance.WorkItemEditParent + window.location.search + '&newItem=' + blnNewItem + '&WorkItemID=' + itemID;
                        break;
			        default:
			            nURL = _pageUrls.Maintenance.AORTabs + window.location.search + '&NewAOR=' + blnNewItem + '&AORID=' + itemID + '&AORReleaseID=' + item2ID;
			            break;
			    }

			    nURL = 'Loading.aspx?Page=' + nURL;

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
            showFrame('Grid', false, 0, '<%=this.GridType %>', true);
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
