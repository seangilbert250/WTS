// Executes and outputs a report to the response stream. You must supply an IFRAME to set the source to.
// frameID: if not null, the frame src will be set to the supplied href
// href: used to supply the frame src
// reportCompleteCB: callback function for when the report is finished rendering
function DownloadReport(frameID, href, reportCompleteCB)
{
    if (frameID != null && frameID.length > 0) {
        ShowDimmer(true, 'Downloading report...', 1);

        if (frameID.indexOf('#') != 0) {
            frameID = '#' + frameID;
        }

        var reportKey = generateUniqueID();

        if (href.indexOf('?') != -1) {
            href += '&reportkey=' + reportKey;
        }
        else {
            href += '?reportkey=' + reportKey;
        }

        setTimeout(function () { IsReportComplete(reportKey, true, reportCompleteCB); }, 1000);

        $(frameID).attr('src', href);
    }
    else {
        MessageBox('Invalid report call. FrameID is missing or invalid.');
    }
}

/// Runs a report synchronously, and returns when the report is complete.
function RunReport()
{
}

/// Queues a report. The report can then run immediately or scheduled for later. The report can be viewed at a later time at the Reports.aspx page.
function QueueReport(REPORT_TYPEID, reportParameters, scheduledDate, openReportManagerPopup, redirectPageToReportManager)
{
    if (openReportManagerPopup == null) openReportManagerPopup = true;
    if (redirectPageToReportManager == null) redirectPageToReportManager = false;

    if (reportParameters != null) {
        ShowDimmer(true, 'Queuing report...', 1);

        if (scheduledDate == null) scheduledDate = new Date();

        PageMethods.QueueReport(REPORT_TYPEID, reportParameters, scheduledDate,
            function (result) { 
                
                ShowDimmer(false);

                if (result.success) {
                    if (scheduledDate <= new Date()) {
                        if (openReportManagerPopup) {
                            successMessage('<b>Report queued. Opening Report Manager.</b>');

                            var nTitle = 'Manage Reports';
                            var nHeight = 650, nWidth = 1100;
                            var nURL = 'Reports_Grid.aspx?id=' + result.id + '&guid=' + result.guid;
                            var left = (screen.width / 2) - (nWidth / 2);
                            var top = (screen.height / 2) - (nHeight / 2);

                            setTimeout(function () {
                                window.open(nURL, 'ManageReportsWindow', 'status=0,menubar=0,resizable=0,scrollbars=0,height=' + nHeight + ',width=' + nWidth + ',left=' + left + ',top=' + top);             
                            }, 3000);
                        }
                        else if (redirectPageToReportManager) {
                            successMessage('<b>Report queued. Transferring to Report Manager.</b>');
                            setTimeout(function () { document.location = 'Reports_Grid.aspx?id=' + result.id + '&guid=' + result.guid; }, 3000);
                        }
                        else {
                            successMessage('<b>Report queued. You can view the report on the Report Manager page.</b>');
                        }
                    }
                    else {
                        successMessage('<b>Report queued. Results will be available on the reports page.</b>');
                    }
                }
                else {
                    warningMessage('<b>Unable to queue report. Check the logs.</b>');
                }
            },

            function (result) {
                MessageBox('Error queuing report. ' + result.error);
            });
    }
    else {
        MessageBox('Invalid report call. Params missing or invalid.');
    }
}


///////////////////////////////////////////////////////////////
// UTILITY FUNCTIONS
///////////////////////////////////////////////////////////////

function IsReportComplete(reportKey, rescheduleIfFalse, reportCompleteCB) {
    PageMethods.IsReportComplete(reportKey,
        function (result) {
            if (result == 'true') {
                ReportComplete(reportKey, reportCompleteCB);
            }
            else if (rescheduleIfFalse) {
                setTimeout(function () { IsReportComplete(reportKey, true, reportCompleteCB); }, 1000);
            }
        },

        function (result) {
            MessageBox('Error checking on report status. ' + result);
            return false;
        });
}

function ReportComplete(reportKey, reportCompleteCB) {
    ShowDimmer(false);

    successMessage('<b>Report complete.</b>');

    if (reportCompleteCB) reportCompleteCB();
}