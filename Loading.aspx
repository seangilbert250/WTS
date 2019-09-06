﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Loading.aspx.cs" Inherits="Loading" Theme="Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Loading...</title>
    <script src="scripts/common.js?v=4"></script>    
    <script src="scripts/shell.js?v=4"></script>
    <style type="text/css">
        #main-content {
            font-family: Arial;
            font-size: 24px;
            text-align: center;
            padding-top: 60px;
        }

        #main-title div {
            font-family: Arial;
            font-size: 14px;
            height: 20px;
            margin-top: 5px;
            vertical-align: bottom;
        }

        #progress-bar {
            background-position: center;
            font-size: 1px;
            font-weight: bold;
            height: 19px;
        }
            #progress-bar span {
                background: transparent;
                height: 100%;
                width: 100%;
            }

        #timer {
            position: relative;
            font-size: 12px;
        }
            #timer b {
                margin-right: 10px;
            }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
    </div>
        <div id="main-content">
            <div id="main-title"><span style="font-size: 16pt;">WTS is gathering data... Please wait...</span></div>
            <div id="progress-bar">
                <img src="images/loaders/progress_bar_blue.gif" height="19" width="220" alt="Loading" /></div>
            <div id="timer"><span>0:00.0</span></div>
        </div>
    </form>
    <script type="text/javascript">
        <!--
    (function () {
        function $(i) { return document.getElementById(i) }
        function reposition() {
            showElapsedTime()
        }
        function showElapsedTime() {
            if (timer) {
                var x = Math.round((new Date() - start) / 100) / 10;
                timer.innerText = Math.floor(x / 60) + ":" + ((x % 60) >= 10 ? "" : 0) + (Math.round((x * 10) % 600) / 10) + (x == Math.round(x) ? ".0" : "")
            }
        }
        document.onreadystatechange = function () {
            //if (this.readyState == "interactive") {
            //    if (parent.popupManager.ActivePopup.Close)
            //        parent.popupManager.ActivePopup.Close();
            //    else
            //        documnet.window.close();
            //}
            if (this.readyState == "loading") started = true;
            else if (started) {
                clearInterval(n);
                showElapsedTime();
                $("progress-bar").style.display = "none";
                $("main-title").innerHTML = "Loading Completed<div>It is now safe to close this window.</div>";
                try { document.title = "Report Loaded" } catch (x) { }
            }
        };

        var strHref = window.location.href;
        strHref = strHref.substring(strHref.indexOf("Loading.aspx?"));
        strHref = strHref.substring(strHref.indexOf("=") + 1);
        var w = 34, p = $("progress-bar").getElementsByTagName("span")[0], n = setInterval(reposition, 40), u = strHref, start = new Date(), timer = $("timer"), started = false;
        if (timer) timer = timer.getElementsByTagName("span")[0];
        location.href = u;
    })()
    // -->
    </script>
</body>
</html>
