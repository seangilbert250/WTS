﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="RQMT_Edit.aspx.cs" Inherits="RQMT_Edit" MasterPageFile="~/EditTabs.master" Theme="Default" %>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" runat="Server">RQMT</asp:Content>
<asp:Content ID="cpHead" ContentPlaceHolderID="head" runat="Server">
    <!-- Copyright (c) 2017 Infinite Technologies, Inc. -->
    <link rel="stylesheet" type="text/css" href="Styles/tooltip.css" />
    <link rel="stylesheet" href="Scripts/cleditor/jquery.cleditor.css" />
</asp:Content>
<asp:Content ID="cpHeaderImage" ContentPlaceHolderID="ContentPlaceHolderHeaderImage" runat="Server">
    
</asp:Content>
<asp:Content ID="cpHeaderText" ContentPlaceHolderID="ContentPlaceHolderHeaderText" runat="Server"></asp:Content>
<asp:Content ID="cpHeaderButtons" ContentPlaceHolderID="ContentPlaceHolderHeaderButtons" runat="Server">    
	<input type="button" id="btnCancel" value="Close" style="vertical-align: middle; display: none;" />
    <input type="button" id="btnHistory" value="History" disabled="disabled" style="vertical-align:middle; display:none;" />
    <input type="button" id="btnSave" value="Save" disabled="disabled" style="vertical-align: middle; display: none;" />
    <input type="button" id="btnDelete" value="Delete" disabled="disabled" style="vertical-align: middle; display: none;" />
</asp:Content>
<asp:Content ID="cpBody" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
<div id="divPageContainer" style="overflow-x: hidden; overflow-y: auto;padding:2px;">
    <div id="div_detailssectioncontainer" style="padding-bottom:5px;">
        <div class="pageContentHeader" style="vertical-align: top; padding: 5px;">
	        <img class="toggleSection" src="Images/Icons/minus_blue.png" title="Show Section" alt="Show Section" height="12" width="12" data-section="details" style="cursor: pointer;" />&nbsp;&nbsp;<span>Details</span>
        </div>
        <div id="section_details" style="padding-bottom:5px;">
            <table style="width: 100%;">
                <tr>
                    <td style="width: 5px;">
                        &nbsp;
                    </td>
                    <td style="width: 65px;">
                        RQMT #:
                    </td>
                    <td>
                        <span id="spnRQMT" runat="server">-</span>
                        <div id="divInfo" style="float: right; display: none;"><span id="spnCreated" runat="server" style="font-size:smaller;"></span><span id="spnUpdated" runat="server" style="padding-left: 30px;font-size:smaller;"></span></div>
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
                        RQMT:
                    </td>
                    <td>
                        <asp:TextBox ID="txtRQMT" runat="server" MaxLength="150" Width="100%" ReadOnly="true" ForeColor="Gray"></asp:TextBox>
                    </td>
                    <td>
                        &nbsp;
                    </td>
                </tr>
            </table>
        </div>
    </div>

    <div id="div_associationssectioncontainer" style="padding-bottom:5px;position:relative;z-index:1000;display:none;">
	    <div class="pageContentHeader" style="vertical-align: top; padding: 5px;">
		    <img class="toggleSection" src="Images/Icons/add_blue.png" title="Show Section" alt="Show Section" height="12" width="12" data-section="associations" style="cursor: pointer;" />&nbsp;&nbsp;<span>Associations&nbsp;<span id="associations_count">(0)</span></span>
	    </div>
        <div id="section_associations" style="padding-bottom:5px;display:none;">
            <iti_Tools_Sharp:Grid ID="grdAssociations" runat="server" AllowPaging="false" PageSize="100" FirstPageText="First" LastPageText="Last" PagerMode="NumericFirstLast" AllowResize="true"
		        CssClass="grid" BodyCssClass="gridBody" SelectedRowCssClass="selectedRow" HeaderCssClass="gridHeader" PagerCssClass="gridPager" FooterCssClass="gridPager" AlternatingRowColor="#dfdfdf" />
        </div>
    </div>

    <div id="div_attributessectioncontainer" style="padding-bottom:5px;display:none;position:relative;z-index:999;">
	    <div class="pageContentHeader" style="vertical-align: top; padding: 5px;">
		    <img class="toggleSection" src="Images/Icons/add_blue.png" title="Show Section" alt="Show Section" height="12" width="12" data-section="attributes" style="cursor: pointer;" /><span id="spanafd">&nbsp;&nbsp;<span>Attributes&nbsp;<span id="attributes_count">(0)</span> / Functionalities <span id="functionalities_count">(0)</span> / Descriptions <span id="descriptions_count">(0)</span></span></span>
	    </div>
        <div id="section_attributes" style="padding-bottom:5px;display:none;">
            <iti_Tools_Sharp:Grid ID="grdAttributes" runat="server" AllowPaging="false" PageSize="100" FirstPageText="First" LastPageText="Last" PagerMode="NumericFirstLast" AllowResize="true"
		        CssClass="grid" BodyCssClass="gridBody" SelectedRowCssClass="selectedRow" HeaderCssClass="gridHeader" PagerCssClass="gridPager" FooterCssClass="gridPager" AlternatingRowColor="#dfdfdf" />
            <br />
            <iti_Tools_Sharp:Grid ID="grdFunctionalities" runat="server" AllowPaging="false" PageSize="100" FirstPageText="First" LastPageText="Last" PagerMode="NumericFirstLast" AllowResize="true"
		        CssClass="grid" BodyCssClass="gridBody" SelectedRowCssClass="selectedRow" HeaderCssClass="gridHeader" PagerCssClass="gridPager" FooterCssClass="gridPager" AlternatingRowColor="#dfdfdf" />
            <br />
            <iti_Tools_Sharp:Grid ID="grdDescriptions" runat="server" AllowPaging="false" PageSize="100" FirstPageText="First" LastPageText="Last" PagerMode="NumericFirstLast" AllowResize="true"
		        CssClass="grid" BodyCssClass="gridBody" SelectedRowCssClass="selectedRow" HeaderCssClass="gridHeader" PagerCssClass="gridPager" FooterCssClass="gridPager" AlternatingRowColor="#dfdfdf" />
        </div>
    </div>

    <div id="div_usagesectioncontainer" style="padding-bottom:5px;display:none;position:relative;z-index:998;">
	    <div class="pageContentHeader" style="vertical-align: top; padding: 5px;">
		    <img class="toggleSection" src="Images/Icons/add_blue.png" title="Show Section" alt="Show Section" height="12" width="12" data-section="usage" style="cursor: pointer;" />&nbsp;&nbsp;<span>Usage&nbsp;<span id="usage_count">(0)</span></span>
	    </div>
        <div id="section_usage" style="padding-bottom:5px;display:none;">
            <iti_Tools_Sharp:Grid ID="grdUsage" runat="server" AllowPaging="false" PageSize="100" FirstPageText="First" LastPageText="Last" PagerMode="NumericFirstLast" AllowResize="true"
		        CssClass="grid" BodyCssClass="gridBody" SelectedRowCssClass="selectedRow" HeaderCssClass="gridHeader" PagerCssClass="gridPager" FooterCssClass="gridPager" AlternatingRowColor="#dfdfdf" />
        </div>
    </div>

    <div id="div_defectssectioncontainer" style="padding-bottom:5px;display:none;position:relative;z-index:995;">
	    <div class="pageContentHeader" style="vertical-align: top; padding: 5px;">
		    <img class="toggleSection" src="Images/Icons/add_blue.png" title="Show Section" alt="Show Section" height="12" width="12" data-section="defects" style="cursor: pointer;" />&nbsp;&nbsp;<span>Defects&nbsp;<span id="defects_count">(0)</span></span>
	    </div>
        <div id="section_defects" style="padding-bottom:5px;display:none;">
            <iti_Tools_Sharp:Grid ID="grdDefects" runat="server" AllowPaging="false" PageSize="100" FirstPageText="First" LastPageText="Last" PagerMode="NumericFirstLast" AllowResize="true"
		        CssClass="grid" BodyCssClass="gridBody" SelectedRowCssClass="selectedRow" HeaderCssClass="gridHeader" PagerCssClass="gridPager" FooterCssClass="gridPager" AlternatingRowColor="#dfdfdf" />
        </div>
    </div>
