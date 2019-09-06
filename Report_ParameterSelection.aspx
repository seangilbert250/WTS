<%@ Page Title="" Language="C#" MasterPageFile="~/Content.master" AutoEventWireup="true" CodeFile="Report_ParameterSelection.aspx.cs" Inherits="Report_ParameterSelection" Theme="Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="headTitle" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="Server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolderMetrics" runat="Server">
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
    <script src="Scripts/jquery-ui-1.10.2.custom.min.js" type="text/javascript"></script>
    <style type="text/css">
        .fieldsList > li {
            padding-top: 3px;
            padding-left: 15px;
            cursor: url("Images/Cursor/openhand.cur"), move;
            white-space: nowrap;
        }

        .tblRollupLevel {
            border-collapse: collapse;
            width: 475px;
            margin-bottom: 15px;
        }

            .tblRollupLevel tbody tr {
                cursor: url("Images/Cursor/openhand.cur"), move;
            }

            .tblRollupLevel td {
                border: 1px solid rgb(200, 200, 200);
                height: 25px;
                padding-left: 7px;
            }

        .thRollupLevelHeader {
            border: 1px solid rgb(200, 200, 200);
            background-color: rgb(220, 220, 220);
            height: 20px;
            text-align: left;
            padding-left: 7px;
        }

        .tblRollupLevel select, .tblRollupLevel img[src$="cross.png"] {
            float: right;
            margin-right: 5px;
        }

        .tblRollupLevel img[src$="cross.png"] {
            height: 14px;
            width: 14px;
            cursor: pointer;
        }

        #columnList {
            display: inline;
            list-style: none;
        }

            #columnList li {
                display: inline;
                cursor: url("Images/Cursor/openhand.cur"), move;
            }

        .ui-sortable-helper, .ui-draggable-dragging {
            cursor: url("Images/Cursor/closedhand.cur"), move;
            background-color: Window;
            display: table;
            width: 100px;
        }

        #reportOptionsTable {
            -webkit-touch-callout: none; /* iOS Safari */
            -webkit-user-select: none; /* Safari */
            -khtml-user-select: none; /* Konqueror HTML */
            -moz-user-select: none; /* Firefox */
            -ms-user-select: none; /* Internet Explorer/Edge */
            user-select: none; /* Non-prefixed version, currently
                                          supported by Chrome and Opera */
        }



    </style>
    <asp:ScriptManager ID="scriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>
    <div style="height: 1500px">
        <table id="reportOptionsTable" style="border-collapse: collapse; min-height: 514px; border: 1px solid gray; float: left; display: none;">
            <tbody>
                <tr id="trFilterManager">
                    <td id="filterBox" style="display: none;">
                        <div id="divPageManagerFooter" style="width: 100%; background-color: white;">
                            <table id="tablePageManagerButtons" class="pageContentHeader" style="width: 100%; border-right: 1px solid #9A9A9A; border-bottom: 1px solid #9A9A9A;">
                                <tr>
                                    <td style="vertical-align: middle; text-align: left; height: 30px;">
                                        <img id="imgShowFilters" src="Images/icons/funnel.png" title="Assign Filters" alt="Assign Filters" style="cursor: pointer; padding-left: 5px;" />
                                        <img id="imgClearFilters" src="Images/icons/eraser.png" title="Clear Filters" alt="Clear Filters" style="cursor: pointer; padding-left: 3px;" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <div id="divFilterManagerBody">
                            <table style="width: 100%;">
                                <tr>
                                    <td id="tdAppliedFilters" style="width: 100%; vertical-align: top; font-size: 12px;">
                                        <div id="divAppliedFilters" class="filterContainer" style="width: 216px; height: 450px;">
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </td>
                    <td style="vertical-align: top; border-left: 1px solid gray; border-right: 1px solid gray; padding: 0px; width: 225px;">
                        <div id="popupContainer" class="popupPageContainer"></div>
                        <div style="background: url(Images/Headers/gridheaderblue.png); padding: 1px; height: 20px; text-align: center; font-weight: bold;">
                            Report Options
                            <img src="Images/icons/help.png" alt="Help" title="CR Report Options" width="15" height="15" style="cursor:pointer;" onclick="loadHelpText('ReportOptions'); return"/>
                        </div>
                        <div id="divOptionsManager" style="width: 100%;">
                            <div id="divOptionsManagerHeader" style="width: 100%; background-color: white;">
                                <table id="tableOptionsManagerHeader" class="pageContentHeader" style="width: 100%; border-bottom: 1px solid #9A9A9A;">
                                    <tr>
                                        <td>Title / Other Report Options
                                        </td>
                                    </tr>
                                </table>
                            </div>
                            <div id="divOptionsManagerBody">
                                <table style="width: 100%;">
                                    <tr>
                                        <td id="tdReportTitleLabel" style="width: 30%; vertical-align: top; font-size: 12px;">Report Title
                                        </td>
                                        <td id="tdReportTitleField" style="width: 70%; vertical-align: top; font-size: 12px;">
                                            <asp:TextBox ID="ReportTitleField" runat="server"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr class="cr_releasedse">
                                        <td id="tdCoverPageCheckbox" style="text-align: right">
                                            <asp:CheckBox ID="CoverPageCheckbox" runat="server" Checked='<%# Eval("CoverPageCheckbox") %>' />
                                        </td>
                                        <td>Add Cover Page
                                        </td>
                                    </tr>
                                    <tr class="cr">
                                        <td id="tdIndexPageCheckbox" style="text-align: right">
                                            <asp:CheckBox ID="IndexPageCheckbox" runat="server" Checked='<%# Eval("IndexPageCheckbox") %>' />
                                        </td>
                                        <td>Add Index Page
                                        </td>
                                    </tr>
                                    <tr style="display: none;">
                                        <td id="tdEmailCheckbox" style="text-align: right">
                                            <asp:CheckBox ID="EmailCheckbox" runat="server" Enabled="false" Checked='<%# Eval("EmailCheckbox") %>' />
                                        </td>
                                        <td>Email Report
                                        </td>
                                    </tr>
                                </table>
                            </div>
                            <div id="divReportViewOptionsHeader" class="cr" style="width: 100%; background-color: white;">
                                <table id="tableReportViewOptionsHeader" class="pageContentHeader" style="width: 100%; border-bottom: 1px solid #9A9A9A;">
                                    <tr>
                                        <td>Report View Options
                                        </td>
                                    </tr>
                                </table>
                            </div>
                            <div id="divReportViewOptionsBody" >
                                <table style="width: 100%;">
                                    <tr>
                                        <td style="width: 30%;"></td>
                                    </tr>
                                    <tr class="cr">
                                        <td id="tdSRsCheckbox" style="text-align: right">
                                            <asp:CheckBox ID="SRsCheckbox" runat="server" Checked='<%# Eval("SRsCheckbox") %>' />
                                        </td>
                                        <td>Include SRs
                                        </td>
                                    </tr>
                                    <tr class="cr">
                                        <td id="tdAORsCheckbox" style="text-align: right">
                                            <asp:CheckBox ID="AORsCheckbox" runat="server" Checked='<%# Eval("AORsCheckbox") %>' />
                                        </td>
                                        <td>Include AORs
                                        </td>
                                    </tr>
                                    <tr class="releasedse">
                                        <td id="tdSummaryMetrics" style="text-align: right">
                                            <asp:CheckBox ID="SummaryMetricsCheckBox" runat="server" Checked='<%# Eval("SummaryMetricsCheckBox") %>' />
                                        </td>
                                        <td>Include Release Review
                                        </td>
                                    </tr>
                                    <tr class="releasedse">
                                        <td id="tdDeployMetrics" style="text-align: right">
                                            <asp:CheckBox ID="DeployMetricsCheckBox" runat="server" Checked='<%# Eval("DeployMetricsCheckBox") %>' />
                                        </td>
                                        <td>Include Deployment Metrics
                                        </td>
                                    </tr>
                                    <tr class="releasedse">
                                        <td id="tdCDRLMetrics" style="text-align: right">
                                            <asp:CheckBox ID="CDRLMetricsCheckbox" runat="server" Checked='<%# Eval("CDRLMetricsCheckBox") %>' />
                                        </td>
                                        <td>Include CDRL Metrics
                                        </td>
                                    </tr>
                                    <tr class="releasedse">
                                        <td id="tdDeploySummaryCheckbox" style="text-align: right">
                                            <asp:CheckBox ID="DeploySummaryCheckbox" runat="server" Checked='<%# Eval("DeploySummaryCheckbox") %>' />
                                        </td>
                                        <td>Include Table of Contents
                                        </td>
                                    </tr>
                                    <tr class="cr_releasedse">
                                        <td id="tdSessionMetricsCheckbox" style="text-align: right">
                                            <asp:CheckBox ID="SessionMetricsCheckbox" runat="server" Checked='<%# Eval("SessionMetricsCheckbox") %>' />
                                        </td>
                                        <td>Include Session Metrics
                                        </td>
                                    </tr>
                                    <tr class="releasedse">
                                        <td id="tdSprintMetricsCheckbox" style="text-align: right">
                                            <asp:CheckBox ID="SprintMetricsCheckbox" runat="server" Checked='<%# Eval("SprintMetricsCheckbox") %>' />
                                        </td>
                                        <td>Include Deployment Session Metrics
                                        </td>
                                    </tr>
                                    <tr class="releasedse">
                                        <td id="tdDSEIndex" style="text-align: right">
                                            <asp:CheckBox ID="DSEIndexCheckBox" runat="server" Checked='<%# Eval("DSEIndexCheckBox") %>' />
                                        </td>
                                        <td>Include CR Index
                                        </td>
                                    </tr>
                                    <tr class="releasedse">
                                        <td id="tdLegends" style="text-align: right">
                                            <asp:CheckBox ID="LegendsCheckBox" runat="server" Checked='<%# Eval("LegendsCheckBox") %>' />
                                        </td>
                                        <td>Include Legends
                                        </td>
                                    </tr>
                                </table>
                            </div>
                            <div id="divLeadingIndicatorHeader" class="cr" style="width: 100%; background-color: white;">
                                <table id="tableLeadingIndicatorHeader" class="pageContentHeader" style="width: 100%; border-bottom: 1px solid #9A9A9A;">
                                    <tr>
                                        <td>Leading Indicator Options
                                        </td>
                                    </tr>
                                </table>
                            </div>
                            <div id="divLeadingIndicatorBody" class="cr">
                                <table style="width: 100%;">
                                    <tr>
                                        <td style="width: 30%;"></td>
                                    </tr>
                                    <tr>
                                        <td id="tdBestCheckbox" style="text-align: right">
                                            <asp:CheckBox ID="BestCheckbox" runat="server" Checked='<%# Eval("BestCheckbox") %>' />
                                        </td>
                                        <td>Include Best Case
                                        </td>
                                    </tr>
                                    <tr>
                                        <td id="tdWorstCheckbox" style="text-align: right">
                                            <asp:CheckBox ID="WorstCheckbox" runat="server" Checked='<%# Eval("WorstCheckbox") %>' />
                                        </td>
                                        <td>Include Worst Case
                                        </td>
                                    </tr>
                                    <tr>
                                        <td id="tdNormCheckbox" style="text-align: right">
                                            <asp:CheckBox ID="NormCheckbox" runat="server" Checked='<%# Eval("NormCheckbox") %>' />
                                        </td>
                                        <td>Include Norm
                                        </td>
                                    </tr>
                                </table>
                            </div>
                            <div id="divAORCRHeader" class="cr" style="width: 100%; background-color: white;">
                                <table id="tableAORCRHeader" class="pageContentHeader" style="width: 100%; border-bottom: 1px solid #9A9A9A;">
                                    <tr>
                                        <td>CR and AOR Options
                                        </td>
                                    </tr>
                                </table>
                            </div>
                            <div id="divShowHideAORCR" class="cr">
                                <table style="width: 100%;">
                                    <tr>
                                        <td style="width: 30%;"></td>
                                    </tr>
                                    <tr>
                                        <td id="tdCRDescrCheckbox" style="text-align: right">
                                            <asp:CheckBox ID="CRDescrCheckbox" runat="server" Checked='<%# Eval("CRDescrCheckbox") %>' />
                                        </td>
                                        <td>Include CR Description
                                        </td>
                                    </tr>
                                    <tr>
                                        <td id="tdAORDescrCheckbox" style="text-align: right">
                                            <asp:CheckBox ID="AORDescrCheckbox" runat="server" Checked='<%# Eval("AORDescrCheckbox") %>' />
                                        </td>
                                        <td>Include AOR Description
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </div>
                    </td>

                    <td id="divRollupLevels" style="padding: 0px; vertical-align: top; height: 100%;display:none">
                        <div id="divRollupLevels" style="overflow: hidden; height: 511px; overflow-y: auto;">
                            <table id="sectionsTable" style="display: table-cell;">
                                <tr>
                                    <th style="background: url(Images/Headers/gridheaderblue.png); height: 20px;">Report Level of Detail
                                        <img src="Images/Icons/help.png" title="Items that are in progress" alt="Features that are in progress"
                                            onclick="MessageBox('Report Field Details do not alter the report and cannot be saved currently')"
                                            height="12" width="12" /></th>
                                </tr>
                                <tr>
                                    <td style="padding: 0px;">
                                        <div>
                                            <table>
                                                <tr>
                                                    <td style="vertical-align: top; padding: 0px;">
                                                        <div id="divAvailableFields">
                                                            <table id="tableLEvelsManagerHeader" class="pageContentHeader" style="width: 100%; border-right: 1px solid #9A9A9A; border-bottom: 1px solid #9A9A9A;">
                                                                <tr>
                                                                    <td>Available Fields
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                            <div id="divAvailFieldsContainer" style="background-color: #EDEDED; padding-right: 5px; padding-left: 2px;">
                                                                <div id="divCRFields" class="cr" runat="server" style="padding-bottom: 10px;">
                                                                    <img class="imgFieldsExpander" src="Images/icons/minus_blue.png" alt="Hide" title="Hide Value Fields" />
                                                                    <u>CR Fields</u>
                                                                    <ul id="ulCRFields" class="fieldsList" runat="server"></ul>
                                                                </div>
                                                                <div id="divSRFields" class="cr" runat="server" style="padding-bottom: 10px;">
                                                                    <img class="imgFieldsExpander" src="Images/icons/minus_blue.png" alt="Hide" title="Hide Comparison Fields" />
                                                                    <u>SR Fields</u>
                                                                    <ul id="ulSRFields" class="fieldsList" runat="server"></ul>
                                                                </div>
                                                                <div id="divAORFields" class="cr" runat="server" style="padding-bottom: 10px;">
                                                                    <img class="imgFieldsExpander" src="Images/icons/minus_blue.png" alt="Hide" title="Hide Comparison Fields" />
                                                                    <u>AOR Fields</u>
                                                                    <ul id="ulAORFields" class="fieldsList" runat="server"></ul>
                                                                </div>
                                                                <!--<div id="divTaskFields" class="cr" runat="server" style="padding-bottom: 10px;">
                                                                <img class="imgFieldsExpander" src="Images/icons/minus_blue_12.png" alt="Hide" title="Hide Comparison Fields" />
                                                                <u>Task Fields</u>
                                                                <ul id="ulTaskFields" class="fieldsList" runat="server"></ul>
                                                            </div> -->
                                                            </div>
                                                        </div>
                                                    </td>
                                                    <td style="vertical-align: top;">
                                                        <div id="divGridLevels" style="height: 400px; margin: 0px 5px;">
                                                            <div style="padding-left: 5px; padding-top: 5px; font-weight: bold;">
                                                                Grid Levels:
                                                           <span id="btnAddLevel" style="float: right; cursor: pointer;">
                                                               <img src="Images/icons/add_blue.png" style="width: 13px; height: 13px;" />
                                                               <span>Add Level</span>
                                                           </span>
                                                            </div>
                                                            <div id="divRollups" style="width: 100%; height: 230px; margin-bottom: 10px;">
                                                                <div style="display: none;">
                                                                    <table id="tblClone">
                                                                        <!--This is used as a template for cloning.-->
                                                                        <thead>
                                                                            <tr>
                                                                                <th colspan="2" class="thRollupLevelHeader">
                                                                                    <span></span>
                                                                                    <img src="Images/icons/cross.png" />
                                                                                </th>
                                                                            </tr>
                                                                        </thead>
                                                                        <tbody>
                                                                            <tr>
                                                                                <td>
                                                                                    <!--clone row-->
                                                                                    <span></span>
                                                                                    <img src="Images/icons/cross.png" />
                                                                                    <select>
                                                                                        <option value="ASC">Ascending</option>
                                                                                        <option value="DESC">Descending</option>
                                                                                    </select>
                                                                                </td>
                                                                            </tr>
                                                                        </tbody>
                                                                        <tfoot>
                                                                            <tr style="border: none !important;">
                                                                                <td style="border: 1px solid gainsboro !important; background-color: #EDEDED; color: gray">
                                                                                    <i>Drop breakout field here</i>
                                                                                </td>
                                                                            </tr>
                                                                        </tfoot>
                                                                    </table>
                                                                </div>
                                                                <table class="tblRollupLevel">
                                                                    <thead>
                                                                        <tr>
                                                                            <th colspan="2" class="thRollupLevelHeader">
                                                                                <span>Level 1</span>
                                                                            </th>
                                                                        </tr>
                                                                    </thead>
                                                                    <tbody>
                                                                    </tbody>
                                                                    <tfoot>
                                                                        <tr>
                                                                            <td style="border: 1px solid gainsboro !important; background-color: #EDEDED; color: gray">
                                                                                <i>Drop breakout field here</i>
                                                                            </td>
                                                                        </tr>
                                                                    </tfoot>
                                                                </table>
                                                            </div>
                                                        </div>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </td>
                </tr>
            </tbody>
        </table>
        <div id="divDimmer" style="position: absolute; filter: alpha(opacity = 60); width: 100%; display: none; background: gray; height: 100%; top: 0px; left: 0px; opacity: 0.6;"></div>
	    <div id="divViewName" style="width: 260px; background-color: white; z-index: 999; display: none;">
		    <table style="width: 100%;">
			    <tr>
				    <td class="pageContentInfo">
					    Grid View Name:
				    </td>
			    </tr>
			    <tr>
                    <td>
                        <select id="ddlSaveView" style="width: 255px;"></select>
                    </td>
                </tr>
			    <tr id="trViewName">
				    <td>
					    <asp:TextBox ID="txtViewName" runat="server" MaxLength="50" Width="250"></asp:TextBox>
				    </td>
			    </tr>
			    <tr>
				    <td>
					    <input type="checkbox" id="chkProcessView" style="vertical-align: middle;" />
					    <label for="chkProcessView" style="vertical-align: middle;">Process View</label>
				    </td>
			    </tr>
			    <tr>
				    <td>
					    <input type="button" id="buttonSaveView" value="Save" />&nbsp;<input type="button" id="buttonCancelView" value="Cancel" />
				    </td>
			    </tr>
		    </table>
	    </div>
    </div>
    <script id="jsEvents" type="text/javascript">
        var _pageUrls = new PageURLs();
        var popupContainer;
        var popupManager;
        var filterBoxLoc;
        var filterBox;

        var numStep = 0;
        var numLevels = 1;
        var numRollups = 0;
        var numGridColumns = 0;

        var level1Attr;
        var level2Attr;
        var level3Attr;
        var level4Attr;
        var currReportExists = false;

        function openMenuItem(url) {
            if (url.indexOf("relatedItems") > 0) {
                var str = url.split("'");
                relatedItems_click(str[1], str[3]);
            }
        }

        function relatedItems_click(action, type) {
            switch (action) {
                case 'ReleaseAssessment':
                    btnReleaseAssessment_click();
                    break;
            }
        }

        function btnReleaseAssessment_click() {
            var nWindow = 'ReleaseAssessmentGrid';
            var nTitle = 'Release Assessment Grid';
            var nHeight = 700, nWidth = 1200;
            var nURL = _pageUrls.Maintenance.AORReleaseAssessment + '?GridType=ReleaseAssessment';
            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

        function clearFilterBox() {
            var parentModule = $('#trSelectedModule').attr('moduleName');
            var filters = filterBoxLoc.filterBox.filters.find({ groups: { ParentModule: parentModule, Module: "Custom" } });
            if (filters) {
                filters.clear();
                filterBoxLoc.filterBox.toTable('', 'Module');
            }
        }

        function validate(reportParameters) {
            var validation = [];
            if (reportParameters["Title"].length == undefined) {
                validation.push('Title cannot be empty.');
            } 
            return validation.join('<br>');
        }

        function today() {
            var today = new Date();
            var dd = today.getDate();
            var mm = today.getMonth() + 1; //January is 0!
            var yyyy = today.getFullYear();
            if (dd < 10) {
                dd = '0' + dd
            }
            if (mm < 10) {
                mm = '0' + mm
            }
            return mm + '/' + dd + '/' + yyyy;
        }

        function loadDefaultFilters() {
            try {
                $('#' + '<%=ReportTitleField.ClientID%>').val(today());

                switch ('<%=this.ReportTypeID %>') {
                    case '2':
                        $('#' + '<%=CoverPageCheckbox.ClientID%>')[0].checked = "True";
                        $('#' + '<%=IndexPageCheckbox.ClientID%>')[0].checked = "True";
                        $('#' + '<%=EmailCheckbox.ClientID%>')[0].checked = "True";
                        $('#' + '<%=SRsCheckbox.ClientID%>')[0].checked = "True";
                        $('#' + '<%=AORsCheckbox.ClientID%>')[0].checked = "True";
                        $('#' + '<%=SessionMetricsCheckbox.ClientID%>')[0].checked = "";
                        $('#' + '<%=BestCheckbox.ClientID%>')[0].checked = "True";
                        $('#' + '<%=WorstCheckbox.ClientID%>')[0].checked = "True";
                        $('#' + '<%=NormCheckbox.ClientID%>')[0].checked = "True";
                        $('#' + '<%=CRDescrCheckbox.ClientID%>')[0].checked = "True";
                        $('#' + '<%=AORDescrCheckbox.ClientID%>')[0].checked = "True";
                        break;
                    case '4':
                        $('#' + '<%=CoverPageCheckbox.ClientID%>')[0].checked = "True";
                        $('#' + '<%=SessionMetricsCheckbox.ClientID%>')[0].checked = "";
                        $('#' + '<%=SprintMetricsCheckbox.ClientID%>')[0].checked = "";
                        $('#' + '<%=DeploySummaryCheckbox.ClientID%>')[0].checked = "True";
                        $('#' + '<%=CDRLMetricsCheckbox.ClientID%>')[0].checked = "True";
                        $('#' + '<%=SummaryMetricsCheckBox.ClientID%>')[0].checked = "True";
                        $('#' + '<%=DeployMetricsCheckBox.ClientID%>')[0].checked = "True";
                        $('#' + '<%=DSEIndexCheckBox.ClientID%>')[0].checked = "True";
                        $('#' + '<%=LegendsCheckBox.ClientID%>')[0].checked = "True";
                        break;
                }

                var parentModule = "Reports";

                var DefaultSystemFilters = '<%=this.DefaultReportFilters%>';
                var filters = DefaultSystemFilters.split("`");

                for (var i = 0; i <= filters.length - 1; i++) {
                    var filterName = filters[i].split('|')[0];
                    var filterField = filters[i].split('|')[1];
                    if (filterField) {
                        var parameterID = filters[i].split('|')[2].split(',,');
                        var parameterName = filters[i].split('|')[3].split(',,');

                        for (var y = 0; y <= parameterID.length - 1; y++) {
                            if (parameterID[y] != '' && parameterName[y] != '') {
                                filterBoxLoc.filterBox.filters.add({ name: filterName, groups: { ParentModule: parentModule, Module: "Custom" } }).parameters.add(parameterID[y], parameterName[y]);
                            }
                        }
                    }
                }
                filterBoxLoc.filterBox.toTable({ groups: { ParentModule: parentModule } }, 'Module');
            }
            catch (e) {
                MessageBox('Error: gatherFilters - ' + e.number + ' : ' + e.message);
            }
        }

        function imgShowFilters_click() {
            var module = 'Reports'
            var myData = true;

            var reportTypeID = '<%=this.ReportTypeID %>';

            ShowDimmer(false);

            var strURL;
            var h = 450, w = 900;
            var window = 'FilterPage';
            var title = 'Filter and Criteria';

            strURL = 'Loading.aspx?Page=FilterPage.aspx?random=' + new Date().getTime()
                + '&parentModule=' + module
                + '&MyData=' + myData;

            if (reportTypeID > 0) {
                strURL += '&Source=Report' + '&Options=' + reportTypeID;
            }

            var openPopup = popupManager.AddPopupWindow(window, title, strURL, h, w, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

        function imgClearFilters_click() {
            try {
                var parentModule = 'Reports';

                if (confirm('You are about to clear the Applied Custom Filters! Are you sure you want to continue?')) {
                    var filters = filterBox.filters.find({ groups: { ParentModule: parentModule, Module: "Custom" } });
                    if (filters) {
                        filters.clear();
                        filterBox.toTable('', 'Module');
                    }

                    filters = filterBox.filters.find({ groups: { ParentModule: parentModule, Module: "Default" } });
                    if (filters) {
                        filters.clear();
                        filterBox.toTable('', 'Module');
                    }

                    PageMethods.ClearFilterSession(parentModule, null, null);
                }
            }
            catch (e) {
                MessageBox('Error: btnClearFilters_Onclick - ' + e.number + ' : ' + e.message);
            }
        }

        function ddlSaveView_change() {
            var $opt = $('#ddlSaveView option:selected');

            if ($opt.text() != '--Create New--') {
                $('#trViewName').hide();
                $('#chkProcessView').prop('checked', $opt.attr('OptionGroup') == 'Process Views');
            }
            else {
                $('#<%=txtViewName.ClientID %>').val('');
                $('#trViewName').show();
                $('#chkProcessView').prop('checked', false);
            }
        }

        function saveView(obj) {
            $('#divDimmer').show();
            var pos = $(obj).position();

            $('#ddlSaveView option').filter(function () { return ($(this).text() === "--Create New--"); }).prop('selected', true);
            $('#trViewName').show();
            $('#chkProcessView').prop('checked', false);

            $('#divViewName').css({
                position: "absolute",
                top: pos.top + 31,
                left: pos.left - 150
            }).slideDown(function () { $('#<%=this.txtViewName.ClientID %>').focus(); });
        }

        function buttonSaveView_click() {
            try {
                var viewID = -1;
                if ($('[id="ddlSaveView"] option:selected').attr('viewid')) {
                    viewID = $('[id="ddlSaveView"] option:selected').attr('viewid');
                } 

                var viewName;
                if (viewID === -1) {
                    viewName = $('#<%=txtViewName.ClientID %>').val();

                    if (viewName == '' || viewName == '- Default Parameters -') {
                        $('#divViewName').hide();
                        $('#divDimmer').hide();
                        MessageBox("Please enter or select a name to save the custom filter.");
                        return false;
                    }
                } else {
                    viewName = $('[id="ddlSaveView"] option:selected').text();
                }
                
                var reportParameters = getReportParameters(true);
                var validation = validate(reportParameters);
                if (validation.length == 0) {
                    var reportLevels = getReportLevels();
                    //Check if view exists and if user wants to overwrite the exists view or not
                    var exists =
                        $('#ddlSaveView option').filter(function () {
                            return $(this).text() === viewName;
                        }).length > 0;
                    if (!exists) {
                        PageMethods.SaveCustomView(viewID, viewName, '<%=this.ReportTypeID%>', ($('#chkProcessView').is(':checked') ? 1 : 0), reportParameters, reportLevels, saveCustomView_done);
                    } else {
                        if (confirm("This View Already Exists. Do You Want To Overwrite This View?")) {//Save report    
                            $('[id="ddlSaveView"] option').each(function () {
                                if ($(this).val() === viewName) viewID = $(this).attr('viewid');
                            });
                            PageMethods.SaveCustomView(viewID, viewName, '<%=this.ReportTypeID%>', ($('#chkProcessView').is(':checked') ? 1 : 0), reportParameters, reportLevels, saveCustomView_done);
                        }
                    }
                }
                else {
                    MessageBox('Invalid entries: <br><br>' + validation);
                }
            } catch (e) {
                MessageBox('Invalid report filters');
            }
        }

        function deleteView() {
            var viewID;
            var viewName;
            if ($('[id*="SavedViewsDDL"] option:selected').val()) {
                viewID = $('[id*="SavedViewsDDL"] option:selected').attr('viewid');
                viewName = $('[id*="SavedViewsDDL"] option:selected').val();
            } 
            if (viewName == "- Default Parameters -") {
                MessageBox('You cannot delete the Default report view.');
            }
            else if ($('[id*="SavedViewsDDL"] option:selected').attr('MyView') !== '1') {
                MessageBox('You cannot delete a report view which was not created by you.');
            }
            else {
                if (confirm("Are you sure you want to delete: " + viewName + "?")) {
                    PageMethods.DeleteView(viewID, '<%=this.ReportTypeID%>', DeleteView_Done);
                }
            }
        }

        function DeleteView_Done(result) {
            deleteResult = JSON.parse(result);
            if (deleteResult["deleted"]) {
                refreshPage(0);
            } else {
                MessageBox('Unable to delete this view. ' + deleteResult["error"]);
            }
        }

        function getReportLevels() {
            var rollups = [];

            //Step1- Gather the data from the page to create an object of the page
            $('.tblRollupLevel').each(function () { //If there are rollup levels with no attributes in them, get rid of them. It trips up the formatting of the save object. 
                if ($('tbody', this).find('tr').length == 0) {
                    $(this).find('img[src$="cross.png"]').trigger('click');
                }
            });

            $('.tblRollupLevel tbody tr').each(function () {
                var rollup = {};
                rollup.levelID = $(this).closest('tbody').parent().find('.thRollupLevelHeader span').text().replace(/\D/g, '');
                rollup.attr = $('span', this).text();
                rollup.order = $('select option:selected', this).val();
                rollups.push(rollup);
            });

            //Step2- Create a JSON object from the data
            try {
                var saveObj = {};
                saveObj.rollups = JSON.stringify(rollups);
            }
            catch (e) {
                var x = 1;
            }

            //Step3- Return the Json
            return JSON.stringify(saveObj);
        }

        function getReportParameters(save) {
            var parentModuleFilter = "Reports";
            var filters = filterBoxLoc.filterBox.toJSON({ groups: { ParentModule: parentModuleFilter } });
            var params;
            if (filters != '{null:null}') {
                params = JSON.parse(filters);
            } else {
                params = JSON.parse("{\"Dummy\":{\"value\":\"0\",\"text\":\"0\"}}");
            }
            var reportParameters = new Object();
            reportParameters["Type"] = "pdf";
            reportParameters["Title"] = $('#' + '<%=ReportTitleField.ClientID%>').val();

            switch ('<%=this.ReportTypeID %>') {
                case '2':
                    reportParameters["CoverPage"] = $('#' + '<%=CoverPageCheckbox.ClientID%>')[0].checked;
                    reportParameters["IndexPage"] = $('#' + '<%=IndexPageCheckbox.ClientID%>')[0].checked;
                    reportParameters["EmailSupport"] = $('#' + '<%=EmailCheckbox.ClientID%>')[0].checked;
                    reportParameters["IncludeSRs"] = $('#' + '<%=SRsCheckbox.ClientID%>')[0].checked;
                    reportParameters["IncludeAORs"] = $('#' + '<%=AORsCheckbox.ClientID%>')[0].checked;
                    reportParameters["IncludeSessionMetrics"] = $('#' + '<%=SessionMetricsCheckbox.ClientID%>')[0].checked;
                    reportParameters["IncludeBestCase"] = $('#' + '<%=BestCheckbox.ClientID%>')[0].checked;
                    reportParameters["IncludeWorstCase"] = $('#' + '<%=WorstCheckbox.ClientID%>')[0].checked;
                    reportParameters["IncludeNormCase"] = $('#' + '<%=NormCheckbox.ClientID%>')[0].checked;
                    reportParameters["HideCRDescr"] = $('#' + '<%=CRDescrCheckbox.ClientID%>')[0].checked;
                    reportParameters["HideAORDescr"] = $('#' + '<%=AORDescrCheckbox.ClientID%>')[0].checked;
                    reportParameters["SavedView"] = $('[id*="SavedViewsDDL"]').val();
                    break;
                case '4':
                    reportParameters["CoverPage"] = $('#' + '<%=CoverPageCheckbox.ClientID%>')[0].checked;
                    reportParameters["IncludeSessionMetrics"] = $('#' + '<%=SessionMetricsCheckbox.ClientID%>')[0].checked;
                    reportParameters["IncludeSprintMetrics"] = $('#' + '<%=SprintMetricsCheckbox.ClientID%>')[0].checked;
                    reportParameters["IncludeDeploymentSummary"] = $('#' + '<%=DeploySummaryCheckbox.ClientID%>')[0].checked; 
                    reportParameters["IncludeCDRLMetrics"] = $('#' + '<%=CDRLMetricsCheckbox.ClientID%>')[0].checked;
                    reportParameters["IncludeDeploymentMetrics"] = $('#' + '<%=DeployMetricsCheckBox.ClientID%>')[0].checked;
                    reportParameters["IncludeSummaryMetrics"] = $('#' + '<%=SummaryMetricsCheckBox.ClientID%>')[0].checked;
                    reportParameters["IncludeDSEIndex"] = $('#' + '<%=DSEIndexCheckBox.ClientID%>')[0].checked;
                    reportParameters["IncludeLegends"] = $('#' + '<%=LegendsCheckBox.ClientID%>')[0].checked;
                    reportParameters["SavedView"] = $('[id*="SavedViewsDDL"]').val();
                    break;
            }
            
            if (save) {
                for (var key in params) {
                    reportParameters[key] = params[key].value + ",," + params[key].text;
                }
                return reportParameters;
            }
            else {
                <%--var emptyFields = false;
                if (!params["Release Version"] || !params["AOR Workload Type"] || !params["Contract"] || !params["Visible To Customer"] || !params["Workload Allocation"]) {
                    emptyFields = true;
                }
                if (emptyFields) {
                    PageMethods.getEmptyFilters(params, '<%=this.ReportTypeID%>', function (fullParams) {

                        for (var key in fullParams) {
                            reportParameters[key] = fullParams[key].value;
                        }
                        var validation = validate(reportParameters);
                        if (validation.length == 0) {
                            QueueReport('<%=this.ReportTypeID%>', reportParameters);
                        }
                        else {
                            MessageBox('Invalid entries: <br><br>' + validation);
                        }
                    });
                } else {--%>
                    for (var key in params) {
                        reportParameters[key] = params[key].value;
                    }
                    var validation = validate(reportParameters);
                    if (validation.length == 0) {
                        QueueReport('<%=this.ReportTypeID%>', reportParameters);
                    }
                    else {
                        MessageBox('Invalid entries: <br><br>' + validation);
                    }
                //}
            }
        }

        function reportBuilderButton_click() {
            var nURL = _pageUrls.Maintenance.CRReportBuilder + window.location.search;

            window.location.href = 'Loading.aspx?Page=' + nURL;
        }

        function saveCustomView_done(result) {
            var saved = false;
            var collectionName = '';
            var customFilter = '';
            var errorMsg = '';

            $('#divViewName').hide();
            $('#divDimmer').hide();
            try {
                var obj = jQuery.parseJSON(result);
                if (obj) {
                    if (obj.saved && obj.saved.toUpperCase() == 'TRUE') {
                        saved = true;
                    }
                    if (obj.error) {
                        errorMsg = obj.error;
                    }
                }

                if (saved) {
                    MessageBox("Report View saved successfully");
                    refreshPage(obj.viewid);
                }
                else {
                    MessageBox("Error: Report View was not saved... " + errorMsg);
                }
            }
            catch (e) {
                MessageBox("Error: Report View was not saved...");
            }
        }

        function loadSavedView(refresh) {
            var parentModule = "Reports";

            var curFilters = filterBoxLoc.filterBox.filters.find({ groups: { ParentModule: parentModule, Module: "Custom" } });
            if (curFilters) {
                curFilters.clear();
                filterBoxLoc.filterBox.toTable('', 'Module');
                $('#' + '<%=ReportTitleField.ClientID%>').val("");

                switch ('<%=this.ReportTypeID %>') {
                    case '2':
                        $('#' + '<%=CoverPageCheckbox.ClientID%>')[0].checked = "";
                        $('#' + '<%=IndexPageCheckbox.ClientID%>')[0].checked = "";
                        $('#' + '<%=EmailCheckbox.ClientID%>')[0].checked = "";
                        $('#' + '<%=SRsCheckbox.ClientID%>')[0].checked = "";
                        $('#' + '<%=AORsCheckbox.ClientID%>')[0].checked = "";
                        $('#' + '<%=SessionMetricsCheckbox.ClientID%>')[0].checked = "";
                        $('#' + '<%=BestCheckbox.ClientID%>')[0].checked = "";
                        $('#' + '<%=WorstCheckbox.ClientID%>')[0].checked = "";
                        $('#' + '<%=NormCheckbox.ClientID%>')[0].checked = "";
                        $('#' + '<%=CRDescrCheckbox.ClientID%>')[0].checked = "";
                        $('#' + '<%=AORDescrCheckbox.ClientID%>')[0].checked = "";
                        break;
                    case '4':
                        $('#' + '<%=CoverPageCheckbox.ClientID%>')[0].checked = "";
                        $('#' + '<%=SessionMetricsCheckbox.ClientID%>')[0].checked = "";
                        $('#' + '<%=SprintMetricsCheckbox.ClientID%>')[0].checked = "";
                        $('#' + '<%=DeploySummaryCheckbox.ClientID%>')[0].checked = "";
                        $('#' + '<%=CDRLMetricsCheckbox.ClientID%>')[0].checked = "";
                        $('#' + '<%=DeployMetricsCheckBox.ClientID%>')[0].checked = "";
                        $('#' + '<%=SummaryMetricsCheckBox.ClientID%>')[0].checked = "";
                        $('#' + '<%=DSEIndexCheckBox.ClientID%>')[0].checked = "";
                        $('#' + '<%=LegendsCheckBox.ClientID%>')[0].checked = "";
                        break;
                }
            }
            if ($('[id*="SavedViewsDDL"]').val() === "- Default Parameters -" && refresh !== true) {
                loadDefaultFilters();
                clearLevels();
            } else {
                var viewID = '<%=this.ViewID%>';
                if ($('[id*="SavedViewsDDL"] option:selected').attr('viewid')) {
                    viewID = $('[id*="SavedViewsDDL"] option:selected').attr('viewid');
                }

                PageMethods.loadCustomView(viewID, '<%=this.ReportTypeID%>', function (result) {
                    var params = JSON.parse(result);
                    var reportLevels = params["Report Levels"];
                    var reportParameters = params["Report Parameters"];
                    reportParameters = JSON.parse(reportParameters);
                    $('#' + '<%=ReportTitleField.ClientID%>').val(reportParameters["Title"]);

                    switch ('<%=this.ReportTypeID %>') {
                        case '2':
                            if (reportParameters["CoverPage"] == "True") $('#' + '<%=CoverPageCheckbox.ClientID%>')[0].checked = reportParameters["CoverPage"];
                            if (reportParameters["IndexPage"] == "True") $('#' + '<%=IndexPageCheckbox.ClientID%>')[0].checked = reportParameters["IndexPage"];
                            if (reportParameters["IncludeSRs"] == "True") $('#' + '<%=SRsCheckbox.ClientID%>')[0].checked = reportParameters["IncludeSRs"];
                            if (reportParameters["IncludeAORs"] == "True") $('#' + '<%=AORsCheckbox.ClientID%>')[0].checked = reportParameters["IncludeAORs"];
                            if (reportParameters["IncludeSessionMetrics"] == "True") $('#' + '<%=SessionMetricsCheckbox.ClientID%>')[0].checked = reportParameters["IncludeSessionMetrics"];
                            if (reportParameters["IncludeBestCase"] == "True") $('#' + '<%=BestCheckbox.ClientID%>')[0].checked = reportParameters["IncludeBestCase"];
                            if (reportParameters["IncludeWorstCase"] == "True") $('#' + '<%=WorstCheckbox.ClientID%>')[0].checked = reportParameters["IncludeWorstCase"];
                            if (reportParameters["IncludeNormCase"] == "True") $('#' + '<%=NormCheckbox.ClientID%>')[0].checked = reportParameters["IncludeNormCase"];
                            if (reportParameters["HideCRDescr"] == "True") $('#' + '<%=CRDescrCheckbox.ClientID%>')[0].checked = reportParameters["HideCRDescr"];
                            if (reportParameters["HideAORDescr"] == "True") $('#' + '<%=AORDescrCheckbox.ClientID%>')[0].checked = reportParameters["HideAORDescr"];
                            break;
                        case '4':
                            if (reportParameters["CoverPage"] == "True") $('#' + '<%=CoverPageCheckbox.ClientID%>')[0].checked = reportParameters["CoverPage"];
                            if (reportParameters["IncludeSessionMetrics"] == "True") $('#' + '<%=SessionMetricsCheckbox.ClientID%>')[0].checked = reportParameters["IncludeSessionMetrics"];
                            if (reportParameters["IncludeSprintMetrics"] == "True") $('#' + '<%=SprintMetricsCheckbox.ClientID%>')[0].checked = reportParameters["IncludeSprintMetrics"];
                            if (reportParameters["IncludeDeploymentSummary"] == "True") $('#' + '<%=DeploySummaryCheckbox.ClientID%>')[0].checked = reportParameters["IncludeDeploymentSummary"];
                            if (reportParameters["IncludeCDRLMetrics"] == "True") $('#' + '<%=CDRLMetricsCheckbox.ClientID%>')[0].checked = reportParameters["IncludeCDRLMetrics"];
                            if (reportParameters["IncludeDeploymentMetrics"] == "True") $('#' + '<%=DeployMetricsCheckBox.ClientID%>')[0].checked = reportParameters["IncludeDeploymentMetrics"];
                            if (reportParameters["IncludeSummaryMetrics"] == "True") $('#' + '<%=SummaryMetricsCheckBox.ClientID%>')[0].checked = reportParameters["IncludeSummaryMetrics"];
                            if (reportParameters["IncludeDSEIndex"] == "True") $('#' + '<%=DSEIndexCheckBox.ClientID%>')[0].checked = reportParameters["IncludeDSEIndex"];
                            if (reportParameters["IncludeLegends"] == "True") $('#' + '<%=LegendsCheckBox.ClientID%>')[0].checked = reportParameters["IncludeLegends"];
                        break;
                    }

                    for (var key in reportParameters) {
                        switch ('<%=this.ReportTypeID %>') {
                            case '2':
                            case '4':
                                if (key === "Release Version" || key === "AOR Workload Type" || key === "Visible To Customer" || key === "Contract" || key === "Workload Allocation" || key === "Deployment" || key === "System Suite") {
                                    var parameterType = reportParameters[key].split(',,');
                                    var parameterID = parameterType[0].split(',');
                                    var parameterName;
                                    if (parameterType[1].indexOf('Cyber, Severs, Tech Stack') >= 0) {
                                        var pm = parameterType[1].replace('Cyber, Severs, Tech Stack,', '');
                                        parameterName = pm.split(',');
                                        parameterName.unshift('Cyber, Severs, Tech Stack');
                                    } else {
                                        parameterName = parameterType[1].split(',');
                                    }
                                    for (var i = 0; i < parameterID.length; i++) {
                                        filterBoxLoc.filterBox.filters.add({ name: key, groups: { ParentModule: parentModule, Module: "Custom" } }).parameters.add(parameterID[i], parameterName[i]);
                                    }
                                }
                                break;
                            case '3': //todo: handle names with ","
                                if (key === "AOR" || key === "Product Version" || key === "Resource" || key === "Status" || key === "System(Task)" || key === "Work Area") {
                                    var parameterType = reportParameters[key].split(',,');
                                    var parameterID = parameterType[0].split(',');
                                    var parameterName = parameterType[1].split(',');
                                    for (var i = 0; i < parameterID.length; i++) {
                                        filterBoxLoc.filterBox.filters.add({ name: key, groups: { ParentModule: parentModule, Module: "Custom" } }).parameters.add(parameterID[i], parameterName[i]);
                                    }
                                }
                                break;
                        }
                    }
                    filterBoxLoc.filterBox.toTable({ groups: { ParentModule: parentModule } }, 'Module');
                    if (reportLevels) setLevels(reportLevels);
                });
            }
        }

        function setLevels(strSettings) {
            //First clear the current settings
            clearLevels();

            var jsonSettings = JSON.parse(strSettings);
            jsonRollupSettings = JSON.parse(jsonSettings.rollups);

            //The settings are already passed in from the database when entering this function so just build the grid now
            if (typeof jsonSettings == 'undefined' || jsonSettings.length == 0) return;
            var maxLevelID = 1;
            $.each(jsonRollupSettings, function (key, value) { //simulate drop event for each item in the collection
                var rollup = this;
                var trTarget;
                var liToDrop;

                if (parseInt(rollup.levelID) > maxLevelID) {
                    $('#btnAddLevel').trigger('click');
                    maxLevelID++;
                }

                if ($('.tblRollupLevel').eq(parseInt(rollup.levelID) - 1).find('tbody tr').length > 0) { //If there are trs in the table then insert the tr into the tbody
                    trTarget = $('.tblRollupLevel').eq(parseInt(rollup.levelID) - 1).find('tbody tr:last');
                }
                else { //If there are no trs in the table then insert the tr into the thead
                    trTarget = $('.tblRollupLevel').eq(parseInt(rollup.levelID) - 1).find('thead tr:last');
                }

                var liToDrop;
                //Get the existing li reference that corresponds to the rollup, because we need that to build our breakoutfield
                $('.fieldsList > li').each(function () {
                    if ($(this).text() == rollup.attr) {
                        liToDrop = this;
                    }
                });
                buildBreakoutField(liToDrop, trTarget);
                //Get an instance of the tr just added and set its order
                trAdded = $('.tblRollupLevel').eq(parseInt(rollup.levelID) - 1).find('tbody tr:last');
                $(trAdded).find('select').val(rollup.order);
                $(trAdded).find('select').trigger('change');
            });

        }

        function clearLevels() {

            $('.tblRollupLevel').each(function () {

                //Delete all inner Breakout Field rows
                var trArray = $(this).find('tr');
                $('tr', this).each(function () {

                    //Delete the Breakout rows first
                    var regex = /Level \d/i;
                    if ((m = regex.exec($(this)[0].innerText)) == null) {
                        $(this).find('img[src$="cross.png"]').trigger('click');
                    }
                });
                //Delete the outer row level last
                removeLevelNoPrompt($(this));
            });
        }

        function showHideFieldList(imgObj) {
            try {
                if ($(imgObj).prop("alt") == "Show") {
                    $(imgObj).prop("alt", "Hide").prop("title", "Hide Fields").prop("src", "images/icons/minus_blue.png");
                    $(imgObj).parent().find("ul:first").show();
                }
                else {
                    $(imgObj).prop("alt", "Show").prop("title", "Show Fields").prop("src", "images/icons/add_blue.png");
                    $(imgObj).parent().find("ul:first").hide();
                }
            }
            catch (e) {
                var x = 1;
            }
        }

        function remove(img) {
            if ($(img).closest('tr').find('th').length > 0) { //check if the delete img is a part of the table header, if so delete the table
                removeLevel($(img).closest('table'));
            }
            else { //it must be part of the table row, delete the row. 
                removeBreakout($(img).closest('tr'));
            }
        }

        function removeLevel(tbl) { //delete each row in tbody, then delete the table. 

            var regex = /Level (\d)/i;
            var currLevel = regex.exec(tbl[0].innerHTML)[1];
            if (parseInt(currLevel) == numLevels) { //Dont allow a user to remove Intermediary Levels
                $('tbody tr', tbl).each(function (idx) {
                    removeBreakout(this);
                });
                $(tbl).remove();
                numLevels--;
                reorderTables(); //fixes table order sequence and styling, so there aren't any orphaned levels
            }
            else {
                MessageBox('You can only delete the max level');
            }
        }

        function removeLevelNoPrompt(tbl) { //delete each row in tbody, then delete the table. 

            var regex = /Level (\d)/i;
            var currLevel = regex.exec(tbl[0].innerHTML)[1];
            if (parseInt(currLevel) != 1) { //Delete all except the top row
                $('tbody tr', tbl).each(function (idx) {
                    removeBreakout(this);
                });
                $(tbl).remove();
                numLevels--;
                reorderTables(); //fixes table order sequence and styling, so there aren't any orphaned levels
            }
        }

        function removeBreakout(tr) {
            var listTxt = $('span:first', tr).text(); //get the attribute to be deleted. 
            $('.fieldsList > li').each(function () { //find the attribute in the fields list and show it, so it can be reused. 
                if ($(this).text() == listTxt) {
                    $(this).show();
                    numRollups--;
                }
            });
            $(tr).remove();
        }

        function buildBreakoutField(listItem, tr) { //this adds a row to the rollup level when an item from "available fields" is dropped onto the table.
            var txt = $(listItem).text();
            var rowClone = $('#tblClone tbody tr:eq(0)').clone();
            $(rowClone).droppable({
                accept: '#divAvailFieldsContainer li, #<%=divCRFields.ClientID%> li, #<%=divSRFields.ClientID%> li, #<%=divAORFields.ClientID%> li, #<%=divTaskFields.ClientID%> li, tr',
                drop: function (event, ui) {
                    switch (ui.draggable[0].tagName) { //if you pull this code out into its own function, for some reason it breaks. 
                        case 'LI':
                            buildBreakoutField(ui.draggable, this);
                            if ($(ui.draggable).parent().attr('ID') == 'columnList') { //if the dropped list item is part of the columns list, we want to remove it, not just hide it. 
                                $(ui.draggable).remove();
                            }
                            break;
                        case 'TR':
                            addBreakoutRow(ui.draggable, this);
                            break;
                        default:
                            break;
                    }
                }
            });
            $('span', rowClone).first().text(txt); //breakout field
            if ($(tr).parent()[0].tagName == 'THEAD') { //check if the dropped on row is a table header. If so, insert into the top of tbody
                if ($(tr).closest('table').find('span:contains(' + txt + ')').length == 0) {
                    if (canInsertBreakPoint($(tr).closest('table').find('span')[0].innerHTML.replace(/[^0-9.]/g, ''), rowClone) > -1) {
                        $(tr).closest('table').find('tbody').first().prepend(rowClone);
                    }
                }

            }
            else if ($(tr).parent()[0].tagName == 'TFOOT') { //check for footer. if so insert in the bottom of tbody 
                if ($(tr).closest('table').find('span:contains(' + txt + ')').length == 0) {
                    if (canInsertBreakPoint($(tr).closest('table').find('span')[0].innerHTML.replace(/[^0-9.]/g, ''), rowClone) > -1) {
                        $(tr).closest('table').find('tbody').first().append(rowClone);
                    }
                }
            }
            else {
                if ($(tr).closest('table').find('span:contains(' + txt + ')').length == 0) {
                    if (canInsertBreakPoint($(tr).closest('table').find('span')[0].innerHTML.replace(/[^0-9.]/g, ''), rowClone) > -1) {
                        $(tr).after(rowClone); //if it gets here, then it was dropped on a row in the tbody. Insert before the dropped on row. 
                    }
                }
            }

            $('#divCRFields').show();
            $('#divSRFields').show();
        }

        function canInsertBreakPoint(level, rowClone) { //Find out if the string your attempting to insert is eligible to be inserted at the requested level
            var stringToInsert = rowClone[0].innerText.replace(/\(.*/, '').trim();
            stringToInsert = stringToInsert.replace('Ascending', '').trim();
            stringToInsert = stringToInsert.replace('Descending', '').trim();
            if (level == 1) {
                return level1Attr.indexOf(stringToInsert);
            }
            else if (level == 2) {
                return level2Attr.indexOf(stringToInsert);
            }
            else if (level == 3) {
                return level3Attr.indexOf(stringToInsert);
            }
            else if (level == 4) {
                return level4Attr.indexOf(stringToInsert);
            }
            return -1;
        }

        function addBreakoutRow(trDropped, trTarget) { //drag and drop a row from one level to another
            var rowClone = $(trDropped).clone().removeClass().removeAttr('style'); //clone the dropped row, and then strip off the styling jquery UI adds to draggables. 
            $(rowClone).droppable({
                accept: '#divAvailFieldsContainer li, #<%=divCRFields.ClientID%> li, #<%=divSRFields.ClientID%> li, #<%=divAORFields.ClientID%> li, #<%=divTaskFields.ClientID%> li, tr',
                drop: function (event, ui) {
                    switch (ui.draggable[0].tagName) {
                        case 'TR':
                            addBreakoutRow(ui.draggable, this);
                            break;
                        case 'LI':
                            buildBreakoutField(ui.draggable, this);
                            if ($(ui.draggable).parent().attr('ID') == 'columnList') {
                                $(ui.draggable).remove();
                            }
                            break;
                        default:
                            break;
                    }
                }
            });
            if ($(trTarget).parent()[0].tagName == 'THEAD') { //inser the row in a reasonable place based on where it was dropped. 
                if ($(trTarget).closest('table').find('span:contains(' + txt + ')').length == 0) {
                    if (canInsertBreakPoint($(trTarget).closest('table').find('tbody')[0].innerHTML.replace(/[^0-9.]/g, ''), rowClone) > -1) {
                        $(trTarget).closest('table').find('tbody').first().prepend(rowClone);
                    }
                }
            }
            else if ($(trTarget).parent()[0].tagName == 'TFOOT') {
                if ($(trTarget).closest('table').find('span:contains(' + txt + ')').length == 0) {
                    if (canInsertBreakPoint($(trTarget).closest('table').find('tbody')[0].innerHTML.replace(/[^0-9.]/g, ''), rowClone) > -1) {
                        $(trTarget).closest('table').find('tbody').first().append(rowClone);
                    }
                }
            }
            else {
                if ($(trTarget).closest('table').find('span:contains(' + txt + ')').length == 0) {
                    if (canInsertBreakPoint($(trTarget).closest('table').find('tbody')[0].innerHTML.replace(/[^0-9.]/g, ''), rowClone) > -1) {
                        $(trTarget).before(rowClone);
                    }
                }
            }
            $(trDropped).remove();
        }

        function reorderTables() {
            var marginLeftOffset = 15; //each table level is offset to the right by 15 pxs
            var widthOffset = 15; //Tables are fixed-width because it is easier to format. If you offset by 15 pixels, you have to reduce the width by 15 px
            var width = 475; //initial table width. 
            $('#divRollups > .tblRollupLevel').each(function (idx) { //go through and dynmically style the tables
                $(this).css('margin-left', marginLeftOffset * idx);
                $(this).css('width', width - widthOffset * idx);
                $('tr:first', this).find('span:first').text('Level ' + (idx + 1).toString()); //text for table headers, 1-n levels
            });
        }

        function btnAddLevel_click() {
            if (numLevels >= 4) return;
            var tbl = $('#tblClone').clone().removeAttr('id').addClass('tblRollupLevel'); //clone tbl
            $('tbody tr', tbl).remove(); //For convenience the clonable table has a row in it for cloning rows. Get rid of it so that initial table is empty. 
            $('tr', tbl).droppable({ //add drop events
                accept: '#divAvailFieldsContainer li, #<%=divCRFields.ClientID%> li, #<%=divSRFields.ClientID%> li, #<%=divAORFields.ClientID%> li, #<%=divTaskFields.ClientID%> li, tr',
                drop: function (event, ui) {
                    switch (ui.draggable[0].tagName) {
                        case 'TR':
                            addBreakoutRow(ui.draggable, this);
                            break;
                        case 'LI':
                            buildBreakoutField(ui.draggable, this);
                            if ($(ui.draggable).parent().attr('ID') == 'columnList') {
                                $(ui.draggable).remove();
                            }
                            break;
                        default:
                            break;
                    }
                }
            });
            $('#divRollups > .tblRollupLevel').last().after(tbl); //put new table on the bottom. 
            $('.tblRollupLevel tbody').sortable();
            reorderTables(); //adjust styling and header text so everything is consistent. 
            numLevels++;
        }

        function buildSaveView() {
            $('#<%=txtViewName.ClientID %>').val('');
            $('#ddlSaveView').html($('select[id*="SavedViewsDDL"]').html());
            $('#ddlSaveView option:first').remove();
            $('#ddlSaveView').prepend('<option>--Create New--</option>');
            $('#ddlSaveView option').filter(function () { return ($(this).text() === "--Create New--"); }).prop('selected', true).change();
        }

        function refreshPage(viewID) {
            var reportURL = '';

            switch ('<%=this.ReportTypeID %>') {
                case '2':
                    reportURL = _pageUrls.Reports.CR;
                    break;
                case '3':
                    reportURL = _pageUrls.Reports.Task;
                    break;
                case '4':
                    reportURL = _pageUrls.Reports.Release_DSE;
                    break;
            }

            if (viewID > 0) {
                window.location.href = 'Loading.aspx?Page=' + reportURL + '&viewid=' + viewID;
            } else {
                window.location.href = 'Loading.aspx?Page=' + reportURL;
            }
        }

        function loadHelpText(strType) {

            var nHTML = '';
            var pTitle = '';

            switch (strType) {
                case 'SavedViews':
                    pTitle = 'REPORT VIEWS';
                    nHTML += '<br><ul class="helpList"><li><b>Load Saved View</b></li>';
                    nHTML += '<ul><li>Since the report has many parameters and filters, “Load Saved View”';
                    nHTML += '<br>allows users to select a view that applies all selected parameters and ';
                    nHTML += '<br>filters that were saved for the view by a user.They exist as both Process – ';
                    nHTML += '<br>those saved for everyone in the system to use and Custom – and those ';
                    nHTML += '<br>saved by the logged-in user.</li></ul></ul><br>';
                    break;
                case 'ReportOptions': 
                    pTitle = 'REPORT OPTIONS';

                    switch ('<%=this.ReportTypeID %>') {
                        case '2':
                            nHTML += '<br><ul class="helpList"><li><b>Report Options</b></li>';
                            nHTML += '<ul><li><b>Title / Other Report Options</b></li>';
                            nHTML += '<ul><li><b>Report Title:</b> Ability to enter desired title of the CR Report.This ';
                            nHTML += '<br>will appear at the top of every page of the report.</li>';
                            nHTML += '<li><b>Add Cover Page:</b> Functionality to include or exclude the Cover ';
                            nHTML += '<br>Page of the CR Report.The Cover Page provides an overview of ';
                            nHTML += '<br>the Release, Contract, Mission, and Workload Allocations of the ';
                            nHTML += '<br>report.</li>';
                            nHTML += '<li><b>Add Index Page:</b> Functionality to include or exclude the Index ';
                            nHTML += '<br>Page of the CR Report.The Index Page provides the page numbers ';
                            nHTML += '<br>of all CRs, AORs, and SRs that were referenced in the ';
                            nHTML += '<br>report.</li></ul>';
                            nHTML += '<li><b>Report View Options</b></li>';
                            nHTML += '<ul><li><b>Include SRs:</b> Functionality to include or exclude SR content within ';
                            nHTML += '<br>the CR Report.';
                            nHTML += '<li><b>Include AORs:</b> Functionality to include or exclude AOR content ';
                            nHTML += '<br>within the CR Report.</li>';
                            nHTML += '<li><b>Include Session Metrics:</b> Functionality to include or exclude Session Metrics ';
                            nHTML += '<br>within the CR Report.</li></ul>';
                            nHTML += '<li><b>Leading Indicator Options</b></li>';
                            nHTML += '<ul><li><b>Include Best Case:</b> Functionality to include or exclude the best ';
                            nHTML += '<br>case of an AOR PD2TDR within the CR Report.';
                            nHTML += '<li><b>Include Worst Case:</b> Functionality to include or exclude the worst ';
                            nHTML += '<br>case of an AOR PD2TDR within the CR Report.';
                            nHTML += '<li><b>Include Norm:</b> Functionality to include or exclude the norm of an ';
                            nHTML += '<br>AOR PD2TDR within the CR Report.</li></ul>';
                            nHTML += '<li><b>CR and AOR Options</b></li>';
                            nHTML += '<ul><li><b>Include CR Description:</b> Functionality to include or exclude CR ';
                            nHTML += '<br>descriptions within the CR Report.';
                            nHTML += '<li><b>Include AOR Description:</b> Functionality to include or exclude AOR ';
                            nHTML += '<br>descriptions within the CR Report.</li></ul></ul><br>';
                            break;
                        case '3':
                            nHTML += '<br><ul class="helpList"><li><b>Report Options</b></li>';
                            nHTML += '<ul><li><b>Title / Other Report Options</b></li>';
                            nHTML += '<ul><li><b>Report Title:</b> Ability to enter desired title of the Task Report.This ';
                            nHTML += '<br>will appear at the top of every page of the report.</li></ul></ul><br>';
                            break;
                        case '4':
                            nHTML += '<br><ul class="helpList"><li><b>Report Options</b></li>';
                            nHTML += '<ul><li><b>Title / Other Report Options</b></li>';
                            nHTML += '<ul><li><b>Report Title:</b> Ability to enter desired title of the Release DSE Report.This ';
                            nHTML += '<br>will appear at the top of every page of the report.</li>';
                            nHTML += '<li><b>Add Cover Page:</b> Functionality to include or exclude the Cover ';
                            nHTML += '<br>Page of the Release DSE Report.The Cover Page provides an overview of ';
                            nHTML += '<br>the Release and Contract of the ';
                            nHTML += '<br>report.</li>';
                            nHTML += '<li><b>Include Release Session Metrics:</b> Functionality to include or exclude Session Metrics ';
                            nHTML += '<br>within the Release DSE Report.</li>';
                            nHTML += '<li><b>Include Deployment Session Metrics:</b> Functionality to include or exclude Sprint Metrics ';
                            nHTML += '<br>within the Release DSE Report.</li>';
                            nHTML += '<li><b>Include Deployment Summary:</b> Functionality to include or exclude Deployment Summary ';
                            nHTML += '<br>within the Release DSE Report.</li></ul></ul><br>';
                            break;
                    }
                    break;
            }
            MessageBox(nHTML, pTitle);
        }
    </script>

    <script id="jsInit" type="text/javascript">
        function initVariables() {
            popupContainer = document.getElementById('popupContainer');
            popupManager = new PopupWindowManager(popupContainer);

            switch ('<%=this.ReportTypeID %>') {
                case '2':
                    level1Attr = ["Release Version", "Contract", "Work Type"];
                    level2Attr = ["CR Title", "CR Primary Websys", "CR Work Priority", "CR Coord", "Customer Priority", "ITI Priority", "Last Updated", "Description", "Rationale", "Customer Impact", "SRs"];
                    level3Attr = ["Work Type"];
                    level4Attr = ["AOR Name", "PD2TDR status", "Primary Websys", "Workload Priority", "Description", "AOR Counts"];
                    break;
            }

            if (top.popupManager.IsPopupOpen('CRReport') || top.popupManager.IsPopupOpen('DSEReport')) {
                filterBoxLoc = window;
                $('#filterBox').show();
                filterBox = new filterContainer('divAppliedFilters');
            } else {
                filterBoxLoc = defaultParentPage;
            }
        }

        function initDisplay() {
            switch ('<%=this.ReportTypeID %>') {
                case '2':
                    $('.releasedse').hide();
                    $('#reportOptionsTable').show();
                    $('#' + '<%=ReportTitleField.ClientID%>').val(today());
                    $('#' + '<%=CoverPageCheckbox.ClientID%>')[0].checked = "True";
                    $('#' + '<%=IndexPageCheckbox.ClientID%>')[0].checked = "True";
                    $('#' + '<%=EmailCheckbox.ClientID%>')[0].checked = "True";
                    $('#' + '<%=SRsCheckbox.ClientID%>')[0].checked = "True";
                    $('#' + '<%=AORsCheckbox.ClientID%>')[0].checked = "True";
                    $('#' + '<%=SessionMetricsCheckbox.ClientID%>')[0].checked = "";
                    $('#' + '<%=BestCheckbox.ClientID%>')[0].checked = "True";
                    $('#' + '<%=WorstCheckbox.ClientID%>')[0].checked = "True";
                    $('#' + '<%=NormCheckbox.ClientID%>')[0].checked = "True";
                    $('#' + '<%=CRDescrCheckbox.ClientID%>')[0].checked = "True";
                    $('#' + '<%=AORDescrCheckbox.ClientID%>')[0].checked = "True";
                    $('[id$="menuRelatedItems"]').css('display', 'inline-block');
                    $('[id$="menuRelatedItems"]').css('vertical-align', 'middle');
                    $('[id$="menuRelatedItems"]').css('line-height', 'initial');
                    break;
                case '3':
                    $('.cr,.cr_releasedse,.releasedse').hide();
                    $('#reportOptionsTable').show();
                    $('input[value="Report Builder"]').hide();
                    $('[id$="menuRelatedItems"]').css('display', 'none');
                    break;
                case '4':
                    $('.cr').hide();
                    $('#reportOptionsTable').show();
                    $('input[value="Report Builder"]').hide();
                    $('#' + '<%=ReportTitleField.ClientID%>').val(today());
                    $('#' + '<%=CoverPageCheckbox.ClientID%>')[0].checked = "True";
                    $('#' + '<%=SessionMetricsCheckbox.ClientID%>')[0].checked = "";
                    $('#' + '<%=SprintMetricsCheckbox.ClientID%>')[0].checked = "";
                    $('#' + '<%=DeploySummaryCheckbox.ClientID%>')[0].checked = "True";
                    $('#' + '<%=CDRLMetricsCheckbox.ClientID%>')[0].checked = "True";
                    $('#' + '<%=DeployMetricsCheckBox.ClientID%>')[0].checked = "True";
                    $('#' + '<%=SummaryMetricsCheckBox.ClientID%>')[0].checked = "True";
                    $('#' + '<%=DSEIndexCheckBox.ClientID%>')[0].checked = "True";
                    $('#' + '<%=LegendsCheckBox.ClientID%>')[0].checked = "True";
                    $('[id$="menuRelatedItems"]').css('display', 'none');
                    break;
            }
        }

        function initEvents() {
            $('#imgShowFilters').click(function () { imgShowFilters_click(); });
            $('#imgClearFilters').click(function () { imgClearFilters_click(); });
            $('#ddlSaveView').on('change', function () { ddlSaveView_change(); });
            $('#buttonSaveView').click(function () { buttonSaveView_click(); return false; });
            $('#buttonCancelView').click(function () { $('#divViewName').slideUp(function () { $('#divDimmer').hide(); }); return false; });
            $('#buttonDeleteView').click(function () { deleteView(); return false; });
            $('select[id*="SavedViewsDDL"] option[OptionGroup="Process Views"]').wrapAll('<optgroup label="Process Views">');
            $('select[id*="SavedViewsDDL"] option[OptionGroup="Custom Views"]').wrapAll('<optgroup label="Custom Views">');
            $('select[id*="SavedViewsDDL"]').val('- Default Parameters -');
            $('select[id*="SavedViewsDDL"]').change(function () { loadSavedView(); return false; });
            $(".imgFieldsExpander").click(function () { showHideFieldList(this); });
            $('#btnAddLevel').click(function () { btnAddLevel_click(); return false; });
            $('#divGridLevels').on('click', '.tblRollupLevel img[src$="cross.png"]', function () { remove(this); return false; });
            $('#divAvailFieldsContainer li, #<%=divCRFields.ClientID%> li, #<%=divSRFields.ClientID%> li').draggable({
                revert: true,
                revertDuration: 0,
                stack: ".draggable",
                containment: "document",
                helper: "clone"
            });

            $('#divAvailFieldsContainer li, #<%=divCRFields.ClientID%> li, #<%=divSRFields.ClientID%> li, #<%=divAORFields.ClientID%> li, #<%=divTaskFields.ClientID%> li').droppable({
                accept: '#columnList > li, tr',
                drop: function (event, ui) {
                    switch (ui.draggable[0].tagName) {
                        case 'LI':
                            var txt = $(ui.draggable).text();
                            $('.fieldsList > li').each(function () {
                                if ($(this).text() == txt) {
                                    $(this).show();
                                }
                            });
                            // Move draggable into droppable
                            $(ui.draggable).clone().appendTo(droppable);
                            fixGrdCommas();
                            numGridColumns--;
                            break;
                        case 'TR':
                            var txt = $('span:first', ui.draggable).text();
                            $('.fieldsList > li').each(function () {
                                if ($(this).text() == txt) {
                                    $(this).show();
                                }
                            });
                            $(ui.draggable).remove();
                            break;
                        default:
                            break;
                    }
                }
            });
            $('.tblRollupLevel tr').droppable({
                accept: '#<%=divCRFields.ClientID%> li, #<%=divSRFields.ClientID%> li, #<%=divAORFields.ClientID%> li, #<%=divTaskFields.ClientID%> li, tr',
                drop: function (event, ui) {
                    switch (ui.draggable[0].tagName) {
                        case 'LI':
                            buildBreakoutField(ui.draggable, this);
                            break;
                        case 'TR':
                            addBreakoutRow(ui.draggable, this);
                            break;
                        default:
                            break;
                    }
                }
            });
            $('.tblRollupLevel tbody').sortable();
            buildSaveView();
            //clearFilterBox();
            if (('<%=this.ReportTypeID%>' == "2" || '<%=this.ReportTypeID%>' == "3" || '<%=this.ReportTypeID%>' == "4") && '<%=this.ViewID%>' != "") {
                loadSavedView(true);
                clearLevels();
                $('[id*="SavedViewsDDL"] option[viewid="' + '<%=this.ViewID%>' + '"]').prop('selected', true);
            }
        }

        $(document).ready(function () {
            initVariables();
            initDisplay();
            initEvents();
        });

    </script>
</asp:Content>

