<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AOR_Scheduled_Deliverables_Edit.aspx.cs" Inherits="AOR_Scheduled_Deliverables_Edit" MasterPageFile="~/EditTabs.master" Theme="Default" %>
<%@ Register Src="~/Controls/MultiSelect.ascx" TagPrefix="wts" TagName="MultiSelect" %>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" runat="Server">AOR</asp:Content>
<asp:Content ID="cpHead" ContentPlaceHolderID="head" runat="Server">
	<!-- Copyright (c) 2017 Infinite Technologies, Inc. -->        
</asp:Content>
<asp:Content ID="cpHeaderImage" ContentPlaceHolderID="ContentPlaceHolderHeaderImage" runat="Server">
	<img src="Images/Icons/pencil.png" alt="Details" width="15" height="15" />
	<asp:HiddenField ID="itisettings" runat="server" EnableViewState="True" />
</asp:Content>
<asp:Content ID="cpHeaderText" ContentPlaceHolderID="ContentPlaceHolderHeaderText" runat="Server">Details</asp:Content>
<asp:Content ID="cpHeaderButtons" ContentPlaceHolderID="ContentPlaceHolderHeaderButtons" runat="Server">
	<input type="button" id="btnCancel" value="Cancel" style="vertical-align: middle; display: none;" />
	<input type="button" id="btnSave" value="Save" style="vertical-align: middle; display: none;" />
