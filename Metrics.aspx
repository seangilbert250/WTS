﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Metrics.aspx.cs" Inherits="Metrics" Theme="Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>Metrics</title>
	<link rel="shortcut icon" href="Images/fav_icon.ico" type="image/x-icon" />
	<link href="Styles/jquery-ui.css" rel="Stylesheet" type="text/css" />
	<script type="text/javascript" src="Scripts/shell.js"></script>
	<script type="text/javascript" src="Scripts/common.js"></script>
	<script type="text/javascript" src="Scripts/jquery-1.11.2.js"></script>
	<script type="text/javascript" src="Scripts/jquery-ui.js"></script>
	<script type="text/javascript" src="Scripts/jquery.json-2.4.min.js"></script>
</head>
<body>
	<form id="form1" runat="server">
		<div id="divPageContainer" class="pageContainer" style="overflow-y: scroll; overflow-x: hidden;">
			<div id="divUpdateMetrics" style="width: 100%; padding: 10px; display: none;">
				<img id="imgLoading" src="Images/Loaders/loader_2.gif" alt="Loading..." style="width: 19px; height: 19px; display: none;" />
				<input type="button" id="buttonUpdateMetrics" value="Update Metrics" />
			</div>

			<div id="divWorkloadMetricsContainer" style="width: 100%;">
				<div id="divWorkloadMetricsHeader" class="pageContentHeader" style="padding: 5px;">
					<div class="attributesRequired" style="width: 10px; display: inline;">
						<img id="imgHideWorkloadMetrics" class="hideSection" sectionname="WorkloadMetrics" alt="Hide Workload Metrics" title="Hide Workload Metrics" src="images/icons/minus_blue.png" height="10" width="10" style="cursor: pointer;" />
						<img id="imgShowWorkloadMetrics" class="showSection" sectionname="WorkloadMetrics" alt="Show Workload Metrics" title="Show Workload Metrics" src="images/icons/add_blue.png" height="10" width="10" style="cursor: pointer; display: none;" />
					</div>
					<div class="attributesLabel" style="padding-left: 5px; display: inline;">Workload Summary:</div>
				</div>
				<div id="divWorkloadMetrics" class="attributesValue" style="height:190px; padding:10px 0px 10px 0px;">
					<iti_Tools_Sharp:Grid ID="gridWorkloadMetrics" runat="server" Style="width: 800px;" AllowPaging="false" PageSize="30" FirstPageText="First" LastPageText="Last" PagerMode="NumericFirstLast" AllowResize="true"
						CssClass="grid" BodyCssClass="metricsRow" SelectedRowCssClass="metricsRow" HeaderCssClass="metricsHeader" PagerCssClass="gridPager" FooterCssClass="gridPager" AlternatingRowColor="#ffffff">
					</iti_Tools_Sharp:Grid>
				</div>
			</div>

