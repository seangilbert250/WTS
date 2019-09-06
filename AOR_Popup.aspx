﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AOR_Popup.aspx.cs" Inherits="AOR_Popup" MasterPageFile="~/Grids.master" Theme="Default" %>
<%@ Register Src="~/Controls/MultiSelect.ascx" TagPrefix="wts" TagName="MultiSelect" %>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" runat="Server">AOR</asp:Content>
<asp:Content ID="cpHead" ContentPlaceHolderID="head" runat="Server">
    <!-- Copyright (c) 2017 Infinite Technologies, Inc. -->
	<script type="text/javascript" src="Scripts/filter.js"></script>
</asp:Content>
<asp:Content ID="cpHeaderText" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="Server">
    <span><%=this.Type == "CR AOR" ? "AOR" : this.Type %></span>
</asp:Content>
<asp:Content ID="cpPageContentInfo" ContentPlaceHolderID="cphPageContentInfo" runat="Server">
    <table style="width: 100%;">
		<tr>
			<td>
                <span id="spnRelease" style="padding-right: 5px; display: none;">Release:&nbsp;<asp:DropDownList ID="ddlReleaseQF" runat="server" Width="100px"></asp:DropDownList></span>
                <span id="spnFieldChanged" style="display: none;">Field Changed:&nbsp;<asp:DropDownList ID="ddlFieldChangedQF" runat="server" Width="205px"></asp:DropDownList></span>
                <span id="spnTxtSearch" style="display: none">Search by Name: <asp:TextBox ID="txtSearch" runat="server" MaxLength="255"></asp:TextBox></span>
                <input type="button" id="btnSearchClear" value="Clear" style="display:none" />&nbsp;&nbsp;
                <input type="button" id="btnAddNew" value="Add" style="vertical-align:middle;display:none" /> <!-- this is a true add new item button that pops open new windows -->
                <input type="button" id="btnAdd" value="Add" disabled="disabled" style="vertical-align: middle; display: none;" /> <!-- this behaves more like a save button -->
                <input type="button" id="btnSelect" value="Select" style="vertical-align: middle; display: none;" />
			</td>
		</tr>
	</table>
