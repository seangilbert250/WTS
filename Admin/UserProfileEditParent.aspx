<%@ Page Language="C#" AutoEventWireup="true" CodeFile="UserProfileEditParent.aspx.cs" Inherits="UserProfileEditParent" Theme="Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Review/Edit User Profile</title>

	<link rel="stylesheet" href="../Styles/jquery-ui.css" />
	<script type="text/javascript" src="../Scripts/shell.js"></script>
	<script type="text/javascript" src="../Scripts/common.js"></script>
	<script type="text/javascript" src="../Scripts/popupWindow.js"></script>
	<script type="text/javascript" src="../Scripts/jquery-1.11.2.js"></script>
	<script type="text/javascript" src="../Scripts/jquery-ui.js"></script>
	<script type="text/javascript" src="../Scripts/jquery.json-2.4.min.js"></script>
</head>
<body>
    <form id="form1" runat="server">
		<div id="divPage" class="pageContainer">
			<div id="divUserHeader" class="pageContentHeader">
				<table cellpadding="0" cellspacing="0" style="width: 100%;">
					<tr>
						<td style="text-align: left; padding-left: 4px; font-size: 14px; height: 30px;">Review/Edit User - <span id="labelUserName" runat="server" style="font-size: 14px;"></span>
						</td>
						<td>&nbsp;</td>
						<td style="text-align: right; padding-right: 0px; width: 50px;">
							<button id="buttonBackToGrid" runat="server" value="Close" style="padding: 1px 2px 1px 2px; width: 45px; float: left; display: inline;">Back to Grid</button>
						</td>
					</tr>
				</table>
			</div>
			<div id="divUserHeaderButtons" class="pageContentInfo" style="height: auto; padding: 2px; border-bottom: 1px solid gainsboro;">
				<table cellpadding="0" cellspacing="0" style="width: 100%; text-align: left;">
					<tr>
						<td>
							<table cellpadding="0" cellspacing="0" style="white-space: nowrap;">
								<tr>
									<td style="text-align: left; padding-top: 2px;">
										<img id="imgRefresh" alt="Refresh Page" title="Refresh Page" src="../Images/Icons/arrow_refresh_blue.png" width="15" height="15" style="cursor: pointer; margin-left: 4px;" />
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
					<li><a href="#divProfile" onclick="Tab_click('profile');">Profile</a></li>
					<li><a href="#divOptions" onclick="Tab_click('options');">Options</a></li>
					<li><a href="#divRoles" onclick="Tab_click('roles');">Roles</a></li>
					<li><a href="#divCertifications" onclick="Tab_click('certifications');">Certifications</a></li>
					<li><a href="#divHardware" onclick="Tab_click('hardware');">Hardware</a></li>
				</ul>
				<div id="divProfile">
					<iframe id="frameProfile" name="frameProfile" class="contentFrame" runat="server" src="javascript:'';" scrolling="no" frameborder="0" style="width: 100%; height: 100%; margin: 0px; padding: 0px;" title="">User Profile</iframe>
				</div>
				<div id="divOptions">
					<iframe id="frameOptions" name="frameOptions" class="contentFrame" runat="server" src="javascript:'';" scrolling="no" frameborder="0" style="width: 100%; height: 100%; margin: 0px; padding: 0px;" title="">User Options</iframe>
				</div>
				<div id="divRoles">
					<iframe id="frameRoles" name="frameRoles" class="contentFrame" runat="server" src="javascript:'';" scrolling="no" frameborder="0" style="width: 100%; height: 100%; margin: 0px; padding: 0px;" title="">User Roles</iframe>
				</div>
				<div id="divCertifications">
					<iframe id="frameCertifications" name="frameCertifications" class="contentFrame" runat="server" src="javascript:'';" scrolling="no" frameborder="0" style="width: 100%; height: 100%; margin: 0px; padding: 0px;" title="">User Certifications</iframe>
				</div>
				<div id="divHardware">
					<iframe id="frameHardware" name="frameHardware" class="contentFrame" runat="server" src="javascript:'';" scrolling="no" frameborder="0" style="width: 100%; height: 100%; margin: 0px; padding: 0px;" title="">User Hardware</iframe>
				</div>
			</div>
		</div>

		<script id="jsEvents" type="text/javascript">
			var _pageUrls;
			var _userID = 0;

			function continueMove(blnIsFromTabClick) {
				var cont = false;

				//TODO: check if need to change
				cont = true;

				return cont;
			}

			function refreshPage() {
				document.location.href = '../Loading.aspx?Page=' + document.location.href;
			}

			function resizePage() {
				try {
					var heightModifier = 0;
					heightModifier += 10; //$('#mainPageFooter').height();

					resizePageElement('divPage', heightModifier + 2);
					resizePageElement('divTabsContainer', heightModifier + 3);

					resizePageElement('<%=this.frameProfile.ClientID%>', heightModifier + 11);
					resizePageElement('<%=this.frameOptions.ClientID%>', heightModifier + 11);
					resizePageElement('<%=this.frameRoles.ClientID%>', heightModifier + 11);
					resizePageElement('<%=this.frameCertifications.ClientID%>', heightModifier + 11);
					resizePageElement('<%=this.frameHardware.ClientID%>', heightModifier + 11);

					resizePageElement('divProfile', heightModifier + 11);
					resizePageElement('divOptions', heightModifier + 11);
					resizePageElement('divRoles', heightModifier + 11);
					resizePageElement('divCertifications', heightModifier + 11);
					resizePageElement('divHardware', heightModifier + 11);

				}
				catch (e) {
					var m = e.message;
				}
			}

			function Tab_click(tabName) {
				$('div', $('#divTabsContainer')).hide();

				switch (tabName.toUpperCase()) {
					case 'PROFILE':
						if ($('#<%=this.frameProfile.ClientID%>') && $('#<%=this.frameProfile.ClientID%>').attr('src') == "javascript:'';") {
							$('#<%=this.frameProfile.ClientID%>').attr('src', 'UserProfile_AddEdit.aspx' + window.location.search);
						}
						$('#divProfile').show();

						break;
					case 'OPTIONS':
						if ($('#<%=this.frameOptions.ClientID%>') && $('#<%=this.frameOptions.ClientID%>').attr('src') == "javascript:'';") {
							$('#<%=this.frameOptions.ClientID%>').attr('src', 'UserProfile_Options.aspx' + window.location.search);
						}
						$('#divOptions').show();

						break;
					case 'ROLES':
						if ($('#<%=this.frameRoles.ClientID%>') && $('#<%=this.frameRoles.ClientID%>').attr('src') == "javascript:'';") {
							$('#<%=this.frameRoles.ClientID%>').attr('src', 'UserProfile_Roles.aspx' + window.location.search);
						}
						$('#divRoles').show();

						break;
					case 'CERTIFICATIONS':
						if ($('#<%=this.frameCertifications.ClientID%>') && $('#<%=this.frameCertifications.ClientID%>').attr('src') == "javascript:'';") {
							var url = 'UserProfile_Certifications.aspx' + window.location.search;
							$('#<%=this.frameCertifications.ClientID%>').attr('src', url);
						}
						$('#divCertifications').show();

						break;
					case 'HARDWARE':
						if ($('#<%=this.frameHardware.ClientID%>') && $('#<%=this.frameHardware.ClientID%>').attr('src') == "javascript:'';") {
							var url = 'UserProfile_Hardware.aspx' + window.location.search;
							$('#<%=this.frameHardware.ClientID%>').attr('src', url);
						}
						$('#divHardware').show();

						break;
				}

				resizePage();
			}

			function buttonBackToGrid_click() {
				var contin = continueMove();
				if (contin == 'cancel') {
					return;
				}

				if (parent.ShowFrame) {
					parent.ShowFrame('Grid');
				}
				else if (closeWindow) {
					closeWindow();
				}
			}

		</script>

    </form>

	<script type="text/javascript" src="../Scripts/jquery-ui.js"></script>
	<script type="text/javascript">

		$(document).ready(function () {
			try {
				_pageURLs = new PageURLs();
				_userID = parseInt('<%=this.UserId %>');

				$(window).resize(resizePage);

				$('#imgRefresh').click(function () { refreshPage(); });
				$('#<%=this.buttonBackToGrid.ClientID %>').click(function () { buttonBackToGrid_click(); return false; });

				$('#divTabsContainer').tabs({
					heightStyle: "fill"
					, collapsible: false
					, active: 0
				});

				if ('<%=this.IsNew.ToString().ToUpper() %>' == 'TRUE') {
					$("#divTabsContainer").tabs("option", "disabled", [1,2,3,4]);
				}

				Tab_click('profile');

				resizePage();
			} catch (e) {
				var m = e.message;
			}
		});
	</script>
</body>
</html>
