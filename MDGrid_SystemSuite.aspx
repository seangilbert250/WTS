﻿<%@ Title="" Language="C#" EnableViewState="false" EnableEventValidation="false" AutoEventWireup="true" CodeFile="MDGrid_SystemSuite.aspx.cs" Inherits="MDGrid_SystemSuite" Theme="Default" MasterPageFile="~/Grids.master" %>
<%@ Register Src="~/Controls/MultiSelect.ascx" TagPrefix="wts" TagName="MultiSelect" %>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" runat="Server">Master Data - System Suite Grid</asp:Content>
<asp:Content ID="cpHead" ContentPlaceHolderID="head" runat="Server"></asp:Content>
<asp:Content ID="cpHeaderText" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="Server">
    <table cellpadding="0" cellspacing="0" border="0" style="width: 100%">
        <tr>
            <td>
             System Suite (<span id="spanRowCount" runat="server">0</span>)
            </td>
            <td style="height: 20px; text-align: right;" >
                <img id="imgHelp" alt="Help" title="Help" src="images/icons/help.png" width="15"
                     height="15" style="cursor: pointer; margin-right: 10px; float: right;" />
            </td>   
        </tr>
    </table>
</asp:Content>
<asp:Content ID="cpGridSetting" ContentPlaceHolderID="cphGridSettings" runat="Server">
	<table id="tableQuickFilters" cellpadding="0" cellspacing="0">
		<tr>
			<td style="padding-left: 5px;">Child View:
                <asp:DropDownList ID="ddlChildView" runat="server" TabIndex="1" Style="width: 145px;">               
                    <asp:ListItem Text="System" Value="0" />
					<asp:ListItem Text="Work Area" Value="1" />
					<asp:ListItem Text="Resource" Value="2" />
					<%--<asp:ListItem Text="Work Activity" Value="3" />--%>
				</asp:DropDownList>
            </td>
            <td id="tdRelease" style="padding-left: 5px; display: none">Release:
                <asp:DropDownList ID="ddlRelease" runat="server" TabIndex="1" Style="width: 100px;"></asp:DropDownList>
            </td>
		</tr>
	</table>
</asp:Content>
<asp:Content ID="cpPageContentInfo" ContentPlaceHolderID="cphPageContentInfo" runat="Server">
	<table cellpadding="0" cellspacing="0" style="float: right; padding-right: 2px;">
		<tr>
			<td>
				<input type="button" id="buttonSync" value="Sync Resources" style="display: none;"/>
				<input type="button" id="buttonNew" value="Add Suite" disabled="disabled" />
				<input type="button" id="buttonSave" value="Save" disabled="disabled" />
				<input type="button" id="buttonDelete" value="Delete" disabled="disabled" />
			</td>
		</tr>
	</table>