</asp:Content>
<asp:Content ID="cpBody" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
    <iti_Tools_Sharp:Grid ID="grdData" runat="server" AllowPaging="true" PageSize="25" FirstPageText="First" LastPageText="Last" PagerMode="NumericFirstLast" AllowResize="true"
		CssClass="grid" BodyCssClass="gridBody" SelectedRowCssClass="selectedRow" HeaderCssClass="gridHeader" PagerCssClass="gridPager" FooterCssClass="gridPager" AlternatingRowColor="#dfdfdf">
	</iti_Tools_Sharp:Grid>
    <table id="tblCRs" style="width: 100%; border-collapse:collapse; display: none;">
        <tr>
            <td style="width: 25px;">
                <img id="imgShowFiltersCR" src="Images/Icons/funnel.png" title="Assign Filters" alt="Assign Filters" width="15" height="15" style="cursor: pointer; margin: 4px; visibility: hidden;" />
            </td>
            <td>
                <span id="spnFilterCountCR" style="visibility: hidden;">0 Filters Applied</span>
            </td>
            <td style="text-align: right; padding-right: 5px;">
                <span id="spnSelectedCountCR">0 CRs Checked</span>
            </td>
        </tr>
    </table>
    <iframe id="frmCRs" src="javascript:'';" frameborder="0" scrolling="no" style="width: 100%; height: 617px; display: none;"></iframe>
    <table id="tblTasks" style="width: 100%; border-collapse:collapse; display: none;">
        <tr>
            <td style="width: 25px;">
                <img id="imgShowFilters" src="Images/Icons/funnel.png" title="Assign Filters" alt="Assign Filters" width="15" height="15" style="cursor: pointer; margin: 4px;" />
            </td>
            <td style="width: 160px;">
                <span id="spnFilterCount">0 Filters Applied</span>
            </td>
            <td>
                Work Task #:&nbsp;<input type="text" id="txtTaskSearch" placeholder="Search for multiple work tasks by comma separating work task #s." style="width: 350px;" />&nbsp;<input type="button" id="btnTaskSearch" value="Search" />
            </td>
            <td style="text-align: right; padding-right: 5px;">
                <span id="spnSelectedCount">0 Work Tasks Checked</span>
            </td>
        </tr>
    </table>
    <div id="divSelectedLinks" style="position:absolute; background-color:white; border: 1px solid black; overflow:auto; display: none; margin-right:0">
        <span></span>
        <a href="javascript: showSelectedTaskTR(false)" >Close</a>
    </div>
    <table id="tblSubTasks" style="width: 100%; border-collapse:collapse; display: none;">
        <tr>
            <td style="width: 25px;">
                Status:
            </td>
            <td style="width: 160px;">
                <wts:MultiSelect runat="server" ID="msSubTaskStatus" Visible="false"
                    ItemLabelColumnName="STATUS"
                    ItemValueColumnName="STATUSID"
                    ItemSelectedColumnName="DefaultSelected"
                    CustomAttributes=""
                    IsOpen="false"
                    KeepOpen="false"
                    Width="100%"
                    MaxHeight="330"
                    HideDDLButton="true"
                    />
            </td>
        </tr>
    </table>
    <table id="tblSystems" style="width: 100%; border-collapse:collapse; display: none;">
        <tr>
            <td style="width: 25px;">
                System:
            </td>
            <td style="width: 160px;">
                <wts:MultiSelect runat="server" ID="msSystem" Visible="false"
                    ItemLabelColumnName="WTS_SYSTEM"
                    ItemValueColumnName="WTS_SystemID"
                    ItemSelectedColumnName="DefaultSelected"
                    CustomAttributes=""
                    IsOpen="false"
                    KeepOpen="false"
                    Width="100%"
                    MaxHeight="330"
                    HideDDLButton="true"
                    />
            </td>
            <td>
                <img id="imgSubTaskRefresh" src="images/icons/arrow_refresh_blue.png" title="Refresh Subtasks" alt="Refresh Subtasks" width="15" height="15" style="cursor:pointer; margin:4px;" />
            </td>
            <td style="text-align: right; padding-right: 5px;">
                <span id="spnSubTaskSelectedCount">0 Subtasks Checked</span>
            </td>
        </tr>
    </table>

    <iframe id="frmResources" src="javascript:'';" frameborder="0" scrolling="no" style="width: 100%; height: 617px; display: none;"></iframe>
	<div id="divResources" style="padding: 10px; display: none;">
		<table style="width: 100%;">
			<tr>
				<td>
					<div id="divAORResources" runat="server"></div>
				</td>
			</tr>
		</table>
	</div>
    <div id="divSelectedAORMove" class="pageContentHeader" style="display: none;">
        <table style="border-collapse: collapse; width: 100%;">
                <tr>
                    <td style="width: 50px;">
                        From AOR:
                    </td>
                    <td style="width: 300px;">
                        <span id="selectedFromAOR"></span> 
                    </td>
                    <td style="width: 50px;">
                        To AOR:
                    </td>
                    <td style="width: 300px;">
                        <span id="selectedToAOR"></span> 
                    </td>
                </tr>
            </table>
    </div>
    <div id="divMoveWorkTask" style="display: none;">
        <div class="pageContentHeader" style="padding: 5px;">
            Select AOR
		</div>
        <div style="padding: 10px;">
            <table style="border-collapse: collapse; width: 100%;">
                <tr>
                    <td style="width: 70px;">
                        AOR Name:
                    </td>
                    <td>
                        <asp:TextBox ID="txtAORName" runat="server" MaxLength="150" Width="98%"></asp:TextBox>
                    </td>
                    <td style="width: 95px;">
                        <input type="button" id="btnSearch" value="Search" />
                        <input type="button" id="btnClear" value="Clear" />
                    </td>
                </tr>
            </table>
        </div>
        <div id="divAORMoveFrom" class="step" style="height: 545px; overflow-x: hidden; overflow-y: auto; text-align: center; display:none;">Please click the Search button to select an AOR.</div>     
        <div id="divAORMoveTo" class="step" style="height: 545px; overflow-x: hidden; overflow-y: auto; text-align: center;display:none;">Please click the Search button to select an AOR.</div>     
    </div>
    <div id="divFooter" class="PopupFooter" style="display: none;">
            <table style="border-collapse: collapse; width: 100%;">
                <tr>
                    <td style="text-align: right;">
                        <input type="button" id="btnBack" value="Back" style="display: none;" />
                        <input type="button" id="btnNext" value="Next" disabled="disabled" style="display: none;" />
                        <input type="button" id="btnSave" value="Save" style="display: none;" />
                    </td>
                </tr>
            </table>
        </div>
    <iframe id="frmTasks" class="step" src="javascript:'';" frameborder="0" scrolling="no" style="width: 100%; height: 617px; display: none;"></iframe>
    <div id="divAttachment" style="padding: 10px; display: none;">
        <table style="width: 100%;">
            <tr>
                <td style="width: 5px;">
                    <span style="color: red;">*</span>
                </td>
                <td style="width: 135px;">
                    Type:
                </td>
                <td>
                    <asp:DropDownList ID="ddlType" runat="server" Width="250px" style="background-color: #F5F6CE;"></asp:DropDownList>
                </td>
                <td style="width: 5px;">
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td>
                    <span style="color: red;">*</span>
                </td>
                <td>
                    Attachment Name:
                </td>
                <td>
                    <asp:TextBox ID="txtAORAttachmentName" runat="server" Width="99%"></asp:TextBox>
                </td>
                <td>
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td>
                    &nbsp;
                </td>
                <td>
                    Description:
                </td>
                <td>
                    <asp:TextBox ID="txtDescription" runat="server" Width="99%"></asp:TextBox>
                </td>
                <td>
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td>
                    <span style="color: red;">*</span>
                </td>
                <td>
                    File:
                </td>
                <td id="fileUploader">
                    <asp:FileUpload ID="fileUpload" runat="server" Width="100%" AllowMultiple="True" />
                </td>
                <td>
                    &nbsp;
                </td>
            </tr>
        </table>
        <asp:Button ID="btnSubmit" runat="server" style="display: none;" />
    </div>
    <iframe id="frmDownload" style="display: none;"></iframe>
    <iframe id="frmPreviousAttachment" class="step" src="javascript:'';" frameborder="0" scrolling="no" style="width: 100%; height: 617px; display: none;"></iframe>
    <div id="divAOR" style="height: 290px; overflow-x: hidden; overflow-y: auto; display: none;">
        <table style="width: 100%; border-collapse: collapse;">
            <tr class="gridHeader">
                <th style="border-top: 1px solid grey; text-align: center; width: 50px;">
                    <a href="" onclick="addAOR(); return false;" style="color: blue;">Add</a>
                </th>
                <th style="border-top: 1px solid grey; text-align: center; width: 300px;">
                    AOR Name
                </th>
                <th style="border-top: 1px solid grey; text-align: center;">
                    Description
                </th>
                <th style="border-top: 1px solid grey; border-right: none; text-align: center; width: 125px;">
                    Release
                </th>
            </tr>
            <tr class="gridBody">
                <td style="text-align: center;">
                    <a href="" onclick="removeAOR(this); return false;" style="color: blue;">Remove</a>
                </td>
                <td style="text-align: center;">
                    <input type="text" maxlength="150" field="AOR Name" style="width: 95%;" />
                </td>
                <td style="text-align: center;">
                    <textarea rows="2" maxlength="500" field="Description" style="width: 95%;"></textarea>
                </td>
                <td style="border-right: none; text-align: center;">
                    <select field="Release" style="width: 95%; background-color: #F5F6CE;"><%=Uri.UnescapeDataString(this.ReleaseOptions) %></select>
                </td>
            </tr>
        </table>
	</div>
    <div id="divArchiveAOR" style="display: none;">
        <table>
            <tr>
                <td style="padding: 5px;">
                    Would you like to copy this AOR's tasks to another AOR?&nbsp;&nbsp;
                    <asp:DropDownList ID="ddlCopyTasksToAOR" runat="server" style="background-color: #F5F6CE;">
                        <asp:ListItem Value="0" Text="No" Selected="True"></asp:ListItem>
                        <asp:ListItem Value="1" Text="Yes"></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr id="trCopyTasksToAOR" style="display: none;">
                <td>
                    <asp:RadioButtonList ID="rblCopyTasksToAOR" runat="server">
                        <asp:ListItem Value="0" Text="Existing AOR" Selected="True"></asp:ListItem>
                        <asp:ListItem Value="1" Text="New AOR"></asp:ListItem>
                    </asp:RadioButtonList>
                </td>
            </tr>
            <tr id="trCopyTasksToAORSelect" style="display: none;">
                <td style="padding: 5px;">
                    <asp:DropDownList ID="ddlCopyTasksToAORExisting" runat="server" style="background-color: #F5F6CE; width: 100%;"></asp:DropDownList>
                    <table id="tblCopyTasksToAORNew" style="width: 100%; border-collapse: collapse; display: none;">
                        <tr class="gridHeader">
                            <th style="border-left: 1px solid gray; border-top: 1px solid grey; text-align: center; width: 300px;">
                                AOR Name
                            </th>
                            <th style="border-top: 1px solid grey; text-align: center;">
                                Description
                            </th>
                            <th style="border-top: 1px solid grey; text-align: center; width: 125px;">
                                Release
                            </th>
                        </tr>
                        <tr class="gridBody">
                            <td style="border-left: 1px solid gray; text-align: center;">
                                <asp:TextBox ID="txtCopyTasksToAORName" runat="server" MaxLength="150" style="width: 95%;"></asp:TextBox>
                            </td>
                            <td style="text-align: center;">
                                <asp:TextBox ID="txtCopyTasksToAORDescription" runat="server" Rows="2" MaxLength="500" style="width: 95%;"></asp:TextBox>
                            </td>
                            <td style="text-align: center;">
                                <asp:DropDownList ID="ddlCopyTasksToAORRelease" runat="server" style="width: 95%; background-color: #F5F6CE;"></asp:DropDownList>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
	</div>
    <div id="divAppliedFilters" class="filterContainer" style="display: none;"></div>

    <script src="Scripts/multiselect/jquery.multiple.select.js" type="text/javascript"></script>
	<link rel="stylesheet" href="Styles/multiple-select.css" />

    <asp:ScriptManager ID="scriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>
    
    <script id="jsVariables" type="text/javascript">
        var _pageUrls;
        var filterBox = new filterContainer('divAppliedFilters');
        var _selectedCRStatusesQF = '';
        var _selectedSystemsQF = '';
        var _selectedReleasesQF = '';
        var _selectedDeploymentsQF = '';
        var _selectedCRContractsQF = '';
        var arrCRs = [];
        var arrTasks = [];
        var arrSelectedTaskRows = JSON.stringify([]);
        var arrSubTasks = [];
        var arrResources = [];
        var backupExists = false;
        var _selectedAORID = 0;
        var _selectedAORName = '';
        var _selectedAORReleaseID = 0;
        var _selectedAORWorkType = '';
        var _selectedAORID2 = 0;
        var _selectedAORName2 = '';
        var _selectedAORReleaseID2 = 0;
        var blnNext = false;
    </script>

	<script id="jsEvents" type="text/javascript">
        function toggleQuickFilters_click() {
	        var $imgShowQuickFilters = $('#imgShowQuickFilters');
	        var $imgHideQuickFilters = $('#imgHideQuickFilters');
            var $divQuickFilters = $('#divQuickFilters');
            var addtop = 50;

            $('#trClearAll').hide();

            switch ('<%=this.Type%>') {
                case 'CR':
                    $('#trms_Item0').show();
                    $('#trms_Item10').show();
                    $('#trms_Item10a').show();
                    $('#trms_Item1').show();
                    break;
                case 'Add/Move Deployment AOR':
                    $('#trms_Item0').show();
                    $('#trms_Item2').show();
                    break;
                case 'Release Schedule AOR':
                case 'CR AOR':
                    $('#trms_Item0').show();
                    $('#trms_Item10').show();
                    break;
                default:
                    break;
            }

	        if ($imgShowQuickFilters.is(':visible')) {
                var pos = $imgShowQuickFilters.position();

	            $imgShowQuickFilters.hide();
                $imgHideQuickFilters.show();

	            $divQuickFilters.css({
                    width: '300px',
	                position: 'absolute',
	                top: (pos.top + addtop) + 'px',
	                left: '23px'
	            }).slideDown();
	        }
	        else if ($imgHideQuickFilters.is(':visible')) {
                var pos = $imgHideQuickFilters.position();

	            $imgHideQuickFilters.hide();
	            $imgShowQuickFilters.show();

	            $divQuickFilters.css({
                    width: '300px',
	                position: 'absolute',
	                top: (pos.top + addtop) + 'px',
	                left: '23px'
	            }).slideUp();
            }
	    }

        function imgExport_click() {
            var url = window.location.href;
            url = editQueryStringValue(url, 'Export', true);
            window.location.href = url;
        }

	    function imgRefresh_click() {
	        refreshPage();
        }

	    function ddlCRStatusQF_update() {
            var arrCRStatusQF = $('#<%=Master.FindControl("ms_Item10a").ClientID %>').multipleSelect('getSelects');
			
            _selectedCRStatusesQF = arrCRStatusQF.join(',');
	    }

        function ddlCRContractQF_update() {
            var arrCRContractQF = $('#<%=Master.FindControl("ms_Item1").ClientID %>').multipleSelect('getSelects');

            _selectedCRContractsQF = arrCRContractQF.join(',');
	    }

	    function ddlReleaseQF_change() {
	        refreshPage();
	    }

	    function ddlFieldChangedQF_change() {
	        refreshPage();
	    }

	    function ddlSystemQF_update() {
            var arrSystemQF = $('#<%=Master.FindControl("ms_Item0").ClientID %>').multipleSelect('getSelects');
			
            _selectedSystemsQF = arrSystemQF.join(',');
	    }

	    function ddlReleaseQF_update() {
            var arrReleaseQF = $('#<%=Master.FindControl("ms_Item10").ClientID %>').multipleSelect('getSelects');
			
            _selectedReleasesQF = arrReleaseQF.join(',');
        }

        function ddlDeploymentQF_update() {
            var arrDeploymentQF = $('#<%=Master.FindControl("ms_Item2").ClientID %>').multipleSelect('getSelects');

            _selectedDeploymentsQF = arrDeploymentQF.join(',');
        }

        function btnAddNew_click() {
            
            var usePopup = false;

            var title = 'Add New Item';
            var height = 700;
            var width = 850;
            var url = '';

            switch ('<%=this.Type %>') {
                case 'Task':
                    usePopup = true;
                    title = 'Add Task';
                    height = 700;
                    width = 1400;
                    url = _pageUrls.Maintenance.WorkItemEditParent + '?MyData=false&newItem=true&WorkItemID=0&sourceWorkItemID=0&SelectedStatuses=&SelectedAssigned=&UseLocal=false&ShowPageContentInfo=false';
                    break;
                case 'SubTask':
                    usePopup = true;
                    url = _pageUrls.Maintenance.TaskEdit + '?WorkItemID=<%=TaskID%>&Saved=0&newTask=1&taskID=0&AltRefreshAfterCloseFunction=refreshSubTasks';
                    title = 'Add SubTask';
                    break;
            }

            if (usePopup && url != '') {
                var nWindow = 'AddNewItem';                
                var openPopup = popupManager.AddPopupWindow(nWindow, title, 'Loading.aspx?Page=' + url, height, width, 'PopupWindow', this);

                if (openPopup) openPopup.Open();
            }
        }

        function btnSelect_click() {
            try {
                var arrAdditions = [];
                var validation = validate();

                switch ('<%=this.Type %>') {
                    case 'SubTask':
                        $.each(arrSubTasks, function (index, value) {
                            arrAdditions.push({ 'tasknumber': value });
                        });
                        break;
                    case 'Task':
                        $.each(arrTasks, function (index, value) {
                            arrAdditions.push({ 'taskid': value });
                        });
                        break;
                }

                if ((validation.length == 0 && arrAdditions.length > 0) || '<%=this.Type %>' == 'Resources') {
                    var msg = 'Selecting...';

                    if ('<%=this.Type %>' == 'Task' && '<%=this.SubType%>' == 'SubTask') {
                        msg = 'Selecting...';
                    }
                    else {
                        msg = 'Selecting...';
                    }

                    ShowDimmer(true, msg, 1);                    

                    if ('<%=SelectCallbackFunction%>' != '') {
                        opener['<%=SelectCallbackFunction%>'](arrAdditions);
                        setTimeout(closeWindow, 1);
                        return;
                    }
                    else {
                        
                    }
                }
                else {
                    if (validation.length > 0) {
                        MessageBox('Invalid entries: <br><br>' + validation);
                    }
                    else {
                        MessageBox('Please check at least one item.');
                    }
                }
            }
	        catch (e) {
                ShowDimmer(false);                
	            MessageBox('An error has occurred.');
	        }
        }

	    function btnAdd_click() {
	        try {
	            var arrAdditions = [];
                var validation = validate();
                var intAORID = '<%=this.AORID %>';
                
	            switch ('<%=this.Type %>') {
	            case 'CR':
	                $.each(arrCRs, function (index, value) {
	                    arrAdditions.push({ 'crid': value });
	                });
	                break;
	            case 'SubTask':
	                $.each(arrSubTasks, function (index, value) {
	                    arrAdditions.push({ 'tasknumber': value });
	                });
	                break;
	            case 'Task':                
	            case 'SR Task':
	                $.each(arrTasks, function(index, value) {
	                    arrAdditions.push({ 'taskid': value });
	                });
	                break;
	            case 'Attachment':
	                if (parseInt($('#<%=this.ddlType.ClientID %>').val()) > 0 && $('#<%=this.txtAORAttachmentName.ClientID %>').val().length > 0 && $('#<%=this.fileUpload.ClientID %>').val().length > 0) {
	                    ShowDimmer(true, 'Adding...', 1);
	                    $('#<%=this.btnSubmit.ClientID %>').trigger('click');
	                    return;
	                }
	                else {
	                    MessageBox('Please enter values for required fields.');
                    }
                case 'Previous Attachment':
                    $('#<%=this.grdData.ClientID %>_BodyContainer table input[type="checkbox"]:checked').each(function () {
                        var $obj = $(this).parent();

                        arrAdditions.push({ 'releaseattachmentid': $obj.attr('releaseAttachment_id') });
                    });
                    break;
	            case 'AOR':
	                $('#divAOR tr').not(':first').each(function () {
	                    var $obj = $(this);

	                    if ($obj.find('td').text().indexOf('No AORs') == -1) arrAdditions.push({ 'aorname': $obj.find('input[field="AOR Name"]').val(), 'description': $obj.find('textarea[field="Description"]').val(), 'aorreleaseid': $obj.find('select[field="Release"]').val() });
	                });
	                break;
	            case 'Archive AOR':
	                arrAdditions.push({ 'copytasks': $('#<%=this.ddlCopyTasksToAOR.ClientID %>').val(), 'existingnew': $('#<%=this.rblCopyTasksToAOR.ClientID %> :checked').val(), 'aorreleaseid': $('#<%=this.ddlCopyTasksToAORExisting.ClientID %>').val(), 'aorname': $('#<%=this.txtCopyTasksToAORName.ClientID %>').val(), 'description': $('#<%=this.txtCopyTasksToAORDescription.ClientID %>').val(), 'releaseid': $('#<%=this.ddlCopyTasksToAORRelease.ClientID %>').val() });
	                break;
	            case 'Resources':
	                $.each(arrResources, function (index, value) {
	                    arrAdditions.push(value);
	                });
	                break;
	            case 'MoveSubTask':
	                if (opener.arrSubTasks)
	                    $.each(opener.arrSubTasks, function (index, value) {
	                        arrAdditions.push({ 'subtaskid': value });
	                    });
	                $.each(arrTasks, function(index, value) {
	                    arrAdditions.push({ 'taskid': value });
	                });
                    break;
                case 'MoveWorkTask':
                    intAORID = _selectedAORID2;
                    $.each(arrTasks, function (index, value) {
                        arrAdditions.push({ 'taskid': value });
                    });
                    break;
                case 'Add/Move Deployment AOR':
                    $('#<%=this.grdData.ClientID %>_BodyContainer table input[type="checkbox"]:checked').each(function () {
                        var $obj = $(this).parent();

                        arrAdditions.push({ 'aorreleaseid': $obj.attr('aorrelease_id'), 'deploymentid': $obj.closest('tr').find('select').val(), 'weight': $obj.closest('tr').find('td[weight]').attr('weight') });
                    });
                    break;
                case 'Release Schedule AOR':
	            case 'CR AOR':
	                $('#<%=this.grdData.ClientID %>_BodyContainer table input[type="checkbox"]:checked').each(function () {
	                    var $obj = $(this).parent();

	                    arrAdditions.push({ 'aorreleaseid': $obj.attr('aorrelease_id') });
	                });
                    break;
                case 'Contract':
                    $('#<%=this.grdData.ClientID %>_BodyContainer table input[type="checkbox"]:checked').each(function () {
                        var $obj = $(this).parent();

                        arrAdditions.push({ 'contract_id': $obj.attr('contract_id') });
                    });
                    break;
                case 'Action Team':
                    $('#<%=this.grdData.ClientID %>_BodyContainer table input[type="checkbox"]:checked').each(function () {
                        var $obj = $(this).parent();

                        arrAdditions.push({ 'resourceid': $obj.attr('resource_id') });
                    });
                    break;
                case 'Deployment':
                    $('#<%=this.grdData.ClientID %>_BodyContainer table input[type="checkbox"]:checked').each(function () {
                        var $obj = $(this).parent();

                        arrAdditions.push({'releaseassessmentid': '<%=this.ReleaseAssessmentID%>', 'deploymentid': $obj.attr('deployment_id') });
                    });
                    break;
	            }

	            if ((validation.length == 0 && arrAdditions.length > 0) || '<%=this.Type %>' == 'Resources') {
	                var msg = 'Saving...';
	                if ('<%=this.Type %>' == 'Archive AOR') {
	                    msg = 'Saving...';
	                }
	                else if ('<%=this.Type %>' == 'Task' && '<%=this.SubType%>' == 'SubTask') {
	                    msg = 'Selecting...';
	                }
	                else {
	                    msg = 'Adding...';
	                }

	                ShowDimmer(true, msg, 1);

	                var nJSON = '{save:' + JSON.stringify(arrAdditions) + '}';

	                if ('<%=AddCallbackFunction%>' != '') {	                    
	                    opener['<%=AddCallbackFunction%>'](arrAdditions);
	                    setTimeout(closeWindow, 1);
	                    return;
	                }
	                else {
                        PageMethods.Add(intAORID, '<%=this.SRID %>', '<%=this.CRID %>', '<%=this.DeliverableID%>', '<%=this.Type %>', nJSON, add_done, on_error);
	                }
	            }
	            else {
	                if (validation.length > 0) {
	                    MessageBox('Invalid entries: <br><br>' + validation);
	                }
	                else {
	                    MessageBox('Please check at least one to add.');
	                }
	            }
	        }
	        catch (e) {
                ShowDimmer(false);                
	            MessageBox('An error has occurred.');
	        }
	    }

	    function add_done(result) {
	        ShowDimmer(false);

	        var blnSaved = false;
	        var errorMsg = '';
	        var obj = $.parseJSON(result);

	        if (obj) {
	            if (obj.saved && obj.saved.toUpperCase() == 'TRUE') blnSaved = true;
	            if (obj.error) errorMsg = obj.error;
	        }

	        if (blnSaved) {
	            switch ('<%=this.Type %>') {
                case 'Contract':
                case 'Add/Move Deployment AOR':
                case 'Release Schedule AOR':
                    if (opener.parent.opener && opener.parent.opener.refreshPage) opener.parent.opener.refreshPage(true);
                    if (opener.refreshPage) opener.refreshPage(true);
                    break;
                case 'CR':
                case 'Task':
                case 'MoveWorkTask':
	            case 'SR Task':
	            case 'AOR':
                case 'CR AOR':
                case 'Action Team':
                case 'Previous Attachment':
                case 'Deployment':
	                if (opener.refreshPage) opener.refreshPage(true);
	                break;
	            case 'Archive AOR':
	                MessageBox('AOR has been archived.');
	                defaultParentPage.$('#imgAORHome').trigger('click');
	                break;
	            case 'MoveSubTask':
	                if (opener.refreshPage) opener.refreshPage();
	                if (opener.parent.refreshPage) opener.parent.refreshPage();
	                break;
	            }

	            setTimeout(closeWindow, 1);
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

	        switch ('<%=this.Type %>') {
	        case 'AOR':
	            var $rows = $('#divAOR tr').not(':first');
	            var blnExit = false;

	            $.each($rows, function () {
	                if ($(this).text() == 'No AORs') {
	                    validation.push('Please add at least one AOR.');
	                }
	                else {
	                    if (blnExit) return false;

	                    var nText = $(this).find('input[field="AOR Name"]').val();

	                    if (nText.length == 0) {
	                        if ($.inArray('AOR Name cannot be empty.', validation) == -1) validation.push('AOR Name cannot be empty.');
	                    }

	                    $.each($rows.not($(this)), function () {
	                        if ($(this).find('input[field="AOR Name"]').val() == nText) {
	                            validation.push('AOR Name cannot have duplicates.');
	                            blnExit = true;
	                            return false;
	                        }
	                    });
	                }
	            });
	            break;
	        case 'Archive AOR':
	            if ($('#<%=this.ddlCopyTasksToAOR.ClientID %>').val() == '1' && $('#<%=this.rblCopyTasksToAOR.ClientID %> :checked').val() == '1' &&
	                $('#<%=this.txtCopyTasksToAORName.ClientID %>').val().length == 0) validation.push('AOR Name cannot be empty.');

	            if ($('#<%=this.ddlCopyTasksToAOR.ClientID %>').val() == '1' && $('#<%=this.rblCopyTasksToAOR.ClientID %> :checked').val() == '0' &&
	                $('#<%=this.ddlCopyTasksToAORExisting.ClientID %> option:selected').text().length == 0) validation.push('Please select an AOR.');
                break;
            case 'MoveWorkTask':
                    if (_selectedAORID == undefined || _selectedAORID == 0) {
                    validation.push('Please select an AOR to move Work Tasks From.');
                }

                if (_selectedAORID2 == undefined || _selectedAORID2 == 0) {
                    validation.push('Please select an AOR to move Work Tasks To.');
                }

                if (arrTasks.length == 0) {
                    validation.push('Please select one or more Work Tasks to move.');
                }
                break;
	        }

	        return validation.join('<br>');
	    }

	    function addAOR() {
	        var nHTML = '';
	        var $tbl = $('#divAOR table');

	        if ($tbl.find('td').text().indexOf('No AORs') != -1) $tbl.find('tr').not(':first').remove();

	        nHTML += '<tr class=\"gridBody\"><td style=\"text-align: center;\">';
	        nHTML += '<a href=\"\" onclick=\"removeAOR(this); return false;\" style=\"color: blue;\">Remove</a>';
	        nHTML += '</td><td style=\"text-align: center;\">';
	        nHTML += '<input type="text" maxlength="150" field="AOR Name" style="width: 95%;" />';
	        nHTML += '</td><td style=\"text-align: center;\">';
	        nHTML += '<textarea rows="2" maxlength="500" field="Description" style="width: 95%;"></textarea>';
	        nHTML += '</td><td style=\"border-right: none; text-align: center;\">';
	        nHTML += '<select field=\"Release\" style=\"width: 95%; background-color: #F5F6CE;\">' + decodeURIComponent('<%=this.ReleaseOptions %>') + '</select>';
	        nHTML += '</td></tr>';

	        $tbl.find('tr:first').after(nHTML);
	        $('#btnAdd').prop('disabled', false);
	    }

	    function removeAOR(obj) {
	        $(obj).closest('tr').remove();

	        var $tbl = $('#divAOR table');

	        if ($tbl.find('tr').not(':first').length == 0) $tbl.append('<tr class="gridBody"><td colspan="4" style="border-top: 1px solid grey; text-align: center;">No AORs</td></tr>');

	        $('#btnAdd').prop('disabled', false);
	    }

        function imgShowFilters_click() {
            
	        var nWindow = 'FilterPage';
	        var nTitle = 'Filter and Criteria';
	        var nHeight = 450, nWidth = 900;
	        var nURL = 'FilterPage.aspx?parentModule=Work&MyData=False&Source=AOR';
	        var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

	        if (openPopup) openPopup.Open();
	    }

	    function btnTaskSearch_click() {
	        loadGrid();
        }

        function btnSearch_click() {
            switch ($('.step:visible').attr('id')) {
                case 'divAORMoveFrom':
                    $('#divAORMoveFrom').html('<img src="Images/Loaders/loader_2.gif" alt="Loading..." width="15" height="15" />');
                    break;
                case 'divAORMoveTo':
                    $('#divAORMoveTo').html('<img src="Images/Loaders/loader_2.gif" alt="Loading..." width="15" height="15" />');
                    break;
            }
            PageMethods.AORSearch($('#<%=this.txtAORName.ClientID %>').val(), _selectedAORWorkType, search_done, search_on_error);
        }

        function btnClear_click() {
            $('#<%=this.txtAORName.ClientID %>').val('');
            switch ($('.step:visible').attr('id')) {
                case 'divAORMoveFrom':
                    $('#divAORMoveFrom').html('Please click the Search button to select an AOR.');
                    break;
                case 'divAORMoveTo':
                    $('#divAORMoveTo').html('Please click the Search button to select an AOR.');
                    break;
            }
        }
        
        function search_done(result) {
            var nHTML = '';
            var dt = jQuery.parseJSON(result);
            if (dt != null && dt.length > 0) {
                nHTML += '<table style="border-collapse: collapse; width: 100%;">';
                nHTML += '<tr class="gridHeader">';
                nHTML += '<th style="border-top: 1px solid grey; text-align: center; width: 25px;"></th>';
                nHTML += '<th style="border-top: 1px solid grey; text-align: center; width: 50px;">AOR #</th>';
                nHTML += '<th style="border-top: 1px solid grey; text-align: center; ">AOR Name</th>';
                nHTML += '<th style="border-top: 1px solid grey; text-align: center; border-right: none; width: 200px;">AOR Workload Type</th>';
                nHTML += '</tr>';

                var uniqueName = true;
                $.each(dt, function (rowIndex, row) {
                    nHTML += '<tr class="gridBody">';
                    nHTML += '<td style="text-align: center;">';

                    if ('<%=this.CanEditAOR %>'.toUpperCase() == 'TRUE') nHTML += '<input type="checkbox" aorid="' + row.AORID + '" aorreleaseid="' + row.AORRelease_ID + '" field="AOR" onchange="chkAOR_change(this);" />';

                    nHTML += '</td><td style="text-align: center;">';

                    if ('<%=this.CanViewAOR %>'.toUpperCase() == 'TRUE') {
                        nHTML += '<a href="" onclick="openAOR(' + row.AORID + ', 0); return false;" style="color: blue;">' + row.AORID + '</a>';
                    }
                    else {
                        nHTML += row.AORID;
                    }

                    nHTML += '</td><td>' + row.AORName + '</td>';
                    nHTML += '</td><td style="border-right: none;">' + row.AORWorkType + '</td>';
                    nHTML += '</tr>';

                    if ($('#<%=this.txtAORName.ClientID %>').val().toUpperCase() === row.AORName.toUpperCase()) uniqueName = false;
                });

                nHTML += '</table>';
                
            }
            else {
                $('#btnNext').prop('disabled', true);
                nHTML += 'No results found.<br /><br />';
            }
            switch ($('.step:visible').attr('id')) {
                case 'divAORMoveFrom':
                    $('#divAORMoveFrom').html(nHTML);
                    break;
                case 'divAORMoveTo':
                    $('#divAORMoveTo').html(nHTML);
                    break;
            }
        }

        function search_on_error() {
            $('#divAOR').html('Error gathering data.');
        }

        function chkAOR_change(obj) {
            switch ($('.step:visible').attr('id')) {
                case 'divAORMoveFrom':
                    if ($(obj).is(':checked')) {
                            _selectedAORID = parseInt($(obj).attr('aorid'));
                            _selectedAORName = $(obj).closest('tr').find('td:eq(2)').text();
                            $('#selectedFromAOR').text(_selectedAORID + ' - ' + _selectedAORName);
                            _selectedAORReleaseID = parseInt($(obj).attr('aorreleaseid'));
                            _selectedAORWorkType = $(obj).closest('tr').find('td:eq(3)').text();
                            $('#divAORMoveFrom').find('input[type=checkbox][field=AOR]').not($(obj)).prop('checked', false).prop('disabled', true);
                            if ('<%=this.CanEditAOR %>'.toUpperCase() == 'TRUE') $('#btnNext').prop('disabled', false);
                        }
                        else {
                            _selectedAORID = undefined;
                            _selectedAORName = '';
                            _selectedAORReleaseID = undefined;
                            $('#divAORMoveFrom').find('input[type=checkbox][field=AOR]').not($(obj)).prop('disabled', false);
                            $('#btnNext').prop('disabled', true);
                        }
                    break;
                case 'divAORMoveTo':
                    if ($(obj).is(':checked')) {
                        _selectedAORID2 = parseInt($(obj).attr('aorid'));
                        _selectedAORName2 = $(obj).closest('tr').find('td:eq(2)').text();
                        $('#selectedToAOR').text(_selectedAORID2 + ' - ' + _selectedAORName2);
                        _selectedAORReleaseID2 = parseInt($(obj).attr('aorreleaseid'));
                        $('#divAORMoveTo').find('input[type=checkbox][field=AOR]').not($(obj)).prop('checked', false).prop('disabled', true);
                    }
                    else {
                        _selectedAORID2 = undefined;
                        _selectedAORName2 = '';
                        _selectedAORReleaseID2 = undefined;
                        $('#divAORMoveTo').find('input[type=checkbox][field=AOR]').not($(obj)).prop('disabled', false);
                        $('#btnNext').prop('disabled', true);
                    }
                    break;
            }
            if ('<%=this.CanEditAOR %>'.toUpperCase() == 'TRUE') $('#btnNext').prop('disabled', false);
        }

        function btnNext_click() {
            switch ($('.step:visible').attr('id')) {
                case 'divAORMoveFrom':
                    $('#divMoveWorkTask').hide();
                    $('#divAORMoveFrom').hide();
                    $('#frmTasks').show();
                    $('#divSelectedAORMove').show();
                    ShowDimmer(true, 'Loading...', 1);
                    $('#frmTasks').attr('src', 'AOR_Tasks_Add.aspx' + window.location.search + '&AORID=' + _selectedAORID + '&AORReleaseID=' + _selectedAORReleaseID);
                    $('#btnBack').show();
                    $('#btnNext').show();
                    blnNext = true;
                    break;
                case 'frmTasks':
                    $('#frmTasks').hide();
                    $('#divMoveWorkTask').show();
                    $('#divAORMoveTo').show();
                    $('#divFooter').show();
                    $('#btnBack').show();
                    $('#btnNext').show();
                    $('#<%=this.txtAORName.ClientID %>').val('');
                    btnSearch_click();
                    break;
                case 'divAORMoveTo':
                    $('#divMoveWorkTask').hide();
                    $('#divAORMoveFromTo').hide();
                    $('#frmTasks').show();
                    $('#divSelectedAORMove').show();
                    $('#btnBack').show();
                    $('#btnSave').show();
                    $('#btnNext').hide();
                    break;
            }
        }

        function btnBack_click() {
            switch ($('.step:visible').attr('id')) {
                case 'divAORMoveFrom':
                    $('#divMoveWorkTask').hide();
                    $('#divAORMoveFrom').hide();
                    $('#frmTasks').show();
                    $('#btnSave').hide();
                    blnNext = false;
                    break;
                case 'frmTasks':
                    $('#frmTasks').hide();
                    $('#divMoveWorkTask').show();
                    $('#divAORMoveTo').show();
                    if ($('#btnNext').is(':visible')) {
                        $('#divAORMoveFrom').show();
                        $('#btnBack').hide();
                    } else {
                        $('#divAORMoveTo').show();
                        $('#btnBack').show();
                    }
                    $('#divFooter').show();
                    $('#btnNext').show();
                    $('#btnSave').hide();
                    break;
                case 'divAORMoveTo':
                    $('#divMoveWorkTask').hide();
                    $('#divAORMoveTo').hide();
                    $('#frmTasks').show();
                    $('#btnSave').hide();
                    blnNext = false;
                    break;
            }
        }
        
	    function ddlCopyTasksToAOR_change() {
	        if ($('#<%=this.ddlCopyTasksToAOR.ClientID %>').val() == '1') {
	            $('#trCopyTasksToAOR').show();
	            $('#trCopyTasksToAORSelect').show();
	        }
	        else {
	            $('#trCopyTasksToAOR').hide();
	            $('#trCopyTasksToAORSelect').hide();
	        }
	    }

	    function rblCopyTasksToAOR_change() {
	        if ($('#<%=this.rblCopyTasksToAOR.ClientID %> :checked').val() == '1') {
	            $('#<%=this.ddlCopyTasksToAORExisting.ClientID %>').hide();
	            $('#tblCopyTasksToAORNew').show();
	        }
	        else {
	            $('#<%=this.ddlCopyTasksToAORExisting.ClientID %>').show();
	            $('#tblCopyTasksToAORNew').hide();
	        }
	    }

	    function openAOR(AORID, AORReleaseID) {
	        var nWindow = 'AOR';
	        var nTitle = 'AOR';
	        var nHeight = 700, nWidth = 1400;
	        var nURL = _pageUrls.Maintenance.AORTabs + '?NewAOR=false&AORID=' + AORID + '&AORReleaseID=' + AORReleaseID;
	        var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

	        if (openPopup) openPopup.Open();
        }

        function openDeployment(deploymentID) {
            var nWindow = 'Deployment';
            var nTitle = 'Deployment';
            var nHeight = 700, nWidth = 1000;
            var nURL = _pageUrls.Maintenance.AORScheduledDeliverablesTabs + window.location.search + '&NewDeliverable=false&DeliverableID=' + deploymentID;
            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

        function downloadAORAttachment(AORReleaseAttachmentID) {
            $('#frmDownload').attr('src', 'AOR_Attachments.aspx?Type=Attachment&AORReleaseAttachmentID=' + AORReleaseAttachmentID);
        }

	    function showText(txt, opt) {
	        if (opt == 1) {
	            MessageBox(decodeURIComponent(txt));
	        }
	        else {
	            alert(decodeURIComponent(txt));
	        }
	    }

	    function input_change(obj) {
	        var $obj = $(obj);

	        if ($obj.attr('id') && $obj.attr('id').indexOf('TaskSearch') != -1) {
	            var nVal = $obj.val();

                //$obj.val(nVal.replace(/[^\d,]/g, ''));

                var text = $(obj).val();

                if (/[^0-9-,]|^0+(?!$)/g.test(text)) {
                    $(obj).val(text.replace(/[^0-9-,]|^0+(?!$)/g, ''));
                }
	        }

	        $('#btnAdd').prop('disabled', false);
	    }

	    function txtBox_blur(obj) {
	        var $obj = $(obj);
	        var nVal = $obj.val();

	        $obj.val($.trim(nVal));
	    }

        function refreshPage() {
	        switch ('<%=this.Type %>') {
	        //case 'CR':
	          //  loadCRGrid();
	            //break;
            case 'Task List':
	        case 'Task':
            case 'SubTask':
            case 'SR Task':                    
	            loadGrid();
	            break;
	        case 'Resources':
	            loadResourcesGrid();
	            break;
	        default:
	            var nURL = window.location.href;

	            switch ('<%=this.Type %>') {
                case 'Change History':
	                nURL = editQueryStringValue(nURL, 'ReleaseFilterID', $('#<%=this.ddlReleaseQF.ClientID %>').val());
	                nURL = editQueryStringValue(nURL, 'FieldChangedFilter', $('#<%=this.ddlFieldChangedQF.ClientID %>').val());
                    break;
                case 'CR':
                    // update filter variables
                    ddlCRStatusQF_update();
                    ddlCRContractQF_update();
                    ddlSystemQF_update();
                    ddlReleaseQF_update();

                    $('#<%=Master.FindControl("lblMessage").ClientID %>').hide();
                    nURL = nURL.replace('#', '');
                    nURL = editQueryStringValue(nURL, 'SelectedSystems', _selectedSystemsQF);
                    nURL = editQueryStringValue(nURL, 'SelectedReleases', _selectedReleasesQF);
                    nURL = editQueryStringValue(nURL, 'SelectedCRContracts', _selectedCRContractsQF);
                    nURL = editQueryStringValue(nURL, 'SelectedStatuses', _selectedCRStatusesQF);
                    nURL = editQueryStringValue(nURL, 'txtSearch', $('#<%=txtSearch.ClientID %>').val());
                    break;
                case 'Add/Move Deployment AOR':
                    // update filter variables
                    ddlSystemQF_update();
                    ddlReleaseQF_update();
                    ddlDeploymentQF_update();
                    $('#<%=Master.FindControl("lblMessage").ClientID %>').hide();
                    nURL = nURL.replace('#', '');
                    nURL = editQueryStringValue(nURL, 'SelectedSystems', _selectedSystemsQF);
                    nURL = editQueryStringValue(nURL, 'SelectedReleases', _selectedReleasesQF);
                    nURL = editQueryStringValue(nURL, 'SelectedDeployments', _selectedDeploymentsQF);
                    nURL = editQueryStringValue(nURL, 'txtSearch', $('#<%=txtSearch.ClientID %>').val());
                case 'Release Schedule AOR':
                case 'CR AOR':
                    // update filter variables
                    ddlSystemQF_update();
                    ddlReleaseQF_update();
                    ddlDeploymentQF_update();
                    $('#<%=Master.FindControl("lblMessage").ClientID %>').hide();
                    nURL = nURL.replace('#', '');
	                nURL = editQueryStringValue(nURL, 'SelectedSystems', _selectedSystemsQF);
                    nURL = editQueryStringValue(nURL, 'SelectedReleases', _selectedReleasesQF);
                    nURL = editQueryStringValue(nURL, 'txtSearch', $('#<%=txtSearch.ClientID %>').val());
	                break;
	            }

                window.location.href = 'Loading.aspx?Page=' + nURL;
	            break;
	        }
        }
        
	    function complete(result) {
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
	            if (opener.refreshPage) opener.refreshPage(true);

	            setTimeout(closeWindow, 1);
	        }
	        else if (blnExists) {
	            MessageBox('Attachment Name already exists.');
	        }
            else {
	            MessageBox('Failed to save. <br>' + errorMsg);
	        }
	    }

	    function loadCRGrid() {
	        ShowDimmer(true, 'Loading...', 1);

	        arrCRs = [];
	        $('#frmCRs')[0].contentWindow.loadGrid();
	    }

	    function loadGrid() {
	        ShowDimmer(true, 'Loading...', 1);

	        var filterCount = filterBox.filters.length;
            setFilterSession();
	        $('#spnFilterCount').text(filterCount + ' Filter' + (filterCount != 1 ? 's' : '') + ' Applied');
	        //arrTasks = [];
	        $('#frmTasks')[0].contentWindow.loadGrid();
        }

        function setFilterSession() {
            try {
                $('#divPageDimmer').show();
                
                var filters = null;

                if (filterBox && filterBox.filters && filterBox.filters.length > 0) {
                    filters = filterBox.toJSON({ groups: { ParentModule: 'Work' } });
                }
                    PageMethods.SetFilterSession(filters, setFilterSession_Done, on_error);
                } catch (e) {
                    MessageBox('Error: setFilterSession - ' + e.number + ' : ' + e.message);
                    $('#divPageDimmer').hide();
                }
        }
        function setFilterSession_Done(result) {
            $('#divPageDimmer').hide();

            var obj = jQuery.parseJSON(result);

        }

	    function loadResourcesGrid() {
	        ShowDimmer(true, 'Loading...', 1);

	        arrResources = [];
	        $('#frmResources')[0].contentWindow.loadGrid();
        }

        $('#<%=this.fileUpload.ClientID %>').on("change", function() {
            //No File Chosen / Cancel clicked: Show original File Select.
            if ($('#<%=this.fileUpload.ClientID %>').val().length <= 0) {
                if (backupExists) {
                    chromeRestoreBackup();
                }
            }
        });

        $('#<%=this.fileUpload.ClientID %>').on("click", function () {
            var filechooser = $('#<%=this.fileUpload.ClientID %>');
            if (filechooser.val().length > 0) {
                backupExists = $('#<%=this.fileUpload.ClientID %>').clone();
            }
        });
        function chromeRestoreBackup() {
            $('#<%=this.fileUpload.ClientID %>').remove(); // Remove Old fileuploader 
            backupExists.prependTo($('#fileUploader')); // Append New fileuploader

            // Assign functions to the new fileuploader
            $('#<%=this.fileUpload.ClientID %>').on("change", function () {
                // No File Chosen / Cancel clicked: Show original File Select.
                if ($('#<%=this.fileUpload.ClientID %>').val().length <= 0) {
                    if (backupExists) {
                        chromeRestoreBackup();
                    }
                }
            });
            $('#<%=this.fileUpload.ClientID %>').on("click", function () {
                var filechooser = $('#<%=this.fileUpload.ClientID %>');
                if (filechooser.val().length > 0) {
                    backupExists = $('#<%=this.fileUpload.ClientID %>').clone();
                }
            });
        }

        function showQuickFilterRefreshMessage() {
            var lblMessage = $('#<%=Master.FindControl("lblMessage").ClientID %>');
            lblMessage.show();
            lblMessage.text(_refreshMessage);
        }

        function refreshSubTasks() {
            msSubTaskStatus_close();
            msSystem_close();

            var nURL = 'AOR_SubTasks_Add.aspx' + window.location.search;

            nURL = editQueryStringValue(nURL, 'SelectedStatuses', msSubTaskStatus_getSelections(true).join(','));
            nURL = editQueryStringValue(nURL, 'SelectedSystems', msSystem_getSelections(true).join(','));

	        $('#frmTasks').attr('src', nURL);
        }

        function showSelectedTaskTR(visible, selectedTasks) {
            if (visible) {
                $('#divSelectedLinks').show();
            }
            else {
                $('#divSelectedLinks').hide();
            }
        }
	</script>

    <script id="jsInit" type="text/javascript">
        function initVariables() {
            _pageUrls = new PageURLs();
            _refreshMessage = '<< Click Refresh icon to apply Quick Filter(s)';

            if ('<%=this.SelectedTasks%>' != '') {
                var arr = '<%=this.SelectedTasks%>'.split(',');

                for (var i = 0; i < arr.length; i++) arrTasks.push(arr[i]);
            }

            if ('<%=this.SelectedSubTasks%>' != '') {
                var arr = '<%=this.SelectedSubTasks%>'.split(',');

                for (var i = 0; i < arr.length; i++) arrSubTasks.push(arr[i]);
            }
        }

        function initControls() {
            
            if (typeof filters_AorWork != "undefined") { 
                $.each(filters_AorWork[0], function (rowindex, row) {
                    var filterValues = row.value.split(",");
                    var filterTexts = row.text.split(",");
                    for (var i = 0; i <= filterValues.length - 1; i++) {
                        filterBox.filters.add({ name: rowindex, groups: { ParentModule: "Work", Module: "Custom" } }).parameters.add(filterValues[i], filterTexts[i]);
                    }
                });
                filterBox.toTable({ groups: { ParentModule: "Work" } }, "Module");
            }
        }

        function initDisplay() {
            var lblMessage = $('#<%=Master.FindControl("lblMessage").ClientID %>');

            $('#imgSort').hide();
            $('#imgExport').hide();

            // update filter variables
            ddlCRStatusQF_update();
            ddlCRContractQF_update();
            ddlSystemQF_update();
            ddlReleaseQF_update();
            ddlDeploymentQF_update();
            var nURL = '';
            
            $('#<%=this.grdData.ClientID %>').hide();
            if (('<%=this.Type %>') === "CR AOR" || ('<%=this.Type %>') === "CR" || ('<%=this.Type %>') === "Release Schedule AOR" || ('<%=this.Type %>') === "Add/Move Deployment AOR") $('#tdQuickFilters').show();

            switch ('<%=this.Type %>') {
            case 'Release History':
            case 'Action Team':
                $('#<%=this.grdData.ClientID %>').show();
                break;
            case 'Change History':
                $('#<%=this.grdData.ClientID %>').show();
                $('#spnRelease').show();
                $('#spnFieldChanged').show();
                break;
            case 'CR':
                $('#imgExport').show();
                $('#spnTxtSearch').show();
                $('#btnSearchClear').show();
                $('#tblCRs').show();
                $('#frmCRs').show();
                //$('#spnCRStatus').show();
                //$('#spnCRWebsystem').show();
                ShowDimmer(true, 'Loading...', 1);
                nURL = 'AOR_CRs_Add.aspx' + window.location.search;
                nURL = editQueryStringValue(nURL, 'SelectedSystems', _selectedSystemsQF);
                nURL = editQueryStringValue(nURL, 'SelectedReleases', _selectedReleasesQF);
                nURL = editQueryStringValue(nURL, 'SelectedCRContracts', _selectedCRContractsQF);
                nURL = editQueryStringValue(nURL, 'SelectedStatuses', _selectedCRStatusesQF);

                $('#frmCRs').attr('src', nURL);
                break;
            case 'Task List':
                $('#tblTasks').show();
                $('#frmTasks').show();
                ShowDimmer(true, 'Loading...', 1);
                var src = 'AOR_Tasks_Add.aspx' + window.location.search;
                $('#frmTasks').attr('src', src);
                break;
            case 'Task':
            case 'SubTask':
            case 'SR Task':
                if ('<%=this.Type %>' == 'SubTask') {
                    $('#tblSubTasks').css('display', 'inline');
                    $('#tblSystems').css('display', 'inline');
                }
                else {
                    $('#tblTasks').show();
                }
                $('#frmTasks').show();
                ShowDimmer(true, 'Loading...', 1);
                
                var src = 'AOR_Tasks_Add.aspx' + window.location.search;

                if ('<%=this.SubType%>' == 'SelectTask') {
                    $('#btnSelect').val('Select Task');
                    $('#btnSelect').show();  
                    $('#btnAddNew').show();
                }
                else if ('<%=this.SubType%>' == 'SelectSubTask') {
                    $('#imgRefresh').hide();
                    $('#btnSelect').val('Select Subtask');
                    $('#btnSelect').show();
                    $('#btnAddNew').show();
                    src = 'AOR_SubTasks_Add.aspx' + window.location.search;
                    src = editQueryStringValue(src, 'SelectedStatuses', '0,1,2,3,4,5,7,8,9,11,12,13,64');                    
                }

                $('#frmTasks').attr('src', src);

                break;
            case 'Attachment':
                $('#divAttachment').show();
                break;
            case 'PreviousAttachment':
                $('#<%=this.grdData.ClientID %>').show();
                break;
            case 'AOR':
                $('#btnAdd').val('Save');
                $('#divAOR').show();
                break;
            case 'Archive AOR':
                $('#btnAdd').val('Save');
                $('#divArchiveAOR').show();
                $('#btnAdd').prop('disabled', false);
                break;
            case 'Resources':
                $('#btnAdd').val('Save');
                $('#frmResources').show();
                ShowDimmer(true, 'Loading...', 1);
                $('#frmResources').attr('src', 'AOR_Resources_Add.aspx' + window.location.search);
                break;
            case 'MoveSubTask':
                $('#btnAdd').val('Move');
                $('#tblTasks').show();
                $('#frmTasks').show();
                ShowDimmer(true, 'Loading...', 1);
                $('#frmTasks').attr('src', 'AOR_Tasks_Add.aspx' + window.location.search);
                break;
            case 'MoveWorkTask':
                    $('#divMoveWorkTask').show();
                    $('#divAORMoveFrom').show();
                    $('#divFooter').show();
                    $('#btnNext').show();
                    btnSearch_click();
                break;
            case 'Add/Move Deployment AOR':
                $('#btnAdd').val('Add/Move');
            case 'Release Schedule AOR':
            case 'CR AOR':
                $('#imgExport').show();
                $('#spnTxtSearch').show();
                $('#<%=this.grdData.ClientID %>').show();
                break;
            }

            var showBtnAdd = false;

            if ('<%=this.Type %>' != 'Release History' && '<%=this.Type %>' != 'Change History' && '<%=this.CanEditAOR %>'.toUpperCase() == 'TRUE') {
                showBtnAdd = true;
            }

            if ('<%=this.Type %>' == 'MoveSubTask' && '<%=this.CanEditWorkItem %>'.toUpperCase() == 'TRUE') {
                showBtnAdd = true;
            }

            if ('<%=this.SubType%>' == 'SelectTask' || '<%=this.SubType%>' == 'SelectSubTask' || '<%=this.Type%>' == 'MoveWorkTask' || '<%=this.Type%>' == 'Task List') {
                showBtnAdd = false;
            }

            if (showBtnAdd) {
                $('#btnAdd').show();
            }
            else {
                $('#btnAdd').hide();
            }

            if (<%=this.HideAdd.ToString().ToLower()%>) {
                $('#btnAdd').hide();
                $('#btnAddNew').hide();
            }
        }

        function initEvents() {
            var lblMessage = $('#<%=Master.FindControl("lblMessage").ClientID %>');

            $('#imgExport').click(function () { imgExport_click(); });
            $('#imgRefresh').click(function () { imgRefresh_click(); });
            $('#btnSearch').click(function () { btnSearch_click(); return false; });
            $('#btnClear').click(function () { btnClear_click(); return false; });
            $('#btnBack').click(function () { btnBack_click(); return false; });
            $('#btnNext').click(function () { btnNext_click(); return false; });
            $('#btnSave').click(function () { btnAdd_click(); return false; });
            $('#<%=this.ddlFieldChangedQF.ClientID %>').on('change', function () { ddlFieldChangedQF_change(); });

            $('#btnAddNew').click(function () { btnAddNew_click(); return false; });
            $('#btnSelect').click(function () { btnSelect_click(); return false; });
            $('#btnAdd').click(function () { btnAdd_click(); return false; });
            $('#imgShowFilters').click(function () { imgShowFilters_click(); });
            $('#btnTaskSearch').click(function () { btnTaskSearch_click(); });
            $('#txtTaskSearch').on('keydown', function (event) {
                if (event.keyCode == 13 || event.keyCode == 144) {
                    event.preventDefault();
                    return false;
                }
            });
            $('#txtTaskSearch').on('keyup', function (event) {
                if (event.keyCode == 13 || event.keyCode == 144) $('#btnTaskSearch').trigger('click');
            });

            // when in txtSearch box, don't let enter key submit the form; instead, enter key will do the page refresh from below
            $('#<%=txtSearch.ClientID %>').on('keyup', function (event) {
                $('#<%=Master.FindControl("lblMessage").ClientID %>').show();                
                if (event.keyCode === 13) { refreshPage(); }
            });
            $('#<%=txtSearch.ClientID %>').on('focus', function (event) {
                $('[id$=btnSubmit]').attr('type', 'button');
            });
            $('#<%=txtSearch.ClientID %>').on('blur', function (event) {
                $('[id$=btnSubmit]').attr('type', 'submit'); // reestablish normal submit behavior
            });

            $('input[type="text"], textarea').on('keyup paste', function () { input_change(this); });
            $('input[type="text"], textarea').on('blur', function () { txtBox_blur(this); });
            $('input[type="checkbox"], select').not($('#<%=this.ddlReleaseQF.ClientID %>')).not($('#<%=this.ddlFieldChangedQF.ClientID %>')).on('change', function () { input_change(this); });
            $('#<%=this.ddlCopyTasksToAOR.ClientID %>').on('change', function () { ddlCopyTasksToAOR_change(); });
            $('#<%=this.rblCopyTasksToAOR.ClientID %> input').on('change', function () { rblCopyTasksToAOR_change(); });
            $('#btnSearchClear').on('click', function () {
                if ($('#<%=txtSearch.ClientID %>').val().length > 0) {
                    lblMessage.show();
                    lblMessage.text(_refreshMessage);
                    $('#<%=txtSearch.ClientID %>').val(''); return false;
                }
            });
            $('#btnQuickFilters').click(function () { toggleQuickFilters_click(); });

            var suites = $("#<%=Master.FindControl("ms_Item0").ClientID %> option").map(function () {
                return $(this).attr('OptionGroup');
            }).get();
            var uniqueSuites = _.unique(suites, true);
            $(uniqueSuites).each(function (i, v) {
                $("#<%=Master.FindControl("ms_Item0").ClientID %> option[OptionGroup='" + v + "']").wrapAll("<optgroup label='" + v + "'>");
            });
            
            $('#<%=Master.FindControl("ms_Item0").ClientID %>').multipleSelect({
                placeholder: 'Default',
                width: 'undefined',
                onClick: function () {
                    lblMessage.show();
                    lblMessage.text(_refreshMessage);
                },
                onCheckAll: function () {
                    lblMessage.show();
                    lblMessage.text(_refreshMessage);
                }
            }).change(function () { showQuickFilterRefreshMessage(); ddlSystemQF_update(); });
            
            $('#<%=Master.FindControl("ms_Item10").ClientID %>').multipleSelect({
                placeholder: 'Default',
                width: 'undefined',
                onClick: function () {
                    lblMessage.show();
                    lblMessage.text(_refreshMessage);
                },
                onCheckAll: function () {
                    lblMessage.show();
                    lblMessage.text(_refreshMessage);
                }
            }).change(function () { showQuickFilterRefreshMessage(); ddlReleaseQF_update(); });

            $('#<%=Master.FindControl("ms_Item2").ClientID %>').multipleSelect({
                placeholder: 'Default',
                width: 'undefined',
                onClick: function () {
                    lblMessage.show();
                    lblMessage.text(_refreshMessage);
                },
                onCheckAll: function () {
                    lblMessage.show();
                    lblMessage.text(_refreshMessage);
                }
            }).change(function () { showQuickFilterRefreshMessage(); ddlDeploymentQF_update(); });
                        
            $('#<%=Master.FindControl("ms_Item10a").ClientID %>').multipleSelect({
                placeholder: 'Default',
                width: 'undefined',
                onClick: function () {
                    lblMessage.show();
                    lblMessage.text(_refreshMessage);
                },
                onCheckAll: function () {
                    lblMessage.show();
                    lblMessage.text(_refreshMessage);
                }
            }).change(function () { showQuickFilterRefreshMessage(); ddlCRStatusQF_update(); });

            $('#<%=Master.FindControl("ms_Item1").ClientID %>').multipleSelect({
                placeholder: 'Default',
                width: 'undefined',
                onClick: function () {
                    lblMessage.show();
                    lblMessage.text(_refreshMessage);
                },
                onCheckAll: function () {
                    lblMessage.show();
                    lblMessage.text(_refreshMessage);
                }
            }).change(function () { showQuickFilterRefreshMessage(); ddlCRContractQF_update(); });


            if ('<%=this.Type%>' == 'SubTask') {
                msSubTaskStatus_init();
                msSystem_init();
                $('#imgSubTaskRefresh').on('click', function () { refreshSubTasks(); });
            }
        }

        $(document).ready(function () {
            initVariables();
            initControls();            
            initEvents();
            initDisplay();

            $('#<%=Master.FindControl("lblMessage").ClientID %>').hide();            
        });
    </script>
</asp:Content>