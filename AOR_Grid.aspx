<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AOR_Grid.aspx.cs" Inherits="AOR_Grid" MasterPageFile="~/Grids.master" Theme="Default" %>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" runat="Server">AOR</asp:Content>
<asp:Content ID="cpHead" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="cpHeaderText" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="Server">
    <!-- Copyright (c) 2018 Infinite Technologies, Inc. -->
	<asp:HiddenField ID="itisettings" runat="server" />
    <asp:HiddenField ID="hdnGridSessionKey" runat="server" />
    <span>AOR</span>
</asp:Content>
<asp:Content ID="cpGridSetting" ContentPlaceHolderID="cphGridSettings" runat="Server">
    <table id="tblRelease" runat="server">
        <tr style="height: 30px;">
            <td>
                <span style="margin-left: 10px">Gridview: </span><select id="ddlGridview" runat="server"></select>
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="cpPageContentInfo" ContentPlaceHolderID="cphPageContentInfo" runat="Server">
	<table style="width: 100%;">
		<tr>
			<td>
                <input type="button" id="btnSearch" value="Go to AOR #" />
				<input type="text" id="txtAORSearch" name="Search" tabindex="5" maxlength="4" size="2" />
                <input type="button" id="btnAdd" value="Add/Edit" style="vertical-align: middle; display: none;" />
                <input type="button" id="btnDelete" value="Delete" disabled="disabled" style="vertical-align: middle; display: none;" />
                <input type="button" id="btnEdit" value="View/Edit" disabled="disabled" style="vertical-align: middle; display: none;" />
                <input type="button" id="btnSave" value="Save" disabled="disabled" style="vertical-align: middle; display: none;" />
			    <iti_Tools_Sharp:Menu ID="menuRelatedItems" runat="server" Align="Right" ClientClickEvent="openMenuItem" Text="Related&nbsp;Items&nbsp;<img id=imgRelatedItemsMenu alt=Expand Menu title=Show Related Items Options src=Images/menuDown_Black.gif />" Button="true" Style="margin-right: 10px; display: inline-block; float: right;"></iti_Tools_Sharp:Menu>
            </td>
		</tr>
	</table>
