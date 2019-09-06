﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="FilterPage.aspx.cs" Inherits="FilterPage" theme="Default"%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Filter and Criteria</title>
    <script type="text/javascript" src="scripts/shell.js"></script>
    <script type="text/javascript" src="scripts/filter.js"></script>
    <script type="text/javascript" src="scripts/PopupWindow.js"></script>
    <script type="text/javascript" src="Scripts/jquery-1.11.2.js"></script>
	<script type="text/javascript" src="Scripts/jquery.json-2.4.min.js"></script>

    <link rel="stylesheet" type="text/css" href="App_Themes/Default/Default.css" /> 

    <style type="text/css"> 
       .LockOff { 
          display: none; 
          visibility: hidden; 
       } 

       .LockOn { 
          display: block; 
          visibility: visible; 
          position: absolute; 
          z-index: 999; 
          top: 0px; 
          left: 0px; 
          width: 105%; 
          height: 105%; 
          background-color: #ccc; 
          text-align: center; 
          padding-top: 20%; 
          filter: alpha(opacity=75); 
          opacity: 0.75; 
       } 
    </style> 

    <script type="text/javascript"> 
       function skm_LockScreen(str) 
       { 
            var lock = document.getElementById('skm_LockPane');
            if (lock)
                lock.className = 'LockOn';

            lock.innerHTML = str; 
       } 

       function skm_UnLockScreen() {
           var lock = document.getElementById('skm_LockPane');
           lock.className = 'LockOff';

           lock.innerHTML = '';
       }
    </script>
        
    <style type="text/css">
        .filterFields {
            cursor:default;
            font-size:12px;
            border:solid 1px grey;

            overflow-x:hidden;
            overflow-y:auto;
        }
            .filterFields li {
                padding:3px;
                border-bottom:solid 1px gainsboro;
            }
            .filterFields li:not(.filterFieldsHeader):hover {
                background:#d7e8fc;
            }
        .filterFieldsItem {
            background:url(Images/Headers/page_header_back.gif);
        } 
        .filterFieldsHeader {
            background:url(Images/Headers/gridheaderblue.png);
            font-weight:bold;
            text-align:center;
        }
        .filterFieldsSelected {
            background:#d7e8fc;
        }

        select {
            font-size:12px;
        }
    </style>
</head>
<body>
    <form id="form1" method ="post" runat="server">
         <div id="divFilters" style="display: none;">
            <table id="tableBasicOptions" class="attributes" style="width: 99%; text-align: left; vertical-align: top; padding: 10px;">
                <tr class="attributesRow">
                    <td class="attributesValue" style="border: 1px solid black; padding: 10px;">
                        <label for="ddlSavedFilters" title="Filters:" style="vertical-align: middle;">Filters:</label>
                        <iti_Tools_Sharp:DropDownList ID="ddlSavedFilters" BackColor="White" runat="server" Font-Size="12px" Width="114px" onchange="setCustomFilter();" />
                        <img id="imgSaveFilter" src="Images/Icons/disk.png" title="Save View" alt="Save View" style="cursor: pointer;" />
                        <img id="imgDeleteFilter" src="Images/Icons/delete.png" title="Delete View" alt="Delete View" style="cursor: pointer;" />
                        <asp:HiddenField ID="txtXML" runat="server" />
                        <asp:HiddenField ID="txtDropDown" runat="server" />
                    </td>
                </tr>
            </table>
        </div>
		<div style="text-align: center; padding-top: 10px; padding-left:10px;">
			<table style="width: 100%;">
				<tr style="vertical-align: top;">
					<td>Field
					</td>
					<td>Available Filters
					</td>
					<td>&nbsp;
					</td>
					<td>Selected Filters
					</td>
					<td>Applied Filters
					</td>
				</tr>
				<tr style="vertical-align: top;">
					<td>
