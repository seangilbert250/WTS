﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AOR_Release_Builder.aspx.cs" Inherits="AOR_Release_Builder" Theme="Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <!-- Copyright (c) 2017 Infinite Technologies, Inc. -->
	<title>Release Builder</title>
	<script type="text/javascript" src="Scripts/shell.js"></script>
	<script type="text/javascript" src="Scripts/common.js"></script>
	<script type="text/javascript" src="Scripts/jquery-1.11.2.js"></script>
    <script type="text/javascript" src="Scripts/popupWindow.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        <div style="padding: 10px 0px;">
            <table style="width: 100%;">
                <tr>
                    <td id="tdAORSearch">
                        AOR #:&nbsp;<asp:TextBox ID="txtAORSearch" runat="server" Width="55px"></asp:TextBox>
                    </td>
                    <td style="width: 33%;">
                        Current Release:&nbsp;<asp:DropDownList ID="ddlCurrentRelease" runat="server" Width="155px"></asp:DropDownList>
                    </td>
                    <td id="tdNewRelease" style="display: none;">
                        New Release:&nbsp;<asp:DropDownList ID="ddlNewRelease" runat="server" Width="155px"></asp:DropDownList>
                    </td>
                </tr>
            </table>
        </div>
        <div id="divAOR" style="height: 625px; overflow: auto;"></div>
        <div id="divFooter" class="PopupFooter">
            <table style="border-collapse: collapse; width: 100%;">
                <tr>
                    <td style="text-align: right;">
                        <input type="button" id="btnClose" value="Close" />
                        <input type="button" id="btnSave" value="Save" style="display: none;" />
                    </td>
                </tr>
            </table>
        </div>
        <div id="divOpenTaskOptions" style="border: 1px solid gray; position: absolute; background-color: white; padding: 5px; display: none; z-index: 100;">
            <table>
                <tr>
                    <td>
                        Include open work tasks with Assigned To Rank(s) of:
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:CheckBoxList ID="cblAssignedToRank" runat="server" RepeatDirection="Vertical"></asp:CheckBoxList>
                    </td>
                </tr>
                <tr>
                    <td style="text-align: right;">
                        <input type="button" id="btnCancel" value="Cancel" />
                        <input type="button" id="btnExecuteSave" value="Save" />
                    </td>
                </tr>
            </table>
        </div>
        <div id="divPageDimmer" style="position: absolute; left: 0px; top: 0px; width: 100%; height: 100%; background: grey; filter: alpha(opacity=60); opacity: .60; display: none;"></div>

        <asp:ScriptManager ID="scriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>
    </form>

    <script id="jsVariables" type="text/javascript">
        var _pageUrls;
    </script>
    
	<script id="jsEvents" type="text/javascript">
        function getAORs() {
            $('#<%=this.ddlNewRelease.ClientID %>').html($('#<%=this.ddlCurrentRelease.ClientID %> option').not(':selected').clone());

	        $('#divAOR').html('<img src="Images/Loaders/loader_2.gif" alt="Loading..." width="15" height="15" />');
	        
	        PageMethods.GetAORs($('#<%=this.ddlCurrentRelease.ClientID %>').val(), getAORs_done, getAORs_error);
	    }

	    function getAORs_done(result) {
            var nHTML = '';
            var ds = jQuery.parseJSON(result);

            if (ds == null || ds.length == 0) {
                nHTML = 'No AORs';
            }
            else {
                var dt = ds.AORTask;
                var dtCR = ds.AORCR;

                if (dt == null || dt.length == 0) {
                    nHTML = 'No AORs';
                }
                else {
                    nHTML += '<table style="border-collapse: collapse; width: 100%;">';
                    nHTML += '<tr class="gridHeader">';

                    if ('<%=this.CanEditAOR %>'.toUpperCase() == 'TRUE') {
                        nHTML += '<th style="border-top: 1px solid grey; text-align: center; width: 20px;">';
                        nHTML += '<input type="checkbox" onchange="checkAllAORs(this);" />';
                        nHTML += '</th>';
                    }

                    nHTML += '<th style="border-top: 1px solid grey; text-align: center; width: 40px;">AOR #</th>';
                    nHTML += '<th style="border-top: 1px solid grey; text-align: center; width: 400px;">AOR Name</th>';
                    nHTML += '<th style="border-top: 1px solid grey; text-align: center;">CR Customer Title</th>';
                    nHTML += '<th style="border-top: 1px solid grey; text-align: center; width: 70px;">Open Work Tasks</th>';
                    nHTML += '</tr>';
                
                    $.each(dt, function (rowIndex, row) {
                        nHTML += '<tr class="gridBody">';

                        if ('<%=this.CanEditAOR %>'.toUpperCase() == 'TRUE') {
                            nHTML += '<td style="text-align: center;">';
                            nHTML += '<input type="checkbox" name="chkAOR" aorreleaseid="' + row.AORReleaseID + '" onchange="chkAOR_change(this);" />';
                            nHTML += '</td>';
                        }

                        nHTML += '<td style="text-align: center;">';

                        if ('<%=this.CanViewAOR %>'.toUpperCase() == 'TRUE') {
                            nHTML += '<a href="" onclick="openAOR(' + row.AORID + '); return false;" style="color: blue;">' + row.AORID + '</a>';
                        }
                        else {
                            nHTML += row.AORID;
                        }

                        nHTML += '</td><td>' + row.AORName + '</td>';
                        nHTML += '<td><table style="border-collapse: collapse; width: 100%;">';

                        $.each(dtCR, function (rowIndexCR, rowCR) {
                            if (row.AORReleaseID == rowCR.AORReleaseID) {
                                nHTML += '<tr><td style="border: none;">';
                                
                                if ('<%=this.CanEditAOR %>'.toUpperCase() == 'TRUE') {
                                    nHTML += '<input type="checkbox" name="chkCR" aorreleaseid="' + row.AORReleaseID + '" crid="' + rowCR.CRID + '" disabled />';
                                }

                                if ('<%=this.CanViewCR %>'.toUpperCase() == 'TRUE') {
                                    nHTML += '<a href="" onclick="openCR(' + rowCR.CRID + '); return false;" style="color: blue;">' + decodeURIComponent(rowCR.CRName) + '</a>';
                                }
                                else {
                                    nHTML += decodeURIComponent(rowCR.CRName);
                                }

                                nHTML += '</td></tr>';
                            }
                        });
                        
                        nHTML += '</table></td>';
                        nHTML += '<td style="text-align: center;">';

                        if ('<%=this.CanViewWorkItem %>'.toUpperCase() == 'TRUE' && parseInt(row.OpenTaskCount) > 0) {
                            nHTML += '<a href="" onclick="openTasks(' + row.AORID + '); return false;" style="color: blue;">' + row.OpenTaskCount + '</a>';
                        }
                        else {
                            nHTML += row.OpenTaskCount;
                        }

                        nHTML += '</td></tr>';
                    });

                    nHTML += '</table>';
                }
            }

            $('#divAOR').html(nHTML);
	    }

	    function getAORs_error() {
	        $('#divAOR').html('Error gathering data.');
	    }

	    function checkAllAORs(obj) {
	        $('#divAOR').find('input[name="chkAOR"]').prop('checked', $(obj).is(':checked')).change();
	    }

	    function chkAOR_change(obj) {
	        var $obj = $(obj);
	        var aorreleaseid = $obj.attr('aorreleaseid');
	        var checked = $obj.is(':checked');
	        
	        $('#divAOR').find('input[name="chkCR"][aorreleaseid="' + aorreleaseid + '"]').prop('disabled', !checked).prop('checked', checked);
	    }

	    function openAOR(AORID) {
	        var nWindow = 'AOR';
	        var nTitle = 'AOR';
	        var nHeight = 700, nWidth = 1400;
	        var nURL = _pageUrls.Maintenance.AORTabs + '?NewAOR=false&AORID=' + AORID;
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

	    function openTasks(AORID) {
	        var nWindow = 'AOR';
	        var nTitle = 'AOR';
	        var nHeight = 700, nWidth = 1400;
	        var nURL = _pageUrls.Maintenance.AORTabs + '?NewAOR=false&AORID=' + AORID + '&Tab=Tasks';
	        var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

	        if (openPopup) openPopup.Open();
	    }

	    function validate() {
	        var validation = [];

	        if (!(parseInt($('#<%=this.ddlCurrentRelease.ClientID %>').val()) > 0)) validation.push('Current Release cannot be empty.');
	        if (!(parseInt($('#<%=this.ddlNewRelease.ClientID %>').val()) > 0)) validation.push('New Release cannot be empty.');
	        if ($('#divAOR').find('input[name="chkAOR"]:checked').length == 0) validation.push('At least one AOR must be checked.');

			return validation.join('<br>');
	    }

		function btnClose_click() {
		    closeWindow();
        }

		function btnSave_click() {
		    var validation = validate();

            if (validation.length == 0) {
                $('#divPageDimmer').show();

                var $objDiv = $('#divOpenTaskOptions');

                $objDiv.css({ top: '50%', left: '50%', margin: '-' + ($objDiv.height() / 2) + 'px 0 0 -' + ($objDiv.width() / 2) + 'px' }).show(); //center
		    }
		    else {
		        MessageBox('Invalid entries: <br><br>' + validation);
		    }
        }

        function btnCancel_click() {
            $('#divOpenTaskOptions').hide();
            $('#divPageDimmer').hide();
        }

        function btnExecuteSave_click() {
            QuestionBox('Confirm Save', 'Are you sure you would like to save?', 'Yes,No', 'confirmSave', 300, 300, this);
        }

        function confirmSave(answer) {
            if ($.trim(answer).toUpperCase() == 'YES') {
                try {
				    ShowDimmer(true, 'Saving...', 1);

                    var arrSelections = [];

                    $('#<%=this.cblAssignedToRank.ClientID %> input:checked').each(function () {
                        arrSelections.push($(this).attr('value'));
                    });

				    var arrAORs = [];

				    $('#divAOR').find('input[name="chkAOR"]:checked').each(function () {
				        var AORReleaseID = $(this).attr('aorreleaseid');
				        var arrCRs = [];

				        $('#divAOR').find('input[name="chkCR"][aorreleaseid="' + AORReleaseID + '"]:checked').each(function () {
				            arrCRs.push($(this).attr('crid'));
				        });

				        arrAORs.push({ 'aorreleaseid': AORReleaseID, 'crids': arrCRs.join(',') });
				    });

                    var assignedToRankIDs = arrSelections.join(',');
				    var nAORsJSON = '{save:' + JSON.stringify(arrAORs) + '}';
					
                    PageMethods.Save($('#<%=this.ddlCurrentRelease.ClientID %>').val(), $('#<%=this.ddlNewRelease.ClientID %>').val(), assignedToRankIDs, nAORsJSON, save_done, save_on_error);
			    }
			    catch (e) {
				    ShowDimmer(false);
                    MessageBox('An error has occurred.');
                    $('#btnCancel').trigger('click');
			    }
            }
        }

        function save_done(result) {
            ShowDimmer(false);
            $('#btnCancel').trigger('click');

            var blnSaved = false;
			var errorMsg = '';
			var obj = $.parseJSON(result);

			if (obj) {
				if (obj.saved && obj.saved.toUpperCase() == 'TRUE') blnSaved = true;
				if (obj.error) errorMsg = obj.error;
			}

			if (blnSaved) {
			    if (opener.refreshPage) opener.refreshPage(false);

				MessageBox('Release has been saved.');
				getAORs();
			    //setTimeout(closeWindow, 1);
			}
			else {
				MessageBox('Failed to save. <br>' + errorMsg);
			}
        }

        function save_on_error() {
            ShowDimmer(false);
            MessageBox('An error has occurred.');
            $('#btnCancel').trigger('click');
        }

        function filterAORNumbers() {
            var aorSearch = $('#<%=this.txtAORSearch.ClientID%>').val();

            $('td').find('a').each(function () {
                if ($(this).attr('onclick').indexOf('AOR') > -1) {
                    if ($(this).text().indexOf(aorSearch) > -1 || aorSearch.length === 0) {
                        $(this).closest('tr').show();
                    } else {
                        $(this).closest('tr').hide();
                    }
                }
            });
        }
	</script>

    <script id="jsInit" type="text/javascript">
        function initVariables() {
            _pageUrls = new PageURLs();
        }

        function initDisplay() {
            if ('<%=this.CanEditAOR %>'.toUpperCase() == 'TRUE') {
                $('#tdNewRelease').show();
                $('#btnSave').show();
            }
        }

        function initEvents() {
            $('#<%=this.ddlCurrentRelease.ClientID %>').change(function () { getAORs(); });
            $('#btnClose').click(function () { btnClose_click(); return false; });
            $('#btnSave').click(function () { btnSave_click(); return false; });
            $('#btnCancel').click(function () { btnCancel_click(); return false; });
            $('#btnExecuteSave').click(function () { btnExecuteSave_click(); return false; });
        }

        $(document).ready(function () {
            initVariables();
            initDisplay();
            initEvents();
            $('#<%=this.ddlCurrentRelease.ClientID %>').trigger('change');
            $("#txtAORSearch").bind('keydown', function (e) {
                if (e.keyCode === 13 || e.keyCode === 144) {
                    e.preventDefault();
                }
            });
            $("#txtAORSearch").keyup(function (event) {
                filterAORNumbers();
            });
        });
    </script>
</body>
</html>
