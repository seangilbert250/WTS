<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Task_Edit_Help.aspx.cs" Inherits="Task_Edit_Help" %>

<!DOCTYPE html>
<style>
    dt {
        font-weight: bold;
    }
    li{
        list-style-type: none;
    }
    ul{
        margin-top: 0px;
        margin-bottom: 0px;
    }
</style>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Subtask Attributes</title>
</head>
<body>
    <form id="form1" runat="server">
    <div style="width:1000px; height:600px; overflow: auto">
        <dl>
            <dt>Title </dt>
            <dd>Title of subtask.<br /><br /></dd>
            <dt>Assigned To</dt>
            <dd>The person who the subtask is currently assigned to for action. </dd>
            <dt>Primary Resource</dt>
            <dd>Lead Technical resource on a work task (not necessarily individual directly doing the work).<br /><br /></dd>
            <dt>Priority</dt>
            <dd>Priority of item. </dd>
            <dt>Status </dt>
            <dd>Status of item. <br /><br /></dd>
            <dt>Percent Complete </dt>
            <dd>Percent completion of a task.<br /><br /></dd>
            <dt>Bus Priority Rank </dt>
            <dd>Priority ranking, 1-N, set by Primary Bus Resource for the subtask.<br /><br /></dd>
            <dt>Tech Priority Rank </dt>
            <dd>Priority ranking, 1-N, set by Primary Tech Resource for the subtask.<br /><br /></dd>
            <dt>Planned Start Date</dt>
            <dd>Planned start date developer believes subtask will be started.<br /><br /></dd>
            <dt>Actual Start Date </dt>
            <dd>Actuals start date when the developer started the subtask.<br /><br /></dd>
            <dt>Estimated Effort </dt>
            <dd>
                 Estimated amount of hours need to complete subtask.
                <ul>
                    <li>XS = Less than 100</li>
                    <li>Small = 100- 320 hrs</li>
                    <li>Medium = 321 – 800 hrs</li>
                    <li>Large = 801- 1600 hrs</li>
                    <li>X-Large = 1600-3200</li>
                    <li>2X Large = 3201 +</li>
                </ul>
            </dd>
            <dt>Actual Effort</dt>
            <dd>Actuals amount of hours needed to complete subtask.<br /><br /></dd>
            <dt>Actual End Date</dt>
            <dd>Actual end date when the developer has finished the subtask. <br /><br /></dd>
        </dl>
    </div>
    </form>
</body>
</html>
