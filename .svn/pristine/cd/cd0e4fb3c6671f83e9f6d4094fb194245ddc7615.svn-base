<%@ Page Title="" Language="C#" MasterPageFile="~/Grids.master" AutoEventWireup="true" CodeFile="Reports_Grid.aspx.cs" Inherits="Reports_Grid" Theme="Default" %>
<%@ Register Src="~/Controls/SelectResources.ascx" TagPrefix="wts" TagName="SelectResources" %>
<%@ Register Src="~/Controls/ResourceDropDownList.ascx" TagPrefix="wts" TagName="ResourceDropDownList" %>

<asp:Content ID="cpHead" ContentPlaceHolderID="head" Runat="Server">
	<script type="text/javascript" src="Scripts/jquery-1.11.2.js"></script>
	<script type="text/javascript" src="Scripts/shell.js"></script>
	<script type="text/javascript" src="Scripts/common.js"></script>
	<script type="text/javascript" src="Scripts/popupWindow.js"></script>
    <script type="text/javascript" src="Scripts/reports.js"></script>
</asp:Content>
<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" Runat="Server">Reports</asp:Content>
<asp:Content ID="cpHeaderText" ContentPlaceHolderID="ContentPlaceHolderHeader" Runat="Server">Reports</asp:Content>
<asp:Content ID="cpPageContentInfo" ContentPlaceHolderID="cphPageContentInfo" Runat="Server">
    <wts:ResourceDropDownList runat="server" ID="ctlResource" ShowSelectAllOption="True" ExcludeNotPeople="True" />
    <input type="checkbox" id="chkAutoRefresh" title="Auto-Refresh" value="Auto-Refresh" style="vertical-align:middle; text-align:left;" checked="checked" /><label for="chkAutoRefresh" class="gridArchivedRow" style="vertical-align:middle;">Auto-Refresh</label>
    <input type="checkbox" id="chkShowArchive" title="Show Archived data" value="Show Archive" style="vertical-align:middle; text-align:left;" /><label for="chkShowArchive" class="gridArchivedRow" style="vertical-align:middle;">Show Archived</label>
    <!-- Add, Edit and any other buttons -->
    <input type="button" id="buttonSave" value="Save" disabled="disabled" />&nbsp;