</asp:Content>
<asp:Content ID="cpBody" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
	<div id="divPageContainer" style="overflow-x: hidden; overflow-y: auto;">
		<div id="divAOR" style="padding: 10px;">
			<table style="width: 100%;">
				<tr>
					<td style="width: 5px;">&nbsp;</td>
					<td style="width: 115px;">Release Version:</td>
					<td>
						<span id="spnAOR" runat="server">-</span>
                        <div id="divInfo" style="float: right; display: none;"><span id="spnCreated" runat="server"></span><span id="spnUpdated" runat="server" style="padding-left: 30px;"></span></div>
					</td>
					<td style="width: 5px;">&nbsp;</td>
				</tr>
				<tr>
					<td><span style="color: red;">*</span></td>
					<td>Deployment:</td>
					<td>
						<asp:TextBox ID="txtReleaseDeliverable" runat="server" MaxLength="150" Width="100px" ForeColor="Gray"></asp:TextBox>
					</td>
					<td>&nbsp;</td>
				</tr>
				<tr>
					<td>&nbsp;</td>
					<td style="vertical-align: top;">Description:</td>
					<td>
						<asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" Rows="4" MaxLength="500" Width="100%" ReadOnly="true" ForeColor="Gray"></asp:TextBox>
					</td>
					<td>&nbsp;</td>
				</tr>
                <tr>
					<td>
						&nbsp;
					</td>
					<td style="vertical-align: top;">
						Narrative:
					</td>
					<td>
						<asp:TextBox ID="txtNarrative" runat="server" TextMode="MultiLine" Rows="4" Width="100%" ReadOnly="true" ForeColor="Gray"></asp:TextBox>
					</td>
					<td>
						&nbsp;
					</td>
				</tr>
                <tr>
					<td>
						&nbsp;
					</td>
					<td style="vertical-align: top;">
						Visible To Customer:
					</td>
					<td>
						<asp:CheckBox ID="chkVisible" runat="server" Checked="true" />&nbsp;&nbsp;
					</td>
					<td>
						&nbsp;
					</td>
				</tr>
                <tr>
					<td>
						&nbsp;
					</td>
					<td style="vertical-align: top;">
						Sort Order:
					</td>
					<td>
						<asp:TextBox ID="txtSortOrder" runat="server" ReadOnly="true" Width="100px" ForeColor="Gray"></asp:TextBox>&nbsp;&nbsp;
					</td>
					<td>
						&nbsp;
					</td>
				</tr>
                <tr>
                    <td></td>
                    <td colspan="2">
                        <table>
                            <tr>
                                <td rowspan="2" style="vertical-align: bottom;">
                                    <u>Release Milestones</u>
                                </td>
                                <td colspan="2" style="text-align: center;">
                                    <u>Planned</u>
                                </td>
                                <td colspan="2" style="text-align: center;">
                                    <u>Actual</u>
                                </td>
                            </tr>
                            <tr>
                                <td style="text-align: center;">
                                    <u>Start</u>
                                </td>
                                <td style="text-align: center;">
                                    <u>Finish</u>
                                </td>
                                <td style="text-align: center;">
                                    <u>Start</u>
                                </td>
                                <td style="text-align: center;">
                                    <u>Finish</u>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Development/Test
                                </td>
                                <td>
                                    <asp:TextBox ID="txtPlannedDevTestStart" runat="server" Width="75"/>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtPlannedDevTestEnd" runat="server" Width="75" />
                                </td>
                                <td>
                                    <asp:TextBox ID="txtActualDevTestStart" runat="server" Width="75"/>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtActualDevTestEnd" runat="server" Width="75"/>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    IP-1 Dev/Test
                                </td>
                                <td>
                                    <asp:TextBox ID="txtPlannedIP1Start" runat="server" Width="75"/>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtPlannedIP1End" runat="server" Width="75" />
                                </td>
                                <td>
                                    <asp:TextBox ID="txtActualIP1Start" runat="server" Width="75"/>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtActualIP1End" runat="server" Width="75"/>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    IP-2 Dev/Test
                                </td>
                                <td>
                                    <asp:TextBox ID="txtPlannedIP2Start" runat="server" Width="75"/>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtPlannedIP2End" runat="server" Width="75" />
                                </td>
                                <td>
                                    <asp:TextBox ID="txtActualIP2Start" runat="server" Width="75"/>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtActualIP2End" runat="server" Width="75"/>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    IP-3 Dev/Test
                                </td>
                                <td>
                                    <asp:TextBox ID="txtPlannedIP3Start" runat="server" Width="75"/>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtPlannedIP3End" runat="server" Width="75" />
                                </td>
                                <td>
                                    <asp:TextBox ID="txtActualIP3Start" runat="server" Width="75"/>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtActualIP3End" runat="server" Width="75"/>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Deploy
                                </td>
                                <td>
                                    <asp:TextBox ID="txtPlannedStart" runat="server" Width="75"/>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtPlannedEnd" runat="server" Width="75" />
                                </td>
                                <td>
                                    <asp:TextBox ID="txtActualStart" runat="server" Width="75"/>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtActualEnd" runat="server" Width="75"/>
                                </td>
                            </tr>
                        </table>
                    </td>
				</tr>
                <tr style="display: none;">
                    <td><span style="color: red;"></span></td>
					<td>Investigation Dates:</td>
                    <td>
						<asp:TextBox ID="txtPlannedInvStart" runat="server" Width="75" />
						<asp:TextBox ID="txtPlannedInvEnd" runat="server" Width="75" />
                    </td>
				</tr>
                <tr style="display: none;">
                    <td><span style="color: red;"></span></td>
					<td>Technical Dates:</td>
                    <td>
						<asp:TextBox ID="txtPlannedTechStart" runat="server" Width="75" />
						<asp:TextBox ID="txtPlannedTechEnd" runat="server" Width="75" />
                    </td>
				</tr>
                <tr style="display: none;">
                    <td><span style="color: red;"></span></td>
					<td>Customer Design Dates:</td>
                    <td>
						<asp:TextBox ID="txtPlannedCDStart" runat="server" Width="75" />
						<asp:TextBox ID="txtPlannedCDEnd" runat="server" Width="75" />
                    </td>
				</tr>
                <tr style="display: none;">
                    <td><span style="color: red;"></span></td>
					<td>Coding Dates:</td>
                    <td>
						<asp:TextBox ID="txtPlannedCodingStart" runat="server" Width="75" />
						<asp:TextBox ID="txtPlannedCodingEnd" runat="server" Width="75" />
                    </td>
				</tr>
                <tr style="display: none;">
                    <td><span style="color: red;"></span></td>
					<td>Internal Testing Dates:</td>
                    <td>
						<asp:TextBox ID="txtPlannedITStart" runat="server" Width="75" />
						<asp:TextBox ID="txtPlannedITEnd" runat="server" Width="75" />
                    </td>
				</tr>
                <tr style="display: none;">
                    <td><span style="color: red;"></span></td>
					<td>Customer Validation <br />Testing Dates:</td>
                    <td>
						<asp:TextBox ID="txtPlannedCVTStart" runat="server" Width="75" />
						<asp:TextBox ID="txtPlannedCVTEnd" runat="server" Width="75" />
                    </td>
				</tr>
                <tr style="display: none;">
                    <td><span style="color: red;"></span></td>
					<td>Adoption Dates:</td>
                    <td>
						<asp:TextBox ID="txtPlannedAdoptStart" runat="server" Width="75" />
						<asp:TextBox ID="txtPlannedAdoptEnd" runat="server" Width="75" />
                    </td>
				</tr>
            </table>
		</div>
    </div>
	<asp:ScriptManager ID="scriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>
	<script id="jsVariables" type="text/javascript">
		var _pageUrls;
	</script>

	<script id="jsEvents" type="text/javascript">
	    $.extend({ icompare: function(a, b) {
	        return $(a).not(b).get().length === 0 && $(b).not(a).get().length === 0;
	    }});

		function imgRefresh_click() {
			refreshPage();
		}

		function imgSettings_click() {
			openSettings();
		}

		function btnCancel_click() {
            parent.closeWindow();
		}

		function btnSave_click() {
			try {
				var validation = validate();
	            if (validation.length === 0) {
                    ShowDimmer(true, 'Saving...', 1);
                    PageMethods.Save('<%=this.NewDeliverable %>', '<%=this.DeliverableID%>', $('#<%=this.txtReleaseDeliverable.ClientID %>').val(), '<%=this.ReleaseID %>', $('#<%=this.txtDescription.ClientID %>').val(), $('#<%=this.txtNarrative.ClientID %>').val(), $('#<%=this.chkVisible.ClientID %>').is(':checked'),
                        $('#<%=this.txtPlannedStart.ClientID %>').val(), $('#<%=this.txtPlannedEnd.ClientID %>').val(),
                        $('#<%=this.txtPlannedInvStart.ClientID %>').val(), $('#<%=this.txtPlannedInvEnd.ClientID %>').val(),
                        $('#<%=this.txtPlannedTechStart.ClientID %>').val(), $('#<%=this.txtPlannedTechEnd.ClientID %>').val(),
                        $('#<%=this.txtPlannedCDStart.ClientID %>').val(), $('#<%=this.txtPlannedCDEnd.ClientID %>').val(),
                        $('#<%=this.txtPlannedCodingStart.ClientID %>').val(), $('#<%=this.txtPlannedCodingEnd.ClientID %>').val(),
                        $('#<%=this.txtPlannedITStart.ClientID %>').val(), $('#<%=this.txtPlannedITEnd.ClientID %>').val(),
                        $('#<%=this.txtPlannedCVTStart.ClientID %>').val(), $('#<%=this.txtPlannedCVTEnd.ClientID %>').val(),
                        $('#<%=this.txtPlannedAdoptStart.ClientID %>').val(), $('#<%=this.txtPlannedAdoptEnd.ClientID %>').val(),
                        $('#<%=this.txtPlannedDevTestStart.ClientID %>').val(), $('#<%=this.txtPlannedDevTestEnd.ClientID %>').val(),
                        $('#<%=this.txtPlannedIP1Start.ClientID %>').val(), $('#<%=this.txtPlannedIP1End.ClientID %>').val(),
                        $('#<%=this.txtPlannedIP2Start.ClientID %>').val(), $('#<%=this.txtPlannedIP2End.ClientID %>').val(),
                        $('#<%=this.txtPlannedIP3Start.ClientID %>').val(), $('#<%=this.txtPlannedIP3End.ClientID %>').val(),
                        $('#<%=this.txtActualStart.ClientID %>').val(), $('#<%=this.txtActualEnd.ClientID %>').val(),
                        $('#<%=this.txtActualDevTestStart.ClientID %>').val(), $('#<%=this.txtActualDevTestEnd.ClientID %>').val(),
                        $('#<%=this.txtActualIP1Start.ClientID %>').val(), $('#<%=this.txtActualIP1End.ClientID %>').val(),
                        $('#<%=this.txtActualIP2Start.ClientID %>').val(), $('#<%=this.txtActualIP2End.ClientID %>').val(),
                        $('#<%=this.txtActualIP3Start.ClientID %>').val(), $('#<%=this.txtActualIP3End.ClientID %>').val(),
                        $('#<%=this.txtSortOrder.ClientID %>').val() > 0 ? $('#<%=this.txtSortOrder.ClientID %>').val() : 0, 0, save_done, on_error);
				}
				else {
					MessageBox('Invalid entries: <br><br>' + validation);
				}
			}
			catch (e) {
				ShowDimmer(false);
				MessageBox('An error has occurred.' + e);
			}
		}

		function save_done(result) {
			ShowDimmer(false);

			var blnExists = false;
			var newID = '', errorMsg = '';
			var obj = $.parseJSON(result);

			if (obj) {
				if (obj.exists && obj.exists.toUpperCase() == 'TRUE') blnExists = true;
				if (obj.newID && parseInt(obj.newID) > 0) newID = obj.newID;
				if (obj.error) errorMsg = obj.error;
			}

            if (blnExists) {
                MessageBox('Deployment already exists.');
            }
            else if (errorMsg.length > 0) {
                MessageBox('Failed to save. <br>' + errorMsg);
            } else {
                MessageBox('Deployment saved. <br>' + errorMsg);
                parent.opener.refreshPage();
                parent.closeWindow();
            }
		}

		function on_error() {
            ShowDimmer(false);
			MessageBox('An error has occurred.');
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

		function reorderSectionList() {
			var items = $('#iti_sections').children();
			var data = JSON.parse($(defaultParentPage.itisettings).text());
            var newArray = [];
	        var oldArray = [];

	        $.each(items, function (i, v) {
	            newArray.push('' + v.id.replace("div", "chk").replace("Container", ""));
            });

            $.each(data.sectionexpanded, function (i, v) {
	            oldArray.push(i);
	        });

            if (!$.icompare(newArray, oldArray)) {
	            var newSectionOrder = [];
	            var newSectionExpanded = {};

                $.each(items, function (i, v) {
                    var newItem = i + 1;
                    newSectionOrder.push(newItem.toString());
                    newSectionExpanded['' + v.id.replace("div","chk").replace("Container","") + ''] = true;
                });

	            data.sectionorder = newSectionOrder;
                data.sectionexpanded = newSectionExpanded;
	            $(defaultParentPage.itisettings).text(JSON.stringify(data));
	        }

			var orderedItems = $.map(data.sectionorder, function (value) {
				return items.get(value - 1);
			});

			$('#iti_sections').empty().html(orderedItems);
		}

	    function showSectionList() {
			var data = JSON.parse($(defaultParentPage.itisettings).text());
			var imgArray = $('#iti_sections').find('img');

			$.each(data.sectionexpanded, function (i, item) {
				$.each(imgArray, function (j, value) {
					var $obj = $(imgArray[j]);
					var section = $obj.data('section');

					if (section === i.substring(3)) {
						if (item === false) {
							$obj.attr("src", "Images/Icons/minus_blue.png");
							$obj.attr("title", "Hide Section");
							$obj.attr("alt", "Hide Section");
							$obj.data("section", i.substring(3));
						} else {
							$obj.attr("src", "Images/Icons/add_blue.png");
							$obj.attr("title", "Show Section");
							$obj.attr("alt", "Show Section");
							$obj.data("section", i.substring(3));
						}
						imgToggleSection_click($obj);
					}
				});
			});
		}

		function validate() {
		    var validation = [];

            if ($('#<%=this.txtReleaseDeliverable.ClientID %>').val().length == 0) validation.push('Release Deliverable cannot be empty.');

			return validation.join('<br>');
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
				case 'Allocation':
					$obj.attr('fieldChanged', '1');
					$obj.closest('tr').attr('rowChanged', '1');
					break;
			}

			if ($obj.attr('field') == 'Allocation') {
				var nVal = $obj.val();

				$obj.val(nVal.replace(/[^\d]/g, ''));
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

		function refreshPage(newID) {
			var nURL = window.location.href;

			if (newID != undefined && parseInt(newID) > 0) {
				if (parent.refreshPage) parent.refreshPage(newID);
			}

			window.location.href = 'Loading.aspx?Page=' + nURL;
		}

		function resizePage() {
			resizePageElement('divPageContainer');
        }

        function initDatePickers() {
            $('#<%=this.txtPlannedStart.ClientID %>').datepicker();
            $('#<%=this.txtPlannedInvStart.ClientID %>').datepicker();
            $('#<%=this.txtPlannedTechStart.ClientID %>').datepicker();
            $('#<%=this.txtPlannedCDStart.ClientID %>').datepicker();
            $('#<%=this.txtPlannedCodingStart.ClientID %>').datepicker();
            $('#<%=this.txtPlannedITStart.ClientID %>').datepicker();
            $('#<%=this.txtPlannedCVTStart.ClientID %>').datepicker();
            $('#<%=this.txtPlannedAdoptStart.ClientID %>').datepicker();
            $('#<%=this.txtPlannedDevTestStart.ClientID %>').datepicker();
            $('#<%=this.txtPlannedIP1Start.ClientID %>').datepicker();
            $('#<%=this.txtPlannedIP2Start.ClientID %>').datepicker();
            $('#<%=this.txtPlannedIP3Start.ClientID %>').datepicker();
            $('#<%=this.txtPlannedEnd.ClientID %>').datepicker();
            $('#<%=this.txtPlannedInvEnd.ClientID %>').datepicker();
            $('#<%=this.txtPlannedTechEnd.ClientID %>').datepicker();
            $('#<%=this.txtPlannedCDEnd.ClientID %>').datepicker();
            $('#<%=this.txtPlannedCodingEnd.ClientID %>').datepicker();
            $('#<%=this.txtPlannedITEnd.ClientID %>').datepicker();
            $('#<%=this.txtPlannedCVTEnd.ClientID %>').datepicker();
            $('#<%=this.txtPlannedAdoptEnd.ClientID %>').datepicker();
            $('#<%=this.txtPlannedDevTestEnd.ClientID %>').datepicker();
            $('#<%=this.txtPlannedIP1End.ClientID %>').datepicker();
            $('#<%=this.txtPlannedIP2End.ClientID %>').datepicker();
            $('#<%=this.txtPlannedIP3End.ClientID %>').datepicker();
            $('#<%=this.txtActualStart.ClientID %>').datepicker();
            $('#<%=this.txtActualDevTestStart.ClientID %>').datepicker();
            $('#<%=this.txtActualIP1Start.ClientID %>').datepicker();
            $('#<%=this.txtActualIP2Start.ClientID %>').datepicker();
            $('#<%=this.txtActualIP3Start.ClientID %>').datepicker();
            $('#<%=this.txtActualEnd.ClientID %>').datepicker();
            $('#<%=this.txtActualDevTestEnd.ClientID %>').datepicker();
            $('#<%=this.txtActualIP1End.ClientID %>').datepicker();
            $('#<%=this.txtActualIP2End.ClientID %>').datepicker();
            $('#<%=this.txtActualIP3End.ClientID %>').datepicker();
        }
	</script>

	<script id="jsInit" type="text/javascript">
		function initVariables() {
            _pageUrls = new PageURLs();
            _rscSelIdx = 10000;
		}

		function initDisplay() {
			if ('<%=this.CanEditDeliverable %>'.toUpperCase() == 'TRUE') {
				$('input[type="text"], textarea').css('color', 'black');
				$('input[type="text"], textarea').removeAttr('readonly');
				$('#btnCancel').show();
				$('#btnSave').show();
			}

		    if ('<%=this.NewDeliverable %>'.toUpperCase() == 'FALSE') {
		        $('#divInfo').show();
				$('#trReleaseStatus').show();
				$('#divHistoryContainer').show();
			}

            resizePage();
		}

		function initEvents() {
			$('#imgRefresh').click(function () { imgRefresh_click(); });
			$('#imgSettings').click(function () { imgSettings_click(); });
			$('#btnCancel').click(function () { btnCancel_click(); return false; });
			$('#btnSave').click(function () { btnSave_click(); return false; });
			$('.toggleSection').on('click', function () { imgToggleSection_click(this); });
			$('input[type="text"], textarea').on('keyup paste', function () { input_change(this); });
			$('select, input[type="checkbox"]').on('change', function () { input_change(this); });
            $('input[type="text"], textarea').on('blur', function () { txtBox_blur(this); });
            initDatePickers();

            $(window).resize(resizePage);
		}

		$(document).ready(function () {
			initVariables();
			initDisplay();
            initEvents();
		});
	</script>
</asp:Content>