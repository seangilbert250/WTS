<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AOR_Summary.aspx.cs" Inherits="AOR_Summary" MasterPageFile="~/EditTabs.master" Theme="Default" %>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" runat="Server">AOR Summary</asp:Content>
<asp:Content ID="cpHead" ContentPlaceHolderID="head" runat="Server">
    <!-- Copyright (c) 2017 Infinite Technologies, Inc. -->
    <link rel="stylesheet" href="Styles/multiple-select.css" />
</asp:Content>
<asp:Content ID="cpBody" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
    <div id="divPageContainer" style="overflow-x: hidden; overflow-y: auto;">
        <table style="width: 98%">
            <tr>
                <td style="vertical-align: top; padding-right: 15px;">
                    <div id="divDialog" style="height: 25px; overflow-x: hidden; overflow-y: auto; text-align: center; display: none;">Please select a Release and Contract for the Summary View to display.</div>
                    <span id="spnRelease">Release:&nbsp;<select id="ddlReleaseQF" runat="server" multiple="true" style="width: 150px;"></select></span>&nbsp;&nbsp;
                    <span id="spnContract">Contract:&nbsp;<select id="ddlContractQF" runat="server" multiple="true" style="width: 150px;"></select></span>&nbsp;&nbsp;
                    <div id="divAlert" runat="server" style="display: inline-block; vertical-align: top; float: right"></div>
                    <div id="divMetric" runat="server" style="padding: 10px 0px 10px 10px; overflow-x: hidden; overflow-y: auto;"></div>
                </td>
            </tr>
        </table>
    </div>

    <script src="Scripts/multiselect/jquery.multiple.select.js" type="text/javascript"></script>

    <script id="jsVariables" type="text/javascript">
        var _pageUrls;
        var _selectedReleasesQF = '';
        var _selectedDeliverablesQF = '';
        var _selectedContractsQF = '';
    </script>

    <script id="jsEvents" type="text/javascript">
        function ddlReleaseQF_update() {
            var arrReleaseQF = $('#<%=this.ddlReleaseQF.ClientID %>').multipleSelect('getSelects');
            var arrDeliverableQF = [];

            var allSelectedOptGroup = [];
            $('#<%=this.ddlReleaseQF.ClientID%>').each(function () {
                var optgroups = $(this).children('optgroup');
                optgroups.each(function () {
                    var options = $(this).children('option');
                    var length = options.length;

                    if (options.filter(':selected').length == length) {
                        var optionID = $(this).children('option:last').val();
                        optionID = optionID.slice(0, optionID.indexOf('.'));

                        arrReleaseQF.push(optionID);
                        $(this).children('option').each(function () {
                            arrReleaseQF.splice(arrReleaseQF.indexOf($(this).val()), 1);
                        });
                    } else {
                        options.filter(':selected').each(function () {
                            var optionID = $(this).val();
                            optionID = optionID.slice(optionID.indexOf('.') + 1);
                            if (optionID != 0) arrDeliverableQF.push(optionID);
                        });
                    }

                    options.filter(':selected').each(function () {
                        if (arrReleaseQF.indexOf($(this).val()) > -1) {
                            arrReleaseQF.splice(arrReleaseQF.indexOf($(this).val()), 1);
                        }
                    });
                });
            });
            _selectedDeliverablesQF = arrDeliverableQF.join(',');
            _selectedReleasesQF = arrReleaseQF.join(',');
        }

        function ddlReleaseQF_close() {
            refreshPage();
        }

        function ddlContractQF_update() {
            var arrContractQF = $('#<%=this.ddlContractQF.ClientID %>').multipleSelect('getSelects');

            _selectedContractsQF = arrContractQF.join(',');
        }

        function ddlContractQF_close() {
            refreshPage();
        }

        function suiteTabClick(suiteID, tableIndex) {
            $('[summary="' + tableIndex + '"]').children().children().each(function () {
                if ($(this).attr("suite") != suiteID && $(this).attr("suite") != undefined) {
                    $(this).hide();
                } else {
                    $(this).show();
                }
            });
        }

        function openContract() {
            var nWindow = 'Contract';
            var nTitle = 'Release Metrics by Deployments';
            var nHeight = 325, nWidth = 625;
            var nURL = window.location.href;
            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

        function openCR(CRID) {
            var nWindow = 'CR';
            var nTitle = 'CR';
            var nHeight = 700, nWidth = 1000;
            var nURL = _pageUrls.Maintenance.AORCRTabs + '?NewCR=false&CRID=' + CRID;
            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

        function displayAlertAORs(alertType) {
            var nWindow = 'AORAlert';
            var nTitle = 'AOR Alert';
            var nHeight = 500, nWidth = 650;
            var nURL = _pageUrls.AORSummaryPopup + '?Type=Alert&Alert=' + alertType;
            var openPopup = popupManager.AddPopupWindow(nWindow, nTitle, 'Loading.aspx?Page=' + nURL, nHeight, nWidth, 'PopupWindow', this);

            if (openPopup) openPopup.Open();
        }

        function refreshPage() {
            ddlReleaseQF_update();
            ddlContractQF_update();

            var nURL = window.location.href;

            nURL = editQueryStringValue(nURL, 'SelectedReleases', _selectedReleasesQF);
            nURL = editQueryStringValue(nURL, 'SelectedDeliverables', _selectedDeliverablesQF);
            nURL = editQueryStringValue(nURL, 'SelectedContracts', _selectedContractsQF);

            if (opener) {
                opener.window.location.href = 'Loading.aspx?Page=' + nURL;
                setTimeout(closeWindow, 1);
            } else {
                window.location.href = 'Loading.aspx?Page=' + nURL;
            }
        }

        function resizePage() {
            resizePageElement('divPageContainer');
        }
    </script>

    <script id="jsInit" type="text/javascript">
        function initVariables() {
            _pageUrls = new PageURLs();
        }

        function initControls() {

            var numReleaseGroups = '<%=this.QFReleaseGroups%>'.split(',');
            $.each(numReleaseGroups, function (index, value) {
                $('#<%=this.ddlReleaseQF.ClientID %> option[OptionGroup="' + value + '"]').wrapAll('<optgroup label="' + value + '">');
            });

            $('#<%=this.ddlReleaseQF.ClientID %>').multipleSelect({
                placeholder: 'Default'
                , width: 'undefined'
                , onOpen: function () { ddlReleaseQF_update(); }
                , onClose: function () { ddlReleaseQF_close(); }
            }).change(function () { ddlReleaseQF_update(); });

            $('#<%=this.ddlContractQF.ClientID %>').multipleSelect({
                placeholder: 'Default'
                , width: 'undefined'
                , onOpen: function () { ddlContractQF_update(); }
                , onClose: function () { ddlContractQF_close(); }
            }).change(function () { ddlContractQF_update(); });

            $('.summaryContainer').tabs({
                heightStyle: "content",
                collapsible: false,
                active: 0
            });
        }

        function initDisplay() {
            $('#divPage').hide();
            $('[role="tab"][aria-selected="true"]').each(function () {
                $(this).find('a').trigger('click');
            });
            if (opener) $('#divDialog').show();
            resizePage();
        }

        function initEvents() {
            $(window).resize(resizePage);
            ddlContractQF_update();
           // if (_selectedContractsQF.length === 0) openContract();
        }

        $(document).ready(function () {
            initVariables();
            initControls();
            initDisplay();
            initEvents();
        });
    </script>
</asp:Content>