</asp:Content>
<asp:Content ID="cpBody" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
    <iti_Tools_Sharp:Grid ID="grdData" runat="server" AllowPaging="true" PageSize="25" FirstPageText="First" LastPageText="Last" PagerMode="NumericFirstLast" AllowResize="true"
        OnGridRowDataBound="grdData_OnGridRowDataBound" OnGridHeaderRowDataBound="grdData_OnGridHeaderRowDataBound"
		CssClass="grid" BodyCssClass="gridBody" SelectedRowCssClass="selectedRow" HeaderCssClass="gridHeader" PagerCssClass="gridPager" FooterCssClass="gridPager" AlternatingRowColor="#dfdfdf">
	</iti_Tools_Sharp:Grid>
    <script src="Scripts/multiselect/jquery.multiple.select.js" type="text/javascript"></script>
    <link rel="stylesheet" href="Styles/multiple-select.css" />
    <asp:ScriptManager ID="scriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>
    <script src="Scripts/autosize.js"></script>
    <script id="jsVariables" type="text/javascript">
        var _pageUrls;
        var _currentLevel = '1';
        var _selectedAORID = 0, _selectedAORReleaseID = 0;
        var arrSelectedEntity = [];
    </script>

    <script id="jsEvents" type="text/javascript">
        function toggleQuickFilters_click() {
            var $imgShowQuickFilters = $('#imgShowQuickFilters');
            var $imgHideQuickFilters = $('#imgHideQuickFilters');
            var $divQuickFilters = $('#divQuickFilters');
            var addtop = 0;
            var addLeft = 0;

            if (parseInt(_currentLevel) > 1) {
                addtop = 30;
                addLeft = 5;
            }else{
                addtop = 58;
                addLeft = 273;
            }

            if ($imgShowQuickFilters.is(':visible')) {
                $imgShowQuickFilters.hide();
                $imgHideQuickFilters.show();

                var pos = $imgShowQuickFilters.position();
                $divQuickFilters.css({
                    width: '300px',
                    position: 'absolute',
                    top: (pos.top + addtop) + 'px',
                    left: (pos.left + addLeft) + 'px'
                }).slideDown();
            }
            else if ($imgHideQuickFilters.is(':visible')) {
                $imgHideQuickFilters.hide();
                $imgShowQuickFilters.show();

                var pos = $imgHideQuickFilters.position();
                $divQuickFilters.css({
                    width: '300px',
                    position: 'absolute',
                    top: (pos.top + addtop) + 'px',
                    left: (pos.left + addLeft) + 'px'
                }).slideUp();
            }
        }

        function relatedItems_click(action, type) {
            switch (action) {
                case 'ReleaseBuilder':
                    btnReleaseBuilder_click();
                    break;
                case 'MassChange':
                    btnMassChange_click();
                    break;
                case 'CRReport':
                    btnCRReport_click();
                    break;
                case 'DSEReport':
                    btnDSEReport_click();
                    break;
                case 'MoveTask':
                    btnMoveWorkTask_click();
                    break;
                case 'ReleaseAssessment':
                    btnReleaseAssessment_click();
                    break;
            }
        }

        function openMenuItem(url) {
            if (url.indexOf("relatedItems") > 0) {
                var str = url.split("'");
                relatedItems_click(str[1], str[3]);
            }
        }

        function ddlRelease_get() {
            return $('#<%=Master.FindControl("ms_Item0").ClientID %>').multipleSelect('getSelects').join(',');
        }

        function ddlAORType_get() {
            return $('#<%=Master.FindControl("ms_Item10").ClientID %>').multipleSelect('getSelects').join(',');
        }


        function ddlVisibleToCustomer_get() {
            return $('#<%=Master.FindControl("ms_Item10a").ClientID %>').multipleSelect('getSelects').join(',');
        }

        function ddlContainsTasks_get() {
            return $('#<%=Master.FindControl("ms_Item2").ClientID %>').multipleSelect('getSelects').join(',');
        }

        function ddlContract_get() {
            return $('#<%=Master.FindControl("ms_Item1").ClientID %>').multipleSelect('getSelects').join(',');
        }

        function ddlTaskStatus_get() {
            return $('#<%=Master.FindControl("ms_Item9").ClientID %>').multipleSelect('getSelects').join(',');
        }

        function ddlAORProductionStatus_get() {
            return $('#<%=Master.FindControl("ms_Item9a").ClientID %>').multipleSelect('getSelects').join(',');
        }

        function setDefaultPageItisettings(paramItiSettings) {
            $(defaultParentPage.itisettings).text(paramItiSettings);
        }

        function getDefaultPageItisettings() {
            return $(defaultParentPage.itisettings).text();
        }


        function imgRefresh_click() {
            var itiSettings = JSON.parse(getDefaultPageItisettings());

            updateAndSetCurrentQFs(itiSettings);
            PageMethods.SaveQuickFiltersToSession(JSON.stringify(itiSettings), imgRefresh_click_Done, on_error);
            refreshPage(false);
        }

        function saveQFtoSession() {
             var itiSettings = JSON.parse(getDefaultPageItisettings());
            updateAndSetCurrentQFs(itiSettings);
            PageMethods.SaveQuickFiltersToSession(JSON.stringify(itiSettings), SaveQuickFiltersToSession_done, on_error);

        }

        function SaveQuickFiltersToSession_done() {

        }
        function imgRefresh_click_Done(results) {

            refreshPage(false);
        }

        function updateAndSetCurrentQFs(itiSettings_Param) {

            itiSettings_Param.QFContract = ddlContract_get();
            itiSettings_Param.QFRelease = ddlRelease_get();
            itiSettings_Param.QFAORType = ddlAORType_get();
            itiSettings_Param.QFVisibleToCustomer = ddlVisibleToCustomer_get();
            itiSettings_Param.QFContainsTasks = ddlContainsTasks_get();
            itiSettings_Param.QFTaskStatus = ddlTaskStatus_get();
            itiSettings_Param.QFAORProductionStatus = ddlAORProductionStatus_get();
            itiSettings_Param.QFShowArchiveAOR = $('#<%=Master.FindControl("chk_Item11").ClientID %>')[0].checked;
        }

        function imgQFSave_click() {
            var itiSettings = JSON.parse(getDefaultPageItisettings());
            var gridViewId = $("#<%=ddlGridview.ClientID %> option:selected").val();
            var processView = $("#<%=ddlGridview.ClientID %> option:selected").attr('OptionGroup') === "Custom Views" ? "0" : "1";

            $('#tdSaveQuickFiltersToDB').hide();

            updateAndSetCurrentQFs(itiSettings);
            setDefaultPageItisettings(JSON.stringify(itiSettings))

            PageMethods.SaveQuickFiltersToDB(gridViewId, $(defaultParentPage.itisettings).text(), processView, btnSaveQuickFilters_Done, on_error);
        }

        function btnSaveQuickFilters_Done(results) {
            if (results) alert("Your filters were saved.");
            else alert("Your filters were not saved.");
        }

        function imgSettings_click() {
            openSettings();
        }

        function linkResouces_click(aorid) {
            var nWindow = 'AORResources';
            var nTitle = 'AOR Resources';
            var nHeight = 700, nWidth = 1000;
            var nURL = _pageUrls.Maintenance.AORPopup + window.location.search + '&Type=Resources' + '&AORID=' + aorid;
            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

        function btnMassChange_click() {
            arrSelectedEntity.length = 0;
            $('.masschange').each(function () {
                var $obj = $(this);
                var objVal = '';
                if ($obj.find('input[type=checkbox]').is(':checked')) {
                    var $obj = $(this);
                    if ($obj.attr('strFieldID').indexOf("ID") <= 0) {
                        if ($.inArray($obj.attr('strFieldID'), arrSelectedEntity) == -1) arrSelectedEntity.push({ 'strEntity': $obj.attr('strEntity'), 'strField': $obj.attr('strField'), 'strFieldID': $obj.attr('strFieldID') });
                    }

                }
            });

            var nWindow = 'EntityMassChange';
            var nTitle = 'Mass Change';
            var nHeight = 400, nWidth = 1200;
            var nURL = _pageUrls.Maintenance.MassChangeWizard + window.location.search;
            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

        function btnMoveWorkTask_click() {
            var nWindow = 'MoveWorkTask';
            var nTitle = 'Move Work Task';
            var nHeight = 700, nWidth = 1000;
            var nURL = _pageUrls.Maintenance.AORPopup + window.location.search + '&Type=MoveWorkTask';

            if ($('.selectedRow').length == 1 && '<%=this.DCC.Contains("AOR Name") %>'.toUpperCase() == 'TRUE') {
                var txtAORName = $('.selectedRow td:eq(<%=this.DCC.IndexOf("AOR Name") %>)').find('input[type=text]').val();

                nURL += '&AORName=' + txtAORName;
            }

            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

        function btnAORWizard_click() {
            var nWindow = 'AORWizard';
            var nTitle = 'Add/Edit AOR';
            var nHeight = 400, nWidth = 1200;
            var nURL = _pageUrls.Maintenance.AORWizard + window.location.search;

            if ($('.selectedRow').length == 1 && '<%=this.DCC.Contains("AOR Name") %>'.toUpperCase() == 'TRUE') {
                var txtAORName = $('.selectedRow td:eq(<%=this.DCC.IndexOf("AOR Name") %>)').find('input[type=text]').val();

                nURL += '&AORName=' + txtAORName;
            }

            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

        function btnReleaseBuilder_click() {
            var nWindow = 'ReleaseBuilder';
            var nTitle = 'Release Builder';
            var nHeight = 700, nWidth = 1000;
            var nURL = _pageUrls.Maintenance.AORReleaseBuilder + window.location.search;
            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

        function btnCRReport_click() {
            var nWindow = 'CRReport';
            var nTitle = 'CR Report';
            var nHeight = 700, nWidth = 1000;
            var nURL = _pageUrls.Reports.CR;
            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

        function btnDSEReport_click() {
            var nWindow = 'DSEReport';
            var nTitle = 'DSE Report';
            var nHeight = 700, nWidth = 1000;
            var nURL = _pageUrls.Reports.Release_DSE;
            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

        function btnReleaseAssessment_click() {
            var nWindow = 'ReleaseAssessmentGrid';
            var nTitle = 'Release Assessment Grid';
            var nHeight = 700, nWidth = 1200;
            var nURL = _pageUrls.Maintenance.AORReleaseAssessment + '?GridType=ReleaseAssessment';
            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

        function btnAdd_click() {
            btnAORWizard_click();

            //var nWindow = 'AddAOR';
            //var nTitle = 'Add AOR';
            //var nHeight = 346, nWidth = 1000;
            //var nURL = _pageUrls.Maintenance.AORPopup + window.location.search + '&Type=AOR';
            //var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            //if (openPopup) openPopup.Open();

            //if (parent.showFrameForEdit) {
            //    parent.showFrameForEdit('AOR', true, 0, true);
            //}
            //else {
            //    var nWindow = 'AOR';
            //    var nTitle = 'AOR';
            //    var nHeight = 700, nWidth = 1000;
            //    var nURL = _pageUrls.Maintenance.AORTabs + window.location.search + '&NewAOR=true&AORID=0';
            //    var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            //    if (openPopup) openPopup.Open();
            //}
        }

        function ddlGridView_change() {
            var gridViewId = $("#<%=ddlGridview.ClientID %> option:selected").val();
            var gridView = "AOR";

            PageMethods.GetTier1Data(gridViewId, gridView, ddlGridView_Change_Done, on_error);
        }

        function ddlGridView_Change_Done(results) {
            var itisettings_results = JSON.parse(results);

            setDefaultPageItisettings(results);
            $("#<%= itisettings.ClientID %>").val(results);

            if (itisettings_results.QFRelease !== undefined) {
                $('#<%=Master.FindControl("ms_Item1").ClientID %>').multipleSelect('setSelects', itisettings_results.QFContract.split(',')); //ddlContract
                $('#<%=Master.FindControl("ms_Item0").ClientID %>').multipleSelect('setSelects', itisettings_results.QFRelease.split(',')); //ddlRelease
                $('#<%=Master.FindControl("ms_Item10").ClientID %>').multipleSelect('setSelects', itisettings_results.QFAORType.split(',')); //ddlAORType
                $('#<%=Master.FindControl("ms_Item10a").ClientID %>').multipleSelect('setSelects', itisettings_results.QFVisibleToCustomer.split(',')); //ddlVisibleToCustomer
                $('#<%=Master.FindControl("ms_Item2").ClientID %>').multipleSelect('setSelects', itisettings_results.QFVisibleToCustomer.split(',')); //ddlContainsTasks
                $('#<%=Master.FindControl("ms_Item9").ClientID %>').multipleSelect('setSelects', itisettings_results.QFTaskStatus.split(',')); //ddlTaskStatus
                $('#<%=Master.FindControl("ms_Item9a").ClientID %>').multipleSelect('setSelects', itisettings_results.QFAORProductionStatus.split(',')); //ddlAORProductionStatus
                $('#<%=Master.FindControl("chk_Item11").ClientID %>').checked = itisettings_results.QFShowArchiveAOR;
            }

            refreshPage(false);
        }

        function btnDelete_click() {
            QuestionBox('Confirm AOR Delete', 'Are you sure you want to delete this AOR? All history will be lost and it will be disassociated from all meetings.', 'Yes,No', 'confirmAORDelete', 300, 300, this);
        }

        function confirmAORDelete(answer) {
            if ($.trim(answer).toUpperCase() == 'YES') {
                try {
                    ShowDimmer(true, 'Deleting...', 1);

                    PageMethods.DeleteAOR(_selectedAORID, delete_done, on_error);
                }
                catch (e) {
                    ShowDimmer(false);
                    MessageBox('An error has occurred.');
                }
            }
        }

        function delete_done(result) {
            ShowDimmer(false);

            var blnDeleted = false;
            var errorMsg = '';
            var obj = $.parseJSON(result);

            if (obj) {
                if (obj.deleted && obj.deleted.toUpperCase() == 'TRUE') blnDeleted = true;
                if (obj.error) errorMsg = obj.error;
            }

            if (blnDeleted) {
                MessageBox('AOR has been deleted.');
                refreshPage(true);
            }
            else {
                MessageBox('Failed to delete. <br>' + errorMsg);
            }
        }

        function btnEdit_click() {
            var obj = parent;

            for (var i = 1; i < parseInt(_currentLevel); i++) {
                obj = obj.parent;
            }

            if (obj.showFrameForEdit) {
                obj.showFrameForEdit('AOR', false, _selectedAORID, true, _selectedAORReleaseID);
            }
            else {
                var nWindow = 'AOR';
                var nTitle = 'AOR';
                var nHeight = 700, nWidth = 1000;
                var nURL = _pageUrls.Maintenance.AORTabs + window.location.search + '&NewAOR=false&AORID=' + _selectedAORID + '&AORReleaseID=' + _selectedAORReleaseID;
                var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

                if (openPopup) openPopup.Open();
            }
        }

        function btnSave_click() {
            try {
                var validation = validate();

                if (validation.length == 0) {
                    var arrChanges = [];

                    $('span[fieldChanged="1"],textarea[fieldChanged="1"],input[fieldChanged="1"], select[fieldChanged="1"]').each(function() {
                        var $obj = $(this);
                        var objVal = '';
                        if ($obj.find('input[type=checkbox]').is(':checkbox')){
                            objVal = $obj.find('input[type=checkbox]').prop('checked')? 1: 0;
                        }
                        else{
                            objVal = $obj.val();
                        }

                        arrChanges.push({'typeName': $obj.attr('typeName'),'typeID': $obj.attr('typeID'), 'field': $obj.attr('field'), 'value': objVal});
                    });

                    if (arrChanges.length > 0) {
                        ShowDimmer(true, 'Saving...', 1);

                        var nJSON = '{update:' + JSON.stringify(arrChanges) + '}';

                        PageMethods.SaveChanges(nJSON, save_done, on_error);
                    }
                    else {
                        MessageBox('You have not made any changes.');
                    }
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

            var blnSaved = false;
            var errorMsg = '';
            var obj = $.parseJSON(result);

            if (obj) {
                if (obj.saved && obj.saved.toUpperCase() == 'TRUE') blnSaved = true;
                if (obj.error) errorMsg = obj.error;
            }

            if (blnSaved) {
                var fieldCount = $('span[fieldChanged="1"],textarea[fieldChanged="1"],input[fieldChanged="1"], select[fieldChanged="1"]').length;
                var rowCount = $('tr[rowChanged="1"]').length;

                MessageBox(fieldCount + ' item(s) in ' + rowCount + ' row(s) have been saved.');
                refreshPage(true);
            }
            else {
                MessageBox('Failed to save. <br>' + errorMsg);
            }
        }

        function on_error() {
            ShowDimmer(false);
            MessageBox('An error has occurred.');
        }

        function validate() {
            var validation = [];

            $.each($('#<%=this.grdData.ClientID %>_BodyContainer table input[field="AOR Name"]'), function() {
                var nText = $(this).val();

                if (nText.length == 0) {
                    if ($.inArray('AOR Name cannot be empty.', validation) == -1) validation.push('AOR Name cannot be empty.');
                }

                if ($('#<%=this.grdData.ClientID %>_BodyContainer table input[field="AOR Name"][value="' + nText + '"]').not($(this)).length > 0) {
                    if ($.inArray('AOR Name cannot have duplicates.', validation) == -1) validation.push('AOR Name cannot have duplicates.');
                }
            });

            return validation.join('<br>');
        }

        function imgExport_click() {
            var url = window.location.href;
            url = editQueryStringValue(url, 'Export', true);
            window.location.href = url;
        }

        function openSettings() {
            if (parent.openSettings) {
                parent.openSettings();
                return;
            }

            var nTitle = 'AOR PARAMETERS';
            var nHeight = 800, nWidth = 700;
            var nURL = 'ITI_Settings.aspx?GridView=AOR&ActiveTab=1';
            var openPopup = popupManager.AddPopupWindow('ITI_Settings', nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

        function displayAllRows(obj) {
            var $obj = $(obj);

            $('#<%=this.grdData.ClientID %>_BodyContainer table td img').each(function () {
                $(this).click();
            });

            if ($obj.attr('title') == 'Expand') {
                $obj.attr('src', 'Images/Icons/minus_blue.png');
                $obj.attr('title', 'Collapse');
                $obj.attr('alt', 'Collapse');
            }
            else {
                $obj.attr('src', 'Images/Icons/add_blue.png');
                $obj.attr('title', 'Expand');
                $obj.attr('alt', 'Expand');

            }
        }

        function displayNextRow(obj) {
            var $obj = $(obj);

            if ($obj.attr('title') != 'Loading...') {
                var $row = $obj.closest('tr');
                var blnShow = true;

                if ($row.next().find('iframe').length === 0) {
                    $obj.attr('src', 'Images/Loaders/loader_2.gif');
                    $obj.attr('title', 'Loading...');
                    $obj.attr('alt', 'Loading...');
                    $obj.css('cursor', 'default');

                    var nURL = '';

                    if (_currentLevel === '<%=this.LevelCount %>') {
                        return;
                    }
                    else {
                        nURL = 'AOR_Grid.aspx' + window.location.search;
                        nURL = editQueryStringValue(nURL, 'CurrentLevel', (parseInt(_currentLevel) + 1));
                    }

                    var filters = [];
                    var headerIndex = 1;

                    if ('<%=hasDoubleHeader %>' == 'True'){
                        headerIndex = 2;
                    }

                    $.each($row.find('td'), function(i) {
                        var nText = $('.gridHeader:eq(' + headerIndex + ') th:eq(' + i + ')').text();
                        var nVal = encodeURIComponent($(this).text());

                        if (nText.match(/_ID$/)) filters.push(nText + '=' + nVal);
                    });

                    nURL = editQueryStringValue(nURL, 'Filter', ('<%=this.Filter %>' !== '' ? encodeURIComponent('<%=this.Filter %>|') : '') + filters.join('|'));

                    var nHTML = '<tr>';
                    nHTML += '<td colspan=' + $row.find('td:visible').length + ' style="padding-top: 5px; border: none; border-bottom: 1px solid grey;">';
                    nHTML += '<table style="border-collapse: collapse; width: 100%;">';
                    nHTML += '<tr>';
                    nHTML += '<td style="width: 15px; vertical-align: top;">';
                    nHTML += '<img src="Images/Icons/tree_branch.gif" alt="Child Grid" />';
                    nHTML += '</td>';
                    nHTML += '<td>';
                    nHTML += '<iframe src=' + nURL + ' width="100%" height="' + $row.height() + 'px" frameBorder="0"></iframe>';
                    nHTML += '</td>';
                    nHTML += '</tr>';
                    nHTML += '</table>';
                    nHTML += '</td>';
                    nHTML += '</tr>';

                    $(nHTML).insertAfter($row);
                }
                else {
                    if ($row.next().is(':visible')) blnShow = false;
                }

                if (blnShow) {
                    if ($obj.attr('title') == 'Expand') {
                        $obj.attr('src', 'Images/Icons/minus_blue.png');
                        $obj.attr('title', 'Collapse');
                        $obj.attr('alt', 'Collapse');
                    }

                    $row.next().show();
                }
                else {
                    $obj.attr('src', 'Images/Icons/add_blue.png');
                    $obj.attr('title', 'Expand');
                    $obj.attr('alt', 'Expand');
                    $row.next().hide();
                }

                resizeFrame();
            }
        }

        function row_click(obj) {
            if ($(obj).attr('aor_id')) {
                _selectedAORID = $(obj).attr('aor_id');

                $('#btnDelete').prop('disabled', false);
                $('#btnEdit').prop('disabled', false);
            }

            if ($(obj).attr('aorrelease_id')) {
                _selectedAORReleaseID = $(obj).attr('aorrelease_id');

                $('#btnEdit').prop('disabled', false);
            }
        }

        function chkAll(obj) {
            var $obj = $(obj);
            var blnChecked = $(obj).find('input').is(':checked')
            $('.masschange').find('input[type=checkbox]').prop('checked', blnChecked);
            //$('.masschange').find('input[type=checkbox]').parent().attr('fieldChanged', '1');

                if ($(obj).is(':checked')) blnAllChecked = true;
                else blnAllChecked = false;

                $('#buttonMove').prop('disabled', false);
        }

        function input_change(obj) {
            var $obj = $(obj);

            switch($obj.attr('field')) {
            case 'Description':
                autosize($obj);
                resizeGrid();
                break;
            case 'Sort':
                var nVal = $obj.val();
                var blnNegative = nVal.indexOf('-') != -1 ? true : false;

                nVal = nVal.replace(/[^\d]/g, '');

                if (blnNegative) nVal = '-' + nVal;

                $obj.val(nVal);
                break;
            case 'Rank':
                var nVal = $obj.val();

                $obj.val(nVal.replace(/[^\d]/g, ''));
                break;
            }

            $obj.attr('fieldChanged', '1');
            $obj.closest('tr').attr('rowChanged', '1');
            $('#btnSave').prop('disabled', false);

        }

        function txtBox_blur(obj) {
            var $obj = $(obj);
            var nVal = $obj.val();

            switch($obj.attr('field')) {
            case 'Sort':
                if (nVal == '-') $obj.val('');
                return;
            }

            $obj.val($.trim(nVal));
        }

        function lbEditWorkItem_click(recordId) {
            var UseLocal = false; //to use local storage enabling next/previous on task edit page, or to not, that is the question.

            var obj = parent;

            for (var i = 1; i < parseInt(_currentLevel); i++) {
                obj = obj.parent;
            }

            if (obj.showFrameForEdit) {
                obj.showFrameForEdit('TASK', false, recordId, true);
            }
            else {
                var title = '', url = '';
                var h = 700, w = 1400;

                title = 'Primary Task - [' + recordId + ']';
                url = _pageUrls.Maintenance.WorkItemEditParent
                    + '?WorkItemID=' + recordId
                    + '&UseLocal=True';

                //open in a popup
                var openPopup = popupManager.AddPopupWindow('WorkItem', title, 'Loading.aspx?Page=' + url, h, w, 'PopupWindow', this);
                if (openPopup) {
                    openPopup.Open();
                }
            }
        }

        function openWorkItem(workItemID, taskNumber, taskID, blnSubTask) {
            var nWindow = 'WorkTask';
            var nTitle = 'Work Task';
            var nHeight = 700, nWidth = 1400;
            var nURL = _pageUrls.Maintenance.WorkItemEditParent;

            if (parseInt(workItemID) > 0) {
                nTitle += ' - [' + workItemID + ']';
                nURL += '?workItemID=' + workItemID;
            }

            if (blnSubTask == '1') {
                nWindow = 'WorkSubTask';
                nTitle = 'Subtask - [' + workItemID + ' - ' + taskNumber + ']';
                nHeight = 700, nWidth = 850;
                nURL = _pageUrls.Maintenance.TaskEdit + '?workItemID=' + workItemID + '&taskID=' + taskID;
            }

            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

        function openCR(CRID) {
            var nWindow = 'CR';
            var nTitle = 'CR';
            var nHeight = 700, nWidth = 1000;
            var nURL = _pageUrls.Maintenance.AORCRTabs + window.location.search + '&NewCR=false&CRID=' + CRID;
            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

        function openAttachment(AORID, AORReleaseID) {
            var nWindow = 'Attachments';
            var nTitle = 'AOR # ' + AORID + ' - Attachments';
            var nHeight = 700, nWidth = 1200;
            var nURL = _pageUrls.Maintenance.AORAttachments + window.location.search + '&NewAOR=false&AORID=' + AORID + '&AORReleaseID=' + AORReleaseID;
            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

        function refreshPage(blnRetainPageIndex) {
            if (getDefaultPageItisettings() !== "") {
                $("#<%=itisettings.ClientID %>").val(getDefaultPageItisettings());
            }
            PageMethods.UpdateSession(getDefaultPageItisettings(), $('[id$=hdnGridSessionKey]').val(), UpdateSession_Done, on_error, blnRetainPageIndex);
        }

        function UpdateSession_Done(isPageMethodSuccess, successParameter, pageMethodName) {
            var nURL = window.location.href.replace('#', '');
            nURL = editQueryStringValue(nURL, 'GridPageSize', $('#<%=Master.FindControl("ddlItem5").ClientID %>').val());
            nURL = editQueryStringValue(nURL, 'GridPageIndex', successParameter ? '<%=grdData.PageIndex %>' : '0');
            window.location.href = 'Loading.aspx?Page=' + nURL;
        }

        function resizeGrid() {
            setTimeout(function () { <%=this.grdData.ClientID %>.ResizeGrid(); if (parent.resizeFrame) parent.resizeFrame(); }, 1);
        }

        function resizeFrame() {
            var $grid = $('#<%=this.grdData.ClientID %>_Grid');
            var headerTop = itiGrid_getAbsoluteTop($grid[0]);
            var bodyTableHeight = $('#<%=this.grdData.ClientID %>_BodyContainer table').height();
            var pagerHeight = $('#<%=this.grdData.ClientID %>_PagerContainer').is(':visible') ? $('#<%=this.grdData.ClientID %>_PagerContainer').height() : 0;

            if (parseInt(_currentLevel) > 1) {
                var nHeight = headerTop + bodyTableHeight + pagerHeight + 1;
                var nFrame = getMyFrameFromParent();

                $(nFrame).height(nHeight);
                resizeGrid();
                if (parent.resizeFrame) parent.resizeFrame();
            }
            else {
                resizeGrid();

                var bodyHeight = $('#<%=this.grdData.ClientID %>_BodyContainer').height();
                if (bodyTableHeight < bodyHeight) bodyHeight = bodyTableHeight - pagerHeight + 3;
                var pagerTop = headerTop + bodyHeight + pagerHeight - 5;
                $('#<%=this.grdData.ClientID %>_PagerContainer').css('top', pagerTop + 'px');
            }
        }

        function completeLoading() {
            var nFrame = getMyFrameFromParent();
            var $obj = $(nFrame).parents().eq(5).prev().find('td:eq(0) img'); //innerTD, innerTR, innerTBODY, innerTABLE, outerTD, outerTR, previousOuterTR

            $obj.attr('src', 'Images/Icons/minus_blue.png');
            $obj.attr('title', 'Collapse');
            $obj.attr('alt', 'Collapse');
            $obj.css('cursor', 'pointer');
        }

        function btnSearch_click() {
            PageMethods.Search($('#txtAORSearch').val(), search_done, search_on_error);
        }

        function search_done(result) {
            var dt = jQuery.parseJSON(result);

            if (dt != null && dt.length === 1) {
                _selectedAORID = dt[0].AORID;
                _selectedAORReleaseID = dt[0].AORRelease_ID;
                btnEdit_click();
            } else {
                MessageBox('Could not find AOR: ' + $('#txtAORSearch').val());
            }
        }

        function search_on_error() {
            MessageBox('Could not find AOR: ' + $('#txtAORSearch').val());
        }

        function formatSearch(obj) {
            var text = $(obj).val();

            if (/[^0-9]|^0+(?!$)/g.test(text)) {
                $(obj).val(text.replace(/[^0-9]|^0+(?!$)/g, ''));
            }
        }

        function key_press(obj) {
            if (event.keyCode == 13 || event.keyCode == 144) {
                $('#btnSearch').trigger('click');
                $('#<%=menuRelatedItems.ClientID %>').trigger('click'); // Temp fix, iti_tools_sharp menu seems to be a bit buggy on keypress
            }
        }
    </script>

    <script id="jsInit" type="text/javascript">
        function initVariables() {
            _pageUrls = new PageURLs();
            _currentLevel = '<%=this.CurrentLevel %>';
        }

        function initDisplay() {
            $('#imgSort').hide();

            $('#trClearAll').hide();
            $('#tdQuickFilters').show();

            $('.itiMenuText:contains("Release Builder")').closest('tr').hide();


            if ('<%=this.CanEditAOR %>'.toUpperCase() == 'TRUE') {
                if (parseInt(_currentLevel) == 1 || '<%=this.DCC.Contains("AOR_ID") %>'.toUpperCase() === 'TRUE' || '<%=this.DCC.Contains("WORKTASK_ID") %>'.toUpperCase() === 'TRUE' || '<%=this.DCC.Contains("PARENTWORKTASK_ID") %>'.toUpperCase() === 'TRUE' || '<%=this.DCC.Contains("CRNAME_ID") %>'.toUpperCase() === 'TRUE') {
                    $('.itiMenuText:contains("Mass Change")').closest('tr').show();
                }
                if (parseInt(_currentLevel) == 1) {
                    $('.itiMenuText:contains("Release Builder")').closest('tr').show();
                    $('.itiMenuText:contains("Mass Change")').closest('tr').show();
                    $('#btnAdd').show();
                }

                if ($('.saveable').length > 0) {
                    $('#btnDelete').show();
                    $('#btnSave').show();
                }
            }

            if ('<%=this.CanViewAOR %>'.toUpperCase() === 'TRUE' && '<%=this.grdData.Rows.Count %>' !== '0' && '<%=this.DCC.Contains("AOR_ID") %>'.toUpperCase() === 'TRUE') $('#btnEdit').show();

            if (parseInt(_currentLevel) > 1) {
                $('#pageContentHeader').hide();
                $('.itiMenuText:contains("CR Report")').closest('tr').hide();
                if ('<%=this.DCC.Contains("AOR_ID") %>'.toUpperCase() !== 'TRUE') $('#<%=menuRelatedItems.ClientID %>').hide();
                if ('<%=this.DCC.Contains("AOR_ID") %>'.toUpperCase() === 'TRUE' || '<%=this.DCC.Contains("WORKTASK_ID") %>'.toUpperCase() === 'TRUE' || '<%=this.DCC.Contains("PARENTWORKTASK_ID") %>'.toUpperCase() === 'TRUE' || '<%=this.DCC.Contains("CRNAME_ID") %>'.toUpperCase() === 'TRUE') {
                    $('#<%=menuRelatedItems.ClientID %>').show();
                }
                $('#trItem5').show();
                $('#trms_Item0').hide();
                $('#trms_Item1').hide();
                $('#trms_Item2').hide();
                $('#trms_Item9').hide();
                $('#trms_Item10').hide();
                $('#trms_Item10a').hide();
                $('#trchk_Item11').hide();
                $('#imgExport').hide();
                $('#btnSearch').hide();
                $('#txtAORSearch').hide();

                if(<%=gridRowCnt%> <= 12){
                    $('#tdQuickFilters').hide();
                }

                resizeFrame();
                completeLoading();
            }
            else {
                $('#trms_Item0').show();
                $('#trms_Item1').show();
                $('#trms_Item2').show();
                $('#trms_Item9').show();
                $('#trms_Item10').show();
                $('#trms_Item10a').show();
                $('#trchk_Item11').show();
                $('#tdSettings').show();
                $('#trItem5').hide();
                $('#imgExport').show();
                resizeGrid();
            }
        }

        function showQFPrompt(lblMessage) {
            lblMessage.show();
            lblMessage.text('<< Click Refresh icon to apply Quick Filter(s)');
            $('#tdSaveQuickFiltersToDB').show();
        }

        function initEvents() {
            var lblMessage = $('#<%=Master.FindControl("lblMessage").ClientID %>');

            $('#imgExport').click(function () { imgExport_click(); });
            $('#imgQFSave').click(function () { imgQFSave_click(); });
            $('#imgRefresh').click(function () { imgRefresh_click(); });
            $('#imgSettings').click(function () { imgSettings_click(); });
            $('#txtAORSearch').on('keyup', function () {
                formatSearch(this);
                key_press(this);
            });
            $('#btnSearch').click(function () { btnSearch_click(); return false; });
            $('#btnAdd').click(function () { btnAdd_click(); return false; });
            $('#btnDelete').click(function () { btnDelete_click(); return false; });
            $('#btnEdit').click(function () { btnEdit_click(); return false; });
            $('#btnSave').click(function () { btnSave_click(); return false; });
            $('.gridBody').on('click', function () { row_click(this); });
            $('.selectedRow').on('click', function () { row_click(this); });
            $('#btnQuickFilters').click(function () { toggleQuickFilters_click(); });

            //Release
            $('#<%=Master.FindControl("ms_Item0").ClientID %>').multipleSelect({
                placeholder: 'Default'
                , width: 'undefined'
                , onClick: function () { showQFPrompt(lblMessage); },
                onCheckAll: function () { showQFPrompt(lblMessage); }
                ,onOpen: function() {  resizeFrame(); }
            }).change(function () {
                resizeFrame();
                saveQFtoSession();
            });

            $('#<%=Master.FindControl("ddlItem5").ClientID %>').on('change', function () { showQFPrompt(lblMessage); });

            //Contract
            $('#<%=Master.FindControl("ms_Item1").ClientID %>').multipleSelect({
                placeholder: 'Default'
                , width: 'undefined'
                , onClick: function () { showQFPrompt(lblMessage); },
                onCheckAll: function () { showQFPrompt(lblMessage); }
            }).change(function () {
	            resizeFrame();
	            saveQFtoSession();
            });

            //Contains Tasks
            $('#<%=Master.FindControl("ms_Item2").ClientID %>').multipleSelect({
                placeholder: 'Default'
                , width: 'undefined'
                , onClick: function () { showQFPrompt(lblMessage); },
                onCheckAll: function () { showQFPrompt(lblMessage); }
            }).change(function () {
	            resizeFrame();
	            saveQFtoSession();
            });

            //Work Task Status
            $('#<%=Master.FindControl("ms_Item9").ClientID %>').multipleSelect({
                placeholder: 'Default'
                , width: 'undefined'
                , onClick: function () { showQFPrompt(lblMessage); },
                onCheckAll: function () { showQFPrompt(lblMessage); }
            }).change(function () {
	            resizeFrame();
	            saveQFtoSession();
            });

            //Workload Allocation
            $('#<%=Master.FindControl("ms_Item9a").ClientID %>').multipleSelect({
                placeholder: 'Default',
                width: 'undefined',
                onClick: function () { showQFPrompt(lblMessage); },
                onCheckAll: function () { showQFPrompt(lblMessage); }
            }).change(function () {
	            resizeFrame();
	            saveQFtoSession();
            });

            //AOR Workload Type
            $('#<%=Master.FindControl("ms_Item10").ClientID %>').multipleSelect({
                placeholder: 'Default'
                , width: 'undefined'
                , onClick: function () { showQFPrompt(lblMessage); },
                onCheckAll: function () { showQFPrompt(lblMessage); }
                ,onOpen: function() { resizeFrame(); }
            }).change(function () {
                resizeFrame();
                saveQFtoSession();
             });

            //Visible To Customer
            $('#<%=Master.FindControl("ms_Item10a").ClientID %>').multipleSelect({
                placeholder: 'Default'
                , width: 'undefined'
                , onClick: function () { showQFPrompt(lblMessage); },
                onCheckAll: function () { showQFPrompt(lblMessage); }
                ,onOpen: function() { resizeFrame(); }
            }).change(function () {
	            resizeFrame();
	            saveQFtoSession();
            });

            $('#<%=Master.FindControl("chk_Item11").ClientID %>').on("click",function () {saveQFtoSession();});

            lblMessage.hide();
            $('#tdSaveQuickFiltersToDB').hide();
        }


        $(document).ready(function () {
            if (getDefaultPageItisettings() === "") {
                setDefaultPageItisettings($("#<%=itisettings.ClientID %>").val());
            }
            if ('<%=IsConfigured.ToString().ToUpper() %>' === 'FALSE') openSettings();
            var itisettings = JSON.parse(getDefaultPageItisettings());
            initVariables();
            initDisplay();
            initEvents();

            $('#<%=ddlGridview.ClientID %>').on("change", function () { ddlGridView_change(); return false; });
            $("#<%=ddlGridview.ClientID %> option[OptionGroup='Process Views']").wrapAll("<optgroup label='Process Views'>");
            $("#<%=ddlGridview.ClientID %> option[OptionGroup='Custom Views']").wrapAll("<optgroup label='Custom Views'>");
            $('#<%=ddlGridview.ClientID %> option').filter(function () { return $.trim($(this).text()) === itisettings.viewname; }).prop('selected', true);
        });
    </script>
</asp:Content>
