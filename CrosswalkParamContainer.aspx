<%@ Page Title="" Language="C#" MasterPageFile="~/Popup.master" AutoEventWireup="true" CodeFile="CrosswalkParamContainer.aspx.cs" Inherits="CrosswalkParamContainer" Theme="Default" %>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" Runat="Server">Crosswalk Parameters</asp:Content>
<asp:Content ID="cpHead" ContentPlaceHolderID="head" Runat="Server">
	<script type="text/javascript" src="scripts/tablednd.js"></script>
	<link rel="stylesheet" href="Styles/jquery-ui.css" />
	<style type="text/css">
		.draggableRow {
			border-top:solid 1px gray;
			border-bottom:solid 1px gray;
			padding:2px;
		}
		.draggableRowLock {
			border-top:solid 1px gray;
			border-bottom:solid 1px gray;
			padding:2px;
			width:16px;
		}
		.draggableRowChild {
			border-top:solid 1px gainsboro;
			border-bottom:solid 1px gainsboro;
			border-left:solid 1px gainsboro;
			padding:2px;
		}
		.draggableRowChildLock {
			border-top:solid 1px gainsboro;
			border-bottom:solid 1px gainsboro;
			border-right:solid 1px gainsboro;
			padding:2px;
			width:16px;
		}
	</style>
</asp:Content>
<asp:Content ID="cpBody" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
	<asp:ScriptManager ID="scriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>

	<div id="divPage" class="pageContainer">
		<div id="divTabsContainer" class="mainPageContainer">
			<ul id="Tabs" runat="server">
				<li><a href="#divBasic" onclick="Tab_click('basic');">Parameters</a></li>
				<li><a href="#divAdvanced" onclick="Tab_click('advanced');">Advanced Parameters</a></li>
			</ul>
			<div id="divBasic">
				<table id="tableBasicOptions" class="attributes" style="width: 99%; text-align: left; vertical-align: top; padding: 10px;" cellpadding="0" cellspacing="0">
					<tr class="attributesRow">
						<td class="attributesLabel" style="width:60px;">Grid View:</td>
						<td class="attributesValue">
							<asp:DropDownList ID="ddlView" runat="server" Style="font-size: 12px; padding-left: 0px;" Enabled="true" AppendDataBoundItems="false"></asp:DropDownList>
							<img id="imgSaveView" src="Images/Icons/disk.png" title="Save View" alt="Save View" style="cursor: pointer;" />
							<img id="imgDeleteView" src="Images/Icons/delete.png" title="Delete View" alt="Delete View" style="cursor: pointer;" />
						</td>
					</tr>
				</table>
			</div>
			<div id="divAdvanced">
				<table id="tableAdvancedOptions" style="width:99%; vertical-align:top; text-align:left; padding:5px;" cellpadding="0" cellspacing="0">
					<tr>
						<td style="padding:5px;">Grid View (Advanced):</td>
					</tr>
					<tr>
						<td style="border: 1px solid black; padding:10px;">
							<asp:DropDownList ID="ddlViewAdv" runat="server" Style="font-size: 12px; padding-left: 0px;" Enabled="true" AppendDataBoundItems="false"></asp:DropDownList>
							<img id="imgSaveViewAdv" src="Images/Icons/disk.png" title="Save View" alt="Save View" style="cursor: pointer;" />
							<img id="imgDeleteViewAdv" src="Images/Icons/delete.png" title="Delete View" alt="Delete View" style="cursor: pointer;" />
						
                        
				            <input type="checkbox" id="chkUseColumnOrdering" style="vertical-align: middle; visibility:hidden" />
                            <label for="chkUseColumnOrdering" style="vertical-align: middle; visibility:hidden">Apply Column Ordering (Known Issues)</label>


                        </td>
					</tr>
					<tr>
						<td style="padding:5px;">Rollup/Sort Options:</td>
					</tr>
					<tr>
						<td style="border: 1px solid black; padding:5px;">
							<table id="tableAttributes" cellpadding="0" cellspacing="0" style="width: 100%; vertical-align: top; text-align: left; padding: 5px 0px 5px 0px;">
								<tr class="attributesRow">
									<td class="attributesLabel" style="width:60px;">Rollup By:</td>
									<td class="attributesValue" style="width: 100px;">
										<asp:DropDownList runat="server" ID="ddlRollupGroup">
											<asp:ListItem Text="Priority" Value="Priority" />
											<asp:ListItem Text="Status" Value="Status" />
										</asp:DropDownList>
									</td>
									<td class="attributesLabel" style="width:105px;">Default Task Sort:</td>
									<td class="attributesValue">
										<asp:DropDownList runat="server" ID="ddlDefaultTaskSort">
											<asp:ListItem Text="Primary Tech. Rank" Value="Tech" />
											<asp:ListItem Text="Primary Bus. Rank" Value="Bus" />
											<asp:ListItem Text="Secondary Tech. Rank" Value="Secondary" />
