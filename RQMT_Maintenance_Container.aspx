﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="RQMT_Maintenance_Container.aspx.cs" Inherits="RQMT_Maintenance_Container" Theme="Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <!-- Copyright (c) 2017 Infinite Technologies, Inc. -->
	<title>RQMT Maintenance</title>
	<script type="text/javascript" src="Scripts/shell.js"></script>
	<script type="text/javascript" src="Scripts/common.js"></script>
	<script type="text/javascript" src="Scripts/jquery-1.11.2.js"></script>
    <script type="text/javascript" src="Scripts/underscore-min.js"></script>
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
                    case 'RQMT':
                        nURL = _pageUrls.Maintenance.RQMTGrid;
                        break;
                    case 'RQMTDESCRIPTION':
                        nURL = _pageUrls.Maintenance.RQMTDescriptionGrid;
                        break;
				    default:
                        nURL = _pageUrls.Maintenance.RQMTGrid;
				        break;
				}

                nURL = 'Loading.aspx?Page=' + nURL + window.location.search + '&GridPageIndex=<%=_lastGridPageIndex%>';

				$('#frameGrid').attr('src', nURL);
			}

			$('#frameGrid').show();

			resizePage();
		}

		function showFrameForEdit(type, blnNewItem, itemID, blnRefresh, bldParams) {
			$('#frameEdit').hide();
            $('#frameGrid').hide();

            /* FROM CALLING PAGE:
            var bldParams = '';
            bldParams += 'WTS_SYSTEM_SUITEID=' + _SystemSuiteID + '|';
            bldParams += 'WTS_SYSTEMID=' + sysid + '|';
            bldParams += 'WORKAREA_ID=' + waid + '|';
            bldParams += 'WORKLOADGROUP_ID=' + wgid + '|';
            bldParams += 'RQMTTYPE_ID=' + rtid + '|';
            bldParams += 'RSET_ID=' + rsetid + '|';
            bldParams += 'RQMT_ID=' + rqmtid;
            */
            
            var pArr = bldParams != null ? bldParams.split('|') : null;
            var suiteid = _.find(pArr, function (p) { return p.indexOf('WTS_SYSTEM_SUITEID') == 0 }) != null ? _.find(pArr, function (p) { return p.indexOf('WTS_SYSTEM_SUITEID') == 0 }).split('=')[1] : 0;
            var sysid = _.find(pArr, function (p) { return p.indexOf('WTS_SYSTEMID') == 0 }) != null ? _.find(pArr, function (p) { return p.indexOf('WTS_SYSTEMID') == 0 }).split('=')[1] : 0;
            var waid = _.find(pArr, function (p) { return p.indexOf('WORKAREA_ID') == 0 }) != null ? _.find(pArr, function (p) { return p.indexOf('WORKAREA_ID') == 0 }).split('=')[1] : 0;
            var wgid = _.find(pArr, function (p) { return p.indexOf('WORKLOADGROUP_ID') == 0 }) != null ? _.find(pArr, function (p) { return p.indexOf('WORKLOADGROUP_ID') == 0 }).split('=')[1] : 0;
            var rtid = _.find(pArr, function (p) { return p.indexOf('RQMTTYPE_ID') == 0 }) != null ? _.find(pArr, function (p) { return p.indexOf('RQMTTYPE_ID') == 0 }).split('=')[1] : 0;
            var rsetid = _.find(pArr, function (p) { return p.indexOf('RSET_ID') == 0 }) != null ? _.find(pArr, function (p) { return p.indexOf('RSET_ID') == 0 }).split('=')[1] : 0;
            var rqmtid = _.find(pArr, function (p) { return p.indexOf('RQMT_ID') == 0 }) != null ? _.find(pArr, function (p) { return p.indexOf('RQMT_ID') == 0 }).split('=')[1] : 0;

			if (blnRefresh) {
			    $('#frameEdit').attr('src', "javascript:'';");
                
			    var nURL = '';
			    
                switch (type.toUpperCase()) {
                    case 'RQMTBUILDER':
                        if (pArr != null) {
                            nURL = _pageUrls.Maintenance.RQMTBuilder + window.location.search + '&NewRQMT=' + blnNewItem + '&RQMTSetRQMTSystemID=' + itemID + '&Type=' + (blnNewItem ? 'Add' : 'Edit') +
                                '&SystemSuiteID=' + suiteid +
                                '&WTS_SYSTEMID=' + sysid +
                                '&WorkAreaID=' + waid +
                                '&WorkloadGroupID=' + wgid +
                                '&RQMTTypeID=' + rtid +
                                '&RQMTSetID=' + rsetid +
                                '&RQMTID=' + rqmtid;
                        }
                        else {
                            nURL = _pageUrls.Maintenance.RQMTBuilder + window.location.search + '&NewRQMT=' + blnNewItem + '&RQMTSetRQMTSystemID=' + itemID + '&Type=' + (blnNewItem ? 'Add' : 'Edit');
                        }
                        break;
                    case 'RQMT':
                        nURL = _pageUrls.Maintenance.RQMTTabs + window.location.search + '&NewRQMT=' + blnNewItem + '&RQMTID=' + itemID;
                        break;
                    case 'RQMTDESCRIPTION':
                        return;
			        default:
                        nURL = _pageUrls.Maintenance.RQMTTabs + window.location.search + '&NewRQMT=' + blnNewItem + '&RQMTID=' + itemID;
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

        function refreshGrid() {            
			var nURL = 'Loading.aspx?Page=' + _pageUrls.Maintenance.RQMTGrid + window.location.search;
			$('#frameGrid').attr('src', nURL);
        }

        function refreshPage() {
            window.location.href = window.location.href;
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