</asp:Content>
<asp:Content ID="cpBody" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
	<iti_Tools_Sharp:Grid ID="grdMD" runat="server" AllowPaging="true" PageSize="50" FirstPageText="First" LastPageText="Last" PagerMode="NumericFirstLast" AllowResize="true"
		CssClass="grid" BodyCssClass="gridBody" SelectedRowCssClass="selectedRow" HeaderCssClass="gridHeader" PagerCssClass="gridPager" FooterCssClass="gridPager" AlternatingRowColor="#dfdfdf" EmptyDataText="No data..." >
	</iti_Tools_Sharp:Grid>

	<div id="divPageDimmer" style="position: absolute; left: 0px; top: 0px; width: 100%; height: 100%; background: grey; filter: alpha(opacity=60); opacity: .60; display: none;"></div>
	<div id="divSaving" style="position: absolute; left: 35%; top: 15%; padding: 10px; background: white; border: 1px solid grey; font-size: 18px; text-align: center; display: none;">
		<table>
			<tr>
				<td>WTS is Saving Data... Please wait...</td>
			</tr>
			<tr>
				<td>
					<img alt='' src="Images/loaders/progress_bar_blue.gif" /></td>
			</tr>
		</table>
	</div>
    <div id="divSyncResourcesPopup" runat="server" style="position:absolute; display:none; background-color:#ffffff;">
        <div class="pageContentInfo" style="text-align: right;">
            <input type="button" id="btnSyncResource" value="Sync Resources" style="margin-left:3px;margin-top:3px;" />
            &nbsp;<input type="button" id="btnCloseSyncResources" value="Close" style="margin-top:3px;" />
        </div>
        <div style="padding: 3px;">
            <span>Release: </span>
            <div style="padding-right: 100px; float: right;">
                <wts:MultiSelect runat="server" ID="msRelease"
                ItemLabelColumnName="ProductVersion"
                ItemValueColumnName="ProductVersionID"
                IsOpen="false"
                KeepOpen="true"
                Width="100%"
                MaxHeight="330"
                />
            </div>
        </div>
        <div style="padding: 20px 3px;">
            <span>System Suite: </span>
            <div style="padding-right: 100px; float: right;">
                <wts:MultiSelect runat="server" ID="msSystemSuite"
                ItemLabelColumnName="WTS_SYSTEM_SUITE"
                ItemValueColumnName="WTS_SYSTEM_SUITEID"
                IsOpen="false"
                KeepOpen="true"
                Width="100%"
                MaxHeight="330"
                />
            </div>
        </div>
        <div style="padding: 3px;">
            <span>Intake Team: </span>
            <asp:CheckBox ID="chkActionTeam" runat="server" style="margin-right: 76%; float: right;"></asp:CheckBox>
        </div>
    </div>
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>

	<script id="jsVariables" type="text/javascript">

	    var _pageURLs = new PageURLs();
	    var _idxDelete = 0, _idxID = 0, _idxName = 0, _idxDescription = 0, _idxSortOrder = 0, _idxArchive = 0;
	    var _selectedId = 0;
	    var _htmlDeleteImage = '<img src="Images/Icons/delete.png" height="12" width="12" alt="Click to Delete New Row" title="Delete New Row" onclick="deleteNewRow(this);" style="cursor:pointer;" />';
	    var imgHelp = document.getElementById("imgHelp");
	</script>
    	<script id="jsAJAX" type="text/javascript">

    	    function GetColumnValue(row, ordinal, blnoriginal_value) {
    	        try {
    	            var tdval = $(row).find('td:eq(' + ordinal + ')');
    	            var val = '';
    	            if ($(tdval).length == 0) { return ''; }

    	            if ($(tdval).children.length > 0) {
    	                if ($(tdval).find("select").length > 0) {
    	                    if (blnoriginal_value) {
    	                        val = $(tdval).find("select").attr('original_value');
    	                    }
    	                    else {
    	                        val = $(tdval).find("select").val();
    	                    }
    	                }
    	                else if ($(tdval).find('input[type=checkbox]').length > 0) {
    	                    if (blnoriginal_value) {
    	                        val = $(tdval).find('input[type=checkbox]').parent().attr("original_value");
    	                    }
    	                    else {
    	                        if ($(tdval).find('input[type=checkbox]').prop('checked')) { val = '1'; }
    	                        else { val = '0'; }
    	                    }
    	                }
    	                else if ($(tdval).find('input[type=text]').length > 0) {
    	                    if (blnoriginal_value) {
    	                        val = $(tdval).find('input[type=text]').attr('original_value');

    	                    }
    	                    else {
    	                        val = $(tdval).find('input[type=text]').val();
    	                    }
    	                }
    	                else if ($(tdval).find('input[type=number]').length > 0) {
    	                    if (blnoriginal_value) {
    	                        val = $(tdval).find('input[type=number]').attr('original_value');

    	                    }
    	                    else {
    	                        val = $(tdval).find('input[type=number]').val();
    	                    }
    	                }
    	                else if ($(tdval).find('input').length > 0) {
    	                    if (blnoriginal_value) {
    	                        val = $(tdval).find('input').attr('original_value');

    	                    }
    	                    else {
    	                        val = $(tdval).find('input').val();
    	                    }
    	                }
    	                else {
    	                    val = $(tdval).text();
    	                }

    	            }
    	            else {
    	                val = $(tdval).text();
    	            }
    	            return val;
    	        } catch (e) { return ''; }
    	    }

    	    function save() {
    	        try {
    	            var changedRows = [];
    	            var id = 0;
    	            var original_value = '', name = '', description = '', sortOrder = '', archive = '';

    	            $('.gridBody, .selectedRow', $('#<%=this.grdMD.ClientID%>_Grid')).each(function (i, row) {
				    var changedRow = [];
				    var changed = false;

				    if (_dcc[0].length > 0 && $(this)[0].hasAttribute('fieldChanged')) {
				        for (var i = 0; i <= _dcc[0].length - 1; i++) {
				            var newval = GetColumnValue(row, i);
				            var oldval = GetColumnValue(row, i, true);
				            if (newval != oldval) {
				                changed = true;
				                break;
				            }
				        }
				        if (changed) {
				            for (var i = 0; i <= _dcc[0].length - 1; i++) {
				                changedRow.push('"' + _dcc[0][i].ColumnName + '":"' + encodeURIComponent(GetColumnValue(row, i)) + '"');
				            }
				            var obj = '{' + changedRow.join(',') + '}';
				            changedRows.push(obj);
				        }
				    }
				});

				if (changedRows.length == 0) {
				    MessageBox('You have not made any changes');
				}
				else {
				    ShowDimmer(true, "Updating...", 1);
				    var json = '[' + changedRows.join(",") + ']';
				    PageMethods.SaveChanges(json, save_done, on_error);
				}
            } catch (e) {
                ShowDimmer(false);
                MessageBox('There was an error gathering data to save.\n' + e.message);
            }
        }
        function save_done(result) {
            try {
                ShowDimmer(false);

                var saved = false;
                var ids = '', errorMsg = '';

                var obj = jQuery.parseJSON(result);

                if (obj) {
                    if (obj.saved && obj.saved.toUpperCase() == 'TRUE') {
                        saved = true;
                    }
                    if (obj.ids) {
                        ids = obj.ids;
                    }
                    if (obj.error) {
                        errorMsg = obj.error;
                    }
                }

                if (saved) {
                    MessageBox('Items have been saved.');
                    refreshPage();
                }
                else {
                    MessageBox('Failed to save items. \n' + errorMsg);
                    refreshPage();
                }
            } catch (e) { }
        }

        function deleteItem(itemId) {
            try {
                ShowDimmer(true, "Deleting...", 1);

                PageMethods.DeleteItem(parseInt(itemId), deleteItem_done, on_error);

            } catch (e) {
                ShowDimmer(false);
                MessageBox('There was an error gathering data to save.\n' + e.message);
            }
        }
        function deleteItem_done(result) {
            ShowDimmer(false);

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
                    MessageBox('Item has been deleted.');
                    refreshPage();
                }
                else {
                    MessageBox('Failed to delete item. \n' + errorMsg);
                }
            } catch (e) { }
        }

        function on_error(result) {
            ShowDimmer(false);

            var resultText = 'An error occurred when communicating with the server';/*\n' +
                    'readyState = ' + result.readyState + '\n' +
                    'responseText = ' + result.responseText + '\n' +
                    'status = ' + result.status + '\n' +
                    'statusText = ' + result.statusText;*/

            MessageBox('save error:  \n' + resultText);
        }


        function reviewSystems(systemSuiteID) {
            try {
                ShowDimmer(true, "Saving...", 1);
                if (confirm('This will mark this item as reviewed.' + '\n' + 'Do you wish to continue?')) {
                    PageMethods.ReviewSystems(systemSuiteID, reviewSystems_done, on_error);
                } else {
                    ShowDimmer(false);
                }
            } catch (e) {
                ShowDimmer(false);
                MessageBox('There was an error gathering data to save.\n' + e.message);
            }
        }

        function reviewSystems_done(result) {
            ShowDimmer(false);

            var saved = false;
            var id = '', errorMsg = '';

            try {
                var obj = jQuery.parseJSON(result);

                if (obj) {
                    if (obj.saved && obj.saved.toUpperCase() == 'TRUE') {
                        saved = true;
                    }
                    if (obj.savedIds) {
                        id = obj.savedIds;
                    }
                    if (obj.error) {
                        errorMsg = obj.error;
                    }
                }

                if (saved) {
                    $('#<%=this.grdMD.ClientID%>_Grid').find('tr[itemid="' + id + '"]').children()['<%=this.DCC["System Review"].Ordinal%>'].innerHTML = '<img alt="Systems Reviewed" title="Systems Reviewed" src="images/icons/check.png" width="15" height = "15" style="cursor: pointer;" />';
                }
                else {
                    MessageBox('Failed to save item. \n' + errorMsg);
                }
            } catch (e) { }
        }

        function reviewResources(systemSuiteID) {
            try {
                ShowDimmer(true, "Saving...", 1);
                if (confirm('This will mark this item as reviewed.' + '\n' + 'Do you wish to continue?')) {
                    PageMethods.ReviewResources(systemSuiteID, reviewResources_done, on_error);
                } else {
                    ShowDimmer(false);
                }
            } catch (e) {
                ShowDimmer(false);
                MessageBox('There was an error gathering data to save.\n' + e.message);
            }
        }

        function reviewResources_done(result) {
            ShowDimmer(false);

            var saved = false;
            var id = '', errorMsg = '';

            try {
                var obj = jQuery.parseJSON(result);

                if (obj) {
                    if (obj.saved && obj.saved.toUpperCase() == 'TRUE') {
                        saved = true;
                    }
                    if (obj.savedIds) {
                        id = obj.savedIds;
                    }
                    if (obj.error) {
                        errorMsg = obj.error;
                    }
                }

                if (saved) {
                    $('#<%=this.grdMD.ClientID%>_Grid').find('tr[itemid="' + id + '"]').children()['<%=this.DCC["Resource Review"].Ordinal%>'].innerHTML = '<img alt="Resources Reviewed" title="Resources Reviewed" src="images/icons/check.png" width="15" height = "15" style="cursor: pointer;" />';
                }
                else {
                    MessageBox('Failed to save item. \n' + errorMsg);
                }
            } catch (e) { }
        }
	</script>

	<script id="jsEvents" type="text/javascript">
		
	    function refreshPage() {
            var qs = document.location.href;
            qs = editQueryStringValue(qs, 'ChildView', $('#<%=this.ddlChildView.ClientID %> option:selected').val());
            qs = editQueryStringValue(qs, 'ReleaseID', $('#<%=this.ddlRelease.ClientID %> option:selected').val());

			document.location.href = 'Loading.aspx?Page=' + qs;
        }

        function ddlChildView_change() {
            refreshPage();
        }

        function ddlRelease_change() {
            refreshPage();
        }

        function imgExport_click() {
            window.location.href += '&Export=1';
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

        function buttonSync_click() {
            var nWindow = 'SyncResourcesToSuite';
            var nTitle = 'Sync Resources';
            var nHeight = 350, nWidth = 450;
            var nURL = null;

            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, null, nHeight, nWidth, 'PopupWindow', this, false, '#<%=divSyncResourcesPopup.ClientID%>');

            if (openPopup) openPopup.Open();
        }

        function btnSyncResource_click() {
            var arrReleases = msRelease_getSelections(true);
            var arrSuites = msSystemSuite_getSelections(true);
            var actionTeam = top.$('#<%=this.chkActionTeam.ClientID%>')[0].checked;

            if (arrReleases.length == 0) {
                dangerMessage('Please select a Release', null, true, $(popupManager.GetPopupByName('SyncResourcesToSuite').Window).find('[id*=divSyncResourcesPopup]')[0]);
            } else if (arrSuites.length == 0) {
                dangerMessage('Please select a System Suite', null, true, $(popupManager.GetPopupByName('SyncResourcesToSuite').Window).find('[id*=divSyncResourcesPopup]')[0]);
            }
            else {
                popupManager.RemovePopupWindow('SyncResourcesToSuite');

                ShowDimmer(true, "Saving...", 1);
                var releases = arrReleases.join(",");
                var suites = arrSuites.join(",");
                PageMethods.SaveResource(releases, suites, actionTeam, save_done, on_error);
            }
        }

        function btnCloseSyncResources_click() {
            popupManager.RemovePopupWindow('SyncResourcesToSuite');
        }

        function deleteNewRow(img) {
            $(img).closest('tr').remove();
            resizeFrame();	
        }

        function buttonNew_click() {
            var grdMD = <%=this.grdMD.ClientID%>;
			
		    var nRow = grdMD.Body.Rows[0].cloneNode(true);
		    $(nRow.cells[_idxID]).text('0');//.innerText = '0';
		    $(nRow.cells).each(function(i, td){
		        if($(td).find('input:text').length > 0) {
		            $(td).find('input:text').attr('original_value', '');
		            $(td).find('input:text').text('');
		            $(td).find('input:text').val('');
		        }
		        else if($(td).find('input:checkbox').length > 0) {
		            $(td).find('input:checkbox').attr('original_value', '');
		            $(td).find('input:checkbox').attr('checked', false);
		            $(td).find('input:checkbox').prop('checked', false);
		        }
		        else if($(td).children('input').length > 0) {
		            $(td).find('input').attr('original_value', '');
		            $(td).find('input').text('');
		            $(td).find('input').val('');
		        }
		        else if($(td).children('select').length > 0) {
		            $(td).find('select').attr('original_value', '');
		            $(td).find('select').on('change keyup mouseup', function () { ddl_change(this); });
		            $(td).find('select').on('click focus', function () { LoadList(this); });
		        }
		        else{
		            $(td).html('&nbsp;');
		        }
		    });

            $(nRow).attr('fieldChanged', true);
		    grdMD.Body.Rows[0].parentNode.insertBefore(nRow,grdMD.Body.Rows[0]);
		    //add delete button
            $(nRow.cells[_idxDelete]).html(_htmlDeleteImage);
            $(nRow).show();
            resizeFrame();
		}


	    function buttonDelete_click(){
	        if (!_selectedId || _selectedId == '' || _selectedId == 0) {
	            MessageBox('You must specify an item to delete.');
	            return;
	        }
            
	        if (confirm('This will permanently delete this item.' + '\n' + 'Do you wish to continue?')) {
	            deleteItem(_selectedId);
	        }
	    }
		function imgDelete_click(itemID, item) {
		    if (!itemID || itemID == '' || itemID == 0) {
		        MessageBox('You must specify an item to delete.');
		        return;
		    }

		    if (confirm('This will permanently delete this item.' + '\n' + 'Do you wish to continue?')) {
		        deleteItem(itemID, item);
		    }
		}

		function buttonSave_click() {
		    save();
		}

		function row_click(row) {
		    if ($(row).attr('itemID')) {
		        _selectedId = $(row).attr('itemID');
		        $('#buttonAssignTo').attr('disabled', false);
		        $('#buttonDelete').attr('disabled', false);
		    }
		}

		function activateSaveButton(sender) {
		    if (_canEdit) {
		        $('#buttonSave').attr('disabled', false);
                $('#buttonSave').prop('disabled', false);
                $(sender).closest('tr').attr('fieldChanged', true);
		    }
		}

		function txt_change(sender) {
		    var original_value = '', new_value = '';
		    if ($(sender).attr('original_value')) {
		        original_value = $(sender).attr('original_value');
		    }

		    new_value = $(sender).val();

		    if (new_value != original_value) {
		        activateSaveButton(sender);
		    }
		}

		function imgShowHideChildren_click(sender, direction, id) {
		    try {
		        if (id == "0" || id == "ALL") {
		            var itemId = '0';

		            $('[Name="img' + direction + '"]').each(function () {
		                itemId = $(this).attr('itemId');
		                if (requestId && +requestId > 0) {
		                    imgShowHideChildren_click(this, direction, itemId);
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
                        $(row).show();
		                $(row).attr('nohighlight', 'true');

		                td = $('td:eq(<%=(this.DCC == null 
							|| !this.DCC.Contains("WTS_SYSTEM_SUITE")) ? 0 : this.DCC["WTS_SYSTEM_SUITE"].Ordinal %>)', row)[0];
						loadChildGrid(td, id);
					});
                }
                else {
                    $('tr[Name="gridChild_' + id + '"]').hide();
                }

                $(sender).hide();
                $(sender).siblings().show();
                resizeFrame();
            } catch (e) {
                var msg = e.message;
            }
        }
        //TODO Handle code below with child
        function loadChildGrid(td, id) {
            var url = 'Loading.aspx?Page=';
            switch ($('#<%=this.ddlChildView.ClientID %> option:selected').val()) {
                case "0": // System
                    url += 'MDGrid_SystemSuite_Assignment.aspx';
                    url += '?WTS_SYSTEM_SUITEID=' + id;
                    break;
                case "1": // Work Area
                    url += 'MDGrid_SystemSuite_WorkArea.aspx?';
                    url += '&WTS_SYSTEM_SUITEID=' + id;
                    break;
                case "2": // Resource
                    url += 'MDGrid_Resource.aspx?&CurrentLevel=1';
                    url += '&ProductVersionID=' + <%=this._qfReleaseID%>;
                    url += '&WTS_SYSTEM_SUITEID=' + id;
                    break;
                case "3": // Work Activity
                    url += 'MDGrid_Suite_WorkActivity.aspx?&CurrentLevel=1';
                    url += '&WTS_SYSTEM_SUITEID=' + id;
                    break;
                default:
                    break;
            }

            $('iFrame', $(td)).each(function () {
                var src = $(this).attr('src');
                if (src == "javascript:''") {
                    $(this).attr('src', url);
                }

                $(this).show();
            });
        }

        function resizeGrid() {
    setTimeout(function () { <%=this.grdMD.ClientID %>.ResizeGrid(); }, 1);
}

	</script>

	<script id="jsInit" type="text/javascript">

	    function initVariables() {
	        try {
	            _pageUrls = new PageURLs();
                _currentLevel = '<%=this.CurrentLevel %>';
	            _canEdit = ('<%=this.CanEdit.ToString().ToUpper()%>' == 'TRUE');
				_canView = (_canEdit || ('<%=this.CanView.ToString().ToUpper()%>' == 'TRUE'));
			    _isAdmin = ('<%=this.IsAdmin.ToString().ToUpper()%>' == 'TRUE');


                //TODO LOOK INTO MDGrid_Scope.aspx line number 370 
			   // if (_dcc[0] && _dcc[0].length > 0) {
			//	    _idxID = parseInt(
			//	    _idxName = parseInt(
			//	    _idxDescription = parseInt('
			//	    _idxSortOrder = parseInt('
			//	    _idxArchive = parseInt('
	            //	}
            } catch (e) {

            }
        }
		
        $(document).ready(function () {

            initVariables();
            if ($('#<%=this.ddlChildView.ClientID %> option:selected').val() === "2") {
                $('#tdRelease').show();
                $('#buttonSync').show();
            }
			
            $(':input').css('font-family', 'Arial');
            $(':input').css('font-size', '12px');
			
            $('#imgReport').hide();
            $('#imgExport').click(function () { imgExport_click(); });
            $('#imgRefresh').click(function () { refreshPage(); });
            $('#imgSort').click(function () { imgSort_click(); });
            if (_canEdit) {
                $('input:text').on('change keyup mouseup', function () { txt_change(this); });
                $('input:checkbox').on('change', function () { txt_change(this); });
                $('input').on('change keyup mouseup', function () { txt_change(this); });

                $('#buttonAssignTo').click(function(event){ buttonAssignTo_Click(); return false;});

                $('#buttonNew').attr('disabled', false);
                $('#buttonNew').click(function (event) { buttonNew_click(); return false; });
                $('#buttonSync').click(function (event) { buttonSync_click(); return false; });

                $('#buttonSave').click(function (event) { buttonSave_click(); return false; });

                $('#buttonDelete').click(function (event) { buttonDelete_click(); return false; });

                $('#btnSyncResource').click(function (event) { btnSyncResource_click(); return false; });
                $('#btnCloseSyncResources').on('click', function () { btnCloseSyncResources_click(); });
            }

            $('.gridBody').click(function (event) { row_click(this); });
            $('.selectedRow').click(function (event) { row_click(this); });

            $('#<%=this.ddlChildView.ClientID %>').change(function () { ddlChildView_change(); return false; });
            $('#<%=this.ddlRelease.ClientID %>').change(function () { ddlRelease_change(); return false; });

            $(imgHelp).click(function () { MessageBox('Under Construction, "Needs help text".'); });
            msRelease_init();
            msSystemSuite_init();
            $('.ms-drop, .bottom').css('width', '100%');
            resizeFrame();
		});
	</script>
    </asp:Content>