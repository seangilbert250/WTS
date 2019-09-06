<%@ Page Title="" Language="C#" MasterPageFile="~/Grids.master" AutoEventWireup="true" CodeFile="WorkRequestGrid.aspx.cs" Inherits="WorkRequestGrid" Theme="Default" %>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" runat="Server">WTS - Work Request Grid</asp:Content>
<asp:Content ID="cpHead" ContentPlaceHolderID="head" runat="Server"></asp:Content>
<asp:Content ID="cpHeaderText" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="Server">Work Request (<span id="spanRowCount" runat="server">0</span>)</asp:Content>
<asp:Content ID="cpGridSetting" ContentPlaceHolderID="cphGridSettings" runat="Server">
	<table id="tableQuickFilters" cellpadding="0" cellspacing="0">
		<tr>
			<td style="padding-left:5px; display:none;">
				View:
				<asp:DropDownList ID="ddlView" runat="server" TabIndex="1" AppendDataBoundItems="true" Style="width: 85px;">
					<asp:ListItem Text="Enterprise" Value="0" />
					<asp:ListItem Text="My Data" Value="1" />
				</asp:DropDownList>
			</td>
		</tr>
	</table>
</asp:Content>
<asp:Content ID="cpPageContentInfo" ContentPlaceHolderID="cphPageContentInfo" runat="Server">
	<table cellpadding="0" cellspacing="0" style="float: right; padding-right: 2px;">
		<tr>
			<td>
				Show Archived<input type="checkbox" id="chkShowArchived" style="vertical-align:middle;" />
				&nbsp;
				<input type="button" id="buttonGoToWorkItem" value="Go to Work Item #" />
				<input type="text" id="txtWorkItem" name="GoTo" tabindex="2" maxlength="6" size="3" />
				&nbsp;
				<input type="button" id="buttonGoToWorkRequest" value="Go to Work Request #" />
				<input type="text" id="txtWorkRequest" name="GoTo" tabindex="3" maxlength="6" size="3" />
				&nbsp;
				<input type="button" id="buttonNewSR" value="Add SR" disabled="disabled" />
				<input type="button" id="buttonNewRequest" value="Add Work Request" disabled="disabled" />
				<input type="button" id="buttonNewWorkItem" value="Add Work Item" disabled="disabled" />
			</td>
			<%--<td style="padding: 0px; margin: 0px;">
				<iti_Tools_Sharp:Menu ID="menuReports" runat="server" Align="Right" ClientClickEvent="openMenuItem" Text="Reports <img id=imgReportsMenu alt=Expand Menu title=Show Reports Options src=Images/menuDown_Black.gif />" Button="true" Style="padding: 0px; margin: 0px; display: inline-block; float: left;"></iti_Tools_Sharp:Menu>
			</td>
			<td style="padding: 0px; margin: 0px;">
				<iti_Tools_Sharp:Menu ID="menuRelatedItems" runat="server" Align="Right" ClientClickEvent="openMenuItem" Text="Related Items <img id=imgRelatedItemsMenu alt=Expand Menu title=Show Related Items Options src=Images/menuDown_Black.gif />" Button="true" Style="padding: 0px; margin: 0px; display: inline-block; float: left;"></iti_Tools_Sharp:Menu>
			</td>--%>
		</tr>
	</table>