</asp:Content>
<asp:Content ID="cpBody" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
	<input type="hidden" id="txtSortField" runat="server" value="Organization" />
	<input type="hidden" id="txtSortDirection" runat="server" value="ASC" />
    <iti_Tools_Sharp:Grid ID="gridReports" runat="server" AllowPaging="true" PageSize="25" FirstPageText="First" LastPageText="Last" PagerMode="NumericFirstLast" AllowResize="true"
		CssClass="grid" BodyCssClass="gridBody" SelectedRowCssClass="selectedRow" HeaderCssClass="gridHeader" PagerCssClass="gridPager" FooterCssClass="gridPager" AlternatingRowColor="#dfdfdf">
	</iti_Tools_Sharp:Grid>

	<div id="popupContainer" class="popupPageContainer"></div>

	<asp:ScriptManager ID="scriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>
    <iframe id="frmDownload" style="display: none;"></iframe>

    <wts:SelectResources id="SelectEmailResources" runat="server" 
            IncludeNotPeople="False" IncludeHeader="False" IncludeSaveButton="True" KeepCheckedResourcesOnTop="True" AllowSaveWithNoSelections="False"
            SaveFunctionName="imgMail_selectionsSaved" IncludePopupCode="True" Title="Email Report" />

	<script type="text/javascript">
    	var _selectedId = 0;
    	var popupContainer = document.getElementById('popupContainer');
    	var popupManager = new PopupWindowManager(popupContainer);

        function chkShowArchive_change() {
            var show = false;
            show = $('#chkShowArchive').is(':checked');

            var cssClassList = 'gridBody,gridSelectedRow';
            showHideArchived(cssClassList, show);

            <%=this.gridReports.ClientID %>.RedrawGrid();
        }

        function imgDelete_click(reportQueueID, reportName) {
            if (reportQueueID == 0 || reportQueueID == undefined) {
                alert('Please select a row to delete.');
                return false;
            }

            if (!confirm('Are you sure you would like to permanently delete ' + reportName + '?\nThis cannot be undone.')) {
                return false;
            }

            ShowDimmer(true, "Deleting...", 1);

            try {
                PageMethods.DeleteReport(reportQueueID, deleteReport_Done, on_error);
            } catch (e) {
            	ShowDimmer(false);
            }

            return false;
        }

        function imgMail_click(reportQueueID, reportName) {
            if (reportQueueID == 0 || reportQueueID == undefined) {
                alert('Please select a row to email.');
                return false;
            }

            ShowSelectEmailResourcesPopup(false);            

            return false;
        }

        function on_error(result) {
            var obj = jQuery.parseJSON(result);

            dangerMessage('<b>' + obj.Error + '</b>');
        }

        function imgMail_selectionsSaved(checkedIDs, notcheckedIDs, checkedNames, checkedEmails) {
            if (checkedIDs != null && checkedIDs.length > 0) {
                ShowDimmer(true, 'Emailing report...', 1);

                PageMethods.EmailReport(_selectedId, checkedIDs, checkedNames, checkedEmails, imgMail_emailDone, on_error);
            }
            else {
                dangerMessage('<b>No users selected.</b>');
            }
        }

        function imgMail_emailDone(result) {
            var obj = jQuery.parseJSON(result);

            // because this part usually goes really fast, delay the hiding of the first dimmer so user can read it
            setTimeout(function () {
                ShowDimmer(false);

                if (obj.Success == "true") {
                    successMessage('<b>Email successful.</b>');
                }
                else {
                    dangerMessage('<b>Unable to email report. ' + obj.Error + '</b>');
                }
            }, 1500);
        }

        function deleteReport_Done(result) {
        	ShowDimmer(false);

        	try {
                var obj = jQuery.parseJSON(result);
                var exists = false, deleted = false, archived = false;
                var errorMsg = '';

                if (obj) {
                	if (obj.Exists && obj.Exists.toUpperCase() == 'TRUE') {
                		exists = true;
                	}
                	if (obj.Deleted && obj.Deleted.toUpperCase() == 'TRUE') {
                		deleted = true;
                	}
                	if (obj.Archived && obj.Archived.toUpperCase() == 'TRUE') {
                		archived = true;
                	}
                	if (obj.Error) {
                		errorMsg = obj.Error;
                	}
                }

                if (deleted) {
                    MessageBox('Successfully deleted report.');
                    refreshGrid(false);
                }
                else if (archived) {
                    alert('Report successfully archived.');
                    refreshGrid(false);
                }
                else {
                	MessageBox('Failed to delete report.  ' + errorMsg);
                }
            } catch (e) {
                //TODO: since the error is in the "done" procedure, log message instead of displaying
                refreshGrid(false);
            }
        }

        function lnkDownload_click(reportQueueID, reportName) {

            if (reportQueueID == 0 || reportQueueID == undefined) {
                alert('Please select a row.');
                return false;
            }

            DownloadReport('frmDownload', 'Reports_Grid.aspx?Download=1&ReportQueueID=' + reportQueueID);
        }

        function on_error(result) {
        	ShowDimmer(false);

        	var resultText = 'An error occurred when communicating with the server';/*\n' +
            'readyState = ' + result.readyState + '\n' +
            'responseText = ' + result.responseText + '\n' +
            'status = ' + result.status + '\n' +
            'statusText = ' + result.statusText;*/

            alert('save error:  \n' + resultText);
        }

        function refreshGrid(exportGrid) {
            if (exportGrid === undefined || !exportGrid) {
                exportGrid = false;
            }
            var url = window.location.href;
            var showArchive = $('#chkShowArchive').is(':checked');
            url = editQueryStringValue(url, 'ShowArchived', showArchive.toString());
            url = editQueryStringValue(url, 'Export', exportGrid);
            url = editQueryStringValue(url, 'WTS_RESOURCEID', $('[id*=ddlResource]').val());

            window.location.href = url;
        }

        function refreshQueuedReportStatuses() {
            // this function refreshes just the queued/executing reports to see if they are finished
            var refreshNeeded = false;

            var tbl = $('[id$=gridReports_Grid]');
            if (tbl.children().length > 0 && tbl.children().children().length > 0) { // first child is tbody, second child is rows
                var rows = $(tbl).children().children(); 

                for (var i = 0; i < rows.length; i++) {
                    var statusTd = $(rows[i]).find('td[status=1]');

                    if ($(statusTd).text() == 'Queued' || $(statusTd).text() == 'Executing') {
                        refreshNeeded = true;
                        break;
                    }
                }                
            }

            if (popupManager.IsPopupOpen('SelectEmailResources_Popup')) {
                setTimeout(function () { refreshQueuedReportStatuses(); }, 15000); // since popup is open, we re-try again in 5 seconds
            }
            else {
                if (refreshNeeded && $('#chkAutoRefresh').is(':checked')) {
                    refreshGrid(false);
                }
            }
        }

        function imgSort_click() {
        	getSortOption();

        	return false;
        }
        function getSortOption() {
        	var url = 'SortOption.aspx?SortFields=' + '<%=this.SortFields %>' +
                '&SelectedSortField=' + $('#<%=this.txtSortField.ClientID %>').val() +
                '&SelectedSortDirection=' + $('#<%=this.txtSortDirection.ClientID %>').val();

			var nPopup = popupManager.AddPopupWindow('Sort Options', 'Sort Options', url, 70, 315, 'PopupWindow', this);
			if (nPopup) {
				nPopup.Open();
			}

			return false;
		}
		function getSortOption_done(sortOptions) {
			if (sortOptions == null) { return; }

			$('#<%=this.txtSortField.ClientID %>').val(sortOptions.Field);
            $('#<%=this.txtSortDirection.ClientID %>').val(sortOptions.Direction);

        	refreshGrid(false);
        }

        function row_click(row) {            
            _selectedId = $(row).attr('ReportQueueID');
            return false;
        }

        function activateSaveButton(active) {
            var disabled = active != null && !active ? true : false;            

            $('#buttonSave').attr('disabled', disabled);
            $('#buttonSave').prop('disabled', disabled);
        }

        function buttonSave_click() {
            var changes = '';
            var itemsArchived = false;

            var chks = $('input[id*=chkArchive_]');
            for (var i = 0; i < chks.length; i++) {
                var chk = chks[i];

                var span = $(chk).parent();
                var origValue = $(span).attr('original_value');
                var newValue = $(chk).is(':checked') ? '1' : '0';

                if (origValue != newValue) {
                    if (newValue == '1') itemsArchived = true;
                    if (changes.length > 0) changes += ';';
                    changes += $(span).attr('itemid') + '=' + newValue;
                }
            }
            
            if (changes.length > 0) {
                PageMethods.SaveReports(changes, reports_Saved, on_error);
            }
            else {
                activateSaveButton(false);
            }
        }

        function reports_Saved(result) {
            successMessage('<b>Reports saved.</b>');

            var needRefresh = false;

            if (!<%=this.ShowArchived.ToString().ToLower()%> && $('input[id*=chkArchive_]:checked').length > 0) {
                needRefresh = true;
            }
            else {
                // we aren't refreshing, but we should update the 'original_values' attributes to match current values
                var chks = $('input[id*=chkArchive_]');
                for (var i = 0; i < chks.length; i++) {
                    var chk = chks[i];

                    var span = $(chk).parent();                    
                    var newValue = $(chk).is(':checked') ? '1' : '0';
                    $(span).attr('original_value', newValue);                    
                }
            }

            if (needRefresh) {
                refreshGrid(false);
            }
            else {
                activateSaveButton(false);
            }
        }
        


        function initializeEvents() {
            // are we going to allow exports from this grid?
            //$('#imgExport').click(function (event) { refreshGrid(true); });
            $('#imgExport').hide();
            $('#imgRefresh').click(function () { refreshGrid(false); });

            $('.gridBody').click(function (event) { row_click(this); });
            $('.selectedRow').click(function (event) { row_click(this); });
            $('#chkShowArchive').on('change', function () { refreshGrid(false); });
            $('#buttonSave').click(function (event) { buttonSave_click(); return false; });
            $('[id*=chkArchive]').click(function () { activateSaveButton(); });
            $('#chkAutoRefresh').on('change', function () { if ($('#chkAutoRefresh').is(':checked')) setTimeout(function () { refreshQueuedReportStatuses(); }, 5000); });
            $('[id*=ddlResource]').on('change', function () { refreshGrid(); });
		}

        $(document).ready(function () {
        	initializeEvents();

            var showArchived = '<%=this.ShowArchived %>';
            $('#chkShowArchive').attr('checked', showArchived.toUpperCase() == 'TRUE' ? true : false);
            $('#chkShowArchive').prop('checked', showArchived.toUpperCase() == 'TRUE' ? true : false);

            setTimeout(function () { refreshQueuedReportStatuses(); }, 15000);
        });
    </script>
</asp:Content>

