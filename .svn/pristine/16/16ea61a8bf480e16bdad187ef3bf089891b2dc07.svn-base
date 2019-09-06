<%@ Page Language="C#" AutoEventWireup="true" CodeFile="WorkRequestEditParent.aspx.cs" Inherits="WorkRequestEditParent" Theme="Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>Review/Edit Work Request</title>

	<link rel="stylesheet" href="Styles/jquery-ui.css" />
	<script type="text/javascript" src="Scripts/jquery-1.11.2.js"></script>
	<script type="text/javascript" src="Scripts/common.js"></script>
	<script type="text/javascript" src="Scripts/shell.js"></script>
	<script type="text/javascript" src="Scripts/popupWindow.js"></script>
</head>
<body>
    <form id="form1" runat="server">
		<div id="divPage" class="pageContainer">
			<div id="divWorkRequestHeader" class="pageContentHeader" style="font-size: 13px;">
				<table cellpadding="0" cellspacing="0" style="width: 100%;">
					<tr>
						<td style="text-align: left; padding-left: 4px; font-size: 14px; height: 30px;">Review/Edit Work Request - <span id="labelWorkRequestNumber" runat="server" style="font-size: 14px;"></span>
						</td>
						<td>&nbsp;</td>
						<td style="text-align: right; padding-right: 0px; width: 95px;">
							<button id="buttonBackToGrid" runat="server" value="Back to Grid" style="padding: 1px 2px 1px 2px; width: 88px; float: left; display: inline;">Back to Grid</button>
						</td>
					</tr>
				</table>
			</div>
			<div id="divWorkRequestHeaderButtons" class="pageContentInfo" style="height: auto; padding: 2px; border-bottom: 1px solid gainsboro;">
				<table cellpadding="0" cellspacing="0" style="width: 100%; text-align: left;">
					<tr>
						<td>
							<table cellpadding="0" cellspacing="0" style="white-space: nowrap;">
								<tr>
									<td style="text-align: left; padding-top: 2px;">
										<img id="imgRefresh" alt="Refresh Page" title="Refresh Page" src="images/icons/arrow_refresh_blue.png" width="15" height="15" style="cursor: pointer; margin-left: 4px;" />
									</td>
								</tr>
							</table>
						</td>
						<td style="text-align: right; white-space: nowrap;">
							<table cellpadding="0" cellspacing="0" style="float: right; padding-right: 2px;">
								<tr>
									<td style="display: none;">
										<input type="button" id="buttonSave" value="Save" />
									</td>
								</tr>
							</table>
						</td>
					</tr>
				</table>
			</div>

			<div id="divTabsContainer" class="mainPageContainer">
				<ul id="Tabs" runat="server">
					<li><a href="#divDetails" onclick="Tab_click('details');">Details</a></li>
					<li><a href="#divNotes" onclick="Tab_click('notes');">Notes</a></li>
					<li><a href="#divAttachments" onclick="Tab_click('attachments');">Attachments</a></li>
				</ul>
				<div id="divDetails">
					<iframe id="frameDetails" name="frameDetails" class="contentFrame" runat="server" src="javascript:'';" scrolling="no" frameborder="0" style="width: 100%; height: 100%; margin: 0px; padding: 0px;" title="">Work Request Details</iframe>
				</div>
				<div id="divNotes">
					<iframe id="frameNotes" name="frameNotes" class="contentFrame" runat="server" src="javascript:'';" scrolling="no" frameborder="0" style="width: 100%; height: 100%; margin: 0px; padding: 0px;" title="">Work Request Notes</iframe>
				</div>
				<div id="divAttachments">
					<iframe id="frameAttachments" name="frameAttachments" class="contentFrame" runat="server" src="javascript:'';" scrolling="no" frameborder="0" style="width: 100%; height: 100%; margin: 0px; padding: 0px;" title="">Work Request Attachments</iframe>
				</div>
			</div>
		</div>

		<script id="jsEvents" type="text/javascript">
			var _pageUrls;

			function continueMove(blnIsFromTabClick) {
				var cont = false;

				//TODO: check if need to change
				cont = true;

				return cont;
			}

			function refreshPage() {
				document.location.href = 'Loading.aspx?Page=' + document.location.href;
			}

			function resizePage() {
				try {
					var heightModifier = 0;
					heightModifier += 10; //$('#mainPageFooter').height();

					resizePageElement('divPage', heightModifier + 2);
					resizePageElement('divTabsContainer', heightModifier + 3);

					resizePageElement('<%=this.frameDetails.ClientID%>', heightModifier + 12);
					resizePageElement('<%=this.frameNotes.ClientID%>', heightModifier + 12);
					resizePageElement('<%=this.frameAttachments.ClientID%>', heightModifier + 12);

					resizePageElement('divDetails', heightModifier + 11);
					resizePageElement('divNotes', heightModifier + 11);
					resizePageElement('divAttachments', heightModifier + 11);

				}
				catch (e) {
					var m = e.message;
				}
			}

			function Tab_click(tabName) {
				$('div', $('#divTabsContainer')).hide();

				switch (tabName.toUpperCase()) {
					case 'DETAILS':
						if ($('#<%=this.frameDetails.ClientID%>') && $('#<%=this.frameDetails.ClientID%>').attr('src') == "javascript:'';") {
							$('#<%=this.frameDetails.ClientID%>').attr('src', 'WorkRequest_Details.aspx' + window.location.search);
						}
						$('#divDetails').show();

						break;
					case 'NOTES':
						if ($('#<%=this.frameNotes.ClientID%>') && $('#<%=this.frameNotes.ClientID%>').attr('src') == "javascript:'';") {
							$('#<%=this.frameNotes.ClientID%>').attr('src', 'WorkRequest_Comments.aspx' + window.location.search);
						}
						$('#divNotes').show();

						break;
					case 'ATTACHMENTS':
						if ($('#<%=this.frameAttachments.ClientID%>') && $('#<%=this.frameAttachments.ClientID%>').attr('src') == "javascript:'';") {
							var url = 'WorkRequest_Attachments.aspx' + window.location.search;
							$('#<%=this.frameAttachments.ClientID%>').attr('src', url);
						}
						$('#divAttachments').show();

						break;
				}

				resizePage();
			}


			function gotoId(id) {
				var contin = continueMove();
				if (!contin || contin == 'cancel') {
					return;
				}

				if (id != 0) {
					if (parent.ShowFrameForWorkItem) {
						var newWorkRequest = false;
						var workRequestId = id;
						var sourceWorkRequestId = 0;
						var workRequestNum = '';
						var sortValue = '';
						parent.ShowFrameForWorkRequest(newWorkRequest, workRequestId, sourceWorkRequestId, workRequestNum, true);
					}
				}
			}
			function buttonFirst_click() {
				gotoId(+'<%=this.FirstWorkRequestID %>');
			}
			function buttonPrevious_click() {
				gotoId(+'<%=this.PreviousWorkRequestID %>');
			}
			function buttonNext_click() {
				gotoId(+'<%=this.NextWorkRequestID %>');
            }
            function buttonLast_click() {
            	gotoId(+'<%=this.LastWorkRequestID %>');
            }

            function buttonBackToGrid_click() {
            	var contin = continueMove();
            	if (contin == 'cancel') {
            		return;
            	}

            	if (parent.ShowFrame) {
            		parent.ShowFrame('Grid', false, 0, 0, '', false);
            	}
            	else if (closeWindow) {
            		closeWindow();
            	}
            }

		</script>
    </form>

	<script type="text/javascript" src="Scripts/jquery-ui.js"></script>
	<script type="text/javascript">

		$(document).ready(function () {
			try {
				_pageURLs = new PageURLs();

				$(window).resize(resizePage);

				$('#imgRefresh').click(function () { refreshPage(); });
				$('#<%=this.buttonBackToGrid.ClientID %>').click(function () { buttonBackToGrid_click(); return false; });

				$('#divTabsContainer').tabs({
					heightStyle: "fill"
					, collapsible: false
					, active: 0
				});

				if ('<%=this.WorkRequestID %>' == '0') {
					$("#divTabsContainer").tabs("option", "disabled", [1, 2]);
				}

				Tab_click('details');

				resizePage();
			} catch (e) {
				var m = e.message;
			}
		});
	</script>
</body>
</html>
