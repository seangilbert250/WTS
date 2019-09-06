<%@ Page Title="" Language="C#" MasterPageFile="~/EditTabs.master" AutoEventWireup="true" CodeFile="WorkItem_Attachments.aspx.cs" Inherits="WorkItem_Attachments" Theme="Default" %>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" runat="Server">Work Item Attachments</asp:Content>
<asp:Content ID="cpHead" ContentPlaceHolderID="head" runat="Server"></asp:Content>
<asp:Content ID="cpHeaderImage" ContentPlaceHolderID="ContentPlaceHolderHeaderImage" runat="Server"></asp:Content>
<asp:Content ID="cpHeaderText" ContentPlaceHolderID="ContentPlaceHolderHeaderText" runat="Server">Attachments (<span id="spanRowCount" runat="server">0</span>)</asp:Content>
<asp:Content ID="cpHeaderMisc" ContentPlaceHolderID="ContentPlaceHolderHeaderMisc" runat="Server">
</asp:Content>
<asp:Content ID="cpHeaderButtons" ContentPlaceHolderID="ContentPlaceHolderHeaderButtons" runat="Server">
	<table cellpadding="0" cellspacing="0" style="float: right;">
		<tr>
			<td>
				<input type="button" id="btnAdd" value="Add" disabled="disabled" />
				<input type="button" id="btnDelete" value="Delete" disabled="disabled" />
			</td>
		</tr>
	</table>
</asp:Content>
<asp:Content ID="cpBody" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
	<asp:ScriptManager ID="scriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>

	<iti_Tools_Sharp:Grid ID="gridAttachments" runat="server" AllowPaging="true" PageSize="30" FirstPageText="First" LastPageText="Last" PagerMode="NumericFirstLast" AllowResize="true"
		CssClass="grid" BodyCssClass="gridBody" SelectedRowCssClass="selectedRow" HeaderCssClass="gridHeader" PagerCssClass="gridPager" FooterCssClass="gridPager" AlternatingRowColor="#dfdfdf">
	</iti_Tools_Sharp:Grid>

	<script type="text/javascript">
		var _pageUrls;
		var _canEdit = false;
		var _selectedId = 0, _workItemID = 0;
		var _idxID = 0, _idxFileName = 0, _idxAttachmentType = 0, _idxTitle = 0;

		function refreshGrid() {
			document.location.href = 'Loading.aspx?Page=' + document.location.href;
		}

		function buttonAdd_onclick() {
			try {
				var url = 'Attachment_Edit.aspx' 
					+ window.location.search 
					+ '&random=' + new Date().getTime();

				var filterPopup = popupManager.AddPopupWindow('AddNewAttachment', 'Add New Attachment'
					, 'Loading.aspx?Page=' + url, 300, 600, 'PopupWindow', window.self);
				if (filterPopup) {
					filterPopup.Open();
				}
			}
			catch (e) { }
		}
		
		function lbEditAttachment_click(attachmentID) {
			var url = '';
			_selectedId == attachmentID;

			try {
				var fileName = <%=this.gridAttachments.ClientID %>.SelectedRow.cells[_idxFileName].innerText;

				var selAttachmentType = <%=this.gridAttachments.ClientID %>.SelectedRow.cells[_idxAttachmentType].innerText;
				var selDescription = <%=this.gridAttachments.ClientID %>.SelectedRow.cells[_idxTitle].innerText;

				url = 'Attachment_Edit.aspx' 
					+ window.location.search 
					+ '&edit=1' 
					+ '&attachmentID=' + attachmentID 
					+ '&fileName=' + fileName 
					+ '&attachmentType=' + selAttachmentType 
					+ '&description=' + selDescription 
					+ '&random=' + new Date().getTime();

				var popup = popupManager.AddPopupWindow('EditAttachment', 'Edit Attachment - [' + fileName + ']'
					, 'Loading.aspx?Page=' + url, 200, 550, 'PopupWindow', window.self);
				
				if (popup) {
					popup.Open();
				}
			} catch (e) { }
		}
		
		function buttonDownload_onclick(attachmentId) {
			try{
				window.open('Download_Attachment.aspx?attachmentID=' + attachmentId);
            }
			catch(e){ }
		}
		function enableButtons() {
			$('#btnDelete').attr('disabled', false);
		}
		
		function btnDelete_click() {
			if (_selectedId == null || _selectedId == 0) {
				MessageBox('You must select a file to delete first.');
			}
			else{
				var selectedFileName = $(<%=this.gridAttachments.ClientID %>.SelectedRow).find('td:eq(' + _idxFileName + ')').text();
				
				if (confirm('Are you sure you want to delete: ' + selectedFileName + '?')) {
					PageMethods.DeleteAttachment(_workItemID, _selectedId
						, deleteAttachment_Done, OnError);
				}
			}
		}
		function deleteAttachment_Done(result) {
			var deleted = false;
			var id = '', errorMsg = '';

			try {
				var obj = jQuery.parseJSON(result);

				if (obj) {
					if (obj.deleted && obj.deleted.toUpperCase() == 'TRUE') {
						deleted = true;
					}
					if (obj.id) {
						id = obj.id;
					}
					if (obj.error) {
						errorMsg = obj.error;
					}
				}
				
				if (deleted) {
					MessageBox('Attachment has been deleted.');
					refreshGrid();
				}
				else {
					MessageBox('Failed to delete attachment. \n' + errorMsg);
				}
			} catch (e) {
				
			}
		}
		function OnError(result) {
			var resultText = 'An error occurred when communicating with the server';/*\n' +
                    'readyState = ' + result.readyState + '\n' +
                    'responseText = ' + result.responseText + '\n' +
                    'status = ' + result.status + '\n' +
                    'statusText = ' + result.statusText;*/

			MessageBox('save error:  \n' + resultText);
		}
		
		function row_click(row) {
			if ($(row).attr('AttachmentID')) {
				_selectedId = $(row).attr('AttachmentID');
			}

			if(_canEdit) {
				$('#btnDelete').attr('disabled', false);
				$('#btnDelete').prop('disabled', false);
			}
		}

		$(document).ready(function () {
			$('#imgReport').hide();
			$('#imgSort').hide();
			$('#imgExport').hide();
			$('#imgRefresh').click(function () { refreshGrid(); });
			$('#btnAdd').click(function (event) { buttonAdd_onclick(); });
			$('#btnDelete').click(function (event) { btnDelete_click(); });
			$('#btnEdit').click(function (event) { buttonEdit_onclick(); });

			$('.gridBody').click(function (event) { row_click(this); });
			$('.selectedRow').click(function (event) { row_click(this); });

			try {
				_canEdit = ('<%=this.CanEdit.ToString().ToUpper() %>' == 'TRUE');
				if(_canEdit) {
					$('#btnAdd').attr('disabled', false);
					$('#btnAdd').prop('disabled', false);
				}

				_workItemID = parseInt('<%=this.WorkItemID %>');
				_idxID = '<%=this.dc == null ? 0 : this.dc["ATTACHMENTID"].Ordinal%>';
				_idxFileName = '<%=this.dc == null ? 0 : this.dc["FILENAME"].Ordinal%>';
				_idxAttachmentType = '<%=this.dc == null ? 0 : this.dc["ATTACHMENTTYPE"].Ordinal%>';
				_idxTitle = '<%=this.dc == null ? 0 : this.dc["TITLE"].Ordinal%>';
			} catch (e) { }
		});
	</script>
</asp:Content>

