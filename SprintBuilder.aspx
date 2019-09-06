﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SprintBuilder.aspx.cs" Inherits="SprintBuilder" Theme="Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="shortcut icon" href="Images/fav_icon.ico" type="image/x-icon" />
    <link href="Styles/jquery-ui.css" rel="Stylesheet" type="text/css" />
    <link rel="stylesheet" type="text/css" href="App_Themes/Default/Default.css" />
    <script type="text/javascript" src="Scripts/shell.js"></script>
    <script type="text/javascript" src="Scripts/common.js"></script>
    <script type="text/javascript" src="scripts/filter.js"></script>
    <script type="text/javascript" src="scripts/popupWindow.js"></script>
    <script type="text/javascript" src="Scripts/jquery-1.11.2.js"></script>
    <script type="text/javascript" src="Scripts/jquery-ui.js"></script>
    <script type="text/javascript" src="Scripts/jquery.json-2.4.min.js"></script>
    <script type="text/javascript" src="Scripts/iti_FilterContainer.js"></script>
    <script type="text/javascript" src="Scripts/SmoothMovement.js"></script>
    <script src="Scripts/multiselect/jquery.multiple.select.js" type="text/javascript"></script>
    <link rel="stylesheet" href="Styles/multiple-select.css" />
    <script type="text/javascript" src="Scripts/underscore-min.js"></script>
    <script type="text/javascript" src="Scripts/underscore.string.min.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        <div id="divSprintBuilder" runat="server" style="overflow: auto">
            <table style="width: 100%" cellpadding="0" cellspacing="0">
                <tr class="pageContentHeader">
                    <td>
                        <div style="width: 99%">
                            <table cellpadding="0" cellspacing="0">
                                <tr id="trmsWebsys">
                                    <td style="padding-left: 5px">
                                        <asp:Label runat="server" ID="lblRelease">Release: </asp:Label>
                                    </td>
                                    <td style="padding-left: 5px">
                                        <asp:DropDownList runat="server" ID="ddlProductVersion"></asp:DropDownList>
                                    </td>
                                    <td style="padding-left: 5px">
                                        <asp:Label runat="server" ID="lblSession">Session: </asp:Label>
                                    </td>
                                    <td style="padding-left: 5px">
                                        <asp:DropDownList runat="server" ID="ddlSession"></asp:DropDownList>
                                    </td>
                                    <td style="padding-left: 15px">
                                        <table>
                                            <tr>
                                                <td>
                                                    <div class="form-group">
                                                        <asp:Label runat="server" ID="lblmsWebsys"> System:  </asp:Label>
                                                        <select id="msWebsys" multiple="true" runat="server">
                                                        </select>
                                                    </div>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                    <td style="text-align: left; padding-top: 2px;">
                                        <img id="imgRefresh" alt="Refresh Page" title="Refresh Page" src="images/icons/arrow_refresh_blue.png" width="15" height="15" style="cursor: pointer; margin-left: 4px;" />
                                    </td>
                                    <td>
                                        <asp:Label runat="server" ID="lblMessage" Style="display: none; color: green;"></asp:Label>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </td>
                    <td style="float: right; padding-right: 10px; padding-top: 5px">
                        <input type="button" id="btnSave" value="Save" disabled="disabled" style="vertical-align: middle; display: none;" />
                    </td>
                </tr>
                <tr>
                    <td colspan="4">
                        <div id="divGrid" runat="server" style="height: 650px; width: 100%; overflow: auto">
                            Under Construction
                        </div>
                    </td>
                </tr>
            </table>
        </div>
        <div id="divSprintHistoryModal" class="modal" style="display: none">
            <!-- Modal content -->
            <div class="modal-content" style="width: 900px">
                <div class="modal-header">
                    <img class="modal-close" src="Images/Icons/closeButtonRed.png" alt="close" onclick="closeModal()" />
                    <span id="spnSprintHistory">Sprint History</span>
                </div>
                <div id="divSprintHistory" class="modal-body">
                </div>
            </div>
        </div>

        <asp:ScriptManager ID="scriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>

        <script id="jsEvents" type="text/javascript">
            var _selectedSystems = '';

            function imgRefresh_click(productID, sessionID) {
                var nURL = window.location.href;
                nURL = editQueryStringValue(nURL, 'selectedSystems', _selectedSystems);
                if (productID != undefined) { nURL = editQueryStringValue(nURL, 'productID', productID); }
                if (sessionID != undefined) { nURL = editQueryStringValue(nURL, 'sessionID', sessionID); }

                window.location.href = 'Loading.aspx?Page=' + nURL;
            }

            function showTaskGrid(img) {
                var releaseId = $(img).closest('td').attr('releaseid');
                var systemId = $(img).closest('td').attr('systemid');

                if ($(img).attr('title') == 'Show Section') {
                    $(img).attr('src', 'Images/Icons/minus_blue.png');
                    $(img).attr('title', 'Hide Section');
                    $(img).attr('alt', 'Hide Section');
                    $("#divMgmtWebsys[systemid='" + systemId + "'][releaseid='" + releaseId + "']").show();
                } else {
                    $(img).attr('src', 'Images/Icons/add_blue.png');
                    $(img).attr('title', 'Show Section');
                    $(img).attr('alt', 'Show Section');
                    $("#divMgmtWebsys[systemid='" + systemId + "'][releaseid='" + releaseId + "']").hide();
                }
            }


            function input_change(obj) {
                var $obj = $(obj);

                if ($obj.attr('id') != 'msWebsys') {
                    $obj.attr('fieldChanged', '1');
                    $obj.closest('tr').attr('rowChanged', '1');

                    $('#btnSave').prop('disabled', false);
                }
            }

            function validate() {
                var validation = [];
                $('#<%=this.divSprintBuilder.ClientID %> [rowChanged=1]').each(function () {
                    var WORKITEMID = -1;
                    var WORKITEM_TASKID = -1;
                    var TASKID = -1;
                    var SUBTASKID = -1;

                    if ($(this).find("td[id='tdWORKITEMID']").text() != '') { WORKITEMID = $(this).find("td[id='tdWORKITEMID']").text(); }
                    if ($(this).find("td[id='tdWORKITEM_TASKID']").text() != '') { WORKITEM_TASKID = $(this).find("td[id='tdWORKITEM_TASKID']").text(); }
                    if ($(this).find("td[id='tdAORReleaseTaskID']").text() != '') { TASKID = $(this).find("td[id='tdAORReleaseTaskID']").text(); }
                    if ($(this).find("td[id='tdAORReleaseSubTaskID']").text() != '') { SUBTASKID = $(this).find("td[id='tdAORReleaseSubTaskID']").text(); }

                    if ($(this).find("input").val() == '') {
                        $(this).find("span[id='spnWorkTaskNo']").val();
                        if ($(this).find("td[id='tdWorkTaskNo']").find('span').text() == '') {
                            validation.push('Justification required.');
                        } else {
                            validation.push($(this).find("td[id='tdWorkTaskNo']").find('span').text() + ': Justification required.');
                        }
                    }
                });
                return validation.join('<br>');
            }

            function btnSave_click(close) {
                try {
                    var validation = validate();

                    if (validation.length === 0) {
                        ShowDimmer(true, 'Saving...', 1);

                        //AOR Estimation
                        var arrSprint = [];
                        $('#<%=this.divSprintBuilder.ClientID %> [rowChanged=1]').each(function () {
                            var WORKITEMID = -1;
                            var WORKITEM_TASKID = -1;
                            var TASKID = -1;
                            var SUBTASKID = -1;

                            if ($(this).find("td[id='tdWORKITEMID']").text() != '') { WORKITEMID = $(this).find("td[id='tdWORKITEMID']").text(); }
                            if ($(this).find("td[id='tdWORKITEM_TASKID']").text() != '') { WORKITEM_TASKID = $(this).find("td[id='tdWORKITEM_TASKID']").text(); }
                            if ($(this).find("td[id='tdAORReleaseTaskID']").text() != '') { TASKID = $(this).find("td[id='tdAORReleaseTaskID']").text(); }
                            if ($(this).find("td[id='tdAORReleaseSubTaskID']").text() != '') { SUBTASKID = $(this).find("td[id='tdAORReleaseSubTaskID']").text(); }

                            arrSprint.push({
                                'TASKID': TASKID,
                                'SUBTASKID': SUBTASKID,
                                'WORKITEMID': WORKITEMID,
                                'WORKITEM_TASKID': WORKITEM_TASKID,
                                'WORKMGMTID': $(this).find("select option:selected").val(),
                                'JUSTIFICATION': $(this).find("input").val()
                            });
                        });
                        var nSprintJSON = '{save:' + JSON.stringify(arrSprint) + '}';

                        PageMethods.Save(nSprintJSON, save_done, on_error);
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
                    if (obj.error) errorMsg = obj.error;
                }

                if (blnSaved) {
                    MessageBox('Sprint Builder has been saved');
                    $('#imgRefresh').trigger('click');
                }
                else {
                    MessageBox('Failed to save. <br>' + errorMsg);
                }
            }

            function on_error() {
                ShowDimmer(false);
                MessageBox('An error has occurred.');
            }

            function ddlProductVersion_Change(ddl) {
                imgRefresh_click($(ddl).find('option:selected').val(), undefined);
            }

            function ddlSession_Change(ddl) {
                imgRefresh_click(undefined, $(ddl).find('option:selected').val());
            }

            function spnWorkTaskNo(obj) {
                var WORKITEMID = $(obj).closest('tr').find("td[id='tdWORKITEMID']").text();
                var WORKTASK_ITEMID = $(obj).closest('tr').find("td[id='tdWORKITEM_TASKID']").text();

                PageMethods.GetHistory(WORKITEMID, WORKTASK_ITEMID, getHistory_done, on_error);
            }

            function getHistory_done(result) {
                var nHTML = '';
                var dt = jQuery.parseJSON(result);

                if (dt == null || dt.length == 0) {
                    nHTML = 'No History';
                }
                else {
                    nHTML += '<table cellpadding="0" cellspacing="0">';
                    nHTML += '<tr class="gridHeader">';
                    nHTML += '<td style="font-weight: bold">Field Changed</td>';
                    nHTML += '<td style="font-weight: bold">Old Value</td>';
                    nHTML += '<td style="font-weight: bold">New Value</td>';
                    nHTML += '<td style="font-weight: bold">Created By</td>';
                    nHTML += '<td style="font-weight: bold">Updated By</td>';
                    nHTML += '</tr>';

                    $.each(dt, function (rowIndex, row) {
                        nHTML += '<tr class="gridBody">';

                        nHTML += '<td>' + row.FieldChanged + '</td>';
                        nHTML += '<td>' + row.OldValue + '</td>';
                        nHTML += '<td>' + row.NewValue + '</td>';
                        nHTML += '<td>' + row.CREATEDBY + ': ' + row.CREATEDDATESTRING + '</td>';
                        nHTML += '<td>' + row.UPDATEDBY + ': ' + row.UPDATEDDATESTRING + '</td>';

                        if (rowIndex == dt.length - 1) nHTML += '</tr>';
                    });

                    nHTML += '</table>';
                }

                $('#divSprintHistory').html(nHTML);
                $('#divSprintHistoryModal').show();
                //resizeFrame();
            }

            function closeModal() {
                try {
                    if ($("#divSprintHistoryModal:visible").length > 0) {
                        $("#divSprintHistoryModal").hide();
                    }
                } catch (ex) {

                }
            }

            function ddlSystem_update() {
                var arrSystem = $('#msWebsys').multipleSelect('getSelects');

                _selectedSystems = arrSystem.join(',');

                resizeFrame();
            }

        </script>
        <script id="jsInit" type="text/javascript">
            function resizePage() {
                resizePageElement('divSprintBuilder');
            }

            function initControls() {

                
                var suites = $('#msWebsys option').map(function () {
                    return $(this).attr('OptionGroup');
                }).get();

                var uniqueSuites = _.unique(suites, true);
                $(uniqueSuites).each(function (i, v) {
                    $("#msWebsys option[OptionGroup='" + v + "']").wrapAll("<optgroup label='" + v + "'>");
                });

                $('#msWebsys').multipleSelect({
                    placeholder: 'Default'
                    , width: 'undefined'
                    , onClick: function () {
                        $('#<%=this.lblMessage.ClientID %>').show();
                    $('#<%=this.lblMessage.ClientID %>').text('<< Click Refresh icon to apply System Filter(s)');
                    },
                    onCheckAll: function () {
                        $('#<%=this.lblMessage.ClientID %>').show();
                    $('#<%=this.lblMessage.ClientID %>').text('<< Click Refresh icon to apply System Filter(s)');
                    }
                    , onOpen: function () { ddlSystem_update(); }
                    , onClose: function () { ddlSystem_update(); }
                }).change(function () { ddlSystem_update(); });


            }


            function initEvents() {
                $('#imgRefresh').click(function () { imgRefresh_click(); });
                $('#btnSave').click(function () { btnSave_click(); return false; });
                $('.toggleSection').click(function () { showTaskGrid(this); });
                $('select').on('change', function () { input_change(this); });
                $('input[type="text"], textarea').not('.date').on('keyup paste', function () { input_change(this); });
                $('#<%= this.ddlProductVersion.ClientID %>').on('change', function () { ddlProductVersion_Change(this); })
                $('#<%= this.ddlSession.ClientID %>').on('change', function () { ddlSession_Change(this); })

                $(window).resize(resizePage);
            }

            function initDisplay() {
                if ('<%=this.CanEditWorkloadMGMT %>'.toUpperCase() == 'TRUE') {
                    $('#btnSave').show();
                }
            }

            $(document).ready(function () {
                initEvents();
                initControls();
                initDisplay();
            });

        </script>
    </form>
</body>
</html>
