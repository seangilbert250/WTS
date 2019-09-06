<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Report_CR.aspx.cs" Inherits="Report_CR" Theme="Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <!-- Copyright (c) 2017 Infinite Technologies, Inc. -->
	<title>CR Report</title>
	<script type="text/javascript" src="Scripts/shell.js"></script>
	<script type="text/javascript" src="Scripts/common.js"></script>
	<script type="text/javascript" src="scripts/filter.js"></script>
	<script type="text/javascript" src="scripts/popupWindow.js"></script>
	<script type="text/javascript" src="Scripts/jquery-1.11.2.js"></script>
	<script type="text/javascript" src="Scripts/jquery-ui.js"></script>
	<script type="text/javascript" src="Scripts/jquery.json-2.4.min.js"></script>
	<script type="text/javascript" src="Scripts/iti_FilterContainer.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="pageContentHeader" style="padding: 5px; font-size: 13px;">
            CR Report
		</div>
		<div class="pageContentInfo" style="padding: 2px;">
            <table style="width: 100%;">
                <tr>
                    <td>
                        Report Title:&nbsp;
                        <span style="color: gray;">CR Report:</span>
                        <input type="text" id="txtTitle" style="width: 400px;" />&nbsp;&nbsp;&nbsp;
                        Report View:&nbsp;
                        <select id="ddlReportView">
                            <option value="CRs Only">CRs Only</option>
                            <option value="Include AORs" selected="selected">Include AORs</option>
                        </select>
                    </td>
                    <td style="text-align: right; padding-right: 5px;">
                        <input type="button" id="btnGenerateReport" value="Generate Report" />
                    </td>
                </tr>
            </table>
        </div>
        <div id="popupContainer" class="popupPageContainer"></div>
        <div style="padding: 5px; min-height:700px">
            <table style="border-collapse: collapse; border: 1px solid gray;">
                <thead>
                    <tr>
                        <th style="background: url(Images/Headers/gridheaderblue.png); height: 20px;">
                            Report Options
                        </th>
                    </tr>
                </thead>
                <tbody>
                    <tr id="trFilterManager">
						<td style="width: 100%;">
							<div id="divFilterManager" style="width: 100%;">
								<div id="divPageManagerFooter" style="width: 100%; background-color: white;">
									<table id="tablePageManagerButtons" class="pageContentHeader" style="width: 100%; border-right: 1px solid #9A9A9A; border-bottom: 1px solid #9A9A9A;">
										<tr>
                                            <td>
                                                Report Filter
                                            </td>
											<td style="vertical-align: middle; text-align: right;">
												<img id="imgShowFilters" src="Images/icons/funnel.png" title="Assign Filters" alt="Assign Filters" style="cursor: pointer; padding-left: 5px;" />
												<img id="imgClearFilters" src="Images/icons/eraser.png" title="Clear Filters" alt="Clear Filters" style="cursor: pointer; padding-left: 3px;" />
											</td>
										</tr>
									</table>
								</div>
								<div id="divFilterManagerBody">
									<table style="width: 100%;">
										<tr>
											<td id="tdAppliedFilters" style="width: 100%; vertical-align: top; font-size: 12px;">
												<div id="divAppliedFilters" class="filterContainer" style="width: 216px;">
												</div>
											</td>
										</tr>
									</table>
								</div>
							</div>
						</td>
					</tr>
                </tbody>
            </table>
        </div>

        <iframe id="frmDownload" style="display: none;"></iframe>
    </form>
    <script id="jsPopup" type="text/javascript">
        var popupContainer = document.getElementById('popupContainer');
        var popupManager = new PopupWindowManager(popupContainer);
	</script>
    <script id="jsFilters" type="text/javascript">
        var filterBox = new filterContainer('divAppliedFilters');

        function imgShowFilters_click() {
            var module = "Reports";
            var option = "Workload";

            if (module == 'DeveloperReview' || module == 'DailyReview' || module == 'AoR') module = 'Work';

            var myData = true;

            if (option == "Workload_Summary") {
                myData = false;
            }

            ShowDimmer(false);
            var strURL = 'Loading.aspx?Page=FilterPage.aspx?random=' + new Date().getTime()
                + '&parentModule=' + module
                + '&MyData=' + myData
                + '&Source=Report';

            var openPopup = popupManager.AddPopupWindow('FilterPage', 'Report Filters'
                , strURL, 450, 900
                , 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

        function imgClearFilters_click() {
            try {
                var parentModule = $('#trSelectedModule').attr('moduleName');

                if (parentModule == 'DeveloperReview' || parentModule == 'DailyReview' || parentModule == 'AoR') parentModule = 'Work';

                if (confirm('You are about to clear the Applied Custom Filters! Are you sure you want to continue?')) {
                    var filters = filterBox.filters.find({ groups: { ParentModule: parentModule, Module: "Custom" } });
                    if (filters) {
                        filters.clear();
                        filterBox.toTable('', 'Module');
                        saveFilters(false, "Load_HomePage");
                    }

                    filters = filterBox.filters.find({ groups: { ParentModule: parentModule, Module: "Default" } });
                    if (filters) {
                        filters.clear();
                        filterBox.toTable('', 'Module');
                        saveFilters(false, "Load_HomePage");
                    }

                    PageMethods.ClearFilterSession(parentModule, null, null);
                }
            }
            catch (e) {
                MessageBox('Error: btnClearFilters_Onclick - ' + e.number + ' : ' + e.message);
            }
        }

	</script>
    <script id="jsEvents" type="text/javascript">
        function btnGenerateReport_click() {
            var validation = validate();

            if (validation.length == 0) {
                $('#frmDownload').attr('src', 'Report_CR.aspx?Download=pdf&ReportView=' + $('#ddlReportView').val() +
                    '&Title=' + encodeURIComponent($.trim($('#txtTitle').val())));
            }
            else {
                MessageBox('Invalid entries: <br><br>' + validation);
            }
        }

        function validate() {
            var validation = [];

            if ($.trim($('#txtTitle').val()).length == 0) validation.push('Title cannot be empty.');

            return validation.join('<br>');
        }
    </script>

    <script id="jsInit" type="text/javascript">
        function initEvents() {
            $('#btnGenerateReport').click(function () { btnGenerateReport_click(); return false; });
        }

        $(document).ready(function () {
            if ('<%=this.Download %>' == '') initEvents();
            $('#imgShowFilters').click(function () { imgShowFilters_click(); });
            $('#imgClearFilters').click(function () { imgClearFilters_click(); });
        });
    </script>
</body>
</html>
