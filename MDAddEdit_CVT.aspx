<%@ Page Title="" Language="C#" MasterPageFile="~/AddEdit.master" AutoEventWireup="true" CodeFile="MDAddEdit_CVT.aspx.cs" Inherits="MDAddEdit_CVT" Theme="Default" %>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" runat="Server">CVT Document</asp:Content>
<asp:Content ID="cpHead" ContentPlaceHolderID="head" runat="Server">
	<link href="Styles/jquery-ui.css" rel="Stylesheet" type="text/css" />
	<script type="text/javascript" src="Scripts/jquery-1.11.2.js"></script>
</asp:Content>
<asp:Content ID="cpHeaderText" ContentPlaceHolderID="ContentPlaceHolderHeaderText" runat="Server">CVT Document Details</asp:Content>
<asp:Content ID="cpHeaderButtons" ContentPlaceHolderID="cphPageContentInfo" runat="Server">
	<input type="button" id="buttonClose" value="Close" />
	<input type="button" id="buttonSave" runat="server" value="Save" />
</asp:Content>
<asp:Content ID="cpBody" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">

</asp:Content>