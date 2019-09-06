<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MultiSelect.ascx.cs" Inherits="Controls_MultiSelect" %>
<table width="100%" cellpadding="0" cellspacing="0" border="0">
    <tr>
        <td style="width: 150px;" ID="labelTD" runat="server">
            <asp:Label runat="server" ID="msLabel"><%=Label%></asp:Label>
        </td>
        <td>
            <div class="form-group" id="ddlTD" runat="server" style="width:100%">
                <select id="ddlItems" multiple="true" runat="server" width="100%">
                </select>
            </div>
        </td>
    </tr>
</table>
<link rel="stylesheet" href="Styles/multiple-select.css" />
<script type="text/javascript" src="Scripts/multiselect/jquery.multiple.select.js"></script>
<style type="text/css">
    .ms-choice {
        width:<%=ButtonWidth%>;
        display:<%=(HideDDLButton ? "none" : "block")%>;
    }
</style>
<script type="text/javascript">
    // this var is used to keep track of the ddl for when it moves onto popup dialogs and off the page
    var _<%=this.ClientID%>_ddlItems;

    function <%=ID%>_init() {
        _<%=this.ClientID%>_ddlItems = $('#<%=this.ClientID%>_ddlItems');

        var groups = '<%=string.Join(";", Groups)%>'.split(';');

        for (var i = 0; i < groups.length; i++) {
            var group = groups[i];

            group = group.replace('<semicolon>', ';');
            group = group.replace('<equals>', '=');

            $("#<%=this.ClientID%>_ddlItems option[OptionGroup='" + group + "']").wrapAll("<optgroup label='" + group + "'>");
        } 

        $('#<%=this.ClientID%>_ddlItems').multipleSelect({
            placeholder: 'Default'
            , width: 'undefined'
            , isOpen: <%=IsOpen.ToString().ToLower()%>
            , keepOpen: <%=KeepOpen.ToString().ToLower()%>
            , maxHeight: '<%=MaxHeight%>'
            , width: '<%=Width%>'
            , minimumCountSelected: '<%=MinimumCountSelected%>'
            , onClick: function () {
                <%=(string.IsNullOrWhiteSpace(OnClickFunctionName) ? "" : OnClickFunctionName + "();")%>
            }
            , onCheckAll: function () {
                <%=(string.IsNullOrWhiteSpace(OnCheckAllFunctionName) ? "" : OnCheckAllFunctionName + "();")%>
            }
            , onClose: function () {
                <%=(string.IsNullOrWhiteSpace(OnOpenFunctionName) ? "" : OnOpenFunctionName + "();")%>
            }
            , onClose: function () {
                <%=(string.IsNullOrWhiteSpace(OnCloseFunctionName) ? "" : OnCloseFunctionName + "();")%>
            }
        }).change(function () { <%=(string.IsNullOrWhiteSpace(OnChangeFunctionName) ? "" : OnChangeFunctionName + "();")%> });    

        //$('#<%=this.ClientID%>_ddlTD button.ms-choice').css('color', 'red');

        $("#<%=this.ClientID%>_ddlTD label").each(function () {
           // if ($(this).text().length == 1) $(this).hide();
        });
    }

    // sends in default selections
    // if selections are a string, they are split using the supplied token (; if not supplied)
    function <%=ID%>_setSelections(parentSelections, childSelections, parentToken, childToken) {
        if (parentToken == null) parentToken = ';';
        if (childToken == null) childToken = ';';

        _<%=this.ClientID%>_ddlItems.multipleSelect('uncheckAll');

        var listOfIDsToCheck = [];

        if (parentSelections != null && <%=(OptionGroupValueColumnName != null ? "true" : "false")%>) {
            if (!$.isArray(parentSelections)) {
                parentSelections = parentSelections.split(parentToken);
            }

            var optionIDs = [];

            for (var i = 0; i < parentSelections.length; i++) {
                var groupOptions = $("#<%=this.ClientID%>_ddlItems option[OptionGroupValue='" + parentSelections[i] + "']");

                for (var x = 0; x < groupOptions.length; x++) {
                    optionIDs.push($(groupOptions[x]).attr('value'));
                }
            }
            _<%=this.ClientID%>_ddlItems.multipleSelect('setSelects', optionIDs);

        }

        if (childSelections != null) {

        }
    }

    function <%=ID%>_getSelections(parentSelections, customAttributes) {
        if (customAttributes != null && customAttributes.length > 0) {
            customAttributes = customAttributes.split(',');
        }

        if (parentSelections && <%=(OptionGroupValueColumnName != null ? "true" : "false")%>) {
            var selectedChildren = _<%=this.ClientID%>_ddlItems.multipleSelect('getSelects');

            if (selectedChildren.length > 0) {
                var parentsSelected = [];

                // first, find all parents that have at least one item selected
                for (var i = 0; i < selectedChildren.length; i++) {
                    var c = _<%=this.ClientID%>_ddlItems.find('option[value=' + selectedChildren[i] + ']');
                    var pID = $(c).attr('OptionGroupValue');

                    if (parentsSelected.indexOf(pID) == -1) {
                        parentsSelected.push(pID);
                    }
                }

                // now, find all parents that have EVERY child selected (when any children are unselected, the parent isn't selected)
                var finalParentSelections = [];
                for (var i = 0; i < parentsSelected.length; i++) {
                    var pID = parentsSelected[i];

                    var notCheckedChildren = _<%=this.ClientID%>_ddlItems.find('option[OptionGroupValue=' + pID + ']').not(':selected');

                    if (notCheckedChildren.length == 0 && finalParentSelections.indexOf(pID) == -1) {
                        var parentOption = _<%=this.ClientID%>_ddlItems.find('option[OptionGroupValue=' + pID + ']');
                        for (var x = 0; customAttributes != null && x < customAttributes.length; x++) {
                            var attr = $(parentOption).attr(s.trim(customAttributes[x]));
                            if (attr == null || attr.length == 0) attr = 'EMPTY';
                            pID += "___" + attr;
                        }

                        finalParentSelections.push(pID);
                    }
                }

                return finalParentSelections;
            }
            else {
                return [];
            }
        }
        else {
            // this will include any checked children, as well as ALL children of checked parents
            var cArr = _<%=this.ClientID%>_ddlItems.multipleSelect('getSelects');

            if (<%=(OptionGroupValueColumnName != null ? "true" : "false")%>) {
                // if there are parents, then the format of the return IDs will be pid___cid
                var finalCArr = [];

                for (var i = 0; i < cArr.length; i++) {
                    var c = cArr[i].split('___');
                    var cID = c[1];

                    var childOption = _<%=this.ClientID%>_ddlItems.find('option[value=' + cArr[i] + ']');
                    for (var x = 0; customAttributes != null && x < customAttributes.length; x++) {
                        var attr = $(childOption).attr(s.trim(customAttributes[x]));
                        if (attr == null || attr.length == 0) attr = 'EMPTY';
                        cID += "___" + attr;
                    }
                    
                    finalCArr.push(cID);
                }

                return finalCArr;
            }
            else {
                return cArr;
            }
        }
    }

    function <%=ID%>_close() {
        _<%=this.ClientID%>_ddlItems.multipleSelect('close');
    }
</script>