</div>

    <asp:ScriptManager ID="scriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>
    <script src="Scripts/cleditor/jquery.cleditor.js"></script>
    <script id="jsEvents" type="text/javascript">
        var RQMTID = <%=RQMTID%>;
        var setAssociationsCount = <%=SetAssociationsCount%>;
        var systemAssociationsCount = <%=SystemAssociationsCount%>;
        var systemDescriptionsCount = <%=SystemDescriptionsCount%>;
        var openSections = '<%=OpenSections%>';
        var hideNonOpenSections = '<%=HideNonOpenSections%>' == '1';
        var descTypeOptions = '<%=descTypeOptions%>';
        var sectionCounts = '<%=sectionCounts%>';
        var displayItemSubSection = '<%=DisplayItemSubSection%>';

        function imgRefresh_click() {
            refreshPage();
        }

        function btnCancel_click() {
            popupManager.GetPopupByName('RQMTEdit').Close();
        }

        function btnDelete_click() {
            QuestionBox('Confirm Delete', 'Are you sure you want to delete this RQMT?', 'Yes,No', 'btnDelete_confirmed', 300, 300, this);
        }

        function btnDelete_confirmed(answer) {
            if (answer == 'Yes') {
                PageMethods.DeleteRQMT(RQMTID, btnDelete_done, on_error);
            }
        }

        function btnDelete_done(result) {
            var obj = $.parseJSON(result);

            if (obj.success == 'true') {
                if (obj.hasdependencies == 'true') {
                    MessageBox('The item could not be deleted because it has dependencies.');
                }
                else {
                    popupManager.GetPopupByName('RQMTEdit').Close();
                }
            }
            else {
                if (obj.hasdependencies == 'true') {
                    MessageBox('The item could not be deleted because it has dependencies.');
                }
                else {
                    MessageBox('The item could not be deleted. ' + obj.error != null ? obj.error : '');
                }
            }
        }

        function btnSave_click() {
            try {
                var validation = validate();
                var blnAltered = 0;

                if (validation.length == 0) {
                    ShowDimmer(true, 'Saving...', 1);

                    // ASSOCIATIONS
                    var section_associations = $('#section_associations');
                    var changedcbs = section_associations.find('input[fieldchanged=1]');
                    var addToSets = [];
                    var deleteFromSets = [];

                    for (var i = 0; i < changedcbs.length; i++) {
                        var span = $(changedcbs[i]).closest('span');

                        var rsetid = span.attr('rsetid');
                        var originalvalue = span.attr('originalvalue');
                        var currentvalue = $(changedcbs[i]).is(':checked') ? '1' : '0';

                        if (currentvalue != originalvalue) {
                            if (currentvalue == '1') {
                                addToSets.push(rsetid);
                            }
                            else if (currentvalue == '0') {
                                deleteFromSets.push(rsetid);
                            }
                        }
                    }

                    addToSets = addToSets.join(',');
                    deleteFromSets = deleteFromSets.join(',');

                    // ATTRIBUTES
                    var section_attributes = $('#section_attributes');
                    var attrrows = section_attributes.find('[id$=grdAttributes_BodyContainer]').find('tr[sysid][rowchanged=1]'); // get all updated rs rows
                    var attrChanges = [];

                    for (var i = 0; i < attrrows.length; i++) {
                        var row = attrrows[i];
                        var sysid = $(row).attr('sysid');

                        var accepted = $(row).find('span[field=ACCEPTED]').find('input').is(':checked');
                        var critid = $(row).find('select[field=CRITICALITY]').val();
                        var statusid = $(row).find('select[field=STATUS]').val();
                        var stageid = $(row).find('select[field=STAGE]').val();

                        attrChanges.push(sysid + '_' + (accepted ? '1' : '0') + '_' + critid + '_' + stageid + '_' + statusid);
                    }

                    attrChanges = attrChanges.join(';');

                    // USAGE
                    var section_usage = $('#section_usage');
                    var usagerows = section_usage.find('[id$=_BodyContainer]').find('tr[rsetrsysid][rowchanged=1]');
                    var usageChanges = [];

                    for (var i = 0; i < usagerows.length; i++) {
                        var row = usagerows[i];
                        var rsetrsysid = $(row).attr('rsetrsysid');

                        var change = rsetrsysid;

                        for (var m = 1; m <= 12; m++) {
                            if ($(row).find('span[field=MONTH_' + m + ']').find('input').is(':checked')) {
                                change += '_1';
                            }
                            else {
                                change += '_0';
                            }
                        }

                        usageChanges.push(change);
                    }

                    usageChanges = usageChanges.join(';');

                    // FUNCTIONALITIES
                    var funcrows = section_attributes.find('[id$=grdFunctionalities_BodyContainer]').find('tr[rsetrsysid][rowchanged=1]');
                    var funcChanges = [];

                    for (var i = 0; i < funcrows.length; i++) {
                        var row = funcrows[i];
                        var rsetrsysid = $(row).attr('rsetrsysid');
                        var funcselections = $(row).attr('funcselections');
                        if (funcselections == null || funcselections.length == 0) {
                            funcselections = '0';
                        }

                        var change = rsetrsysid + '=' + funcselections;

                        funcChanges.push(change);
                    }

                    funcChanges = funcChanges.join(';');

                    // DESCRIPTIONS
                    var descsystemrows = section_attributes.find('[id$=grdDescriptions_BodyContainer]').find('tr[rsid][rsmainrow]');
                    var descDeletes = [];
                    var descChanges = [];

                    for (var i = 0; i < descsystemrows.length; i++) {
                        var rsrow = $(descsystemrows[i]);
                        var rsid = $(rsrow).attr('rsid');
                        var theTABLE = $(rsrow).find('table[descriptiontable=1]');
                        var desctablerows = theTABLE.find('tr');                

                        var descsFound = [];
  
                        for (var x = 0; x < desctablerows.length; x++) {
                            var descrow = $(desctablerows[x]);
                            var descDeleted = descrow.attr('descdeleted') == '1';
                            var rsdescid = descrow.attr('rsdescid');
                            var descText = descrow.find('textarea').val();
                            var descTypeID = descrow.find('select').val()
                            var descDelete = descrow.attr('descdeleted') ? '1' : '0';
                            var descNeedsUpdating = descrow.find('textarea[fieldchanged=1]').length > 0 || descrow.find('select[fieldchanged=1]').length > 0 || descDeleted;
                            var changeMode = rsdescid != '0' ? descrow.find('[name=changemode' + rsdescid + ']:checked').val() : 'all';

                            if (descText.trim().length == 0 && !descDeleted) {
                                MessageBox('Invalid entries: <br><br>Description is incomplete.');
                                ShowDimmer(false);
                                return;
                            }
                                
                            if (_.find(descsFound, function (desc) { debugger;return desc == (descText + '|' + descTypeID) }) != null) {
                                MessageBox('Systems cannot have duplicate descriptions.');
                                ShowDimmer(false);
                                return;
                            }

                            descsFound.push(descText + '|' + descTypeID);

                            if (descNeedsUpdating) {
                                var change = rsid + '<separator>' + rsdescid + '<separator>' + StrongEscape(descText) + '<separator>' + descTypeID + '<separator>' + changeMode + '<separator>' + descDelete;

                                if (descDelete == '1') {
                                    descDeletes.push(change); // by having a separate array for deletes, we can later join them but keep deletes at the top so they are removed before saves (prevents unwanted name conflicts when a new name is the same name as a deleted name, but the new  name could be saved first causing a unique key violation)
                                }
                                else {
                                    descChanges.push(change);
                                }
                            }
                        }
                    }

                    for (var i = 0; i < descDeletes.length; i++) {
                        descChanges.unshift(descDeletes[i]); // put all deletes FIRST in the array to clear them out before new saves
                    }

                    descChanges = descChanges.join('<rqmtsystemseparator>');    

                    PageMethods.Save(<%=this.RQMTID%>, $('[id$=txtRQMT]').val(), addToSets, deleteFromSets, attrChanges, usageChanges, funcChanges, descChanges, <%=NewParentRQMTID%>, save_done, on_error);
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

            var obj = $.parseJSON(result);

            if (obj.success == "true") {
                successMessage('RQMT Saved.');

                if (RQMTID == 0) {
                    refreshPage(obj.newid, true);
                }
                else {
                    refreshPage(null, true);
                }
            }
            else {
                MessageBox('Failed to save. <br>' + obj.error);
            }
        }

        function on_error() {
            ShowDimmer(false);
            MessageBox('An error has occurred.');
        }

        function validate() {
            var validation = [];

            if ($('#<%=this.txtRQMT.ClientID %>').val().length == 0) validation.push('RQMT cannot be empty.');

            var section_attributes = $('#section_attributes');
            var openAvailableFunctionalityPopups = section_attributes.find('div[availablefunctionalities=1]:visible');
            if (openAvailableFunctionalityPopups.length > 0) {
                validation.push('Please close all functionality popups.');
            }

            return validation.join('<br>');
        }

        function txtBox_blur(obj) {
            var $obj = $(obj);
            var nVal = $obj.val();

            $obj.val($.trim(nVal));
        }

        function refreshPage(newID, pageSaved) {
            var nURL = window.location.href;

            var sections = 'details,associations,attributes,usage,defects'.split(',');
            var openSections = '';

            for (var i = 0; i < sections.length; i++) {
                var section = sections[i];

                if ($('#section_' + section).is(':visible')) {
                    openSections += '_' + section + '_';
                }
            }

            if (newID) {
                nURL = editQueryStringValue(nURL, 'RQMTID', newID);
            }
            nURL = editQueryStringValue(nURL, 'OpenSections', openSections);
            if (pageSaved) {
                nURL = editQueryStringValue(nURL, 'PageSaved', '1');
            }

            window.location.href = 'Loading.aspx?Page=' + nURL;
        }

        function resizePage() {
            resizePageElement('divPageContainer');
        }

        function toggleSection(img) {
            var section = $(img).attr('data-section');
            var div = $('#section_' + section);
            div.toggle();

            if (div.is(':visible')) {
                $(img).attr('src', 'images/icons/minus_blue.png');
            }
            else {
                $(img).attr('src', 'images/icons/add_blue.png');
            }
        }

        function input_change(obj) {
            var $obj = $(obj);
            
            $obj.attr('fieldChanged', '1');
            $obj.closest('tr').attr('rowChanged', '1');
            $('#btnSave').prop('disabled', false);
        }

        function toggleUsage(rsetrsysid, all) {
            var section_usage = $('#section_usage');
            var usagerows = section_usage.find('[id$=_BodyContainer]').find('tr[rsetrsysid=' + rsetrsysid + ']');
            var row = usagerows[0]; // should only be one row
            $(row).attr('rowchanged', '1');
            $('#btnSave').prop('disabled', false);

            for (var m = 1; m <= 12; m++) {
                $(row).find('span[field=MONTH_' + m + ']').find('input').prop('checked', all);
            }
        }

        function showAvailableFunctionalities(theDiv, rsetrsysid) {
            //availablefunctionalities=\"1\" rsetrsysid
            var pos = $(theDiv).position();

            var dlg = $(theDiv).closest('td').find('div[availablefunctionalities=1]');

            $(theDiv).closest('td').find('div[availablefunctionalitiesglass=1]').show();

            dlg.css('top', pos.top + $(theDiv).height());
            dlg.css('left', pos.left);
            dlg.show();
        }

        function hideAvailableFunctionalities(closeImg, glassDiv, rsetrsysid) {
            var cbs = null;

            var src = closeImg ? closeImg : glassDiv;

            var theTR = $(src).closest('tr');
            var theTD = $(src).closest('td');
            theTD.find('div[availablefunctionalitiesglass=1]').hide();
            theTD.find('div[availablefunctionalities=1]').hide();
            cbs = theTD.find('div[availablefunctionalities=1]').find('input:checked');

            var selections = '';
            var text = '';

            if (cbs.length > 0) {
                for (var i = 0; i < cbs.length; i++) {
                    if (i > 0) selections += ',';
                    if (i > 0) text += ', ';

                    selections += $(cbs[i]).attr('value');
                    text += $(cbs[i]).attr('text');
                }
            }
            else {
                text = 'NONE';
            }

            theTR.attr('funcselections', selections);
            theTD.find('div[availablefunctionalitiestext=1]').text(text);

            var funcBodyContainer = $('[id$=_grdFunctionalities_BodyContainer]');

            var cnt = 0;
            var functrs = funcBodyContainer.find('tr[funcselections]');
            for (var i = 0; i < functrs.length; i++) {
                var selections = $(functrs[i]).attr('funcselections');
                if (selections != null && selections.length > 0) {                    
                    cnt += selections.split(',').length;
                }
            }
            $('#functionalities_count').html('(' + cnt + ')');
        }

        function addDescription(btn) {
            var theTR = $(btn).closest('tr');
            var theTABLE = theTR.find('table');

            var str = '';
            str += '<tr descriptionrow="1" rsid="' + theTR.attr('rsid') + '" rsdescid="0" rdescid="0">';
            str += ' <td style="text-align:left;vertical-align:top;width:400px;border:0px;">';
            str += '   <textarea rsdesctextarea="1" style="width:400px" rows="3"></textarea>';
            str += '   <div textareaorig="1" style="display:none"></div>';
            str += '   <div resultsdiv="1" style="width:600px;position:absolute;height:100px;overflow-x:hidden;overflow-y:scroll;display:none;background-color:#ffffff;border:1px solid #000000;white-space:normal;"></div>';
            str += ' </td>';

            str += ' <td style="text-align:left;vertical-align:top;width:100px;border:0px;">';
            str += '   <select>';
            str += descTypeOptions;
            str += '   </select>';
            str += ' </td>';

            str += '  <td style="text-align:left;vertical-align:top;border:0px;">';
            str += '    <img src="images/icons/cross.png" style="cursor:pointer;" onclick="deleteDescription(this);">';
            str += '  </td>';

            str += '</tr>';

            theTABLE.append(str);

            theTABLE.find('textarea').off('keyup paste').on('keyup paste', function () { input_change(this); });
            theTABLE.find('select').off('change').on('change', function () { input_change(this); });
            var tas = theTABLE.find('textarea');
            for (var i = 0; i < tas.length; i++) {
                var ta = $(tas[i]);           
                
                ta.cleditor({ width: 800, height: 125 });
                $(ta.cleditor()[0].doc.body).css('background-color', '#f5f6ce');                                
                ta.cleditor()[0].change(function () { descriptionEdited(); return false; });
                ta.cleditor().focus();
            }            

            systemDescriptionsCount++;
            var baseDescRows = systemAssociationsCount > systemDescriptionsCount ? systemAssociationsCount : systemDescriptionsCount;
            var newHt = (50 + (baseDescRows * 60));
            if (newHt > 350) newHt = 350;
            if (newHt < 110) newHt = 110;
            //$('[id$=_grdDescriptions_BodyContainer]').css('height', newHt + 'px');
            //$('[id$=_grdDescriptions_BodyContainer]').attr('baseheight', newHt);
            $('#descriptions_count').html('(' + systemDescriptionsCount + ')');
        }

        function deleteDescription(img) {
            var theTR = $(img).closest('tr');
            var theTA = theTR.find('textarea');

            theTR.attr('descdeleted', '1');            
            theTR.hide();

            theTA.val('--- DELETED ---' + theTR.attr('rsdescid')); // this allows new entries to use this desc's value and prevents "dup" errors from deleted items
            theTA.cleditor()[0].updateFrame();

            $('#btnSave').prop('disabled', false);
            theTR.attr('rowchanged', '1');

            systemDescriptionsCount--;
            var baseDescRows = systemAssociationsCount > systemDescriptionsCount ? systemAssociationsCount : systemDescriptionsCount;
            var newHt = (50 + (baseDescRows * 60));
            if (newHt > 350) newHt = 350;
            if (newHt < 110) newHt = 110;
            //$('[id$=_grdDescriptions_BodyContainer]').css('height', newHt + 'px');
            //$('[id$=_grdDescriptions_BodyContainer]').attr('baseheight', newHt);
            $('#descriptions_count').html('(' + systemDescriptionsCount + ')');
        }

        function descriptionEdited() {            
            var descbodycnt = $('[id$=grdDescriptions_BodyContainer]');            
            var trs = descbodycnt.find('tr[rsid][rsdescid][rdescid]'); // find all the desc tr's (note, we search through all the text areas because the cleditor has issues binding multiple change events that could allow us to pass in the exact ta that was edited - each bind overwrites previous binds even on different cleditor instances - todo: solve that issue
            
            for (var r = 0; r < trs.length; r++) {
                var tr = $(trs[r]);
                var ta = tr.find('textarea[rsdesctextarea=1]');
                var taorig = tr.find('div[textareaorig=1]');

                if (ta.val() != taorig.html()) {
                    input_change(ta);
                }
            }
            
            /*
            var theTD = $(textarea).closest('td');
            var pos = $(textarea).position();

            $('[id$=_grdDescriptions_BodyContainer]').css('height', parseInt($('[id$=_grdDescriptions_BodyContainer]').attr('baseheight')) + 100);

            var resultsDiv = theTD.find('div[resultsdiv]');
            resultsDiv.css('top', pos.top + $(textarea).height());
            resultsDiv.css('left', pos.left);
            resultsDiv.show();            
                        
            PageMethods.SearchDescriptions($(textarea).val(), function (result) { searchDescriptions_done(result, resultsDiv); }, on_error);
            */
        }

        function searchDescriptions_done(result, resultsDiv) {
            var dt = $.parseJSON(result);

            var str = '';

            if (dt.length > 0) {
                for (var i = 0; i < dt.length; i++) {
                    var row = dt[i];

                    str += '<div style="padding-bottom:3px;width:600px;white-space:normal;display:block;" onmouseover="$(this).css(\'background-color\', \'#dbe8ff\')" onmouseout="$(this).css(\'background-color\', \'\')" onmousedown="searchDescriptionClicked(this); return false;" descriptiontypeid="' + row['RQMTDescriptionTypeID'] + '">';
                    str += row['RQMTDescription'] + ' (' + row['RQMTDescriptionType'] + ')';
                    str += '</div>';
                }
            }
            else {
                str = 'No results found.';
            }

            $(resultsDiv).html(str);
        }

        function searchDescriptionClicked(div) {
            var theTD = $(div).closest('td');
            var textarea = theTD.find('textarea');

            var txt = $(div).html();
            var lastIdx = txt.lastIndexOf(' (');
            $(textarea).val(txt.substring(0, lastIdx));

            var dtid = $(div).attr('descriptiontypeid');
            theTD.next().find('select').val(dtid);
            
            var resultsDiv = theTD.find('div[resultsdiv]');
            resultsDiv.hide();
            resultsDiv.html('');

            //$('[id$=_grdDescriptions_BodyContainer]').css('height', $('[id$=_grdDescriptions_BodyContainer]').attr('baseheight'));
        }

        function descriptionEditingEnded(textarea) {
            //var theTD = $(textarea).closest('td');

            //var resultsDiv = theTD.find('div[resultsdiv]');
            //resultsDiv.hide();
            //resultsDiv.html('');

            //$('[id$=_grdDescriptions_BodyContainer]').css('height', $('[id$=_grdDescriptions_BodyContainer]').attr('baseheight'));
        }

        function attachFileToDescription(RQMTDescription_ID) {
			try {
				var url = 'Attachment_Edit.aspx' 
					+ window.location.search 
                    + '&Module=RQMTDESC&RQMTDescription_ID=' + RQMTDescription_ID
					+ '&random=' + new Date().getTime();

				var attPopup = popupManager.AddPopupWindow('AddNewAttachment', 'Add New Attachment'
					, 'Loading.aspx?Page=' + url, 300, 600, 'PopupWindow', window.self);
				if (attPopup) {
					attPopup.Open();
                }

                attPopup.onClose = function () { descriptionAttachmentsUpdated(RQMTDescription_ID); }
			}
			catch (e) { }
        }

        function descriptionAttachmentsUpdated(RQMTDescription_ID) {
            PageMethods.DescriptionAttachmentsUpdated(RQMTDescription_ID, descriptionAttachmentsUpdated_done, on_error);
        }

        function descriptionAttachmentsUpdated_done(result) {
            var dt = $.parseJSON(result);

            var str = '';

            for (var i = 0; i < dt.length; i++) {
                var row = dt[i];

                if (i > 0 && i % 4 == 0) str += '<br />';

                str += '<span descattspan="1" attid="' + row.AttachmentID + '" onmouseover="$(this).find(\'img[deleteimg]\').css(\'opacity\', 1.0);" onmouseout="$(this).find(\'img[deleteimg]\').css(\'opacity\', 0.2);" style=\"line-height:25px;white-space:nowrap;\">&nbsp;&nbsp;&nbsp;<img src="images/icons/attach.png" style="cursor:pointer;width:16px;height:16px;" alt="Download" title="Download" onclick="openDescriptionAttachment(' + row.AttachmentID + ');"><u style="cursor:pointer;" onclick="openDescriptionAttachment(' + row.AttachmentID + ');" alt="' + (row.Description.length > 0 ? row.Description + ' ' : '') + '(' + row.ATTACHMENTTYPE + ')' + '" title="' + (row.Description.length > 0 ? row.Description + ' ' : '') + '(' + row.ATTACHMENTTYPE + ')' + '">' + row.FileName + '</u>&nbsp;<img deleteimg="1" src="images/icons/cross.png" style="width:16px;height:16px;opacity:0.2;cursor:pointer;" onclick="deleteDescriptionAttachment(' + row.AttachmentID + ');"></span>';                
            }

            $('span[rqmtdescattcontainer=1][rqmtdescid=' + row.RQMTDescriptionID + ']').html(str);
        }

        function openDescriptionAttachment(AttachmentID) {
            window.open('Download_Attachment.aspx?attachmentID=' + AttachmentID);
        }

        function deleteDescriptionAttachment(AttachmentID) {
            QuestionBox('Confirm Delete', 'Are you sure you want to delete this attachment?', 'Yes,No', 'deleteDescriptionAttachment_confirmed', 300, 300, this, AttachmentID);
        }

        function deleteDescriptionAttachment_confirmed(answer, AttachmentID) {
            if (answer == 'Yes') {
                PageMethods.DeleteDescriptionAttachment(AttachmentID, function (result) { deleteDescriptionAttachment_done(result, AttachmentID) }, on_error);
            }
        }

        function deleteDescriptionAttachment_done(result, AttachmentID) {
            $('span[descattspan=1][attid=' + AttachmentID + ']').remove();
            successMessage('Attachment deleted');
        }

        function DefectLinkClicked(RQMTID, WTS_SYSTEMID) {
            var nTitle = 'RQMT Defect(s) & Impact';
            var nHeight = 600, nWidth = 1250;
            var nURL = 'RQMTDefectsImpact_Grid.aspx?RQMT_ID=' + RQMTID + '&SYSTEM_ID=' + WTS_SYSTEMID;
            var openPopup = popupManager.AddPopupWindow('RQMTDefects', nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);
            openPopup.onClose = DefectsClosed;

            if (openPopup) openPopup.Open();
        }

        function DefectsClosed() {
            var popup = popupManager.GetPopupByName('RQMTDefects');
            var frmbody = popup.Frame.contentDocument.body;
            var popupBodyContainer = $('[id$=_grdData_BodyContainer]', frmbody);
            
            var cnt = 0;
            var rows = popupBodyContainer.find('tr.selectedRow, tr.gridBody');
            for (var i = 0; i < rows.length; i++) {
                var textarea = $(rows[i]).find('textarea', frmbody);
                var txt = textarea.val();
                if (txt != null && txt.trim().length > 0) {
                    cnt++;
                }
            }

            $('#defects_count').html('(' + cnt + ')');
            
            var defectsBodyContainer = $('[id$=_grdDefects_BodyContainer]');
            var spn = defectsBodyContainer.find('span[sysdefectcount=1][sysid=' + popup.Frame.contentWindow.SYSTEM_ID + ']');
            spn.html('(' + cnt + ')');
        }

        function btnHistory_click() {
            var nTitle = 'RQMT History (RQMT #' + RQMTID + ')';
            var nHeight = 500, nWidth = 1000;
            var nURL = 'Audit_History_Popup.aspx?viewtype=RQMT&itemid=' + RQMTID;
            var openPopup = popupManager.AddPopupWindow('AuditHistory', nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);
            if (openPopup) openPopup.Open();
        }
    </script>

    <script id="jsInit" type="text/javascript">
        function initDisplay() {

            $('#imgRefresh').hide();

            if (<%=this.CanEditRQMT.ToString().ToLower()%>) {
                $('input[type="text"], textarea').css('color', 'black');
                $('input[type="text"], textarea').not('.date').removeAttr('readonly');
                $('select, input[type="checkbox"]').removeAttr('disabled');
                $('#btnCancel').show();
                $('#btnSave').show();
            }

            if (RQMTID > 0 && <%=DisplayItemID%> == 0) {
                if (<%=this.CanDeleteRQMT.ToString().ToLower()%>) {
                    $('#btnDelete').show();
                    $('#btnDelete').prop('disabled', false);
                }
                else {
                    $('#btnDelete').show(); // show it disabled
                    $('#btnDelete').attr('alt', 'This item cannot be deleted.');
                    $('#btnDelete').attr('title', 'This item cannot be deleted.');
                }

                $('#btnHistory').show();
                $('#btnHistory').prop('disabled', false);
            }

            if (!<%=this.NewRQMT.ToString().ToLower()%>) $('#divInfo').show();

            if (!<%=this.CanEditTitle.ToString().ToLower()%>) {
                $('[id$=txtRQMT]').prop('disabled', true);
                $('[id$=txtRQMT]').css('color', '#aaaaaa');
            }

            resizePage();

            $('[id$=_grdAssociations_BodyContainer]').css('height', RQMTID == 0 ? '500px' : '300px');

            var systemrowsheight = 50 + (systemAssociationsCount * 20); // this height represents one row per rqmtsystem the rqmt is associated with
            if (systemrowsheight > 300) systemrowsheight = 300;

            var setrowsheight = 50 + (setAssociationsCount * 20); // this height represents one row per rqmtset the rqmt is associated with
            if (setrowsheight > 300) setrowsheight = 300;

            var baseDescRows = systemAssociationsCount > systemDescriptionsCount ? systemAssociationsCount : systemDescriptionsCount;
            var systemdescrowsheight = (50 + (baseDescRows * 60));
            if (systemdescrowsheight > 350) systemdescrowsheight = 350;
            if (systemdescrowsheight < 110) systemdescrowsheight = 110;

            $('[id$=_grdAttributes_BodyContainer]').css('height', '');            
            $('[id$=_grdUsage_BodyContainer]').css('height', setrowsheight + 'px');
            $('[id$=_grdFunctionalities_BodyContainer]').css('height', '');
            $('[id$=_grdDescriptions_BodyContainer]').css('height', '');
            //$('[id$=_grdDescriptions_BodyContainer]').css('height', systemdescrowsheight + 'px'); // we have 3 rows per description box so we multiply the ht by 3
            //$('[id$=_grdDescriptions_BodyContainer]').attr('baseheight', systemdescrowsheight);
            //$('[id$=_grdDefects_BodyContainer]').css('height', systemrowsheight + 'px'); // COMMENTING OUT BECAUSE THIS ITEM IS LAST AND HAS SOME TOOLTIP POPUPS THAT GET CUT OFF IF WE LIMIT HEIGHT
            var tas = $('[id$=_grdDescriptions_BodyContainer]').find('textarea[rsdesctextarea=1]');
            for (var i = 0; i < tas.length; i++) {
                var ta = $(tas[i]);           
                
                ta.cleditor({ width: 800, height: 125 });
                $(ta.cleditor()[0].doc.body).css('background-color', '#f5f6ce');                                
                ta.cleditor()[0].change(function () { descriptionEdited(); return false; });
                
                //ta.cleditor().bind('blurred', function () { descriptionEditingEnded(rsid, rsdescid, rdescid); return false; })
            }
        }

        function initEvents() {
            $('#imgRefresh').click(function () { imgRefresh_click(); });
            $('#btnCancel').click(function () { btnCancel_click(); return false; });
            $('#btnSave').click(function () { btnSave_click(); return false; });
            $('#btnHistory').click(function () { btnHistory_click(); return false; });
            $('input[type="text"], textarea').on('keyup paste', function () { input_change(this); });
            $('select, input[type="checkbox"]').on('change', function () { input_change(this); });
            $('input[type="text"], textarea').on('blur', function () { txtBox_blur(this); });
            $('.toggleSection').on('click', function () { toggleSection(this); });
            $('#btnDelete').on('click', function () { btnDelete_click(); });

            $(window).resize(resizePage);
        }

        function syncAccordian() {
            var sections = 'details,associations,attributes,usage,defects'.split(',');
            var countsarr = sectionCounts.split(',');

            if (displayItemSubSection == '_descriptions_') {
                $('[id$=grdAttributes_BodyContainer]').hide();
                $('[id$=grdFunctionalities_BodyContainer]').hide();
                $('#spanafd').html('Description');
            }
            
            for (var i = 0; i < sections.length; i++) {
                var section = sections[i];

                if (RQMTID == 0) {
                    if (section == 'details' || section == 'associations') {
                        $('#section_' + section).show();
                        $('img[data-section=' + section + ']').attr('src', 'images/icons/minus_blue.png');
                    }
                }
                else {
                    var sectionIsOpen = openSections.indexOf('_' + section + '_') != -1;

                    if (!hideNonOpenSections || sectionIsOpen) {
                        $('#div_' + section + 'sectioncontainer').show();
                    }

                    if (sectionIsOpen) {
                        $('#section_' + section).show();
                        $('img[data-section=' + section + ']').attr('src', 'images/icons/minus_blue.png');
                    }
                }                
            }

            for (var i = 0; i < countsarr.length; i++) {
                var cnt = countsarr[i];
                $('#' + cnt.split('=')[0] + '_count').html('(' + cnt.split('=')[1] + ')');
            }
        }

        $(document).ready(function () {
            if (<%=pageIsInvalid.ToString().ToLower()%>) {
                $('div').hide();
                MessageBox('RQMTID ' + RQMTID + ' is invalid.');                
                return;
            }
            initDisplay();
            initEvents();
            syncAccordian();
        });
    </script>
</asp:Content>