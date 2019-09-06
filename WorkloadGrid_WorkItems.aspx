﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="WorkloadGrid_WorkItems.aspx.cs" Inherits="WorkloadGrid_WorkItems" Theme="Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>Workload Tasks</title>
	<script type="text/javascript" src="Scripts/shell.js"></script>
	<script type="text/javascript" src="Scripts/common.js"></script>
	<script type="text/javascript" src="Scripts/jquery-1.11.2.js"></script>
</head>
<body>
	<form id="form1" runat="server">
		<div id="divContents">
			<asp:GridView runat="server" ID="gridWork" Style="width: 100%; border: none;" GridLines="None" AllowPaging="true" AllowSorting="false" PageSize="20" CellPadding="0" CellSpacing="0"
				CssClass="grid" RowStyle-CssClass="gridBody" HeaderStyle-CssClass="gridHeader" SelectedRowStyle-CssClass="selecteddrilldown" FooterStyle-CssClass="gridPager" PagerStyle-CssClass="gridPager">
			</asp:GridView>
		</div>

		<script type="text/javascript">
			var _pageUrls;
			var _requestId = 0;
			var _idxDescription = 0;
			var _selectedAssigned = '<%=SelectedAssigned%>';
			var _selectedStatuses = '<%=SelectedStatuses%>';
			function refreshPage() {
				document.location.href = 'Loading.aspx?Page=' + document.location.href;
			}

			function resizeFrame() {
				if (parent.resizeFrames) {
					parent.resizeFrames();
				}
			}

			function imgShowHideChildren_click(sender, direction, id) {
				try {
					if (id == "0") {
						var itemId = '0';

						$('[Name="img' + direction + '"]').each(function () {
							itemId = $(this).attr('requestId');
							if (itemId && +itemId > 0) {
								imgShowHideChildren_click(this, direction, itemId);
							}
						});
					}
					else {

						if (direction.toUpperCase() == "SHOW") {
							//show row/div with child grid frame
							//get frame and pass url(if necessary)
							var td;
							$('tr[Name="gridChild_' + id + '"]').each(function () {
								var row = $(this);
								$(row).show();

								td = $('td:eq(' + _idxDescription + ')', row)[0];
								loadChildGrid(td, id);
							});
						}
						else {
							$('tr[Name="gridChild_' + id + '"]').hide();
						}
					}

					$(sender).hide();
					$(sender).siblings().show();
				} catch (e) {
					var msg = e.message;
				}
			}

			function loadChildGrid(td, id) {
				var url = 'Loading.aspx?Page='
						+ _pageUrls.Maintenance.WorkItemGrid_Tasks;

				url += '?workItemID=' + id;
				url += '&ShowArchived=' + '<%=_showArchived %>';

				$('iFrame', $(td)).each(function () {
					var src = $(this).attr('src');
					if (src == "javascript:''") {
						$(this).attr('src', url);
					}

					$(this).show();
					resizeFrames();
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

				resizePageElement($('#<%=this.gridWork.ClientID %>').attr('id'), 0);
				//<%=this.gridWork.ClientID %>.RedrawGrid();
			}

		</script>

		<script id="jsEvents" type="text/javascript">

			function lbEditWorkItem_click(recordId) {
				if (parent.parent.ShowFrameForWorkloadItem) {
				    parent.parent.ShowFrameForWorkloadItem(false, recordId, recordId, true, '', _selectedStatuses, _selectedAssigned);
				}
				else {
					var title = '', url = '';
					var h = 700, w = 1400;

					title = 'Workload Task - [' + recordId + ']';
					url = _pageUrls.Maintenance.WorkItemEditParent
						+ '?WorkItemID=' + recordId;

					//open in a popup
					var openPopup = popupManager.AddPopupWindow('WorkloadTask', title, 'Loading.aspx?Page=' + url, h, w, 'PopupWindow', this);
					if (openPopup) {
						openPopup.Open();
					}
				}
			}

		</script>

	</form>

	<script id="jsInit" type="text/javascript">

		$(document).ready(function () {
			_pageUrls = new PageURLs();
			try {
				_idxDescription = '<%=this.DCC == null ? 0 : this.DCC["DESCRIPTION"].Ordinal%>';
			} catch (e) { }
			
			$(window).resize(resizeFrame);

			resizeFrame();
		});
	</script>
</body>
</html>
