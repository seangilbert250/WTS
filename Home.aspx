﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Home.aspx.cs" Inherits="Home" Theme="Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>WTS - Home</title>

	<link rel="stylesheet" href="Styles/jquery-ui.css" />
	<script type="text/javascript" src="Scripts/jquery-1.11.2.js"></script>
	<script type="text/javascript" src="Scripts/jquery-ui.js"></script>
	<script type="text/javascript" src="Scripts/common.js"></script>
	<script type="text/javascript" src="Scripts/shell.js"></script>

    <link rel="stylesheet" type="text/css" href="App_Themes/Default/Default.css" /> 

</head>
<body>
	<form id="form1" runat="server">
		<div id="divPage" class="pageContainer">
			<div id="pageContentHeader" class="pageContentHeader">
				<table style="width: 100%;">
					<tr>
						<td style="text-align: left; padding-left: 4px; height: 30px;">
							<asp:Label ID="lblToday" runat="server" Style="font-size: 16px; font-weight:bold;"></asp:Label>
						</td>
					</tr>
				</table>
			</div>
			<div id="pageContentInfo" class="pageContentInfo">
				<table style="width: 100%; height: 100%;">
					<tr>
						<td style="text-align: left; padding-left: 5px; width:20px; display:none;">
							<img id="imgRefresh" alt="Refresh Page" title="Refresh Page" runat="server" src="Images/Icons/arrow_refresh_blue.png" style="cursor: pointer;" />
						</td>
						<td>
							&nbsp;
						</td>
						<td style="text-align:right; padding-right:3px;">
							<span id="spanWorkShortcuts" runat="server" style="display:none;">
								<input type="button" id="buttonGoToWorkItem" value="Go to Work Task #" />
								<input type="text" id="txtWorkItem" name="GoTo" tabindex="2" maxlength="11" size="8" />
								&nbsp;
								<input type="button" id="buttonGoToWorkRequest" value="Go to Work Request #" style="display: none;" />
								<input type="text" id="txtWorkRequest" name="GoTo" tabindex="3" maxlength="6" size="3" style="display: none;" />
								&nbsp;&nbsp;
							</span>
                            <input type="button" id="buttonNewWorkItem" value="Add Primary Task" />
							<input type="button" id="buttonConcerns" value="Workload Concerns" />
							<input type="button" id="buttonEmailHotlist" value="Email Hotlist" style="display: none;" />
							<input type="button" id="buttonSRlist" value="Email SR Report" style="display: none;" />
						</td>
						<td id="tdCalendar" style="text-align: right; width: 18px; padding: 2px 5px 1px 0px; height: 30px; visibility:hidden">
							<img id="imgCalendar" alt="View Calendar" title="View Calendar" src="Images/Icons/calendar_2.png"  width="18" height="18" style="cursor: pointer; margin-left: 4px;"/>
						</td>
					</tr>
				</table>
			</div>
			
			<div id="divTabsContainer" class="mainPageContainer">
				<ul id="HomePageTabs" runat="server">
					<li><a href="#divDashboard" onclick="HomeTab_click('dashboard');">Dashboard</a></li>
					<li><a href="#divNews" onclick="HomeTab_click('news');">WTS News</a></li>
					<li><a href="#divMetrics" onclick="HomeTab_click('metrics');">Metrics</a></li>
