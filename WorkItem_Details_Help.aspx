﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="WorkItem_Details_Help.aspx.cs" Inherits="WorkItem_Details_Help" %>

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
    <title>Help</title>
</head>
<body>
    <form id="form1" runat="server">
    <div style="width:1000px; height:600px; overflow: auto">
        <dl>
            <%--<dt>Work Request</dt>
            <dd>The source or ‘authority’ for which the work is being done.  This may reference the Change Request, the supporting process guidance (e.g. CMMI compliance), the strategic initiative, etc. <br /><br /></dd>--%>
            <%--<dt>Request Phase (PDDTDR)</dt>
            <dd>Describes the life cycle phase associated with the task.
                <ul>
                    <li>P – Planning</li>
                    <li>D – Design</li>
                    <li>D – Develop</li>
                    <li>T – Test</li>
                    <li>D – Deploy</li>
                    <li>R – Review</li>
                    <li>Note:  Not all activities are managed as a lifecycle (e.g. Work Stoppage)</li>
                </ul>
                <br />
            </dd>--%>
            <dt>Resource Group</dt>
            <dd>
                A description of the type of work that is being preformed
                <ul>
                    <li>Build / Test</li>
                    <li>CVT (Customer Verification Test support)</li>
                    <li>Meeting</li>
                    <li>Documentation</li>
                    <li>Support</li>
                    <li>IVT (Internal Verification Test support)</li>
	            </ul>
                <br />
            </dd>
            <dt>Work Activity </dt>
            <dd>
                An attribute for what type the work task can be categorized.
                <ul>
                    <li>Bug – Broken functionality, not working.</li>
                    <li>Enhancement – New requirement not yet in the system.</li>
                    <li>Task – Single item to be completed.</li>
                    <li>Data Update – Request for data to be updated on one of our servers.</li>
                    <li>Posting – Request for an item and/or document to be posted to CAFDEx.</li>
                    <li>IT – Internal IT tasks.</li>
                    <li>Support - If nothing needs to be fixed, but the customer/user may need some coaching/help to accomplish a work task.</li>
                    <li>Training – Training item</li>
                    <li>Testing / CVT – CVT item.</li>
                    <li>Business Development – For Business Development tasks.</li>
                    <li>IA – Cybersecurity/IA Item used by IA team.</li>
                </ul>
                <br />
            </dd>
            <dt>Priority</dt>
            <dd>
                Priority of item.
                <ul>
                    <li>High – Work stoppage or defined deadline.</li>
                    <li>Med – Not a work stoppage or deadline defined, but important.</li>
                    <li>Low – Important task, but can wait to be implemented due to higher priority tasks.</li>
                </ul>
                <br />
            </dd>
            <dt>Status</dt>
            <dd>
                Status of item.
                <ul>
	                <li>New – Task has been approved and is ready for developer to start work.</li>
	                <li>Info Requested – Request for information.</li>
	                <li>Info Provided – Information has been provided.</li>
	                <li>In Progress – Task is in progress.</li>
	                <li>Checked In – Developer has committed code in SVN to be deployed.</li>
	                <li>Deployed – Task has been posted to the CAFDEx servers.</li>
	                <li>Re-Opened – Task was found not to be corrected after testing.</li>
	                <li>Closed – Task is complete.</li>
                </ul>
                <br />
            </dd>
            <dt>System </dt>
            <dd>The Web Application / IT System that the item applies to. (CAFDEx, AMR, etc.) <br /><br /></dd>
            <%--<dt>Allocation Assign </dt>
            <dd>Field for Allocation Assignments Tracking on AoR Report.<br /><br /></dd>--%>
            <dt>Date Needed </dt>
            <dd>Completion date that is needed to satisfy the process.<br /><br /></dd>
            <dt>Submitted By </dt>
            <dd>The person who entered the WTS Task.<br /><br /></dd>
            <dt>Assigned To </dt>
            <dd>The person who the task is currently assigned to for action.</dd>
            <dt>Primary Tech Resource </dt>
            <dd>Lead Technical resource on a work task (not necessarily individual directly doing the work).<br /><br /></dd>
            <dt>Secondary Tech Resource </dt>
            <dd>Secondary Technical resource on the work item who will support the primary resource if needed.<br /><br /></dd>
            <dt>Primary Business Resource</dt>
            <dd>May be directing the work or SME</dd>
            <dt>Resource Priority Rank</dt>
            <dd>Priority rank of work task for the assigned resources; 1-10, 1 being high priority.<br /><br /></dd>
            <dt>Functionality</dt>
            <dd>Field used to group work tasks into common functional groups; Ex: Parameters, Filters, Grid, etc. Useful in assigning based on resource competency.<br /><br /></dd>
            <dt>Work Area</dt>
            <dd>Field used to group work items into common functional areas. EX: QM Obligations, DS Custom, QM RFM.<br /><br /></dd>
            <%--<dt>Planned/Actual Design Start</dt>
            <dd>Planned/actual start dates of design slides, if slides are needed.<br /><br /></dd>
            <dt>Planned/Actual DEV Start</dt>
            <dd>Planned/actual start dates of Development.<br /><br /></dd>
            <dt>Estimated Effort </dt>
            <dd>
                Estimated amount of hours needed to complete task.
                <ul>
                    <li>XS = Less than 100</li>
	                <li>Small = 100- 320 hrs</li>
                    <li>Medium = 321 – 800 hrs</li>
                    <li>Large = 801- 1600 hrs</li>
                    <li>X-Large = 1600-3200 hrs</li>
                    <li>2X Large = 3201 + hrs</li>
                </ul>
                <br />
            </dd>--%>
            <dt>Product Version</dt>
            <dd>The version of the release. This is used for configuration management & indicates version of source code needed to be updated<br /><br /></dd>
            <dt>SR Number </dt>
            <dd>Number that correlates to an SR<br /><br /></dd>
            <%--<dt>Reproduced </dt>
            <dd>Checkboxes to indicate whether Business and/or DEV team are able to reproduce the issue. <br /><br /></dd>--%>
            <dt>Deployment Section</dt>
            <dd>Checkboxes are used to indicate which environments the work task was posted to. Including Environment, Deployed By and Deployed Date.<br /><br /></dd>
            <%--<dt>Testing / Review</dt>
            <dd>Provide relevant information to be able to troubleshoot the issue including CVT step, CVT Status and Tester during IP-2 and IP-3 testing phases during a release.<br /><br /></dd>--%>
            <dt>Description</dt>
            <dd>Short description of item. (breadcrumb trail should be as concise as possible, you do not have to start with system, that is already noted in the “System” field)<br /><br /></dd>
            <dt>Comments </dt>
            <dd>Area to enter any additional information on work tasks.</dd>
        </dl>
    </div>
    </form>
</body>
</html>