</asp:Content>
<asp:Content ID="cpBody" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
	<iti_Tools_Sharp:Grid ID="grdRequest" runat="server" AllowPaging="true" PageSize="50" FirstPageText="First" LastPageText="Last" PagerMode="NumericFirstLast" AllowResize="true"
		CssClass="grid" BodyCssClass="gridBody" SelectedRowCssClass="selectedRow" HeaderCssClass="gridHeader" PagerCssClass="gridPager" FooterCssClass="gridPager" AlternatingRowColor="#dfdfdf">
	</iti_Tools_Sharp:Grid>

	<asp:ScriptManager ID="scriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>


	<script id="jsAJAX" type="text/javascript">

		function on_error(result) {
			var resultText = 'An error occurred when communicating with the server';/*\n' +
					'readyState = ' + result.readyState + '\n' +
					'responseText = ' + result.responseText + '\n' +
					'status = ' + result.status + '\n' +
					'statusText = ' + result.statusText;*/

			MessageBox('save error:  \n' + resultText);
		}

	</script>
	
	<script id="jsEvents" type="text/javascript">
		var _pageUrls;
		var _selectedId = 0, _myView = 0;
		var _canViewRequest = false, _canViewSR = false, _canViewWorkItem = false;
		var _canEditRequest = false, _canEditSR = false, _canEditWorkItem = false;

		function refreshPage() {
			var qs = document.location.href;
			qs = editQueryStringValue(qs, 'RefData', 1);
			qs = editQueryStringValue(qs, 'View', $('#<%=this.ddlView.ClientID %> option:selected').val());
			qs = editQueryStringValue(qs, 'ShowArchived', ($('#chkShowArchived').is(':checked') ? 1 : 0));
			
			document.location.href = 'Loading.aspx?Page=' + qs;
		}

		function imgExport_click() {
			var url = window.location.href;
			url = editQueryStringValue(url, 'Export', true);

			window.open('Loading.aspx?Page=' + url);
		}

		function imgSort_click() {
			try {
				var sortableColumns = '<%=this.SortableColumns%>';
				while (sortableColumns.indexOf('<BR />') > -1) {
					sortableColumns = sortableColumns.replace("<BR />", ' ');
				}
				while (sortableColumns.indexOf('<BR/>') > -1) {
					sortableColumns = sortableColumns.replace("<BR/>", ' ');
				}
				while (sortableColumns.indexOf('<br />') > -1) {
					sortableColumns = sortableColumns.replace("<br />", ' ');
				}
				while (sortableColumns.indexOf('<br/>') > -1) {
					sortableColumns = sortableColumns.replace("<br/>", ' ');
				}

				while (sortableColumns.indexOf('...') > -1) {
					sortableColumns = sortableColumns.replace('...', '');
				}

				while (sortableColumns.indexOf('<BR>') > -1) {
					sortableColumns = sortableColumns.replace('<BR>', ' ');
				}
				while (sortableColumns.indexOf('<br>') > -1) {
					sortableColumns = sortableColumns.replace('<br>', ' ');
				}

				var sURL = 'SortOptions.aspx?sortColumns=' + escape(sortableColumns) + '&sortOrder=' + '<%=Request.QueryString["sortOrder"]%>';
				var nPopup = popupManager.AddPopupWindow("Sorter", "Sort Grid", sURL, 200, 400, "PopupWindow", this.self);
				if (nPopup) {
					nPopup.Open();
				}
			}
			catch (e) {
			}
		}

		function applySort(sortValue) {
			try {
				var pURL = window.location.href;
				pURL = editQueryStringValue(pURL, 'sortOrder', sortValue);
				pURL = editQueryStringValue(pURL, 'sortChanged', 'true');

				window.location.href = 'Loading.aspx?Page=' + pURL;
			}
			catch (e) {
			}
		}

		function formatGoTo(obj) {
			var text = $(obj).val();

			if (/[^0-9]|^0+(?!$)/g.test(text)) {
				$(obj).val(text.replace(/[^0-9]|^0+(?!$)/g, ''));
			}
		}

		function verifyItemExists(itemID, type) {
			PageMethods.ItemExists(itemID, type, function (result) { verifyItemExists_done(itemID, type, result); }, function (result) { verifyItemExists_done(itemID, type, false); });
		}

		function verifyItemExists_done(itemID, type, exists) {
			if (exists) {
				switch (type) {
					case 'Work Item':
						lbEditWorkItem_click(itemID);
						break;
					case 'Work Request':
						lbEditRequest_click(itemID);
						break;
				}
			}
			else {
				MessageBox('Could not find ' + type + ' # ' + itemID);
			}
		}

		function buttonGoToWorkItem_click() {
			var recordID = $('#txtWorkItem').val();

			if (recordID > 0) {
				verifyItemExists(recordID, 'Work Item');
			}
			else {
				MessageBox('Please enter a Work Item #.');
			}
		}

		function buttonGoToWorkRequest_click() {
			var requestID = $('#txtWorkRequest').val();

			if (requestID > 0) {
				verifyItemExists(requestID, 'Work Request');
			}
			else {
				MessageBox('Please enter a Work Request #.');
			}
		}

		function buttonNewSR_click() {

		}

		function buttonNewRequest_click() {
			if (parent.ShowFrameForWorkRequest) {
				parent.ShowFrameForWorkRequest(true, 0, 0, true);
			}
			else {
				var title = '', url = '';
				var h = 700, w = 1000;

				title = 'New Work Request';
				url = _pageUrls.Maintenance.WorkRequestEditParent;

				//open in a popup
				var openPopup = popupManager.AddPopupWindow('NewWorkRequest', title, 'Loading.aspx?Page=' + url, h, w, 'PopupWindow', this);
				if (openPopup) {
					openPopup.Open();
				}
			}
		}

		function buttonNewWorkItem_click() {
			var title = '', url = '';
			var h = 700, w = 1400;

			title = 'Work Item';
			url = _pageUrls.Maintenance.WorkItemEditParent;

			//open in a popup
			var openPopup = popupManager.AddPopupWindow('WorkItem', title, 'Loading.aspx?Page=' + url, h, w, 'PopupWindow', this);
			if (openPopup) {
				openPopup.Open();
			}
		}

		function lbEditWorkItem_click(recordId) {
			if (parent.ShowFrameForWorkloadItem) {
				parent.ShowFrameForWorkloadItem(false, recordId, recordId, true);
			}
			else {
				var title = '', url = '';
				var h = 700, w = 1400;

				title = 'Work Item - [' + recordId + ']';
				url = _pageUrls.Maintenance.WorkItemEditParent
					+ '?WorkItemID=' + recordId;

				//open in a popup
				var openPopup = popupManager.AddPopupWindow('WorkItem', title, 'Loading.aspx?Page=' + url, h, w, 'PopupWindow', this);
				if (openPopup) {
					openPopup.Open();
				}
			}
		}

		function lbEditRequest_click(requestID) {
			if (parent.ShowFrameForWorkRequest) {
				parent.ShowFrameForWorkRequest(false, requestID, requestID, true);
			}
			else {
				var title = '', url = '';
				var h = 700, w = 1000;

				title = 'Work Request - [' + requestID + ']';
				url = _pageUrls.Maintenance.WorkRequestEditParent
					+ '?WorkRequestID=' + requestID;

				//open in a popup
				var openPopup = popupManager.AddPopupWindow('WorkRequest', title, 'Loading.aspx?Page=' + url, h, w, 'PopupWindow', this);
				if (openPopup) {
					openPopup.Open();
				}
			}
		}

		function reports_click(actionType, report) {

		}
		function relatedItems_click(actionType, option) {

		}

		function row_click(row) {
			if ($(row).attr('itemID')) {
				_selectedId = $(row).attr('itemID');
			}
		}

		function openMenuItem(url) {
			if (url) {
				var nWindow;
				var w = 0, h = 0;
				var qsValue = '', strTitle = '';

				//get query string params
				for (var i = 0; i < url.split("&").length; i++) {
					qsValue = url.split("&")[i];

					if (qsValue.indexOf('width=') > -1) {
						qsValue = qsValue.substring(qsValue.indexOf('width='));
						w = qsValue.substring(qsValue.indexOf('width=') + 6);
					}

					if (qsValue.indexOf('height=') > -1) {
						qsValue = qsValue.substring(qsValue.indexOf('height='));
						h = qsValue.substring(qsValue.indexOf('height=') + 7);
					}

					if (qsValue.indexOf('Title=') > -1) {
						qsValue = qsValue.substring(qsValue.indexOf('Title='));
						strTitle = qsValue.substring(qsValue.indexOf('Title=') + 6);
					}
				}

				if (url.indexOf('.aspx?') > -1) {
					//open in a popup
					if (h > document.body.clientHeight - 40) {
						h = document.body.clientHeight - 40;
					}

					var openPopup = popupManager.AddPopupWindow('MenuItem', strTitle, 'Loading.aspx?Page=' + url, h, w, 'PopupWindow', this);
					if (openPopup) openPopup.Open();
				}
				else {

					//open in a window
					if (w > 0 && h > 0) {
						nWindow = window.open(url, 'nWindow', 'status=0,menubar=0,resizable=0,scrollbars=0,height=' + h + ',width=' + w);
					}
					else {
						nWindow = window.open(url);
					}

					if (!nWindow) MessageBox('A new window was not able to open. This could be because you have a popup blocker installed and running.\nEither disable the popup blocker, allow for this site, or hold down the Ctrl key when you click this button.');
				}
			}
		}


		function imgShowHideChildren_click(sender, direction, id) {
			try {
				if (id == "0" || id == "ALL") {
					var requestId = '0';

					$('[Name="img' + direction + '"]').each(function () {
						requestId = $(this).attr('requestId');
						if (requestId && +requestId > 0) {
							imgShowHideChildren_click(this, direction, requestId);
						}
					});
				}

				if (direction.toUpperCase() == "SHOW") {
					//show row/div with child grid frame
					//get frame and pass url(if necessary)
					var td;

					$(sender).closest('tr').each(function () {
						var currentRow = $(this);
						var row = $(currentRow).next('tr[Name="gridChild_' + id + '"]');
						var childType = 'WorkItem';
						$(row).show();

						td = $('td:eq(<%= this.DCC != null && this.DCC["WORKREQUESTID"] != null ? this.DCC["WORKREQUESTID"].Ordinal : 0%>)', row)[0];
						childType = $(row).attr('childType');
						loadChildGrid(td, id, childType);
					});
				}
				else {
					$('tr[Name="gridChild_' + id + '"]').hide();
				}

				$(sender).hide();
				$(sender).siblings().show();
			} catch (e) {
				var msg = e.message;
			}
		}

		function loadChildGrid(td, id, childType) {
			var url = 'Loading.aspx?Page=';

			if (childType == 'SR') {
				url += _pageUrls.Maintenance.WorkloadGrid_SRs;
			}
			else {
				url += _pageUrls.Maintenance.WorkloadGrid_WorkItems;
			}

			url += '?requestId=' + id;

			if (childType == 'WorkItem') url += '&ShowArchived=' + ($('#chkShowArchived').is(':checked') ? 1 : 0);

			$('iFrame', $(td)).each(function () {
				var src = $(this).attr('src');
				if (src == "javascript:''") {
					$(this).attr('src', url);
				}

				$(this).show();
			});
		}

		function resizeFrames() {
			var frame;
			var fPageHeight = 0;

			$('iFrame').each(function () {
				frame = $(this)[0];
				fPageHeight = 0;

				if (frame.contentWindow
					&& frame.contentWindow.document
					&& frame.contentWindow.document.body
					&& frame.contentWindow.document.body.offsetHeight) {
					fPageHeight = frame.contentWindow.document.body.offsetHeight;
				}
				frame.style.height = fPageHeight + 'px';
			});

			resizePageElement($('#<%=this.grdRequest.ClientID %>').attr('id'), 0);
			//<%=this.grdRequest.ClientID %>.RedrawGrid();
		}

	</script>

	<script id="jsInit" type="text/javascript">

		function initVariables() {
			try {
				_pageUrls = new PageURLs();

				_canEditRequest = ('<%=this.CanEditRequest.ToString().ToUpper()%>' == 'TRUE');
				_canEditSR = ('<%=this.CanEditSR.ToString().ToUpper()%>' == 'TRUE');
				_canEditWorkItem = ('<%=this.CanEditWorkItem.ToString().ToUpper()%>' == 'TRUE');
				_canViewRequest = (_canEditRequest || ('<%=this.CanViewRequest.ToString().ToUpper()%>' == 'TRUE'));
				_canViewSR = (_canEditSR || ('<%=this.CanViewSR.ToString().ToUpper()%>' == 'TRUE'));
				_canViewWorkItem = (_canEditWorkItem || ('<%=this.CanViewWorkItem.ToString().ToUpper()%>' == 'TRUE'));

			} catch (e) {

			}
		}

		$(document).ready(function () {

			initVariables();

			$(':input').css('font-family', 'Arial');
			$(':input').css('font-size', '12px');

			$('#imgReport').hide();
			$('#imgExport').click(function () { imgExport_click(); });
			$('#imgRefresh').click(function () { refreshPage(); });
			$('#imgSort').click(function () { imgSort_click(); });
			$('#buttonGoToWorkItem').click(function (event) { buttonGoToWorkItem_click(); return false; });
			$('#buttonGoToWorkRequest').click(function (event) { buttonGoToWorkRequest_click(); return false; });

			if (_canEditSR) {
				$('#buttonNewSR').attr('disabled', false);
				$('#buttonNewSR').click(function (event) { buttonNewSR_click(); return false; });
			}
			if (_canEditRequest) {
				$('#buttonNewRequest').attr('disabled', false);
				$('#buttonNewRequest').click(function (event) { buttonNewRequest_click(); return false; });
			}
			if (_canEditWorkItem) {
				$('#buttonNewWorkItem').attr('disabled', false);
				$('#buttonNewWorkItem').click(function (event) { buttonNewWorkItem_click(); return false; });
			}
			$('.gridBody').click(function (event) { row_click(this); });
			$('.selectedRow').click(function (event) { row_click(this); });

			$('#chkShowArchived').click(function () { $('#imgRefresh').trigger('click'); });
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

			$('#chkShowArchived').prop('checked', '<%=this._showArchived %>' == '1');
		});
	</script>
</asp:Content>