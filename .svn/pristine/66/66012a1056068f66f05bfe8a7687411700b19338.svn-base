<%@ Page Title="" Language="C#" MasterPageFile="~/AddEdit.master" AutoEventWireup="true" CodeFile="ManageRoles.aspx.cs" Inherits="Admin_ManageRoles" Theme="Default" %>

<asp:Content ID="cpHead" ContentPlaceHolderID="head" Runat="Server"></asp:Content>
<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" Runat="Server">View/Add Roles</asp:Content>
<asp:Content ID="cpHeaderText" ContentPlaceHolderID="ContentPlaceHolderHeaderText" runat="Server">View/Add Roles</asp:Content>
<asp:Content ID="cpPageContentInfo" ContentPlaceHolderID="cphPageContentInfo" Runat="Server"></asp:Content>
<asp:Content ID="cpBody" ContentPlaceHolderID="ContentPlaceHolderBody" Runat="Server">
	<asp:ScriptManager ID="scriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>

	<div id="divPageContainer" class="pageContainer" style="overflow-y: auto;">
		<div class="pageContentInfo" style="width: 100%; height: 32px;"></div>
		<div id="divRolesContainer" class="attributes" style="width:100%; padding-top:5px;">
			<div id="divRole" class="attributesRow" style="padding-left:5px;">
				<div name="FirstLabel" class="attributesLabel">Role:</div>
				<div class="attributesValue">
					<asp:TextBox ID="txtRole" runat="server" Width="200"></asp:TextBox>
					<asp:Button ID="buttonAdd" runat="server" Text="Add"  OnClick="buttonAdd_Click" />
				</div>
			</div>
		</div>
		<div id="divExistingRolesContainer" class="attributes" style="width: 100%; padding-top: 10px;">
			<div id="divExistingRoles" class="attributesRow" style="padding-left:5px;">
				<div class="attributesLabel">Existing Roles:</div>
			</div>
			<div id="divQty" class="attributesRow" style="padding-left: 5px;">
				<div name="FirstLabel" class="attributesLabel">&nbsp;</div>
				<div class="attributesValue">
					<asp:ListBox ID="listBoxRoles" runat="server" Width="200" Height="300"></asp:ListBox>
				</div>
			</div>
		</div>
	</div>

    <script type="text/javascript">
        var _selectedId = 0;

        function resizePage() {
        	resizePageElement('divPageContainer', 5);
        }

        function buttonAdd_click() {
            var url = 'Role_AddEdit.aspx?New=true&random=' + new Date().getTime();

            var nPopup = popupManager.AddPopupWindow('Add Role', 'Add Role', url, 250, 450, 'PopupWindow', this);
            if (nPopup) {
                nPopup.Open();
            }

            return false;
        }

        function refreshGrid() {
            window.location.href = window.location.href;
        }

        function row_click(row) {
            _selectedId = $(row).attr('roleId');
            return false;
        } //end row_click

        function setAttributeLabelWidth(width) {
            $('[name="FirstLabel"]').each(function () {
                $(this).width(width);
            });
        }
        $(document).ready(function () {
            $('#pageContentInfo').hide();

            setAttributeLabelWidth(50);

            $(window).resize(resizePage);
            resizePage();

            if (defaultParentPage && defaultParentPage.fadePageIn) {
            	defaultParentPage.fadePageIn();
            }
        });

	</script>
</asp:Content>