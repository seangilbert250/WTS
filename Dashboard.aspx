<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Dashboard.aspx.cs" Inherits="Dashboard" Theme="Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Dashboard</title>

	<script type="text/javascript" src="Scripts/Chart.min.js"></script>
	<script type="text/javascript" src="Scripts/shell.js"></script>
    <script type="text/javascript" src="Scripts/jquery-1.11.2.min.js"></script>
    <script type="text/javascript" src="Scripts/jquery-ui.min.js"></script>
    <script type="text/javascript" src="Scripts/common.js"></script>
    <script type="text/javascript" src="Scripts/pdf.js"></script>
    <script type="text/javascript" src="Scripts/pdf.worker.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        <input type="button" id="btnRefresh" value="Refresh" />
            <input type="button" id="btnRezise" value="Resize" />
    <div style="border: 1px solid LightSkyBlue; border-radius: 8px; background-image: linear-gradient(#87CEFA, #B0E0E6);">
        <br />
        <label id="lblDateTime" runat="server"></label>
        <br />
        <div style="width: 900px; height: 300px;">
            <canvas id="myChart" width="900" height="300"></canvas>
        </div>
    	<iti_Tools_Sharp:Grid ID="grdMD" runat="server" AllowPaging="true" PageSize="50" FirstPageText="First" LastPageText="Last" PagerMode="NumericFirstLast" AllowResize="true"
		    CssClass="grid" BodyCssClass="gridBody" SelectedRowCssClass="selectedRow" HeaderCssClass="gridHeader" PagerCssClass="gridPager" FooterCssClass="gridPager" AlternatingRowColor="#dfdfdf">
	    </iti_Tools_Sharp:Grid>
        </div>
    
        <div>
        <label id="lblDateTimeWorkTypes" runat="server"></label>
       	<iti_Tools_Sharp:Grid ID="grdWorkTypeCounts" runat="server" AllowPaging="true" PageSize="50" FirstPageText="First" LastPageText="Last" PagerMode="NumericFirstLast" AllowResize="true"
		    CssClass="grid" BodyCssClass="gridBody" SelectedRowCssClass="selectedRow" HeaderCssClass="gridHeader" PagerCssClass="gridPager" FooterCssClass="gridPager" AlternatingRowColor="#dfdfdf">
	    </iti_Tools_Sharp:Grid>
        </div>
        <br />
        <div id="currentNewsOverviewDiv" style="border: 1px solid LightSkyBlue; border-radius: 8px; background-image: linear-gradient(#87CEFA, #4682B4);">
           <asp:Image runat="server" ID="Image1" /> Current Weekly Overview:
             <%--<iti_Tools_Sharp:Grid ID="gridNewsOverview" runat="server" AllowPaging="true" PageSize="50" FirstPageText="First" LastPageText="Last" PagerMode="NumericFirstLast" AllowResize="false"
            CssClass="grid" BodyCssClass="gridBody_News" SelectedRowCssClass="selectedRow_News" HeaderCssClass="gridHeader_News" PagerCssClass="gridPager" FooterCssClass="gridPager" AlternatingRowColor="#dfdfdf">
            </iti_Tools_Sharp:Grid>--%>
            <input type="button" id="btnView" value="View" />
            
            <iframe id="frameCurrentNewsOverview" src="javascript:'';" style="height: 100%; width: 100%;"></iframe>

        </div>
    </form>
    
    	<script type="text/javascript">

            function initChart() {
                var opt = [];
                opt['datasetname'] = ' # of Work Tasks';
                opt['values'] = '<%=this.assignedToRanks%>'.split(',');
                opt['valuelabels'] = '<%=this.assignedToRankLabels%>'.split(',');
                opt['backgroundcolor'] = [
                    '#dc3545',
                    '#28a745',
                    '#ffc107',
                    '#17a2b8',
                    '#6c757d'
                ],
                opt['bordercolor'] = [
                    '#dc3545',
                    '#28a745',
                    '#ffc107',
                    '#17a2b8',
                    '#6c757d'
                ]

                var data = createBarChartDataSet(opt);

                opt = [];
                var options = createDefaultChartOptions(opt);

                addChartToCanvas('myChart', 'bar', data, options);
            }



            function btnRefresh_onclick() {
                /*
                 var url = 'Loading.aspx?Page=CurrentNewsOverview.aspx?attachmentID=' + 14929;
                //debugger;
                $('#frameCurrentNewsOverview').attr('src', url);
                */
                this.location.reload();

            }



            //function showMeetingMinutesInPDFViewer(attachmentID, AORMeetingInstanceID, date, showAcceptButton) {
            function buttonView_onclick() {
            var buttonLabels = [];
            var buttonFunctions = [];
            var buttonData = [];

            //if (showAcceptButton) {
            //    buttonLabels.push('Accept');
            //    buttonFunctions.push('acceptMeetingFromPDFViewer');
            //    buttonData.push(AORMeetingInstanceID);
            //}

            //if (date == null || date.length == 0) {
            //    // see if we can find a date from the history table
            //    var row = $('#divAORMIHistory').find('tr[key=' + AORMeetingInstanceID + ']');
            //    var td = row.find('td[name=meetingdatecolumn]');
            //    var m = new moment(new Date(td.html()));
            //    date = m.format('MM/DD/YYYY');
            //}
                debugger;
            showPDFViewer('Download_Attachment.aspx?attachmentID=' + 14929, 'Meeting Minutes - ', null, null,
                null,//showAcceptButton ? buttonLabels : null,
                null,//showAcceptButton ? buttonFunctions : null,
                null//showAcceptButton ? buttonData : null
            );
            }

            function loadCurrentNewsOverview() {
			    //var url = window.location.search;
			    //url = editQueryStringValue(url, 'Saved', '0');
                var url = 'Loading.aspx?Page=CurrentNewsOverview.aspx?attachmentID=' + 14929;
                //debugger;
                $('#frameCurrentNewsOverview').attr('src', url);
                
		    }

            function resizeCurrentNewsOverviewDiv() {
                try {
                    //var heightModifier = 0;
                    //heightModifier += $('#frameCurrentNewsOverview').height();

                    //resizePageElement('currentNewsOverviewDiv', heightModifier + 2);


                    var heightModifier = 0;
                heightModifier += $('#frameCurrentNewsOverview').height();

                var origHeight = $('#currentNewsOverviewDiv').height();

                origHeight += 1000;

                $('#currentNewsOverviewDiv').height(origHeight);



                }
                catch (e) {
                    var m = e.message;
                    debugger;
                }
            }

		$(document).ready(function () {
		    try {
                initChart();
                $('#btnView').click(function (event) { buttonView_onclick(); });
                $('#btnRefresh').click(function (event) { btnRefresh_onclick(); });
                $('#btnRezise').click(function (event) { resizeCurrentNewsOverviewDiv(); });
                loadCurrentNewsOverview();
                
   				
		    } catch (e) {
				var m = e.message;
			}
		});
	</script>
</body>
</html>
