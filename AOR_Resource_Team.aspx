<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AOR_Resource_Team.aspx.cs" Inherits="AOR_Resource_Team" MasterPageFile="~/Grids.master" Theme="Default" %>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" runat="Server">AOR Resources</asp:Content>
<asp:Content ID="cpHead" ContentPlaceHolderID="head" runat="Server">
    <!-- Copyright (c) 2017 Infinite Technologies, Inc. -->
</asp:Content>
<asp:Content ID="cpHeaderText" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="Server">
    <span>Resources</span>
</asp:Content>
<asp:Content ID="cpPageContentInfo" ContentPlaceHolderID="cphPageContentInfo" runat="Server">
	<table style="width: 100%;">
		<tr>
			<td>
                <input type="button" id="btnSyncActionTeam" value="Sync Action Team" style="vertical-align: middle; display: none;" />
                <input type="button" id="btnCancel" value="Cancel" style="vertical-align: middle; display: none;" />
	            <input type="button" id="btnSave" value="Save" disabled="disabled" style="vertical-align: middle; display: none;" />
			</td>
		</tr>
	</table>
</asp:Content>
<asp:Content ID="cpBody" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
    <div id="divPageContainer" class="pageContainer" style="overflow-x: hidden; overflow-y: auto;">
        <div id="divReleaseContainer" data-index="1">
		    <div class="pageContentHeader" style="vertical-align: top; padding: 5px;">
			    <img class="toggleSection" src="Images/Icons/minus_blue.png" title="Hide Section" alt="Hide Section" height="12" width="12" data-section="Release" style="cursor: pointer;" />&nbsp;&nbsp;Release:
		    </div>
		    <div id="divRelease" style="padding: 10px;">
                <table style="width: 100%;">
		            <tr>
			            <td>
				            <div id="divAORResources" runat="server"></div>
			            </td>
		            </tr>
	            </table>
            </div>
        </div>
        <div id="divActionTeamContainer" data-index="1" style="padding-right: 10px;">
		    <div class="pageContentHeader" style="vertical-align: top; padding: 5px;">
			    <img class="toggleSection" src="Images/Icons/minus_blue.png" title="Hide Section" alt="Hide Section" height="12" width="12" data-section="ActionTeam" style="cursor: pointer;" />&nbsp;&nbsp;Action Team:
	            <input type="button" id="btnAdd" value="Add" style="vertical-align: middle; float: right;" />
		    </div>
		    <div id="divActionTeam" style="padding: 10px; height: auto;">
                <table style="width: 100%;">
		            <tr>
			            <td>
				            <div id="divAORActionTeam" runat="server"></div>
                            <div id="divAORActionTeamPopup" runat="server" style="display:none;background-color:#ffffff;">
                            </div>
			            </td>
		            </tr>
	            </table>
            </div>
        </div>
    </div>
    <asp:ScriptManager ID="scriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>

    <script id="jsVariables" type="text/javascript">
        var _pageUrls;
        var _selectedAORReleaseResourceTeamID = 0;
    </script>

    <script id="jsEvents" type="text/javascript">
        function imgRefresh_click() {
            refreshPage(false);
        }

        function btnCancel_click() {
            refreshPage();
        }

        function btnSave_click() {
            try {
                var validation = validate();

                if (validation.length === 0) {
                    ShowDimmer(true, 'Saving...', 1);

                    var arrResources = [], arrActionTeam = [];

                    $('#<%=this.divAORResources.ClientID %> tr').not(':first').each(function () {
                        var $obj = $(this);
                        
                        if ($obj.find('td').text().indexOf('No Resources') === -1 && $obj.find('select[field="Resource"]').val() > 0) arrResources.push({ 'resourceid': $obj.find('select[field="Resource"]').val(), 'allocation': $obj.find('input[field="Allocation"]').val(), 'aorroleid': $obj.find('select[field="ResourceRole"]').val() });
                    });

                    $('#<%=this.divAORActionTeam.ClientID %> tr').not(':first').each(function () {
                        var $obj = $(this);

                        if ($obj.find('td').text().indexOf('No Resources') === -1) arrActionTeam.push({ 'resourceid': $obj.find('select[field="ActionTeam"]').val() });
                    });

                    var nResourcesJSON = '{save:' + JSON.stringify(arrResources) + '}';
                    var nActionTeamJSON = '{save:' + JSON.stringify(arrActionTeam) + '}';

                    PageMethods.Save('<%=this.AORID %>', nResourcesJSON, nActionTeamJSON, save_done, on_error);
                }
                else {
                    MessageBox('Invalid entries: <br><br>' + validation);
                }
            }
            catch (e) {
                ShowDimmer(false);
                MessageBox('An error has occurred.');
            }
        }

        function save_done(result) {
            ShowDimmer(false);

            var blnSaved = false, blnExists = false;
            var newID = '', errorMsg = '';
            var obj = $.parseJSON(result);

            if (obj) {
                if (obj.saved && obj.saved.toUpperCase() == 'TRUE') blnSaved = true;
                if (obj.exists && obj.exists.toUpperCase() == 'TRUE') blnExists = true;
                if (obj.newID && parseInt(obj.newID) > 0) newID = obj.newID;
                if (obj.error) errorMsg = obj.error;
            }

            if (blnSaved) {
                if (parent.parent._newItemCreated != undefined) parent.parent._newItemCreated = true;

                MessageBox('AOR Resources has been saved.');

                refreshPage(newID);
            }
            else if (blnExists) {
                MessageBox('AOR Name already exists.');
            }
            else {
                MessageBox('Failed to save. <br>' + errorMsg);
            }
        }

        function on_error() {
            ShowDimmer(false);
            MessageBox('An error has occurred.');
        }

        function btnSyncActionTeam_click() {
            ShowDimmer(true, "Saving...", 1);
            PageMethods.SyncActionTeam('<%=this.AORReleaseID%>', save_done, on_error);
        }

        function imgToggleSection_click(obj) {
            var $obj = $(obj);
            var section = $obj.data('section');

            if ($obj.attr('title') == 'Show Section') {
                $obj.attr('src', 'Images/Icons/minus_blue.png');
                $obj.attr('title', 'Hide Section');
                $obj.attr('alt', 'Hide Section');
                $('#div' + section).show();
            }
            else {
                $obj.attr('src', 'Images/Icons/add_blue.png');
                $obj.attr('title', 'Show Section');
                $obj.attr('alt', 'Show Section');
                $('#div' + section).hide();
            }
        }

        function validate() {
            var validation = [];
            var $resourceRows = $('#<%=this.divAORResources.ClientID %> tr').not(':first');
            var $actionTeamRows = $('#<%=this.divAORActionTeam.ClientID %> tr').not(':first');
            var blnExit = false;
            var allocationSum = 0;

            $.each($actionTeamRows, function () {
                if (blnExit) return false;

                var resourceID = $(this).find('select[field="ActionTeam"]').val();

                $.each($actionTeamRows.not($(this)), function () {
                    if ($(this).find('select[field="ActionTeam"]').val() == resourceID) {
                        validation.push('Resource cannot have duplicates.');
                        blnExit = true;
                        return false;
                    }
                });
            });

            $.each($('input[field="Allocation"]'), function () {
                allocationSum += parseInt($(this).val());
            });

            if (allocationSum > 100) validation.push('Total Allocation % cannot exceed 100.');

            return validation.join('<br>');
        }

        function addResourceLinkClicked() {
            if ($('#<%=this.divAORResources.ClientID%> table').find('td').text().indexOf('No Resources') != -1) $('#<%=this.divAORResources.ClientID%> table')[0].rows[2].remove();
            var grd = $('#<%=this.divAORResources.ClientID%> table')[0];
            var nRow = grd.rows[1].cloneNode(true);
            $(nRow.cells).each(function (i, td) {
                if ($(td).find('input:text').length > 0) {
                    $(td).find('input:text').attr('original_value', 0);
                    $(td).find('input:text').text(0);
                    $(td).find('input:text').val(0);
                }
                else if ($(td).children('input').length > 0) {
                    $(td).find('input').attr('original_value', 0);
                    $(td).find('input').text(0);
                    $(td).find('input').val(0);
                }
                else if ($(td).children('select').length > 0) {
                    $(td).find('select').attr('original_value', '');
                    $(td).find('select').prop('disabled', false);
                    $(td).find('select').val(0);
                } else if ($(td).children('span[name="enterpriseallocation"]').length > 0) {
                    $(td).find('span[name="enterpriseallocation"]').text('');
                }
            });
            
            grd.rows[1].parentNode.insertBefore(nRow, grd.rows[1]);
            //add delete button
            $(nRow).show();
            $('#btnSave').prop('disabled', false);
            resizePage();
            if (parent.resizeFrame) parent.resizeFrame('Resources');
        }

        function addActionTeamLinkClicked() {
            var nWindow = 'AddResource';
            var nTitle = 'Add Resource';
            var nHeight = 350, nWidth = 600;
            var nURL = null;

            // sync the popup checkboxes in case selections have been made in the select lists since last time the checkboxes were displayed
            syncResourceCheckboxes();
            sortResourceCheckboxes();

            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'QuestionBox', nHeight, nWidth, 'PopupWindow', this, false, '#<%=divAORActionTeamPopup.ClientID%>');

            if (openPopup) openPopup.Open();
        }

        function closeResourceSelections() {
            QuestionBox('Confirm Close', 'Are you sure you would like to close?', 'Yes,No', 'confirmClose', 300, 350, this);
        }

        function confirmClose(answer) {
            if ($.trim(answer).toUpperCase() == 'YES') popupManager.RemovePopupWindow('AddResource');
        }

        function actionTeamPopupSelectionsSaved() {
            var thePopup = popupManager.GetPopupByName('AddResource');
            var cbs = $(thePopup.Body).children('[id*=divAORActionTeamPopup]').find('[name=cbAddActionTeam]'); // the checkbox div exists in the popup body, not the document body, at the time of save

            for (var i = cbs.length - 1; i >= 0; i--) { // insert backwards so that the order stays same as checkboxes
                var cb = cbs[i];
                var row = $(cb).closest('tr');

                var rscID = $(cb).attr('value');

                addActionTeamByID(rscID, $(cb).is(':checked'));
            }

            popupManager.RemovePopupWindow('AddResource');

            syncResourceCheckboxes();
            sortResourceCheckboxes();
        }

        function removeResource(obj, forceResync) {
            $(obj).closest('tr').remove();

            var $tbl = $('#<%=this.divAORResources.ClientID %> table');
            if ($tbl.find('tr').not(':first').length == 0) $tbl.append('<tr class="gridBody"><td colspan="5" style="border-top: 1px solid grey; border-left: 1px solid grey; text-align: center;">No Resources</td></tr>');

            var $tbl = $('#<%=this.divAORActionTeam.ClientID %> table');
            if ($tbl.find('tr').not(':first').length == 0) $tbl.append('<tr class="gridBody"><td colspan="5" style="border-top: 1px solid grey; border-left: 1px solid grey; text-align: center;">No Resources</td></tr>');


            $('#btnSave').prop('disabled', false);

            if (forceResync) {
                syncResourceCheckboxes();
                sortResourceCheckboxes();
            }

            validateAllocations();
        }

        function syncResourceCheckboxes() {
            $('input[name=cbAddResource]').prop('checked', false);
            $('select[name=ddlAddResourceRole]').val('0');
            var sp = $('span[name=spanAddResourceEnterpriseAllocation]');

            var sel = $('select[field=Resource]');
            for (var x = 0; x < sel.length; x++) {
                var val = $(sel[x]).val();
                var selectRow = $(sel[x]).closest('tr');
                var addRow = $('input[name=cbAddResource][value=' + val + ']').closest('tr');

                $('input[name=cbAddResource][value=' + val + ']').prop('checked', true);
                addRow.find('select[name=ddlAddResourceRole]').val($(selectRow).find('select[field=ResourceRole]').val());

                var origEnterpriseAlloc = $(addRow).find('td[field=enterprise_allocation]').attr('orig_enterprise_allocation');

                var newAllocation = parseInt(origEnterpriseAlloc);
                addRow.find('td[field=enterprise_allocation]').find('span').text(newAllocation + ' %');
                addRow.find('td[field=enterprise_allocation]').find('span').css('color', newAllocation > 100 ? 'red' : 'black');
            }

            sel = $('select[field=ActionTeam]');
            for (var x = 0; x < sel.length; x++) {
                var val = $(sel[x]).val();
                var selectRow = $(sel[x]).closest('tr');
                var addRow = $('input[name=cbAddActionTeam][value=' + val + ']').closest('tr');

                $('input[name=cbAddActionTeam][value=' + val + ']').prop('checked', true);
            }
        }

        function addResourceByID(rscID, checked, AORRoleID, alloc, origalloc, origenterprisealloc) {
            var $tbl = $('#<%=this.divAORResources.ClientID %> table');

            if (checked) {
                var cnt = $('select[field=Resource]').length;
                var sel = $('select[field=Resource]:has(option[value=' + rscID + ']:selected)');

                if (sel.length == 0) {
                    var nHTML = '';

                    if ($tbl.find('td').text().indexOf('No Resources') != -1) $tbl.find('tr').not(':first').remove();

                    nHTML += '<tr class=\"gridBody\" rowChanged="1"><td style=\"border-left: 1px solid grey; text-align: center;\">';
                    nHTML += '<a href=\"\" onclick=\"removeResource(this); return false;\" style=\"color: blue;\">Remove</a>';
                    nHTML += '</td><td style=\"text-align: center;\">';
                    nHTML += '<select field="Resource" row=\"' + _rscSelIdx + '\" original_value="0" fieldChanged="1" style="width: 95%; background-color: #F5F6CE;">' + decodeURIComponent('<%=this.UsersOptions %>') + '</select>';
                    nHTML += '</td>';
                    nHTML += '<td style="text-align:center;">';
                    nHTML += '<select field="ResourceRole" row="' + _rscSelIdx + '" original_value="9999" style="width: 95%; background-color: #F5F6CE;">' + decodeURIComponent('<%=this.RoleOptions%>') + '</select>';
                    nHTML += '</td>';
                    nHTML += '<td style=\"text-align: center;\">';
                    nHTML += '<input type="text" value="0" maxlength="3" field="Allocation" row=\"' + _rscSelIdx + '\" orig_allocation="0" fieldChanged="1" style="width: 95%; text-align: center;" onkeyup="this.value = limitNumber(stripAlpha(this.value), 0, 100); recalcEnterpriseAlloc(this, true); return false;" />';
                    nHTML += '</td>';
                    nHTML += '<td style=\"text-align: center;\" field=\"enterprise_allocation\" orig_enterprise_allocation=\"' + origenterprisealloc + '\">';
                    nHTML += '<span name=\"enterpriseallocation\" row=\"' + _rscSelIdx + '\"></span>';
                    nHTML += '</td>';
                    nHTML += '</tr>';

                    $tbl.find('tr:first').after(nHTML);
                    $('select[field=Resource][Row=' + _rscSelIdx + ']').val(rscID);
                    $('select[field=ResourceRole][Row=' + _rscSelIdx + ']').val(AORRoleID);

                    if (alloc == null) { // this means we came from the systems popup, which doesn't have alloc inputs, so we use the add resource table to find some of the data                                            
                        var addRow = $('input[name=cbAddResource][value=' + rscID + ']').closest('tr');
                        alloc = 0;
                        origalloc = addRow.find('input[name=txtAddResourceAllocation]').attr('orig_allocation');
                        origenterprisealloc = addRow.find('td[field=enterprise_allocation]').attr('orig_enterprise_allocation');
                        $('input[field=Allocation][row=' + _rscSelIdx + ']').attr('orig_allocation', origalloc);
                        $('span[name=enterpriseallocation][row=' + _rscSelIdx + ']').parent().attr('orig_enterprise_allocation', origenterprisealloc);
                    }

                    $('input[field=Allocation][Row=' + _rscSelIdx + ']').val(alloc);

                    var newAllocation = parseInt(origenterprisealloc) + (parseInt(alloc) - parseInt(origalloc));
                    $('span[name=enterpriseallocation][row=' + _rscSelIdx + ']').text(newAllocation + '%');

                    $('span[name=enterpriseallocation][row=' + _rscSelIdx + ']').css('color', newAllocation > 100 ? 'red' : 'black');


                    $('#btnSave').prop('disabled', false);

                    _rscSelIdx++;
                }
                else {
                    // item already exists, so get the row and make updates from the grid
                    var theRow = $(sel).closest('tr');

                    if (alloc == null) { // this means we came from the systems popup, which doesn't have alloc inputs, so we use the add resource table to find some of the data                                            
                        var addRow = $('input[name=cbAddResource][value=' + rscID + ']').closest('tr');
                        alloc = theRow.find('input[field=Allocation]').val();
                        origalloc = addRow.find('input[name=txtAddResourceAllocation]').attr('orig_allocation');
                        origenterprisealloc = addRow.find('td[field=enterprise_allocation]').attr('orig_enterprise_allocation');
                    }

                    theRow.find('select[field=ResourceRole]').val(AORRoleID);
                    theRow.find('input[field=Allocation]').val(alloc);
                    var newAllocation = parseInt(origenterprisealloc) + (parseInt(alloc) - parseInt(origalloc));
                    theRow.find('span[name=enterpriseallocation]').text(newAllocation + '%');

                    theRow.find('span[name=enterpriseallocation]').css('color', newAllocation > 100 ? 'red' : 'black');
                }
            }
            else {
                var sel = $('select[field=Resource]');

                for (var x = 0; x < sel.length; x++) {
                    if ($(sel[x]).val() == rscID) {
                        removeResource(sel[x]);
                    }
                }
            }

            validateAllocations();
        }

        function addActionTeamByID(rscID, checked) {
            var $tbl = $('#<%=this.divAORActionTeam.ClientID %> table');

            if (checked) {
                var cnt = $('select[field=ActionTeam]').length;
                var sel = $('select[field=ActionTeam]:has(option[value=' + rscID + ']:selected)');

                if (sel.length == 0) {
                    var nHTML = '';

                    if ($tbl.find('td').text().indexOf('No Resources') != -1) $tbl.find('tr').not(':first').remove();

                    nHTML += '<tr class=\"gridBody\" rowChanged="1"><td style=\"border-left: 1px solid grey; text-align: center;\">';
                    nHTML += '<a href=\"\" onclick=\"removeResource(this); return false;\" style=\"color: blue;\">Remove</a>';
                    nHTML += '</td><td style=\"text-align: center;\">';
                    nHTML += '<select field="ActionTeam" row=\"' + _rscSelIdx + '\" original_value="0" fieldChanged="1" style="width: 95%; background-color: #F5F6CE;">' + decodeURIComponent('<%=this.UsersOptions %>') + '</select>';
                    nHTML += '</td>';
                    nHTML += '<td></td>';
                    nHTML += '<td></td>';
                    nHTML += '<td style="text-align: center;">Manually Added</td>';
                    nHTML += '</tr>';

                    $tbl.find('tr:first').after(nHTML);
                    $('select[field=ActionTeam][Row=' + _rscSelIdx + ']').val(rscID);

                    $('#btnSave').prop('disabled', false);

                    _rscSelIdx++;
                }
            }
            else {
                var sel = $('select[field=ActionTeam]');

                for (var x = 0; x < sel.length; x++) {
                    if ($(sel[x]).val() == rscID) {
                        removeResource(sel[x]);
                    }
                }
            }
            if (parent.resizeFrame) parent.resizeFrame('Resources');
        }

        function validateAllocations() {
            var allocationSum = 0;

            $.each($('input[field="Allocation"]'), function () {
                allocationSum += parseInt($(this).val());
            });

            $('th[header_field=allocation_percentage]').css('color', allocationSum > 100 ? 'red' : 'black');
        }

        function sortResourceCheckboxes(theTable) {
            var rows = null;

            if (theTable != null) {
                rows = $(theTable).find('tr');
            }
            else
                rows = $('#divAORResourcesPopupTable').find('tr'); {
            }

            var sorted = false;

            do {
                sorted = true;

                for (var i = 2; i < rows.length - 2; i++) { // don't eval header row
                    var r1 = rows[i];
                    var r2 = rows[i + 1];

                    var check1 = $(r1).find('input');
                    var check2 = $(r2).find('input');

                    var c1 = $(check1).is(':checked');
                    var c2 = $(check2).is(':checked');

                    var r1OrigSort = parseInt($(r1).attr('origsort'));
                    var r2OrigSort = parseInt($(r2).attr('origsort'));

                    var swap = false;

                    if ((!c1 && c2) ||
                        (c1 && c2 && r1OrigSort > r2OrigSort) ||
                        (!c1 && !c2 && r1OrigSort > r2OrigSort)) {
                        swap = true;
                        sorted = false;
                    }

                    if (swap) {
                        rows[i] = r2;
                        rows[i + 1] = r1;
                        $(r2).detach().insertBefore(r1);
                    }
                }
            } while (!sorted);
        }

        function recalcEnterpriseAlloc(txt, validateAlloc) {
            var val = $(txt).val();
            var origVal = $(txt).attr('orig_allocation');
            var origEnterpriseAlloc = $(txt).closest('tr').find('td[field=enterprise_allocation]').attr('orig_enterprise_allocation');
            var newAllocation = parseInt(origEnterpriseAlloc) + (parseInt(val) - parseInt(origVal));
            if (isNaN(newAllocation)) newAllocation = 0;

            $(txt).closest('tr').find('td[field=enterprise_allocation]').find('a').text(newAllocation + '%');
            $(txt).closest('tr').find('td[field=enterprise_allocation]').find('span').css('color', newAllocation > 100 ? 'red' : 'black');

            if (validateAlloc) {
                validateAllocations();
            }
        }

        function input_change(obj) {
            var $obj = $(obj);

            if ($obj.attr('id') && $obj.attr('id').indexOf('Rank') != -1) {
                var nVal = $obj.val();

                $obj.val(nVal.replace(/[^\d]/g, ''));
            }

            switch ($obj.attr('field')) {
                case 'System':
                case 'Primary':
                case 'Resource':
                case 'ResourceRole':
                case 'Allocation':
                    $obj.attr('fieldChanged', '1');
                    $obj.closest('tr').attr('rowChanged', '1');
                    break;
            }

            if ($obj.attr('field') == 'Allocation') {
                var nVal = $obj.val();

                $obj.val(limitNumber(nVal.replace(/[^\d]/g, ''), 0, 100));
            }

            $('#btnSave').prop('disabled', false);
        }

        function txtBox_blur(obj) {
            var $obj = $(obj);
            var nVal = $obj.val();

            if ($obj.attr('id') && $obj.attr('id').indexOf('Rank') != -1) {
                if (nVal.length == 1) $obj.val('0' + nVal);
                return;
            }

            if ($obj.attr('field') == 'Allocation') {
                if (nVal == '') $obj.val('0');
                return;
            }

            $obj.val($.trim(nVal));
        }

        function refreshPage(blnRetainPageIndex) {
            var nURL = window.location.href;

            window.location.href = 'Loading.aspx?Page=' + nURL;
        }

        function resizePage() {
            resizePageElement('divPageContainer');
        }

        function openAORResource(resourceID, resource) {
            var nWindow = 'ResourceAllocation';
            var nTitle = 'Resource Allocation';
            var nHeight = 500, nWidth = 700;
            var nURL = _pageUrls.MasterData.Edit.AORResource;

            nURL += '?ResourceID=' + resourceID + '&Resource=' + decodeURIComponent(resource) + '&SelectedReleases=' + <%=this.ReleaseID%>;

            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }
    </script>

    <script id="jsInit" type="text/javascript">
        function initVariables() {
            _pageUrls = new PageURLs();
            _rscSelIdx = 10000;
        }

        function initDisplay() {
            $('#imgSort').hide();
            $('#imgExport').hide();
            $('#pageContentHeader').hide();

            if ('<%=this.CanEditAOR %>'.toUpperCase() == 'TRUE') {
                $('#btnCancel').show();
                $('#btnSave').show();
                $('#btnSyncActionTeam').show();
                $('#btnAdd').show();
            }

            syncResourceCheckboxes();
            sortResourceCheckboxes();
            sortResourceCheckboxes('#<%=divAORActionTeamPopup.ClientID%>');

            if (parent.updateTab) parent.updateTab('Resources', <%=this.RowCount %>);

            resizePage();
        }

        function initEvents() {
            $('#imgRefresh').click(function () { imgRefresh_click(); });
            $('#btnCancel').click(function () { btnCancel_click(); return false; });
            $('#btnSave').click(function () { btnSave_click(); return false; });
            $('#btnAdd').click(function () { addActionTeamLinkClicked(); return false; });
            $('#btnSyncActionTeam').click(function () { btnSyncActionTeam_click(); return false; });
            $('.toggleSection').on('click', function () { imgToggleSection_click(this); });
            $('input[type="text"], textarea').on('keyup paste', function () { input_change(this); });
            $('select, input[type="checkbox"]').on('change', function () { input_change(this); });
            $('input[type="text"], textarea').on('blur', function () { txtBox_blur(this); });
            $(window).resize(resizePage);
        }

        $(document).ready(function () {
            initVariables();
            initDisplay();
            initEvents();

            if (parent.resizeFrame) parent.resizeFrame('Resources');
        });
    </script>
</asp:Content>
