<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AOR_Meeting_Tabs.aspx.cs" Inherits="AOR_Meeting_Tabs" Theme="Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <!-- Copyright (c) 2017 Infinite Technologies, Inc. -->
	<title>AOR Meeting Tabs</title>
	<script type="text/javascript" src="Scripts/shell.js"></script>
	<script type="text/javascript" src="Scripts/common.js"></script>
	<script type="text/javascript" src="Scripts/jquery-1.11.2.js"></script>
    <script type="text/javascript" src="Scripts/jquery-ui.js"></script>
    <link rel="stylesheet" href="Styles/jquery-ui.css" />
</head>
<body>
	<form id="form1" runat="server">
        <asp:ScriptManager ID="scriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>
        <div class="pageContentHeader">
            <table style="width: 100%;">
                <tr>
                    <td style="padding-left: 3px;">
                        <asp:Label ID="lblAORMeeting" runat="server"></asp:Label>
                    </td>
                    <td style="text-align: right; padding: 3px;">
			            <iti_Tools_Sharp:Menu ID="menuRelatedItems" runat="server" ClientClickEvent="openMenuItem" Text="Related&nbsp;Items&nbsp;<img id=imgRelatedItemsMenu alt=Expand Menu title=Show Related Items Options src=Images/menuDown_Black.gif />" Button="true" Style="vertical-align: middle; display: inline-block;"></iti_Tools_Sharp:Menu>
                        <input type="button" id="btnBackToGrid" value="Back To Meeting Grid" style="vertical-align: middle; display: none;" />
                    </td>
                </tr>
            </table>
        </div>
        <div id="divTabsContainer" class="mainPageContainer">
			<ul>
				<li><a href="#divDetails">Details</a></li>
                <li style="display: none;"><a href="#divInstances">Meeting Instances (<%=this.MeetingInstanceCount %>)</a></li>
                <li id="liMetricsTab" style="display:none;"><a href="#divMetrics">Metrics</a></li>
			</ul>
            <div id="divDetails">
                <iframe id="frameDetails" src="javascript:'';" frameborder="0" scrolling="no" style="width: 100%;"></iframe>
            </div>
            <div id="divInstances">
                <iframe id="frameInstances" src="javascript:'';" frameborder="0" scrolling="no" style="width: 100%;"></iframe>
            </div>
            <div id="divMetrics" class="tabDiv" style="display:none;">                   
                <div class="pageContentHeader">Meeting #<%=AORMeetingID%>: Meeting Metrics</div>
                <div class="simplemessage hcenter vcenter info" id="divmetricsloading">Loading statistics...</div>
                <div class="simplemessage hcenter vcenter danger" id="divmetricsfailed" style="display:none">Metrics could not be loaded.</div>
                <div id="divmetricscontainer" style="padding:5px;opacity:.5">
                    <table style="border-collapse: collapse; width: 400px;">
                        <thead>
                            <tr class="gridHeader gridFullBorder">
                                <th style="text-align:left;width:300px;">Meeting Metrics</th>
                                <th style="text-align:right;width:100px;">Values</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr class="gridBody gridFullBorder">
                                <td style="text-align:left">Total Meetings</td>
                                <td style="text-align:right;white-space:nowrap;"><div id="divMetricsTotalMeetings">0</div></td>
                            </tr>
                            <tr class="gridBody gridFullBorder">
                                <td style="text-align:left">Average Length (minutes)</td>
                                <td style="text-align:right;white-space:nowrap;"><div id="divMetricsAvgLength">0</div></td>
                            </tr>
                            <tr class="gridBody gridFullBorder">
                                <td style="text-align:left">Average Attendees</td>
                                <td style="text-align:right;white-space:nowrap;"><div id="divMetricsAvgAttendedCount">0</div></td>
                            </tr>
                            <tr class="gridBody gridFullBorder">
                                <td style="text-align:left">Average Resources</td>
                                <td style="text-align:right;white-space:nowrap;"><div id="divMetricsAvgResourcesCount">0</div></td>
                            </tr>
                            <tr class="gridBody gridFullBorder">
                                <td style="text-align:left">% Attended</td>
                                <td style="text-align:right;white-space:nowrap;"><div id="divMetricsAvgAttendedPct">0 %</div></td>
                            </tr>
                            <tr class="gridBody gridFullBorder">
                                <td style="text-align:left">% Highest Attended</td>
                                <td style="text-align:right;white-space:nowrap;"><div id="divMetricsMaxAttendedPct">0 %</div></td>
                            </tr>
                        </tbody>
                    </table> 
                    <br>
                    <table style="border-collapse: collapse; width: 400px;">
                        <thead>
                            <tr class="gridHeader gridFullBorder">
                                <th style="text-align:left;width:300px;">Agenda Metrics</th>
                                <th style="text-align:right;width:100px;">Counts</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr class="gridBody gridFullBorder">
                                <td style="text-align:left">Agenda/Objectives</td>
                                <td style="text-align:right;white-space:nowrap;"><div id="divMetricsTotal_<%=(int)WTS.Enums.NoteTypeEnum.AgendaObjectives%>">0</div></td>
                            </tr>
                            <tr class="gridBody gridFullBorder">
                                <td style="text-align:left">Action Items</td>
                                <td style="text-align:right;white-space:nowrap;"><div id="divMetricsTotal_<%=(int)WTS.Enums.NoteTypeEnum.ActionItems%>">0</div></td>
                            </tr>
                            <tr class="gridBody gridFullBorder">
                                <td style="text-align:left">Burndown Overview</td>
                                <td style="text-align:right;white-space:nowrap;"><div id="divMetricsTotal_<%=(int)WTS.Enums.NoteTypeEnum.BurndownOverview%>">0</div></td>
                            </tr>
                            <tr class="gridBody gridFullBorder">
                                <td style="text-align:left">Notes</td>
                                <td style="text-align:right;white-space:nowrap;"><div id="divMetricsTotal_<%=(int)WTS.Enums.NoteTypeEnum.Notes%>">0</div></td>
                            </tr>
                            <tr class="gridBody gridFullBorder">
                                <td style="text-align:left">Questions/Discussion Points</td>
                                <td style="text-align:right;white-space:nowrap;"><div id="divMetricsTotal_<%=(int)WTS.Enums.NoteTypeEnum.QuestionsDiscussionPoints%>">0</div></td>
                            </tr>
                            <tr class="gridBody gridFullBorder">
                                <td style="text-align:left">Stopping Conditions</td>
                                <td style="text-align:right;white-space:nowrap;"><div id="divMetricsTotal_<%=(int)WTS.Enums.NoteTypeEnum.StoppingConditions%>">0</div></td>
                            </tr>
                        </tbody>
                    </table> 
                    <br>
                    <table style="border-collapse: collapse; width: 400px;">
                        <thead>
                            <tr class="gridHeader gridFullBorder">
                                <th style="text-align:left;width:200px;">Action Item Metrics</th>
                                <th style="text-align:right;width:50px;">New</th>
                                <th style="text-align:right;width:50px;">All</th>
                                <th style="text-align:right;width:50px;">Open</th>
                                <th style="text-align:right;width:50px;">Closed</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr class="gridBody gridFullBorder">
                                <td style="text-align:left"><div id="divlastmeetingtitle">Last Meeting</div></td>
                                <td style="text-align:right"><div id="divlastmeetingnew">0</div></td>
                                <td style="text-align:right"><div id="divlastmeetingall">0</div></td>
                                <td style="text-align:right"><div id="divlastmeetingopen">0</div></td>
                                <td style="text-align:right"><div id="divlastmeetingclosed">0</div></td>
                            </tr>
                            <tr class="gridBody gridFullBorder">
                                <td style="text-align:left"><div id="divsecondtolastmeetingtitle">Previous Last Meeting</div></td>
                                <td style="text-align:right"><div id="divsecondtolastmeetingnew">0</div></td>
                                <td style="text-align:right"><div id="divsecondtolastmeetingall">0</div></td>
                                <td style="text-align:right"><div id="divsecondtolastmeetingopen">0</div></td>
                                <td style="text-align:right"><div id="divsecondtolastmeetingclosed">0</div></td>
                            </tr>
                        </tbody>
                    </table>
                    <br><br>
                    <b>NOTES:</b><br>
                    <ol>
                        <li>Total Meetings includes all meetings of the same parent meeting type</li>
                        <li>Average Length only includes meetings with a length value entered</li>
                        <li>Attendance / Resources totals and averages only count meetings with 2 or more attendees</li>
                        <li>Agenda Metrics only includes notes, action items, and agenda items created on or after 2/12/2018</li>
                    </ol>
                </div>                
            </div>
		</div>
	</form>

    <script type="text/javascript" src="Scripts/jquery-ui.js"></script>
    <script id="jsVariables" type="text/javascript">
        var _pageUrls;
        var _meetingInstanceCount = 0;
    </script>
    
	<script id="jsEvents" type="text/javascript">
	    function btnBackToGrid_click() {
	        if (parent.showFrameForGrid) parent.showFrameForGrid('AORMeeting', false);
        }

        function relatedItems_click(action, type) {
            switch (action) {
                case 'ReleaseAssessment':
                    btnReleaseAssessment_click();
                    break;
            }
        }

        function openMenuItem(url) {
            if (url.indexOf("relatedItems") > 0) {
                var str = url.split("'");
                relatedItems_click(str[1], str[3]);
            }
        }

        function btnReleaseAssessment_click() {
            var nWindow = 'ReleaseAssessmentGrid';
            var nTitle = 'Release Assessment Grid';
            var nHeight = 700, nWidth = 1200;
            var nURL = _pageUrls.Maintenance.AORReleaseAssessment + '?GridType=ReleaseAssessment';
            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

        function tab_click(tabName) {
			switch (tabName.toUpperCase()) {
				case 'DETAILS':
				    if ($('#frameDetails').attr('src') == "javascript:'';") $('#frameDetails').attr('src', 'Loading.aspx?Page=' + _pageUrls.Maintenance.AORMeetingEdit + window.location.search);
				    break;
			    case 'MEETING INSTANCES (' + _meetingInstanceCount + ')':
			        if ($('#frameInstances').attr('src') == "javascript:'';") $('#frameInstances').attr('src', 'Loading.aspx?Page=' + _pageUrls.Maintenance.AORMeetingInstanceContainer + window.location.search);
                    break;
			}

			resizePage();
        }

        function refreshPage(newID) {
            var nURL = window.location.href;

            if (newID != undefined && parseInt(newID) > 0) {
                nURL = editQueryStringValue(nURL, 'NewAORMeeting', 'false');
                nURL = editQueryStringValue(nURL, 'AORMeetingID', newID);
            }

            window.location.href = 'Loading.aspx?Page=' + nURL;
        }

		function resizePage() {
			$('#divTabsContainer div iframe').each(function () {
			    resizePageElement($(this).attr('id'), 0);
            });

            $('#divTabsContainer div').each(function () {                
                if ($(this).attr('id') == 'divMetrics' || $(this).closest('#divMetrics').length == 0) {
                    resizePageElement($(this).attr('id'), -1);
                }
			});
		}

		function updateTab(tabName, newCount) {
		    switch (tabName.toUpperCase()) {
		        case 'MEETING INSTANCES':
		            $('[href="#divInstances"]').text('Meeting Instances (' + newCount + ')');
		            _meetingInstanceCount = newCount;
		            break;
		    }
        }

        function getMeetingMetrics() {
            PageMethods.GetMeetingMetrics(<%=AORMeetingID%>, getMeetingMetrics_done, function () { $('#divmetricsfailed').show(); });
        }

        function getMeetingMetrics_done(result) {
            var dt = jQuery.parseJSON(result);
            
            $('#divmetricsloading').hide();

            if (dt.success == "true") {
                $('#divMetricsTotalMeetings').html(dt.totalmeetings);
                $('#divMetricsAvgLength').html(dt.avglength);
                $('#divMetricsAvgAttendedCount').html(dt.avgattendedcount);
                $('#divMetricsAvgResourcesCount').html(dt.avgresourcescount);
                $('#divMetricsAvgAttendedPct').html(dt.avgattendedpct + ' %');
                $('#divMetricsMaxAttendedPct').html(dt.maxattendedpct + ' %');

                for (var i = 10; i <= 17; i++) {
                    var noteData = dt['notetype_' + i];

                    if (noteData != null) {
                        var noteDataTokens = noteData.split('_');

                        $('#divMetricsTotal_' + i).html(noteDataTokens[0]);
                    }
                }
                
                $('#divlastmeetingtitle').html('Last Meeting (' + (dt.lastmeetingid > 0 ? dt.lastmeetingid : 'N/A') + ')');
                $('#divlastmeetingnew').html(dt.lastmeetingid > 0 ? dt.newitemslastmeeting : 'N/A');
                $('#divlastmeetingall').html(dt.lastmeetingid > 0 ? dt.lastallnotes : 'N/A');
                $('#divlastmeetingopen').html(dt.lastmeetingid > 0 ? dt.lastopennotes : 'N/A');
                $('#divlastmeetingclosed').html(dt.lastmeetingid > 0 ? dt.lastclosednotes : 'N/A');

                $('#divsecondtolastmeetingtitle').html('Previous Last Meeting (' + (dt.secondtolastmeetingid > 0 ? dt.secondtolastmeetingid : 'N/A') + ')');
                $('#divsecondtolastmeetingnew').html(dt.secondtolastmeetingid > 0 ? dt.newitemssecondtolastmeeting : 'N/A');
                $('#divsecondtolastmeetingall').html(dt.secondtolastmeetingid > 0 ? dt.secondtolastallnotes : 'N/A');
                $('#divsecondtolastmeetingopen').html(dt.secondtolastmeetingid > 0 ? dt.secondtolastopennotes : 'N/A');
                $('#divsecondtolastmeetingclosed').html(dt.secondtolastmeetingid > 0 ? dt.secondtolastclosednotes : 'N/A');

                $('#divmetricscontainer').css('opacity', '1.0');
            }
            else {
                $('#divmetricsfailed').show();
            }
        }
	</script>

    <script id="jsInit" type="text/javascript">
        function initVariables() {
            _pageUrls = new PageURLs();
            _meetingInstanceCount = <%=this.MeetingInstanceCount %>;
        }

        function initControls() {
            if ('<%=this.NewAORMeeting %>'.toUpperCase() == 'FALSE') $('[href="#divInstances"]').closest('li').show();

            $('#divTabsContainer').tabs({
                heightStyle: "fill",
				collapsible: false,
				active: 0
            });
        }

        function initDisplay() {
            if (parent.showFrameForGrid) $('#btnBackToGrid').show();

            tab_click('Details');
            resizePage();

            if (<%=AORMeetingID%> != 0) {
                getMeetingMetrics();
                $('#liMetricsTab').show();
            }
        }

        function initEvents() {
            $('#btnBackToGrid').click(function () { btnBackToGrid_click(); return false; });
            $('#divTabsContainer ul li a').click(function () { tab_click($(this).text()); });
            $(window).resize(resizePage);
        }

        $(document).ready(function () {
            initVariables();
            initControls();
            initDisplay();
            initEvents();
        });
    </script>
</body>
</html>