<%--			<div id="divRequestMetricsContainer" style="width: 100%;">
				<div id="divRequestMetricsHeader" class="pageContentHeader" style="padding: 5px;">
					<div class="attributesRequired" style="width: 10px; display: inline;">
						<img id="imgHideRequestMetrics" class="hideSection" sectionname="RequestMetrics" alt="Hide Request Metrics" title="Hide Request Metrics" src="images/icons/minus_blue.png" height="10" width="10" style="cursor: pointer;" />
						<img id="imgShowRequestMetrics" class="showSection" sectionname="RequestMetrics" alt="Show Request Metrics" title="Show Request Metrics" src="images/icons/add_blue.png" height="10" width="10" style="cursor: pointer; display: none;" />
					</div>
					<div class="attributesLabel" style="padding-left: 5px; display: inline;">CR/SR/PTS/Internal Items Summary:</div>
				</div>
				<div id="divRequestMetrics" class="attributesValue" style="height:265px; padding: 10px 0px 10px 0px;">
					<iti_Tools_Sharp:Grid ID="gridRequestMetrics" runat="server" Style="width: 700px;" AllowPaging="false" PageSize="30" FirstPageText="First" LastPageText="Last" PagerMode="NumericFirstLast" AllowResize="true"
						CssClass="grid" BodyCssClass="metricsRow" SelectedRowCssClass="metricsRow" HeaderCssClass="metricsHeader" PagerCssClass="gridPager" FooterCssClass="gridPager" AlternatingRowColor="#ffffff">
					</iti_Tools_Sharp:Grid>
				</div>
			</div>--%>

		</div>

		<script id="jsVariables" type="text/javascript">
			var /*_canViewRequest = false,*/ _canViewWorkload = false;
			var _pageUrls;
		</script>

		<script id="jsEvents" type="text/javascript">

			function showUpdateMetrics() {
				$('a').removeAttr('style');
				$('a').removeAttr('onclick');
				$('#divUpdateMetrics').slideDown();
			}

			function updateMetrics() {
				try {
					$('#buttonUpdateMetrics').hide();
					$('#imgLoading').show();
					defaultParentPage.saveFilters(false, "Load_HomePage", true);
				}
				catch (e) { }
			}

			function refreshPage() {
				var url = document.location.href;

				document.location.href = 'Loading.aspx?Page=' + url;
			}

			function resizePage() {
				resizePageElement('divPageContainer');
			}

			function showHideSection_click(imgId, show, sectionName) {
				if (show) {
					$('#div' + sectionName).show();
					$('#' + imgId).hide();
					$('#' + imgId.replace('Show', 'Hide')).show();
					$('tr[section="' + sectionName + '"]').show();

					switch (sectionName) {
						case "Tasks":
							if ($('#frameTasks').attr('src') == "javascript:'';") {
								loadTasks();
							}
							break;
					}
				}
				else {
					$('#div' + sectionName).hide();
					$('#' + imgId).hide();
					$('#' + imgId.replace('Hide', 'Show')).show();
					$('tr[section="' + sectionName + '"]').hide();
				}
			}

			function lbGoToData_click(metricType, columnType, rowType, view) {
				try {
					var grid = '';
					switch(metricType) {
						case 'Work Tasks':
							grid = 'Workload';
							break;
						//case 'Work Requests':
						//	grid = 'WorkRequest';
						//	break;
						default:
							return;
					}

					defaultParentPage.menuSelection_click('COLLAPSE', /*grid == 'WorkRequest' ? 'AoR' :*/ 'Work');
					defaultParentPage.moduleOption_click('Work', grid);
					defaultParentPage.$('#ddlView_Work option').filter(function () {
						return $(this).text() == view;
					}).prop('selected', true);

					if (view == 'Enterprise') {
						var filters = defaultParentPage.filterBox.filters.find({ groups: { ParentModule: "Work", Module: "Custom" } });
						if (filters) {
							filters.clear();
							defaultParentPage.filterBox.toTable('', 'Module');
						}
						filters = defaultParentPage.filterBox.filters.find({ groups: { ParentModule: "Work", Module: "Default" } });
						if (filters) {
							filters.clear();
							defaultParentPage.filterBox.toTable('', 'Module');
						}
						defaultParentPage.PageMethods.ClearFilterSession("Work", null, null);
					}

					var filterValue = '', filterText = '';
					switch (metricType) {
					    case 'Work Tasks':
					        if (rowType != 'TOTAL') {
					            switch (rowType) {
					                case 'High':
					                    filterValue = 1;
					                    filterText = 'High';
					                    break;
					                case 'Med':
					                    filterValue = 2;
					                    filterText = 'Med';
					                    break;
					                case 'Low':
					                    filterValue = 3;
					                    filterText = 'Low';
					                    break;
					                case 'NA':
					                    filterValue = 4;
					                    filterText = 'NA';
					                    break;
					            }
					            try { defaultParentPage.filterBox.filters.find({ name: "Workload Priority", groups: { ParentModule: "Work", Module: "Custom" } })[0].parameters.findByValue(filterValue).remove(); } catch (e) { }
					            if (filterValue > 0) defaultParentPage.filterBox.filters.add({ name: "Workload Priority", groups: { ParentModule: "Work", Module: "Custom" } }).parameters.add(filterValue, filterText);
					        }
					        if (columnType != 'Priority') {
					            switch (columnType) {
					                case 'New':
					                    filterValue = 1;
					                    filterText = 'New';
					                    break;
					                case 'In-Progress':
					                    filterValue = 5;
					                    filterText = 'In Progress';
					                    break;
					                case 'Re-Opened':
					                    filterValue = 2;
					                    filterText = 'Re-Opened';
					                    break;
					                case 'Info Requested':
					                    filterValue = 3;
					                    filterText = 'Info Requested';
					                    break;
					                case 'Checked-In':
					                    filterValue = 8;
					                    filterText = 'Checked In';
					                    break;
					                case 'Deployed':
					                    filterValue = 9;
					                    filterText = 'Deployed';
					                    break;
					                case 'Closed':
					                    filterValue = 10;
					                    filterText = 'Closed';
					                    break;
					            }
					            try { defaultParentPage.filterBox.filters.find({ name: "Workload Status", groups: { ParentModule: "Work", Module: "Custom" } })[0].parameters.findByValue(filterValue).remove(); } catch (e) { }
					            if (filterValue > 0) defaultParentPage.filterBox.filters.add({ name: "Workload Status", groups: { ParentModule: "Work", Module: "Custom" } }).parameters.add(filterValue, filterText);
					        }
					        break;
					//	case 'Work Requests':
					//		if (rowType != 'TOTAL') {
					//			switch (rowType) {
					//				case 'Admin':
					//					filterValue = 8;
					//					filterText = 'Admin';
					//					break;
					//				case 'CR/PTS':
					//					filterValue = 2;
					//					filterText = 'CR/PTS';
					//					break;
					//				case 'CS':
					//					filterValue = 5;
					//					filterText = 'CS';
					//					break;
					//				case 'IA':
					//					filterValue = 4;
					//					filterText = 'IA';
					//					break;
					//				case 'Internal':
					//					filterValue = 7;
					//					filterText = 'Internal';
					//					break;
					//				case 'R&D':
					//					filterValue = 3;
					//					filterText = 'R&D';
					//					break;
					//				case 'SME-SR':
					//					filterValue = 6;
					//					filterText = 'SME-SR';
					//					break;
					//				case 'Other':
					//					filterValue = 1;
					//					filterText = 'Other';
					//					break;
					//			}
					//			try { defaultParentPage.filterBox.filters.find({ name: "Request Type", groups: { ParentModule: "Work", Module: "Custom" } })[0].parameters.findByValue(filterValue).remove(); } catch (e) { }
					//			if (filterValue > 0) defaultParentPage.filterBox.filters.add({ name: "Request Type", groups: { ParentModule: "Work", Module: "Custom" } }).parameters.add(filterValue, filterText);
					//		}
					//		if (columnType != 'Request Type') {
					//			switch (columnType) {
					//				case 'Investigation':
					//					filterValue = 1;
					//					filterText = 'Investigation';
					//					break;
					//				case 'Planning':
					//					filterValue = 2;
					//					filterText = 'Planning';
					//					break;
					//				case 'Design':
					//					filterValue = 3;
					//					filterText = 'Design';
					//					break;
					//				case 'Develop':
					//					filterValue = 4;
					//					filterText = 'Develop';
					//					break;
					//				case 'Testing':
					//					filterValue = 5;
					//					filterText = 'Testing';
					//					break;
					//				case 'Deploy':
					//					filterValue = 6;
					//					filterText = 'Deploy';
					//					break;
					//				case 'Review':
					//					filterValue = 7;
					//					filterText = 'Review';
					//					break;
					//			}
					//			try { defaultParentPage.filterBox.filters.find({ name: "Request Phase", groups: { ParentModule: "Work", Module: "Custom" } })[0].parameters.findByValue(filterValue).remove(); } catch (e) { }
					//			if (filterValue > 0) defaultParentPage.filterBox.filters.add({ name: "Request Phase", groups: { ParentModule: "Work", Module: "Custom" } }).parameters.add(filterValue, filterText);
					//		}
					//		break;
					}
					defaultParentPage.filterBox.toTable({ groups: { ParentModule: 'Work' } }, 'Module');

					defaultParentPage.$('#buttonGetData').trigger('click');
				}
				catch (e) { }
			}

		</script>
	</form>

	<script id="jsInitialize" type="text/javascript">
		
		function initializeEvents() {

			$(window).resize(resizePage);
			
			$('#buttonUpdateMetrics').click(function () { updateMetrics(); return false; });
			$('#imgRefresh').click(function () { refreshPage(); });
			
			$('.hideSection').click(function (event) {
				showHideSection_click($(this).attr('id'), false, $(this).attr('sectionName'));
			});
			$('.showSection').click(function (event) {
				showHideSection_click($(this).attr('id'), true, $(this).attr('sectionName'));
			});
		}

		$(document).ready(function () {
			_pageUrls = new PageURLs();

<%--			if ('<%=this.CanViewRequest.ToString().ToUpper() %>' == 'TRUE') {
				_canViewRequest = true;
			}--%>
			if ('<%=this.CanViewWorkload.ToString().ToUpper() %>' == 'TRUE') {
				_canViewWorkload = true;
			}

			initializeEvents();

			resizePage();
		});

	</script>
</body>
</html>