<%--											<asp:ListItem Text="Secondary Bus. Rank" Value="Secondary" />--%>
										</asp:DropDownList>
									</td>
								</tr>
							</table>
						</td>
					</tr>
					<tr>
						<td id="tdColumnSelection" style="padding-top:10px;">
							<iframe id="frameColumns" name="frameDetails" class="contentFrame" runat="server" src="javascript:'';" scrolling="no" frameborder="0" style="width: 100%; height: 100%; margin: 0px; padding: 0px;" title="">Crosswalk Columns</iframe>
						</td>
					</tr>
				</table>
			</div>
		</div>
	</div>
	<div id="divFooter" class="pageContentHeader" style="width:100%; position:absolute; bottom:0px; left:0px;">
		<table id="tableFooter" style="width:100%; vertical-align:bottom;" cellpadding="0" cellspacing="0">
			<tr>
				<td style="text-align:right; height:30px; padding: 2px 5px 2px 5px;">
					<input type="button" id="buttonGetData" value="Get Data" />
				</td>
			</tr>
		</table>
	</div>
	<div id="divDimmer" style="position: absolute; filter: alpha(opacity = 60); width: 100%; display: none; background: gray; height: 100%; top: 0px; left: 0px; opacity: 0.6;"></div>
	<div id="divViewName" style="width: 260px; background-color: white; z-index: 999; display: none;">
		<table style="width: 100%;">
			<tr>
				<td class="pageContentInfo">
					Grid View Name:
				</td>
			</tr>
			<tr>
				<td>
					<asp:TextBox ID="txtViewName" runat="server" MaxLength="50" Width="250"></asp:TextBox>
				</td>
			</tr>
			<tr>
				<td>
					<input type="checkbox" id="chkProcessView" style="vertical-align: middle;" />
					<label for="chkProcessView" style="vertical-align: middle;">Process View</label>
				</td>
			</tr>
			<tr>
				<td>
					<input type="button" id="buttonSaveView" value="Save" />&nbsp;<input type="button" id="buttonCancelView" value="Cancel" />
				</td>
			</tr>
		</table>
	</div>

	<script id="jsVariables" type="text/javascript">
		var _pageUrls;
		var _selectedColumnOrder = '', _defaultColumnOrder = ''
			, _selectedColumnOrder_Child = '', _defaultColumnOrder_Child = ''
			, _selectedRollupGroup = ''
			, _selectedSortType = 'Tech';
		var _columnsChanged = false;

		var _UseColumnOrdering = false;  // columnOrderPref on c# side

	</script>
	<script id="jsEvents" type="text/javascript">
		
		function closePage() {
			if (closeWindow) {
				closeWindow();
			}
			else {
				window.close();
			}
		}

		function refreshPage() {
			var qs = document.location.href;

			qs = editQueryStringValue(qs, 'Tab', ($('#divAdvanced').is(':visible') ? 1 : 0));

			qs = editQueryStringValue(qs, 'ddlChanged', 'yes');

			document.location.href = 'Loading.aspx?Page=' + qs;
		}

		function resizePage() {
			try {
				var heightModifier = 0;
				heightModifier += 10;

				resizePageElement('divPage', heightModifier + 2);
				resizePageElement('divTabsContainer', heightModifier + 3);

				resizePageElement('<%=this.frameColumns.ClientID%>', heightModifier + 19);

				resizePageElement('divBasic', heightModifier + 11);
				resizePageElement('divAdvanced', heightModifier + 11);

			}
			catch (e) {
				var m = e.message;
			}
		}

		function Tab_click(tabName) {
			$('div', $('#divTabsContainer')).hide();

			switch (tabName.toUpperCase()) {
				case 'BASIC':
					$('#divBasic').show();

					break;
				case 'ADVANCED':
					if ($('#<%=this.frameColumns.ClientID%>') && $('#<%=this.frameColumns.ClientID%>').attr('src') == "javascript:'';") {
						$('#<%=this.frameColumns.ClientID%>').attr('src', 'Grid_Order.aspx' + window.location.search);
					}
					$('#divAdvanced').show();
					break;
				}
				resizePage();
		}

		function AdvancedTab_click(tabName) {
			$('div', $('#tdAdvancedTabsContainer')).hide();

			switch (tabName.toUpperCase()) {
				case 'PARENT':
					$('#divParent').show();

					break;
				case 'CHILD':
					$('#divChild').show();

					break;
			}

			resizePage();
		}

		function getSelectedColumnOrder(blnDefault) {
			try {
				if (blnDefault) {
					return _defaultColumnOrder;
				}
				else {
					return _selectedColumnOrder;
				}
			}
			catch (e) {
				return "";
			}
		}

		function getSelectedColumnOrderSub(blnDefault) {
			try {
				if (blnDefault) {
					return _defaultColumnOrder_Child;
				}
				else {
					return _selectedColumnOrder_Child;
				}
			}
			catch (e) {
				return "";
			}
		}

		function updateColumnOrder(columnOrder, columnOrderSub, blnSavedView) {
			try {
				var gridView = $('#<%=this.ddlView.ClientID %> option:selected').text();
				var rollupGroup = blnSavedView ? $('#<%=this.ddlView.ClientID %> option:selected').attr('Tier1RollupGroup') : $('#<%=this.ddlRollupGroup.ClientID %>').val();

				_selectedColumnOrder = columnOrder;
				_selectedColumnOrder_Child = columnOrderSub;
				var parentColumns = parseColumnString(_selectedColumnOrder);
				var childColumns = parseColumnString(_selectedColumnOrder_Child);
				_selectedSortType = blnSavedView ? $('#<%=this.ddlView.ClientID %> option:selected').attr('DefaultSortType') : $('#<%=this.ddlDefaultTaskSort.ClientID %>').val();

				PageMethods.SetCrosswalkColumnSettings(gridView, rollupGroup
					, parentColumns, _selectedColumnOrder
					, childColumns, _selectedColumnOrder_Child
					, _selectedSortType
					, function (result) { updateColumnOrder_Done(result, blnSavedView) }, on_error);

				_columnsChanged = true;

				if (opener.updateColumnOrder) {
					opener.updateColumnOrder(_selectedColumnOrder
						, _selectedColumnOrder_Child
						, rollupGroup
						, _selectedSortType);
				}
			}
			catch (e) {
				MessageBox('updateColumnOrder:\n' + e.number + '\n' + e.message);
			}
		}

		function updateColumnOrder_Done(result, blnSavedView) {
			var msg = '';

			if ($('#<%=this.frameColumns.ClientID%>') && blnSavedView) {
				refreshPage();
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
		function parseColumnString(columnOrder) {
			var columnObjects = [];
			var columnParts = [];
			var columnNames = [];
			var show = false;

			//dbname|displayname|visible|orderable|groupname|viewable
			columnObjects = columnOrder.trim().split('~');
			for (var i = 0; i < columnObjects.length; i++) {
				if (columnObjects[i].trim() == '') {
					continue;
				}

				columnParts = columnObjects[i].trim().split('|');
				if (columnParts.length >= 3) {
					if (columnParts[2] == 'true' && columnParts[1].trim() != '' && columnParts[1].trim() != '&nbsp;') {
						columnNames.push(columnParts[1].replace(' ', ''));
					}
				}
			}

			return columnNames.join(',');
		}

		function ddlView_change() {
			var columnOrder = $('#<%=this.ddlView.ClientID %> option:selected').attr('Tier1ColumnOrder');
			var columnOrderSub = $('#<%=this.ddlView.ClientID %> option:selected').attr('Tier2ColumnOrder');

			updateColumnOrder(columnOrder, columnOrderSub, true);
		}

		function ddlViewAdv_change() {
			$('#<%=this.ddlView.ClientID %>').val($('#<%=this.ddlViewAdv.ClientID %>').val());
			$('#<%=this.ddlView.ClientID %>').trigger('change');
		}

		function imgSaveView_click(obj) {
			$('#divDimmer').show();
			var pos = $(obj).position();
			var width = $('#divViewName').outerWidth();
			$('#divViewName').css({
				position: "absolute",
				top: pos.top + "px",
				left: (pos.left) + "px"
			}).slideDown(function () { $('#<%=this.txtViewName.ClientID %>').focus(); });
		}

		function buttonSaveView_click() {
			var viewName = $('#<%=this.txtViewName.ClientID %>').val().trim().toUpperCase();
			if (viewName == 'DEFAULT') {
				MessageBox('You cannot save with grid view name Default.');
			}
			else if (viewName != '') {
				var exists =
					$('#<%=this.ddlView.ClientID %> option').filter(function () {
						return $(this).text().trim().toUpperCase() === viewName;
					}).length > 0;

				if (!exists) {
					confirmViewName('YES');
				}
				else {
					var myView = $('#<%=this.ddlView.ClientID %> option').filter(function () {
						return $(this).text().trim().toUpperCase() === viewName;
					}).first().attr('MyView');

					if (myView == '1') {
						QuestionBox('Confirm Grid View Name', 'Grid view name already exists. Would you like to overwrite?', 'Yes,No', 'confirmViewName', 300, 300, window.self);
					}
					else {
						MessageBox('Grid view name already exists. You cannot overwrite grid view name which you did not create.');
					}
				}
			}
			else {
				MessageBox('Please enter a grid view name.');
			}
		}

		function confirmViewName(answer) {
			if ($.trim(answer).toUpperCase() == 'YES') {
				var gridViewID = 0;
				var viewName = $('#<%=this.txtViewName.ClientID %>').val().trim();
				var exists =
					$('#<%=this.ddlView.ClientID %> option').filter(function () {
						return $(this).text().trim().toUpperCase() === viewName.toUpperCase();
					}).length > 0;
				
				if (exists) {
					gridViewID = 
						$('#<%=this.ddlView.ClientID %> option').filter(function () {
							return $(this).text().trim().toUpperCase() === viewName.toUpperCase();
						}).val();
				}

				var parentColumns = parseColumnString(_selectedColumnOrder);
				var childColumns = parseColumnString(_selectedColumnOrder_Child);
				PageMethods.SaveView(gridViewID, viewName, ($('#chkProcessView').is(':checked') ? 1 : 0), $('#<%=this.ddlRollupGroup.ClientID %>').val(), parentColumns, _selectedColumnOrder, childColumns, _selectedColumnOrder_Child, $('#<%=this.ddlDefaultTaskSort.ClientID %>').val(), btnSaveView_Done, on_error);
			}
		}

		function btnSaveView_Done(result) {
			refreshPage();
		}

		function imgDeleteView_click() {
			if ($.trim($('#<%=this.ddlView.ClientID %> option:selected').text()).toUpperCase() == "DEFAULT") {
				MessageBox('You cannot delete the Default grid view.');
			}
			else if ($('#<%=this.ddlView.ClientID %> option:selected').attr('MyView') != '1') {
				MessageBox('You cannot delete a grid view which was not created by you.');
			}
			else {
				PageMethods.DeleteView($('#<%=this.ddlView.ClientID %>').val(), imgDeleteView_Done, on_error);
			}
		}

		function imgDeleteView_Done(result) {
			refreshPage();
		}

		function ddlRollupGroup_change() {
			var rollupGroup = $('#<%=this.ddlRollupGroup.ClientID %> option:selected').val();

			if (rollupGroup != _selectedRollupGroup) {
				_columnsChanged = true;
				_selectedRollupGroup = rollupGroup;

				if (opener.updateRollupColumns) {
					opener.updateRollupColumns(rollupGroup);
				}
			}

			updateColumnOrder(_selectedColumnOrder, _selectedColumnOrder_Child, false);
		}

		function ddlDefaultTaskSort_change() {
			var sortType = $('#<%=this.ddlDefaultTaskSort.ClientID %>').val();

			if (sortType != _selectedSortType) {
				_columnsChanged = true;
				_selectedSortType = sortType;

				if (opener.updateDefaultRankSortType) {
					opener.updateDefaultRankSortType(sortType);
				}
			}

			updateColumnOrder(_selectedColumnOrder, _selectedColumnOrder_Child, false);
		}

		function buttonGetData_click() {
			if (opener.ApplySettings) {
				opener.ApplySettings();
			}

			setTimeout(closePage, 500);
		}

		function chkUseColumnOrdering_change() {
		    _UseColumnOrdering = $('#chkUseColumnOrdering').prop('checked');
		    PageMethods.SaveColumnOrderingPref(_UseColumnOrdering);
		}

	</script>

	<script id="jsInit" type="text/javascript">

		function initVariables() {
			try {
				_pageUrls = new PageURLs();
				_selectedColumnOrder = '<%=this.SelectedColumnOrder %>';
				_defaultColumnOrder = '<%=this.DefaultColumnOrder %>';
				_selectedColumnOrder_Child = '<%=this.SelectedColumnOrder_Child %>';
				_defaultColumnOrder_Child = '<%=this.DefaultColumnOrder_Child %>';
				_selectedRollupGroup = '<%=this.RollupGroup %>';
			    _selectedSortType = '<%=this.DefaultSortType %>';

			    _UseColumnOrdering = '<%=this.columnOrderPref %>';
			    if (_UseColumnOrdering == "True")
			        $('#chkUseColumnOrdering').attr('checked', true);
                else
			        $('#chkUseColumnOrdering').attr('checked', false);


			} catch (e) {

			}	
		}

		$(document).ready(function () {
			try {
				initVariables();

				var selectedView = <%=ddlView.SelectedValue%>;
                var selectedViewAdvanced = <%=ddlViewAdv.SelectedValue%>;

				$(window).resize(resizePage);

				$('#<%=this.ddlView.ClientID %>').on("change", function () { ddlView_change(); return false; });
				$('#<%=this.ddlViewAdv.ClientID %>').on("change", function () { ddlViewAdv_change(); return false; });
				$('#imgSaveView, #imgSaveViewAdv').click(function () { imgSaveView_click(this); });
				$('#<%=this.txtViewName.ClientID %>').on("keypress", function (e) { if (e.which == 13) { $('#buttonSaveView').trigger('click'); return false; } });
				$('#buttonSaveView').click(function () { buttonSaveView_click(); return false; });
				$('#buttonCancelView').click(function () { $('#divViewName').slideUp(function () { $('#divDimmer').hide(); }); return false; });
				$('#imgDeleteView, #imgDeleteViewAdv').click(function () { imgDeleteView_click(); });
				$('#<%=this.ddlRollupGroup.ClientID %>').on("change", function () { ddlRollupGroup_change(); return false; });
				$('#<%=this.ddlDefaultTaskSort.ClientID %>').on("change", function () { ddlDefaultTaskSort_change(); return false; });
				$('#imgRefresh').click(function () { refreshPage(); });
				$('#buttonGetData').click(function () { buttonGetData_click(); return false; });
				$(document.body).bind('onbeforeunload', function () { buttonGetData_click(); });

				$('#chkUseColumnOrdering').change(function () { chkUseColumnOrdering_change(); return false; });

				var tab = parseInt('<%=ActiveTab %>');
				$('#divTabsContainer').tabs({
					heightStyle: "fill"
					, collapsible: false
					, active: tab
				});

				$("#<%=this.ddlView.ClientID %> option[OptionGroup='Process Views']").wrapAll("<optgroup label='Process Views'>");
				$("#<%=this.ddlView.ClientID %> option[OptionGroup='Custom Views']").wrapAll("<optgroup label='Custom Views'>");
				$("#<%=this.ddlViewAdv.ClientID %> option[OptionGroup='Process Views']").wrapAll("<optgroup label='Process Views'>");
				$("#<%=this.ddlViewAdv.ClientID %> option[OptionGroup='Custom Views']").wrapAll("<optgroup label='Custom Views'>");

				if (tab == 1) Tab_click('Advanced');

				resizePage();

				$('#<%=this.ddlView.ClientID %>').val(selectedView); //I don't know why this needs to be here. The page refreshes when the ddlviews are changed. For some reason the selected value is changed to an option other than what was selected. The effect of selecting that value is correct in that the selected columns change to the correct values, but the drop down list doesn't reads "dev standup" every time, for no apparant reason. The problem happens somewhere in the tab change code, but I couldn't figure it out so this hack fixes it. if and when you do figure it out, feel free to leave a comment explaining how you did it. I am curious. 
			    $('#<%=this.ddlViewAdv.ClientID %>').val(selectedViewAdvanced);

				if ($.trim('<%=this.Grid_View %>') == '' && $('#<%=this.ddlView.ClientID %> > option').length > 0) ddlView_change();


                // SCB These are here to force the ParentColumns to update. Remove?
   			    var columnOrder = $('#<%=this.ddlView.ClientID %> option:selected').attr('Tier1ColumnOrder');
			    var columnOrderSub = $('#<%=this.ddlView.ClientID %> option:selected').attr('Tier2ColumnOrder');

			} catch (e) {
				var m = e.message;
			}
		});
	</script>
</asp:Content>