<%--					<li><a href="#divCVTMetrics" onclick="HomeTab_click('cvtmetrics');">CVT Metrics</a></li>--%>
				</ul>
				<div id="divDashboard">
					<iframe id="frameDashboard" name="frameDashboard" class="contentFrame" runat="server" src="javascript:'';" scrolling="no" frameborder="0" style="width: 100%; height: 100%; margin: 0px; padding: 0px;" title="">Dashboard</iframe>
				</div>
				<div id="divNews">
					<iframe id="frameNews" name="frameNews" class="contentFrame" runat="server" src="javascript:'';" scrolling="no" frameborder="0" style="width: 100%; height: 100%; margin: 0px; padding: 0px;" title="">WTS News</iframe>
				</div>
				<div id="divMetrics">
					<iframe id="frameMetrics" name="frameMetrics" class="contentFrame" runat="server" src="javascript:'';" scrolling="no" frameborder="0" style="width: 100%; height: 100%; margin: 0px; padding: 0px;" title="">Metrics</iframe>
				</div>
				<div id="divCVTMetrics">
					<iframe id="frameCVTMetrics" name="frameCVTMetrics" class="contentFrame" runat="server" src="javascript:'';" scrolling="no" frameborder="0" style="width: 100%; height: 100%; margin: 0px; padding: 0px;" title="" visible="false" >CVT Metrics</iframe>
				</div>
			</div>
		</div>


		<asp:ScriptManager ID="scriptManager1" runat="server" EnablePageMethods="true">
			<Services>
				<asp:ServiceReference Path="~/WorkloadWebmethods.asmx" />
			</Services>
		</asp:ScriptManager>

		<script id="jsAJAX" type="text/javascript">

			function verifyItemExists(itemID, taskNumber, type) {
				//PageMethods.ItemExists(itemID, type, function (result) { verifyItemExists_done(itemID, type, result); }, function (result) { verifyItemExists_done(itemID, type, false); });
                WorkloadWebmethods.ItemExists(itemID, taskNumber, type, function (result) { verifyItemExists_done(itemID, taskNumber, type, result); }, function (result) { verifyItemExists_done(itemID, taskNumber, type, false); });
			}

			function verifyItemExists_done(itemID, taskNumber, type, exists) {
				if (exists && exists.toUpperCase() == "TRUE") {
					switch (type) {
						case 'Primary Task':
							editWorkItem(itemID);
							break;
						case 'Work Request':
							editRequest(itemID);
                            break;
                        case 'Subtask':
                            PageMethods.WorkItem_TaskID_Get(itemID, taskNumber, function (result) { editWorkTask(itemID, taskNumber, result); });
                            break;
					}
				}
				else {
                    if (taskNumber > -1) {
                        MessageBox('Could not find ' + type + ' # ' + itemID + '-' + taskNumber);
                    } else {
                        MessageBox('Could not find ' + type + ' # ' + itemID);
                    }
				}
			}

			function on_error(result) {
				var resultText = 'An error occurred when communicating with the server';/*\n' +
					'readyState = ' + result.readyState + '\n' +
					'responseText = ' + result.responseText + '\n' +
					'status = ' + result.status + '\n' +
					'statusText = ' + result.statusText;*/

				MessageBox('save error:  \n' + resultText);
			}


		</script>

		<script id="jsGoto" type="text/javascript">

			function formatGoTo(obj) {
				var text = $(obj).val();

				if (/[^0-9-]|^0+(?!$)/g.test(text)) {
					$(obj).val(text.replace(/[^0-9-]|^0+(?!$)/g, ''));
				}
			}

			function buttonGoToWorkItem_click() {
				var recordID = $('#txtWorkItem').val();

				if (recordID.length > 0) {
                    if (recordID.indexOf('-') > -1) {
                        var taskNumber = recordID.slice(recordID.indexOf('-') + 1);
                        recordID = recordID.slice(0, recordID.indexOf('-'));
                        verifyItemExists(recordID, taskNumber, 'Subtask');
                    } else {
                        verifyItemExists(recordID, -1, 'Primary Task');
                    }
                }
                else {
                    MessageBox('Please enter a Work Task #.');
                }
			}

			function buttonGoToWorkRequest_click() {
				var requestID = $('#txtWorkRequest').val();

				if (requestID > 0) {
					verifyItemExists(requestID, -1, 'Work Request');
				}
				else {
					MessageBox('Please enter a Work Request #.');
				}
			}

			function editWorkItem(recordId) {
				if (parent.ShowFrameForWorkloadItem) {
					parent.ShowFrameForWorkloadItem(false, recordId, recordId, true);
				}
				else {
					var title = '', url = '';
                    var h = 700, w = 1400;

                    title = 'Primary Task - [' + recordId + ']';
					url = _pageURLs.Maintenance.WorkItemEditParent
						+ '?WorkItemID=' + recordId;

					//open in a popup
					var openPopup = popupManager.AddPopupWindow('WorkItem', title, 'Loading.aspx?Page=' + url, h, w, 'PopupWindow', this);
					if (openPopup) {
						openPopup.Open();
					}
				}
            }

            function editWorkTask(itemID, taskNumber, result) {
                var title = 'Subtask - [' + itemID + '-' + taskNumber + ']';
                var url = 'Loading.aspx?Page=' + _pageURLs.Maintenance.TaskEdit
                    + window.location.search 
                    + '&workItemID=' + itemID
                    + '&newTask=0'
                    + '&taskID=' + result;

                var h = 700, w = 850;

                var openPopup = popupManager.AddPopupWindow('WorkloadSubTask', title, url, h, w, 'PopupWindow', this);
                if (openPopup) {
                    openPopup.Open();
                }
            }

			function editRequest(requestID) {
				if (parent.ShowFrameForWorkRequest) {
					parent.ShowFrameForWorkRequest(false, requestID, requestID, true);
				}
				else {
					var title = '', url = '';
					var h = 700, w = 1000;

					title = 'Work Request - [' + requestID + ']';
					url = _pageURLs.Maintenance.WorkRequestEditParent
						+ '?WorkRequestID=' + requestID;

					//open in a popup
					var openPopup = popupManager.AddPopupWindow('WorkRequest', title, 'Loading.aspx?Page=' + url, h, w, 'PopupWindow', this);
					if (openPopup) {
						openPopup.Open();
					}
				}
			}

		</script>

		<script type="text/javascript">
			var _pageURLs = new PageURLs();
			var _tabToLoad = 'Metrics';
			var _module = 'Work';

			function showUpdateMetrics() {
				var frame = $('#<%=this.frameMetrics.ClientID %>')[0];

				if (frame && frame.contentWindow
					&& frame.contentWindow.showUpdateMetrics) {
					frame.contentWindow.showUpdateMetrics();
				}
			}

			function refreshPage() {
				document.location.href = 'Loading.aspx?Page=' + document.location.href;
			}

			function buttonEmailHotlist_click() {
				WorkloadWebmethods.EmailHotlist(buttonEmailHotlist_done, function (result) { });
			}

			function buttonEmailHotlist_done(result) {
				MessageBox('Email has been sent.');
			}

			function buttonSRlist_click() {
			    WorkloadWebmethods.SRHotlist(buttonSRlist_done, function (result) { });
			}

			function buttonSRlist_done(result) {
			    MessageBox('Email has been sent.');
			}

			function buttonConcerns_click() {
				var title = '', url = '';
				var h = 220, w = 500;

				title = 'Workload Concerns';
				url = _pageURLs.Maintenance.WorkloadConcerns;

				//open in a popup
				var openPopup = popupManager.AddPopupWindow('WorkItem', title, 'Loading.aspx?Page=' + url, h, w, 'PopupWindow', this);
				if (openPopup) {
					openPopup.Open();
				}
            }

            function buttonNewWorkItem_click() {
                if (parent.ShowFrameForWorkloadItem) {
                    parent.ShowFrameForWorkloadItem(true, 0, 0, true);
                }
                else {
                    var title = '', url = '';
                    var h = 700, w = 1400;

                    title = 'Primary Task';
                    url = _pageURLs.Maintenance.WorkItemEditParent;

                    var openPopup = popupManager.AddPopupWindow('WorkItem', title, 'Loading.aspx?Page=' + url, h, w, 'PopupWindow', this);
                    if (openPopup) {
                        openPopup.Open();
                    }
                }
            }

			function imgCalendar_click() {

			}

			function HomeTab_click(tabName) {
				$('div', $('#divTabsContainer')).hide();

				switch (tabName.toUpperCase()) {
					case 'DASHBOARD':
						if ($('#<%=this.frameDashboard.ClientID%>') && $('#<%=this.frameDashboard.ClientID%>').attr('src') == "javascript:'';") {
							$('#<%=this.frameDashboard.ClientID%>').attr('src', _pageURLs.Dashboard);
						}
						$('#divDashboard').show();

						break;
					case 'NEWS':
						if ($('#<%=this.frameNews.ClientID%>') && $('#<%=this.frameNews.ClientID%>').attr('src') == "javascript:'';") {
							$('#<%=this.frameNews.ClientID%>').attr('src', _pageURLs.News);
						}
					    $('#divNews').show();

						break;
					case 'METRICS':
						if ($('#<%=this.frameMetrics.ClientID%>') && $('#<%=this.frameMetrics.ClientID%>').attr('src') == "javascript:'';") {
							$('#<%=this.frameMetrics.ClientID%>').attr('src', _pageURLs.Metrics + '?Module=' + _module);
						}
						$('#divMetrics').show();

						break;
					case 'CVTMETRICS':
						if ($('#<%=this.frameCVTMetrics.ClientID%>') && $('#<%=this.frameCVTMetrics.ClientID%>').attr('src') == "javascript:'';") {
							$('#<%=this.frameCVTMetrics.ClientID%>').attr('src', _pageURLs.CVTMetrics);
						}
						//$('#divCVTMetrics').show();

						break;
					default:
						if ($('#<%=this.frameMetrics.ClientID%>') && $('#<%=this.frameMetrics.ClientID%>').attr('src') == "javascript:'';") {
							$('#<%=this.frameMetrics.ClientID%>').attr('src', _pageURLs.Metrics + '?Module=' + _module);
						}
						$('#divMetrics').show();

						break;
				}
				resizePage();
			}

			function resizePage() {
				try {
					var heightModifier = 0;

					resizePageElement('divPage', heightModifier + 2);
					resizePageElement('divTabsContainer', heightModifier + 3);

					resizePageElement('<%=this.frameDashboard.ClientID%>', heightModifier + 12);
					resizePageElement('<%=this.frameNews.ClientID%>', heightModifier + 12);
					resizePageElement('<%=this.frameMetrics.ClientID%>', heightModifier + 12);
					resizePageElement('<%=this.frameCVTMetrics.ClientID%>', heightModifier + 12);

					resizePageElement('divDashboard', heightModifier + 11);
					resizePageElement('divNews', heightModifier + 11);
					resizePageElement('divMetrics', heightModifier + 11);
					resizePageElement('divCVTMetrics', heightModifier + 11);

				}
				catch (e) {
					var m = e.message;
				}
			}

		</script>
	</form>

	<script type="text/javascript">

		$(document).ready(function () {
			try {
				_tabToLoad = '<%=this.TabToLoad %>';
				_module = '<%=this.Module %>';
				$(window).resize(resizePage);

                $('#imgRefresh').click(function () { refreshPage(); });
                $('#buttonNewWorkItem').click(function (event) { buttonNewWorkItem_click(); return false; });
				$('#buttonEmailHotlist').click(function (event) { buttonEmailHotlist_click(); return false; });
				$('#buttonSRlist').click(function (event) { buttonSRlist_click(); return false; });
				$('#buttonConcerns').click(function () { buttonConcerns_click(); return false; });
				$('#imgCalendar').click(function () { imgCalendar_click(); });

				var activeTab = 0;

				switch (_tabToLoad) {
					case 'Dashboard':
						activeTab = 0;
						break;
					case 'News':
                        activeTab = 1;
						break;
					case 'Metrics':
						activeTab = 2;
						break;
					case 'CVTMetrics':
						activeTab = 3;
						break;
					default:
						_tabToLoad = 'Metrics';
						activeTab = 2;
						break;
				}


				$('#divTabsContainer').tabs({
					heightStyle: "fill"
					, collapsible: false
					, active: activeTab
				});

				//activeTab = 0;
				//HomeTab_click(_tabToLoad);
				//$('#divDashboard').show();

				$("#txtWorkItem").keyup(function (event) {
					formatGoTo(this);

					if (event.keyCode == 13 || event.keyCode == 144) {
						$('#buttonGoToWorkItem').trigger('click');
					}
				});
				$("#txtWorkRequest").keyup(function (event) {
					formatGoTo(this);

					if (event.keyCode == 13 || event.keyCode == 144) {
						$('#buttonGoToWorkRequest').trigger('click');
					}
				});
				$("input:text[name='GoTo']").bind('paste', null, function () {
					formatGoTo(this);
				});
				$('#buttonGoToWorkItem').click(function (event) { buttonGoToWorkItem_click(); return false; });
				$('#buttonGoToWorkRequest').click(function (event) { buttonGoToWorkRequest_click(); return false; });


				resizePage();

			    $('#divCVTMetrics').hide();

				activeTab = 0;
                HomeTab_click(_tabToLoad.toUpperCase());
				//$('#divMetrics').hide();
				//$('#divDashboard').show();


			} catch (e) {
				var m = e.message;
			}
		});
	</script>
</body>
</html>