<%--   						<ul id="lstFilterFields" class="filterFields" runat="server" style="width: 180px;">
						</ul>--%>

						<ul id="lstFilterFields" class="filterFields" runat="server" style="width: 180px;">
						</ul>
                        <div id="skm_LockPane" class="LockOff"></div>
					</td>
					<td>
						<asp:ListBox ID="lstAvailableFilters" runat="server" Style="width: 180px;" SelectionMode="Multiple"></asp:ListBox>
					</td>
					<td style="vertical-align: middle;">
						<table>
							<tr>
								<td>
									<button id="btnAdd" style="width: 100px" onclick="btnAdd_Click(); return false;">>></button>
								</td>
							</tr>
							<tr>
								<td>
									<button id="btnRemove" style="width: 100px" onclick="btnRemove_Click(); return false;"><<</button>
								</td>
							</tr>
                            <tr>
								<td>
									<button id="btnClearField" style="width: 100px" onclick="btnClearField_Click(); return false;">Clear Field</button>
								</td>
							</tr>
							<tr style="position: absolute; bottom: 50px;">
								<td>
									<button id="btnClearAll" style="width: 100px" onclick="btnClearAll_Click(); return false;">Clear All</button>
								</td>
							</tr>
						</table>
					</td>
					<td>
						<asp:ListBox ID="lstSelectedFilters" runat="server" Style="width: 180px;" SelectionMode="Multiple"></asp:ListBox>
					</td>
					<td>
						<div id="lstAppliedFilters" class="filterContainer" style="width: 180px; text-align: left;"></div>
					</td>
				</tr>
			</table>
		</div>
		<div id="filterFooter" class="PopupFooter">
			<table>
				<tr>
					<td>
						<button id="btnSave" onclick="btnSave_Click(); return false;">Finish</button>
					</td>
					<td>
						<button id="btnCancel" onclick="closeWindow(); return false;">Cancel</button>
					</td>
				</tr>
			</table>
		</div>
		<asp:ScriptManager ID="PageMethods" runat="server" EnablePageMethods="true"></asp:ScriptManager>
		<script type="text/javascript">
			var lstFilterFields = document.getElementById('lstFilterFields');
			var lstAvailableFilters = document.getElementById('lstAvailableFilters');
			var btnAdd = document.getElementById('btnAdd');
			var btnRemove = document.getElementById('btnRemove');
            var btnClearField = document.getElementById('btnClearField');
			var btnClearAll = document.getElementById('btnClearAll');
			var lstSelectedFilters = document.getElementById('lstSelectedFilters');
			var lstAppliedFilters = document.getElementById('lstAppliedFilters');
			var filterFooter = document.getElementById('filterFooter');
			var parentModule = '<%=HttpContext.Current.Request.QueryString["parentModule"]%>';
            var filterBox;
            var currentCustomFilter = '';

			if ('<%=this.Source %>' == 'AOR' && opener && opener.filterBox != undefined) {
				filterBox = opener.filterBox.clone(lstAppliedFilters);
            }
            else if ('<%=this.Source %>' == 'Report' && opener && opener.filterBox != undefined) {
                filterBox = opener.filterBox.clone(lstAppliedFilters);
            }
			else {
				 filterBox = top.filterBox.clone(lstAppliedFilters);
			}

			filterBox.editableDisplay = true;
			filterBox.toTable({ groups: { ParentModule: parentModule } }, 'Module');

			var strSelectedFilterField = '';
			var selectedFilterItem = '';

            function filterField_Click(el) {
				if (selectedFilterItem != el) {
					if (selectedFilterItem) {
						selectedFilterItem.className = 'filterFieldsItem';
					}
					selectedFilterItem = el;

					selectedFilterItem.className = 'filterFieldsSelected';
					strSelectedFilterField = ($(selectedFilterItem).attr('field')) ? $(selectedFilterItem).attr('field') : selectedFilterItem.innerText;
				}

				loadSelectedFilter(el.getAttribute('FIELD'));
			}

            function loadSelectedFilter(filterName) {
				lstSelectedFilters.options.length = 0;
				lstAvailableFilters.options.length = 0;
				var filters = filterBox.toJSON({ groups: { ParentModule: parentModule } });

				var myData = ('<%=this._myData.ToString().ToUpper() %>' == 'TRUE');

                PageMethods.LoadFilters(parentModule, filterName, filters, myData, '<%=this.Options %>', loadSelectedFilter_Done, OnError);
			}

            function loadSelectedFilter_Done(results) {
				if (results && results != "null") {
					results = jQuery.parseJSON(results);

					var filter = filterBox.filters.find({ name: strSelectedFilterField, groups: { ParentModule: parentModule, Module: "Custom" } })[0];

					for (var i = 0; i <= results.length - 1; i++) {
						var nOpt = document.createElement('option');
                        nOpt.value = results[i].FilterID;
                        if (/([a-z]\.[a-z])/.test(results[i].FilterValue) || /([A-Z]\.[A-Z])/.test(results[i].FilterValue)) {
                            nOpt.text = capitialize(results[i].FilterValue);
                            if (nOpt.text.length > 20) nOpt.title = capitialize(results[i].FilterValue);
                        } else {
                            nOpt.text = results[i].FilterValue;
                            if (nOpt.text.length > 20) nOpt.title = results[i].FilterValue;
                        }

						var selectedParameter = '';
						if (filter) {
							selectedParameter = filter.parameters.findByValue(nOpt.value);
						}
						if (!selectedParameter) {
							lstAvailableFilters.options.add(nOpt);
						}
					}
					if (filter) {
						for (var i = 0; i <= filter.parameters.length - 1; i++) {
							var nOpt = document.createElement('option');
							nOpt.value = filter.parameters[i].value;
                            nOpt.text = filter.parameters[i].text;
                            if (nOpt.text.length > 20) nOpt.title = filter.parameters[i].text;

							lstSelectedFilters.options.add(nOpt);
						}
					}
				}
				skm_UnLockScreen();
			}

			function OnError(e) {
			    skm_UnLockScreen();
			    MessageBox(e._message);
			};

			function btnAdd_Click() {
				for (var i = 0; i <= lstAvailableFilters.options.length - 1; i++) {
					if (lstAvailableFilters.options[i].selected) {
						var nOpt = document.createElement('option');
						nOpt.value = lstAvailableFilters.options[i].value;
                        nOpt.text = lstAvailableFilters.options[i].text;
                        if (nOpt.text.length > 20) nOpt.title = lstAvailableFilters.options[i].text;

						lstSelectedFilters.options.add(nOpt);
						filterBox.filters.add({ name: strSelectedFilterField, groups: { ParentModule: parentModule, Module: "Custom" } }).parameters.add(nOpt.value, nOpt.text);

						lstAvailableFilters.options.remove(i);
						i--;
					}
				}

				filterBox.toTable({ groups: { ParentModule: parentModule } }, 'Module');
			}

			function btnRemove_Click() {
				for (var i = 0; i <= lstSelectedFilters.options.length - 1; i++) {
					if (lstSelectedFilters.options[i].selected) {
						var nOpt = document.createElement('option');
						nOpt.value = lstSelectedFilters.options[i].value;
                        nOpt.text = lstSelectedFilters.options[i].text;
                        if (nOpt.text.length > 20) nOpt.title = lstSelectedFilters.options[i].text;

						lstAvailableFilters.options.add(nOpt);
						filterBox.filters.find({ name: strSelectedFilterField, groups: { ParentModule: parentModule, Module: "Custom" } })[0].parameters.findByValue(nOpt.value).remove();

						lstSelectedFilters.options.remove(i);
						i--;
					}
				}

				filterBox.toTable({ groups: { ParentModule: parentModule } }, 'Module');
            }

            function btnClearField_Click() {
                for (var i = 0; i <= lstSelectedFilters.options.length - 1; i++) {
                    var nOpt = document.createElement('option');
                    nOpt.value = lstSelectedFilters.options[i].value;
                    nOpt.text = lstSelectedFilters.options[i].text;
                    if (nOpt.text.length > 20) nOpt.title = lstSelectedFilters.options[i].text;
                    lstAvailableFilters.options.add(nOpt);
                    filterBox.filters.find({ name: strSelectedFilterField, groups: { ParentModule: parentModule, Module: "Custom" } })[0].parameters.findByValue(nOpt.value).remove();

                    lstSelectedFilters.options.remove(i);
                    i--;
                }

                filterBox.toTable({ groups: { ParentModule: parentModule } }, 'Module');
            }

			function btnClearAll_Click() {
				var filters = filterBox.filters.find({ groups: { ParentModule: parentModule, Module: "Custom" } });
				if (filters) {
					filters.clear();
					filterBox.toTable({ groups: { ParentModule: parentModule } }, 'Module');
				}

				if (strSelectedFilterField != '') loadSelectedFilter(strSelectedFilterField);
			}

            function btnSave_Click() {
				if ('<%=this.Source %>' == 'AOR' && opener && opener.filterBox != undefined) {
					opener.filterBox = filterBox.clone(opener.filterBox.containerElement);
					opener.filterBox.toTable({ groups: { ParentModule: parentModule } }, 'Module');
					$('#txtTaskSearch', opener.document).val('');

					if (opener.loadGrid) opener.loadGrid();
                }
                else if ('<%=this.Source %>' == 'Report' && opener && opener.filterBox != undefined) {
                    opener.filterBox = filterBox.clone(opener.filterBox.containerElement);
                    opener.filterBox.toTable({ groups: { ParentModule: parentModule } }, 'Module');
                    $('#txtTaskSearch', opener.document).val('');

                    if (opener.loadGrid) opener.loadGrid();
                }
                else if ('<%=this.Source %>' == 'RQMTParams') {
                    top.filterBox = filterBox.clone(top.filterBox.containerElement);
                    top.filterBox.toTable({ groups: { ParentModule: parentModule } }, 'Module');
                    return;
                }
                else {
					top.filterBox = filterBox.clone(top.filterBox.containerElement);
					top.filterBox.toTable({ groups: { ParentModule: parentModule } }, 'Module');
					try { opener.saveFilters(false, "Load_HomePage"); } catch (e) { }
                }
                
				closeWindow();
            }

            function saveCustomFilters(filterName) {
                try {
                    var collectionName = ddlSavedFilters.SelectedText;
                    if (filterName) collectionName = filterName;
                    if (collectionName == '' || collectionName == '- Add New -') {
                        MessageBox("Please enter or select a name to save the custom filter.");
                        return false;
                    }

                    var filterCount = filterBox.filters.find({ groups: { ParentModule: parentModule } });
                    if (filterCount && filterCount.length > 0) {
                        if (confirm('Save custom filters with the name of ' + collectionName + '?')) {
                            var filters = filterBox.toJSON({ groups: { ParentModule: parentModule } });
                            if (filters) PageMethods.SaveCustomFilter(collectionName, false, parentModule, filters, saveCustomFilters_done, on_error);
                        }
                    }
                    else {
                        MessageBox('Please select at least one filter to save.');
                        return false;
                    }
                }
                catch (e) { }
            }
            function saveCustomFilters_done(result) {
                var saved = false;
                var collectionName = '';
                var customFilter = '';
                var errorMsg = '';

                try {
                    var obj = jQuery.parseJSON(result);

                    if (obj) {
                        if (obj.saved && obj.saved.toUpperCase() == 'TRUE') {
                            saved = true;
                        }
                        if (obj.collectionName) {
                            collectionName = obj.collectionName;
                        }
                        if (obj.customFilter) {
                            customFilter = obj.customFilter;
                        }
                        if (obj.error) {
                            errorMsg = obj.error;
                        }
                    }

                    if (saved) {
                        dlFilters = ddlSavedFilters.DropDownList;

                        var option = '';
                        for (var i = 0; i <= dlFilters.options.length - 1; i++) {
                            if (dlFilters.options[i].text == collectionName) {
                                option = dlFilters.options[i];
                                dlFilters.selectedIndex = i;
                                break;
                            }
                        }
                        if (!option) {
                            option = document.createElement('option');
                            option.text = collectionName;
                            dlFilters.options.add(option);
                            dlFilters.selectedIndex = dlFilters.options.length - 1;
                        }
                        option.value = customFilter;
                        currentCustomFilter = '';

                        MessageBox("Custom filter saved successfully...");

                        setCustomFilter();
                    }
                    else {
                        MessageBox("Error: Filters were not saved...");
                    }
                }
                catch (e) { }
            }

            function deleteCustomFilters() {
                try {
                    var collectionName = ddlSavedFilters.SelectedText;
                    if (collectionName == '' || collectionName == '- Add New -') {
                        MessageBox("Please select a filter to delete.");
                        return false;
                    }

                    if (confirm('You are about to delete the ' + collectionName + ' custom filter, are you sure you wish to continue?')) {
                        var filters = filterBox.toJSON({ groups: { ParentModule: parentModule, Module: "Custom" } });
                        if (filters) PageMethods.SaveCustomFilter(collectionName, true, parentModule, filters, deleteCustomFilters_done, on_error);
                    }
                }
                catch (e) { }
            }
            function deleteCustomFilters_done(result) {
                var saved = false;
                var collectionName = '';
                var customFilter = '';
                var errorMsg = '';

                try {
                    var obj = jQuery.parseJSON(result);

                    if (obj) {
                        if (obj.saved && obj.saved.toUpperCase() == 'TRUE') {
                            saved = true;
                        }
                        if (obj.collectionName) {
                            collectionName = obj.collectionName;
                        }
                        if (obj.customFilter) {
                            customFilter = obj.customFilter;
                        }
                        if (obj.error) {
                            errorMsg = obj.error;
                        }
                    }

                    if (saved) {
                        document.location.href = 'Loading.aspx?Page=' + document.location.href;

                        MessageBox("Custom filter deleted...");  
                    }
                    else {
                        MessageBox("There was an error and the filter set was not deleted");
                    }
                }
                catch (e) {
                    alert("Error in deleteCustomFilters_done. " + e.message);
                }
            }

            function loadCustomFilters() {
                try {
                    var sOpt = document.createElement('option');
                    sOpt.value = '0';
                    sOpt.text = '- Add New -';
                    ddlSavedFilters.DropDownList.options.add(sOpt);

                    if (parent.parent && parent.parent.ddlSavedFilters.SelectedValue.length > 0) {
                        ddlSavedFilters.SetSelectedValue(parent.parent.ddlSavedFilters.SelectedValue);
                        ddlSavedFilters.SetSelectedText(parent.parent.ddlSavedFilters.SelectedText);
                    } else {
                        ddlSavedFilters.SetSelectedValue(sOpt.value);
                        ddlSavedFilters.SetSelectedText(sOpt.text);
                    }
                }
                catch (e) {
                }
            }

            function setCustomFilter() {
                if (currentCustomFilter != ddlSavedFilters.SelectedText && ddlSavedFilters.SelectedValue !== "0") {
                    var curFilters = filterBox.filters.find({ groups: { ParentModule: parentModule, Module: "Custom" } });
                    if (curFilters) {
                        curFilters.clear();
                        filterBox.toTable('', 'Module');
                    }
                    if (ddlSavedFilters.DropDownList.value != -1) {
                        var filters = ddlSavedFilters.DropDownList.value.split("`");

                        for (var i = 0; i <= filters.length - 1; i++) {
                            var filterName = filters[i].split('|')[0];
                            var filterField = filters[i].split('|')[1];
                            if (filterField) {
                                var parameterID = filters[i].split('|')[2].split(',');
                                var parameterName = filters[i].split('|')[3].split(',');

                                for (var y = 0; y <= parameterID.length - 1; y++) {
                                    if (parameterID[y] != '' && parameterName[y] != '') {
                                        filterBox.filters.add({ name: filterName, groups: { ParentModule: parentModule, Module: "Custom" } }).parameters.add(parameterID[y], parameterName[y]);
                                    }
                                }
                            }
                        }
                    }
                    filterBox.toTable({ groups: { ParentModule: parentModule } }, 'Module');
                }
                currentCustomFilter = ddlSavedFilters.SelectedText;
            }

            function on_error(result) {
                MessageBox('save error:  \n' + result);
            }

            function capitialize(input) {
                var text = input.split('.');
                var output = '';
                for (var i = 0; i < text.length; i++) {
                    output += text[i].replace(/\w\S*/g, function (txt) { return txt.charAt(0).toUpperCase() + txt.substr(1).toLowerCase(); });
                    if (i < text.length - 1) output += '.';
                }
                return output;
            }

			function resizePage() {
				resizePageElement('lstFilterFields', filterFooter.offsetHeight + 10);
				resizePageElement('lstAvailableFilters', filterFooter.offsetHeight + 8);
				resizePageElement('lstSelectedFilters', filterFooter.offsetHeight + 8);
                resizePageElement('lstAppliedFilters', filterFooter.offsetHeight + 10);

                if (ddlSavedFilters) {
                    ddlSavedFilters.Reposition();
                    $('#ddlSavedFilters_Text').css('left', '66px');
                    $('#ddlSavedFilters_Text').css('margin-top', '4px');
                }
			}

		</script>
    </form>

	<script id="jsInit" type="text/javascript">
		
		$(document).ready(function () {
            $(window).resize(resizePage);
            $('#imgSaveFilter').click(function () { saveCustomFilters(); });
            $('#imgDeleteFilter').click(function () { deleteCustomFilters(); });
            //Hide the filter footer when coming from Sys RQMT Sets Parameter page and show the filters list
            if ('<%= this.Source%>' == 'RQMTParams') {
                $('#filterFooter').hide();
                $('#divFilters').show();
                loadCustomFilters();
                $('#ddlSavedFilters').on('change', function () {
                    if (parent.parent) parent.parent.ddlSavedFilters.SetSelectedValue($(this).val());
                    btnSave_Click();
                });
            } 
            
			resizePage();
		});
	</script>
</body>
</html>