﻿//Javascript - shell.js
//Base functions for UI

//Page Variables
var currentX = 0;
var currentY = 0;
var mousePos = 0;
var mouseIsDown = '';
var pageOpener = '';
var activeBubbleMessages = [];
var defaultParentPage = this;
var pathNameLength = defaultParentPage.location.pathname.split("/").length;
if (pathNameLength > 1) {
    pathNameLength = 2;
}
var pageName = defaultParentPage.location.pathname.split("/")[pathNameLength - 1].toUpperCase();

//End Page Variables

//Default Parent Page
function setDefaultPage() {
    defaultParentPage = top;
}


setDefaultPage();
var popupManager = defaultParentPage.popupManager;
//End Default Parent Page

//Popup Windows
function PopupWindowManager(popupContainer) {
    this.PopupWindows = [];
    this.PopupCount = 0;
    this.ActivePopup = null;
    this.PopupDimmer = new PopupDimmer;
    this.PopupContainer = popupContainer;
    this.GetPopupByName = function (Name) {
        for (var i = 0; i <= this.PopupCount - 1; i++) {
            if (this.PopupWindows[i].Name.toUpperCase() == Name.toUpperCase()) {
                return this.PopupWindows[i];
            }
        }
        return null;
    }
    this.GetPopupByID = function (ID) {
        for (var i = 0; i <= this.PopupCount - 1; i++) {
            if (this.PopupWindows[i].id.toUpperCase() == ID.toUpperCase()) {
                return this.PopupWindows[i];
            }
        }
        return null;
    }
    this.GetPopupIndexByName = function (Name) {
        for (var i = 0; i <= this.PopupCount - 1; i++) {
            if (this.PopupWindows[i].Name.toUpperCase() == Name.toUpperCase()) {
                return i;
            }
        }
        return null;
    }
    this.GetPopupIndexByID = function (ID) {
        for (var i = 0; i <= this.PopupCount - 1; i++) {
            if (this.PopupWindows[i].id.toUpperCase() == ID.toUpperCase()) {
                return i;
            }
        }
        return null;
    }
    this.IsPopupOpen = function (Name) {
        var thePopup = this.GetPopupByName(Name);

        return thePopup != null && thePopup.isOpen;
    }
    this.AddPopupWindow = function (Name, Title, Src, Height, Width, CssClass, openerPage, messageBox, inlineContainerID, noDimmer) {
        if (!this.GetPopupByName(Name)) {
            var nPopupWindow = new PopupWindow(Name, Title, Src, this.PopupContainer, this, Height, Width, CssClass, openerPage, messageBox, inlineContainerID, noDimmer);
            this.PopupWindows[this.PopupCount] = nPopupWindow;
            this.PopupCount++;
            return nPopupWindow;
        }
    };
    this.RemovePopupWindow = function (Name) {
        var currentPopups = this.PopupWindows;
        var newPopups = [];

        showActiveBubbleMessagesFromPopup();

        for (var i = 0; i <= currentPopups.length - 1; i++) {

            if (currentPopups[i].Name.toUpperCase() == Name.toUpperCase()) {
                if (Name.toUpperCase() == 'PDFVIEWER') {
                    currentPopups[i].pdfjsLib.PDFDOC = null; // free pdf doc from memory
                }

                // if we had an inline container, put it back where it came from on the original page
                if (currentPopups[i].InlineContainer != null && Name.toUpperCase() != 'PDFVIEWER') { // we don't save the PDF VIEWER
                    $(currentPopups[i].InlinePlaceHolder).parent().append(currentPopups[i].InlineContainer);
                    $(currentPopups[i].InlineContainer).css('position', 'absolute');
                    $(currentPopups[i].InlineContainer).css('display', 'none');
                    $(currentPopups[i].InlinePlaceHolder).remove();
                    currentPopups[i].InlineContainer = null;
                }

                //check if onclose exist
                try {
                    if (currentPopups[i].onClose) {
                        if (currentPopups[i].onClose() == false) {
                            return;
                        }
                    }
                }
                catch (e) {
                }

                if (currentPopups[i].Window) {

//                    if (navigator.appName == 'Microsoft Internet Explorer') {
//                        currentPopups[i].Frame.document.execCommand('Stop');
//                    }
//                    else {
//                        currentPopups[i].Frame.stop();
//                    }
                    currentPopups[i].Frame.src = "javascript:'';";
                    // delete currentPopups[i].Frame;
                    if (this.PopupCount > 0) {
                        currentPopups[i].ParentContainer.removeChild(currentPopups[i].Window);
                    }
                   // delete currentPopups[i];
                }
            }
            else {
                newPopups[newPopups.length] = currentPopups[i];
            }
        }

        this.PopupWindows = newPopups;
        this.PopupCount = this.PopupWindows.length;
        if (this.PopupCount == 0) {
            this.PopupDimmer.style.display = 'none';
            this.ActivePopup = null;
        }
        else {
            for (var i = this.PopupCount - 1; i >= 0; i--) {
                if (this.PopupWindows[i].isOpen) {
                    this.ActivePopup = this.PopupWindows[i];
                    break;
                }
            }
        }

        for (var i = 0; i <= this.PopupWindows.length - 1; i++) {
            if (this.ActivePopup != this.PopupWindows[i]) {
                this.PopupWindows[i].showDimmer();
            }
            else {
                this.PopupWindows[i].hideDimmer();
            }
        }
    };
    if (this.PopupContainer) {
        this.PopupContainer.appendChild(this.PopupDimmer);
    }
}

function PopupDimmer() {
    var dimmer = document.createElement('div');
    dimmer.style.position = 'absolute';
    dimmer.style.top = '0px';
    dimmer.style.left = '0px';
    dimmer.style.height = '100%';
    dimmer.style.width = '100%';
    dimmer.style.background = 'grey';
    dimmer.style.opacity = 0.6;
    dimmer.style.filter = 'alpha(opacity = 60)';
    dimmer.style.display = 'none';

    return dimmer;
}

function PopupWindowEscape(answer,popupName) {
    if (answer == 'Yes') {
        popupManager.RemovePopupWindow(popupName);
    }
}

function PopupWindow(Name, Title, Src, ParentContainer, PopupWindowManager, Height, Width, CssClass, openerPage, messageBox, inlineContainerID, noDimmer) {
    var nID = Name;
    while (nID.indexOf(' ') > -1) {
        nID = nID.replace(' ','_');
    }
    if (!openerPage) {
        openerPage = window.self;
    }
    this.id = 'PopupWindow_' + nID;
    this.Name = Name;
    this.Title = Title.toUpperCase();
    this.Src = Src;
    this.Height = parseInt(Height);
    this.Width = parseInt(Width);
    this.Opener = openerPage;
    this.CssClass = CssClass;
    this.Window = '';
    this.Dimmer = '';
    this.MessageBox = messageBox;
    this.ParentContainer = ParentContainer;
    this.PopupWindowManager = PopupWindowManager;
    this.InlinePlaceHolder = null;
    this.InlineContainer = null;
    this.NoDimmer = noDimmer;
    this.isOpen = false;
    this.Open = function () {
        var container = document.createElement('div');
        var header = document.createElement('div');
        var body = document.createElement('div');
        var frame = document.createElement('iframe');
        var messageDiv = document.createElement('div');
        var footer = document.createElement('div');
        var dimmer = document.createElement('div');
        var movingDiv = document.createElement('div');

        var wManager = new WindowManager();

        closeActiveBubbleMessagesFromPopup();

        //Create Header
        var headerTable = document.createElement('table');
        var headerTableRow = headerTable.insertRow(0);
        var headerTableCell = headerTableRow.insertCell(0);
        if (Src == null || (Src.indexOf('QuestionBox') == -1 && Src.indexOf('AOR_Wizard.aspx') == -1 && Src.indexOf('AOR_Meeting_Instance_Edit.aspx') == -1 && Src.indexOf('Type=Note Detail') == -1 && Src.indexOf('Type=Edit Note Detail') == -1 && Src.indexOf('Task_Edit.aspx') == -1)) {
            var headerTableCellImage = headerTableRow.insertCell(1);
            var headerPopupCloser = document.createElement('img');
            headerPopupCloser.src = 'Images/Icons/close_button_red.png';
            headerPopupCloser.alt = 'Close';
            headerPopupCloser.style.cursor = 'pointer';
            headerPopupCloser.style.height = '16px';
            headerPopupCloser.style.width = '16px';
            if (navigator.appName == "Netscape") {
                headerPopupCloser.onmousedown = function () { //have to use mousedown for chrome
                    if (popupManager.ActivePopup) {
                        PopupWindowEscape('Yes', popupManager.ActivePopup.Name);
                    }
                }
            }
            else {
                headerPopupCloser.onmouseup = function () {
                    if (popupManager.ActivePopup) {
                        PopupWindowEscape('Yes', popupManager.ActivePopup.Name);
                    }
                }
            }
            headerTableCellImage.style.width = '22px';
            headerTableCellImage.appendChild(headerPopupCloser);
            headerTableCell.style.paddingLeft = '22px';
            headerTableCellImage.style.cursor = 'default';
        }
        headerTableCell.innerText = this.Title;
        headerTable.style.width = '100%';
        header.style.textAlign = 'center';
        header.style.verticalAlign = 'middle';
        header.style.width = '100%';
        header.tag = 'PopupWindow_Header';
        header.appendChild(headerTable);
        header.className = this.CssClass + '_Header';
        header.style.cursor = 'move';
        header.onmousedown = function () {
            enableMovement(this.parentNode);
        }
        header.onmouseup = function () {
            disableMovement(this.parentNode);
        }
        header.onkeyup = function () {
            if (window.event) {
                var key = window.event.keyCode;
                if (window.event.keyCode == 27) {
                    // Escape
                    QuestionBox('Close Popup', 'Are you sure you want to close the current popup window?', 'No,Yes', 'PopupWindowEscape', 200, 200, window.self, popupManager.ActivePopup.Name);
                }
            }
            header.onselectstart = function(){ return false;};
        }

        //Create Body
        if (inlineContainerID) {
            var poundIdx = inlineContainerID.indexOf('#');
            if (poundIdx == -1 || poundIdx > 0) {
                inlineContainerID = '#' + inlineContainerID;
            }

            var ic = $(inlineContainerID);
            if (ic.length == 0) {
                ic = $(inlineContainerID, this.Opener.document);
            }

            if (ic.length == 0) {
                return;
            }

            this.InlineContainer = ic;

            if (this.InlinePlaceHolder == null) {
                this.InlinePlaceHolder = document.createElement('div');
                $(this.InlinePlaceHolder).attr('id', 'InlinePlaceHolder');
                $(this.InlinePlaceHolder).css('position', 'absolute');
                $(this.InlinePlaceHolder).css('display', 'none');
            }

            $(ic).parent().append(this.InlinePlaceHolder);

            //ic = $(ic).clone();
            //$(ic).attr('id', inlineContainerID + '_clone');
            $(ic).css('position', 'absolute');
            $(ic).css('left', '0px');
            $(ic).css('right', '0px');
            $(ic).width('100%');
            $(ic).height('100%');
            $(ic).show();

            $(ic).detach().appendTo(body);

        }
        else if (!this.MessageBox) {
            var strframe = '<iframe id="' + this.Name + '_Frame' + '" name="' + this.Name + '_Frame' + '" frameborder="0" scrolling="no" tag="PopupWindow_Frame" src="' + this.Src + '"  style="width: 100%; height: ' + this.Height + ' +px"  title=""></iframe>';
            body.innerHTML += strframe;
            frame = $(body).find("#" + this.Name + '_Frame')[0];
            frame.popupManager = this.PopupWindowManager;
            frame.pageWindowName = this.Name;
            if (!frame.opener) {
                if (openerPage) {
                    frame.opener = openerPage;
                }
                else {
                    frame.opener = this.Opener;
                }
            }
            frame.className = this.CssClass + '_Frame';
            //body.appendChild(frame);
        }
        else {
            messageDiv.innerHTML = this.Src;
            messageDiv.style.height = '100%';
            messageDiv.style.width = '100%';
            messageDiv.style.padding = '5px';
            messageDiv.style.paddingBottom = '35px';

            body.appendChild(messageDiv);

            var footer = document.createElement('div');
            var footerTable = document.createElement('table');
            var footerTableRow = footerTable.insertRow(0);
            var footerTableCell = footerTableRow.insertCell(0);

            footer.className = 'PopupFooter';
            footerTable.style.width = '100%';
            footerTable.style.height = '100%';
            footerTable.cellPadding = 0;
            footerTable.cellSpacing = 0;
            footerTableCell.style.textAlign = 'right';

            var footerPopupCloser = document.createElement('button');
            footerPopupCloser.innerText = 'Close';
            footerPopupCloser.style.cursor = 'hand';
            footerPopupCloser.onmouseup = function () { //have to use mousedown for chrome
                if (popupManager.ActivePopup) {
                    PopupWindowEscape('Yes', popupManager.ActivePopup.Name);
                }
            }
            footerTableCell.appendChild(footerPopupCloser);
            footer.appendChild(footerTable);
            body.appendChild(footer);
        }

        body.style.width = '100%';
        body.style.background = 'white';
        body.style.overflow = 'hidden';
        body.tag = 'PopupWindow_Body';
        body.className = CssClass + '_Body';


        movingDiv.style.width = '100%';
        movingDiv.style.height = '100%';
        movingDiv.style.left = '0px';
        movingDiv.style.top = '0px';
        movingDiv.style.position = 'absolute';
        movingDiv.style.display = 'none';
        movingDiv.forMoving = true;
        movingDiv.style.background = 'white';
        movingDiv.style.opacity = '0.01';
        movingDiv.style.filter = 'alpha(opacity=1)';
        movingDiv.style.cursor = 'move';
        body.appendChild(movingDiv);

        //Create Container
        container.style.position = 'absolute';
        container.tag = 'PopupWindow_Container';
        container.id = this.id;
        container.className = CssClass + '_Container';
        if (!this.MessageBox) container.style.width = this.Width + 'px';
        container.appendChild(header);
        container.appendChild(body);

        //Create Dimmer
        dimmer.id = 'Popup_Dimmer';
        dimmer.style.position = 'absolute';
        dimmer.style.top = '0px';
        dimmer.style.left = '0px';
        dimmer.style.background = 'grey';
        dimmer.style.opacity = 0.60;
        dimmer.style.filter = 'alpha(opacity = 60)';
        dimmer.style.display = 'none';
        dimmer.style.width = '100%';
        dimmer.style.height = '100%';
        container.appendChild(dimmer);

        if (this.ParentContainer && !this.isOpen) {
            this.ParentContainer.appendChild(container);
            dimmer.style.height = body.offsetHeight + 'px';
            dimmer.style.width = body.offsetWidth + 'px';

            container.style.left = (wManager.getWidth() / 2) - (this.Width / 2) + 'px';
            container.style.top = (wManager.getHeight() / 2) - ((this.Height + header.offsetHeight) / 2) + 'px';

            if (parseInt(container.style.left) < 0) {
                container.style.left = '0px';
            }
            if (parseInt(container.style.top) < 0) {
                container.style.top = '0px';
            }

            this.Window = container;
            this.Dimmer = dimmer;
            this.Header = header;
            this.Body = body;
            this.Frame = frame;
            this.isOpen = true;
            this.PopupWindowManager.ActivePopup = this;
            if (this.NoDimmer == null || !this.NoDimmer) {
                this.PopupWindowManager.PopupDimmer.style.display = 'block';
            }
            popupManager.ActivePopup.SetHeight(this.Height); //ie 8 needs this

            //this is for showing dimmer
            if (popupManager) {
                var popupWindows = popupManager.PopupWindows;
                for (var i = 0; i <= popupWindows.length - 1; i++) {
                    if (popupManager.ActivePopup != popupWindows[i]) {
                        popupWindows[i].showDimmer();
                    }
                    else {
                        popupWindows[i].hideDimmer();
                    }
                }
            }


            //Check if the popup window height is bigger then the containing window height.. If it is shrink the popup to the containing window height minus 40px...
            if (this.MessageBox) {
                var nWidth = this.Body.getElementsByTagName('div')[0].offsetWidth + 10;
                var nHeight = this.Body.getElementsByTagName('div')[0].scrollHeight + 30;

                if (nWidth < 300) {
                    nWidth = 300;
                }

                popupManager.GetPopupByName(this.Name).SetWidth(nWidth);
                popupManager.GetPopupByName(this.Name).SetHeight(nHeight);
            }

            var contHeight = wManager.getHeight();
            if (this.Height >= contHeight) {
                this.Height = contHeight - 40;
                popupManager.ActivePopup.SetHeight(this.Height);
            }
        }
    }
    this.onClose = '';
    this.Close = function () {
//        if (this.Frame.contentWindow) {
//            if (this.Frame.contentWindow.document) {
//                if (this.Frame.contentWindow.closeWindow) {
//                    this.Frame.contentWindow.closeWindow();
//                }
//            }
//        }

        this.PopupWindowManager.RemovePopupWindow(this.Name);
    }
    this.SetHeight = function (nHeight) {
        var wManager = new WindowManager();
        var contHeight = wManager.getHeight();
        if (nHeight >= contHeight) {
            nHeight = contHeight - 40;
        }

        this.Height = nHeight;
        this.Dimmer.style.height = nHeight + 'px';
        this.Body.style.height = nHeight + 'px';
        this.Frame.style.height = nHeight + 'px';
        this.ReCenter();
    }
    this.SetWidth = function (nWidth) {
        this.Width = nWidth;
        this.Window.style.width = nWidth + 'px';
        this.Dimmer.style.width = nWidth + 'px';
        this.ReCenter();
    }
    this.ReCenter = function () {
        var wManager = new WindowManager();
        this.Window.style.left = (wManager.getWidth() / 2) - (this.Width / 2) + 'px';
        var xTop = (wManager.getHeight() / 2) - ((this.Height + this.Header.offsetHeight) / 2)
        if (xTop < 0) { xTop = 0; }
        this.Window.style.top = xTop + 'px';
    }
    this.SetTitle = function (nTitle) {
        this.Title = nTitle;
        this.Header.getElementsByTagName('td')[0].innerText = nTitle;
    }
    this.showDimmer = function () {
        if (this.Dimmer) {
            //this.Dimmer.style.left = '0px';
            this.Dimmer.style.height = '100%';
            this.Dimmer.style.display = 'block';
        }
    }
    this.hideDimmer = function () {
        if (this.Dimmer) {
            this.Dimmer.style.display = 'none';
            var wMgr = new WindowManager();
        }
    }
    return this;
}
//End Popup Windows

//Message,Loading,Question Boxes
//These utilize the Popup Window Object
function MessageBox(Message, Title, Opener) {
    if (!Title) Title = 'Message';
    if (!Opener) Opener = this;
    var nMessage = popupManager.AddPopupWindow('Message', Title, Message, 100, 300, 'PopupWindow', Opener, true);

    if (nMessage) {
        nMessage.Open();
    }
}

function LoadingBox(strLoading) {
    var nLoading = popupManager.AddPopupWindow('Loading', 'Loading', '~/LoadingBox.aspx?strLoading=' + escape(strLoading) + '&random=' + Math.random(), 100, 300, 'PopupWindow', this);
    if (nLoading) {
        nLoading.Open();
        nLoading.Header.style.display = 'none';
        return nLoading;
    }
}

function QuestionBox(pTitle, pQuestion, pButtons, pFunction, pWidth, pHeight, openerPage, param) {
    try {
    	var src = 'QuestionBox.aspx?question=' + pQuestion + '&buttons=' + pButtons + '&function=' + pFunction + '&param=' + param;
        var nQuestion = popupManager.AddPopupWindow('Question', pTitle, src, pHeight > 200 ? pHeight : 200, pWidth > 300 ? pWidth : 300, 'PopupWindow', openerPage);
        if (nQuestion) {
            nQuestion.Open();
        }
    }
    catch (e) {
    }
}

function InputBox(pTitle, pQuestion, pButtons, pFunction, pWidth, pHeight, openerPage, param, maxLength, inputRequired, defaultValue) {
    if (pButtons == null || pButtons == '') pButtons = 'OK,Cancel';
    if (maxLength == null || maxLength == '') maxLength = 100;
    if (inputRequired == null) inputRequired = true;
    if (defaultValue == null) defaultValue = '';

    try {
        var src = 'QuestionBox.aspx?question=' + pQuestion + '&mode=textinput&buttons=' + pButtons + '&function=' + pFunction + '&param=' + param + '&maxlength=' + maxLength + '&req=' + inputRequired + '&defval=' + escape(defaultValue);
        var nQuestion = popupManager.AddPopupWindow('Question', pTitle, src, pHeight > 200 ? pHeight : 200, pWidth > 300 ? pWidth : 300, 'PopupWindow', openerPage);
        if (nQuestion) {
            nQuestion.Open();
        }
    }
    catch (e) {
    }
}

//End Message,Loading,Question Boxes

//Error Handling
function DisplayErrorMessage(Function,Number,Description) {
    var sPath = window.location.pathname;
    var sPage = sPath.substring(sPath.lastIndexOf('/') + 1);

    var nPopup = popupManager.AddPopupWindow('Error', 'Error Message', '~/ErrorPage.aspx?errorNumber=' + Number + '&errorDescription=' + Description + '&errorPage=' + sPage + '&errorFunction=' + Function, 100, 100, 'PopupWindow', this);
    if (nPopup) {
        nPopup.Open();
    }
}
//End Error Handling

//Element Movement
var oPositionX = '';
var oPositionY = '';
var movingElement = '';

document.onmouseup = function () { disableMovement(movingElement); };

function enableMovement(el) {
    var mDivs = el.getElementsByTagName('div');

    for (var i = 0; i <= mDivs.length - 1; i++) {
        if (mDivs[i].forMoving) {
            mDivs[i].style.display = 'block';
            break;
        }
    }

    movingElement = el;
    document.onmousemove = function () {
        moveElement(el);
    }
}

function disableMovement(el) {
    if (el) {
        var mDivs = el.getElementsByTagName('div');

        for (var i = 0; i <= mDivs.length - 1; i++) {
            if (mDivs[i].forMoving) {
                mDivs[i].style.display = 'none';
                break;
            }
        }

    movingElement = '';
    document.onmousemove = '';
    oPositionX = '';
    oPositionY = '';
    }
}

function moveElement(el) {
    var movementEvent = this.event;
    var sPositionX = movementEvent.clientX;
    var sPositionY = movementEvent.clientY;

        if (oPositionX != '') {
            var newPos = parseInt(el.style.left) - (oPositionX - sPositionX);
            if (validateHorizontalMovement(el,newPos)){
                el.style.left = newPos + 'px';
            }
        }
        if (oPositionY != '') {
            var newPos = parseInt(el.style.top) - (oPositionY - sPositionY);
            if (validateVerticalMovement(el,newPos)){
                el.style.top = newPos + 'px';
            }
        }
    oPositionX = sPositionX;
    oPositionY = sPositionY;
}

function validateHorizontalMovement(el,newPos) {
    var wManager = new WindowManager();
    var elLeft = getAbsoluteLeft(el);
    var elRight = el.clientWidth + elLeft;

    if (newPos < elLeft) {
        if (elLeft <= 1) {
            return false;
        }
    }
    else {
        if (elRight >= wManager.getWidth() - 2) {
            return false;
        }
    }
    return true;
}

function validateVerticalMovement(el,newPos) {
    var wManager = new WindowManager();
    var elTop = getAbsoluteTop(el);
    var elBottom = el.clientHeight + elTop;

    if (newPos < elTop) {
        if (elTop <= 1) {
            return false;
        }
    }
    else {
        if (elBottom >= wManager.getHeight() - 2) {
            return false;
        }
    }
    return true;
}
//End Element Movement

//Absolute Positions
function getAbsoluteTop(el) {
    var iReturnValue = 0;
    while (el != null) {
        iReturnValue += el.offsetTop;
        el = el.offsetParent;
    }
    return iReturnValue;
}

function getAbsoluteLeft(el) {
    var iReturnValue = 0;
    while (el != null) {
        iReturnValue += el.offsetLeft;
        el = el.offsetParent;
    }
    return iReturnValue;
}

function getScrollBottom(p_oElem) {
    try {
        element = document.getElementById(p_oElem);
        return element.scrollHeight - element.scrollTop - element.clientHeight - 1;
    }
    catch (e) {
    }
}

function getScrollBarWidth() {
    var inner = document.createElement('p');
    inner.style.width = "100%";
    inner.style.height = "200px";

    var outer = document.createElement('div');
    outer.style.position = "absolute";
    outer.style.top = "0px";
    outer.style.left = "0px";
    outer.style.visibility = "hidden";
    outer.style.width = "200px";
    outer.style.height = "150px";
    outer.style.overflow = "hidden";
    outer.appendChild(inner);

    document.body.appendChild(outer);
    var w1 = inner.offsetWidth;
    outer.style.overflow = 'scroll';
    var w2 = inner.offsetWidth;
    if (w1 == w2) {
    	w2 = outer.getAttribute('clientWidth')
    };

    document.body.removeChild(outer);

    return (w1 - w2);
}
//End Absolute Positions

//Window Mananger
function WindowManager() {
    // We find out what type of browser we are in
    // and set some object properties accordingly.
    this.isIE = false;
    this.isMozilla = false;
    this.isOldIE = false;

    if (window.innerHeight)
        this.isMozilla = true;
    // IE
    else if (document.documentElement && document.documentElement.clientHeight)
        this.isIE = true;
    // IE 4
    else if (document.body.clientHeight)
        this.isOldIE = true;
}
WindowManager.prototype.getHeight = function () {
    var height;
    // Mozilla
    if (this.isMozilla)
        height = window.innerHeight;
    // IE
    else if (this.isIE)
        height = document.documentElement.clientHeight;
    // IE 4
    else if (this.isOldIE)
        height = document.body.clientHeight;

    return height;
};
WindowManager.prototype.getWidth = function () {
    var width;
    // Mozilla
    if (this.isMozilla)
        width = window.innerWidth;
    // IE
    else if (this.isIE)
        width = document.documentElement.clientWidth;
    // IE 4
    else if (this.isOldIE)
        width = document.body.clientWidth;

    return width;
};
function getWindowHeight(wm) {
    var height = 0;
    if (wm.isMozilla) {
        height = window.innerHeight;
    }
    else if (wm.isIE) {
        height = document.documentElement.clientHeight;
    }
    else if (wm.isOldIE) {
        height = document.body.clientHeight;
    }

    return height;
}
//End Window Manager

//Context Menus
function closeContextMenuItems(el,path) {
    try{
        var elementPage = this;
        var element = el || event.srcElement;
        var contextButtonClicked = false;

        var loopedObjects = element;

        //check if the button that opens the Context Menu was pressed, we dont want to close the context menu this way if it was.
        if (element.srcElement) {
            loopedObjects = element.srcElement;
            while (loopedObjects.parentNode) {
                if (loopedObjects.ContextMenuButton) {
                    contextButtonClicked = true;
                    break;
                }
                loopedObjects = loopedObjects.parentNode;
            }
        }
        else {
            while (loopedObjects.parentNode) {
                if (loopedObjects.ContextMenuButton) {
                    contextButtonClicked = true;
                    break;
                }
                loopedObjects = loopedObjects.parentNode;
            }
        }
        //----

        if (!contextButtonClicked) {

            var frames = document.getElementsByTagName('iframe');

            var contextMenuItems = document.getElementsByTagName('div');

            for (var i = 0; i <= contextMenuItems.length - 1; i++) {
                if (contextMenuItems[i].ContextMenu) {
                    contextMenuItems[i].style.display = 'none';
                }
            }

            if (path != 'down') {//if the path is down we dont try to close parent pages or it will infinit loop
                while (elementPage != elementPage.parent) {//closes context menus from parent pages
                    if (elementPage.parent.closeContextMenuItems) {
                        elementPage.parent.closeContextMenuItems(element);
                    }
                    elementPage = elementPage.parent;
                }
            }

            for (var i = 0; i <= frames.length - 1; i++) {//closes context menus from child pages
                if (frames[i].contentWindow.document) {
                    if (frames[i].contentWindow.closeContextMenuItems) {
                        frames[i].contentWindow.closeContextMenuItems(element, 'down');
                    }
                }
            }
        }
    }
    catch (e) {
    }
}

addEventHandler(document, 'click', closeContextMenuItems);
//End Context Menus

//Menu Items
function closeMenuItem(elementToCloseID, openingElementID) {
    var clickedObject = event.srcElement;
    var loopedObjects = clickedObject;
    var elementToClose = document.getElementById(elementToCloseID);
    var openingElement = document.getElementById(openingElementID);
    var blnForObject = false;

    while (loopedObjects.parentNode) {
        if (loopedObjects == elementToClose || loopedObjects == openingElement) {
            blnForObject = true;
            break;
        }
        loopedObjects = loopedObjects.parentNode;
    }

    if (!blnForObject && elementToClose) {
        elementToClose.style.display = 'none';
    }

}
//End Menu Items


//Related Items
function repositionRelatedItems(itemID, btnID) {
    var btnRelatedItems = document.getElementById(btnID);
    var relatedItems = document.getElementById(itemID);

    var top = getAbsoluteTop(btnRelatedItems) + btnRelatedItems.offsetHeight + 2;
    var left = getAbsoluteLeft(btnRelatedItems) - relatedItems.offsetWidth + btnRelatedItems.offsetWidth - 2;

    relatedItems.style.top = top + 'px';
    relatedItems.style.left = left + 'px';
}

function relatedItems_Selected(item, selectorID) {
    var selector = document.getElementById(selectorID);
    if (selector) {
        selector.style.display = 'block';
        selector.style.width = item.offsetWidth - 4 + 'px';
        selector.style.top = item.offsetTop + 2 + 'px';
        selector.style.left = item.offsetLeft + 2 + 'px';
    }
}

function relatedItems_DeSelected(item, selectorID) {
    var selector = document.getElementById(selectorID);
    if (selector) {
        selector.style.display = 'none';
    }
}

function showRelatedItems(btnRelatedItemsID, pageRelatedItemsID) {
    var pageRelatedItems = document.getElementById(pageRelatedItemsID);

    if (pageRelatedItems.style.display != 'block') {
        pageRelatedItems.style.display = 'block';
        moveRelatedItems(btnRelatedItemsID, pageRelatedItemsID);
    }
    else {
        hideRelatedItems(pageRelatedItemsID);
    }
}

function hideRelatedItems(pageRelatedItemsID) {
    var pageRelatedItems = document.getElementById(pageRelatedItemsID);

    pageRelatedItems.style.display = 'none';
}

function moveRelatedItems(btnRelatedItemsID, pageRelatedItemsID) {
    var btnRelatedItems = document.getElementById(btnRelatedItemsID);
    var pageRelatedItems = document.getElementById(pageRelatedItemsID);

    var top = (parseInt(getAbsoluteTop(btnRelatedItems)) + parseInt(btnRelatedItems.offsetHeight) + 5) + 'px';
    var left = (parseInt(getAbsoluteLeft(btnRelatedItems)) - (parseInt(pageRelatedItems.offsetWidth) - parseInt(btnRelatedItems.offsetWidth))) + 'px';

    pageRelatedItems.style.top = top;
    pageRelatedItems.style.left = left;
}
//End Related Items

//Frame Information
function getMyFrameFromParent() {
    var arrFrames = parent.document.getElementsByTagName("IFRAME");
    for (var i = 0; i <= arrFrames.length-1; i++) {
        if (arrFrames[i].contentWindow == window) {
            return arrFrames[i];
        }
    }
}
//End Frame Information

//Manipulate Div Button images for mouseover,mouseout,click
function highlightButton(button, value) {
    try {
        var eDivs = button.getElementsByTagName('div');

        if (value == 0) {
            eDivs[0].style.backgroundPosition = "0px 0px";
            eDivs[1].style.backgroundPosition = "0px 0px";
            eDivs[3].style.backgroundPosition = "0px 0px";
        }
        else if (value == 1) {
            eDivs[0].style.backgroundPosition = "6px 0px";
            eDivs[1].style.backgroundPosition = "600px 0px";
            eDivs[3].style.backgroundPosition = "6px 0px";
        }
        else if (value == 2) {
            eDivs[0].style.backgroundPosition = "3px 0px";
            eDivs[1].style.backgroundPosition = "300px 0px";
            eDivs[3].style.backgroundPosition = "3px 0px";
        }
        else {
            eDivs[0].style.backgroundPosition = "0px 0px";
            eDivs[1].style.backgroundPosition = "0px 0px";
            eDivs[3].style.backgroundPosition = "0px 0px";
        }
    }
    catch (e) {
    }
}
//End Manipulate Div Buttons

//Resize Handlers
function frameContent(frame) {
    var iFrameContent;
    if (frame.contentDocument) { // FF CHROME SAFARI
        iFrameContent = frame.contentDocument;
    }
    else if (frame.contentWindow) { // IE
        iFrameContent = frame.contentWindow.document;
    }
    return iFrameContent;
}

function resizePageElement(elID, modifier) {
    var el = document.getElementById(elID);
    if (el) {
        var wm = new WindowManager;
        if (!modifier) {
            modifier = 0;
        }
        var height = wm.getHeight() - getAbsoluteTop(el) - modifier;

        if (height > 10) {
            if (el) {
                el.style.height = height + 'px';
                el.height = height;
                if (wm.isMozilla) {
                    el.height = height + 'px';
                }
                if (el.tagName.toUpperCase() == 'IFRAME') {
                    if (frameContent) {
                        var content = frameContent(el);
                        if (content.body) {
                            content.body.style.height = height + 'px';
                        }
                    }
                }
            }
        }
    }
}
function resizeElementWidth(elID, width) {
    var elem = $('#' + elID);
    if (elem) {
        elem.width(width);
        if (elem[0].tagName.toUpperCase() == 'IFRAME') {
            if (elem.contentWindow.document.body) {
                elem.contentWindow.document.body.style.width = width;
            }
        }
    }
}

function resizeFrameFromContents(frameID, modifier) {
    var frame = parent.document.getElementById(frameID);

    if (frame) {
        if (!modifier) {
            modifier = 0;
        }
        //var wm = new WindowManager();
        var height = document.body.offsetHeight - modifier;

        if (height > 10) {
            frame.style.height = parseInt(height) + 'px';
        }
    }
}
//End Resize Handlers

//Event Handlers
function addEventHandler(to_element, event, handler) {
    if (to_element.addEventListener) to_element.addEventListener(event, handler, false);
    else if (to_element.attachEvent) to_element.attachEvent('on' + event, handler);
    else return false;
}

//Event Handlers

//Math
function highestCommonFactor(a, b) {
    while (b != 0) {
        var t = b;
        b = a % b;
        a = t;
    }

    return a;
}

function decimalToFraction(num) {
    if (num != Infinity) {
        var decimalArray = num.toString().split(".");
        var leftDecimalPart = decimalArray[0];
        var rightDecimalPart = decimalArray[1];
        var wholeNumber = leftDecimalPart;
        leftDecimalPart = '';
        if (rightDecimalPart) {
            var numerator = leftDecimalPart + rightDecimalPart
            var denominator = Math.pow(10, rightDecimalPart.length);
            var factor = highestCommonFactor(numerator, denominator);
            denominator /= factor;
            numerator /= factor;

            if (wholeNumber != 0 && wholeNumber != -0) {
                return wholeNumber + ' ' + numerator + '/' + denominator;
            }
            else {
                return numerator + '/' + denominator;
            }
        }
        return num;
    }
    else {
        return 0;
    }
}

function fractionToDecimal(num) {
    var values = num.split(' ');
    if (values[1]) {
        values[1] = eval(values[1]);
        var splitValues = values[1].toString().split('.');
        if (splitValues.length > 1) {
            values[0] = parseInt(values[0]) + parseInt(splitValues[0]);
            values[1] = splitValues[1];
        }
    }
    return values.join('.');
}

function stripAlpha(val) {
    return val.replace(/[^\d]/g, '');
}

function limitNumber(num, min, max) {
    if (!isNaN(num)) {
        if (num < min) num = min;
        if (num > max) num = max;
    }

    return num;
}
//-------------

function postToURL(frame, url, params) {
    var form = document.createElement("form");
    form.setAttribute("method", 'post');
    form.setAttribute("action", url);
    form.setAttribute("target", frame);

    for (var key in params) {
        if (params.hasOwnProperty(key)) {
            var hiddenField = document.createElement("input");
            hiddenField.setAttribute("type", "hidden");
            hiddenField.setAttribute("name", key);
            hiddenField.setAttribute("value", params[key]);
            form.appendChild(hiddenField);
        }
    }

    document.body.appendChild(form);
    form.submit();
    document.body.removeChild(form);
}

function postMyURL(form, url, params) {
    form.setAttribute("method", 'post');
    form.setAttribute("action", url);

    for (var key in params) {
        if (params.hasOwnProperty(key)) {
            var hiddenField = document.createElement("input");
            hiddenField.setAttribute("type", "hidden");
            hiddenField.setAttribute("name", key);
            hiddenField.setAttribute("value", params[key]);
            form.appendChild(hiddenField);
        }
    }

    form.submit();
}

//Main Menu Closer
function closeMainMenu(elID) {
    try {
        var srcElement = event.srcElement;

        var mainMenuPanel = null;
        if (elID == 'tdFRMMenu') { mainMenuPanel = defaultParentPage.menuPanelContainer; }
        else if (elID == 'tdFRMTools') { mainMenuPanel = defaultParentPage.toolPanelContainer; }

        while (srcElement) {
            if (srcElement == mainMenuPanel || srcElement.id == elID) {
                return;
            }
            srcElement = srcElement.parentNode;
        }

        if (mainMenuPanel) mainMenuPanel.style.display = 'none';
    }
    catch (e) {
    }

    return;
}

function removeLeadingTrailingCharacter(text, c) {
    try {

        while (text.length > 0 && text.substr(0, 1) == c) {
            text = text.substr(1);
        }

        while (text.length > 0 && text.substr(text.length - 1) == c) {
            text = text.substr(0, text.length - 1);
        }

        return text;
    } catch (e) {
        alert('Error: Scripts/funds_common.js-removeLeadingTrailingCharacter() ' + e.message);
        return text;
    }
} //end removeLeadingTrailingCharacter()

function insertItemsToCheckBoxList(list, items, repeatDirection, repeatCount) {
    try {
        if (list) {
            if (typeof items === 'string') {
                items = items.split(',');
            }
            if (!repeatCount) {
                repeatCount = 1;

                if (repeatDirection == 'HORIZONTAL') {
                    repeatDirection = 'VERTICAL';
                }
                else {
                    repeatDirection = 'HORIZONTAL';
                }
            }
            if (!repeatDirection) {
                repeatDirection = 'HORIZONTAL';
            }

            var itemCounter = 0;

            clearCheckBoxList(list);

            if (items.length > 0) {
                if (repeatDirection.toUpperCase() == 'VERTICAL') {
                    var cellCount = Math.ceil(items.length / repeatCount);
                    var rowCount = repeatCount;
                }
                if (repeatDirection.toUpperCase() == 'HORIZONTAL') {
                    var rowCount = Math.ceil(items.length / repeatCount);
                    var cellCount = repeatCount;
                }

                for (var i = 0; i <= rowCount - 1; i++) {
                    var nRow = list.insertRow();
                    for (var c = 0; c <= cellCount - 1; c++) {
                        if (items[itemCounter]) {
                            var nCell = nRow.insertCell();
                            var nCheck = document.createElement('input');
                            var nLabel = document.createElement('label');

                            nCheck.type = 'checkbox';
                            nCheck.id = list.id + '_' + itemCounter;
                            nCheck.name = list.id + '$' + itemCounter;

                            nLabel.htmlFor = list.id + '_' + itemCounter;
                            nLabel.innerText = items[itemCounter];

                            nCell.appendChild(nCheck);
                            nCell.appendChild(nLabel);
                        }
                        else {
                            break;
                        }

                        itemCounter++;
                    }
                }
            }
        }
    }
    catch (e) {
    }
}

function setSelectedCheckBoxListItems(checkBoxList, selectedItems) {
    if (checkBoxList) {
        selectedItems = ',' + selectedItems.toUpperCase() + ',';
        var checkboxes = checkBoxList.getElementsByTagName('input');
        for (var i = 0; i <= checkboxes.length - 1; i++) {
            var checkBox = checkboxes[i];
            if (checkBox) {
                var checkLabel = checkBox.nextSibling;
                if (checkLabel) {
                    if (selectedItems.indexOf(',' + checkLabel.innerText.toUpperCase() + ',') > -1) {
                        checkBox.checked = true;
                    }
                }
            }
        }
    }
}

function getSelectedCheckBoxListText(checkBoxList) {
    if (checkBoxList) {
        var text = '';
        var checkboxes = checkBoxList.getElementsByTagName('input');
        for (var i = 0; i <= checkboxes.length - 1; i++) {
            var checkBox = checkboxes[i];
            if (checkBox) {
                if (checkBox.checked) {
                    if (text === '') {
                        text = getCheckBoxText(checkBox);
                    }
                    else {
                        text += ',' + getCheckBoxText(checkBox);
                    }
                }
            }
        }
        return text;
    }
    return '';
}

function getSelectedCheckBoxListValue(checkBoxList) {
    if (checkBoxList) {
        var value = '';
        var checkboxes = checkBoxList.getElementsByTagName('input');
        for (var i = 0; i <= checkboxes.length - 1; i++) {
            var checkBox = checkboxes[i];
            if (checkBox) {
                if (checkBox.checked) {
                    if (value === '') {
                        value = checkBox.getAttribute('value')
                    }
                    else {
                        value += ',' + checkBox.getAttribute('value')
                    }
                }
            }
        }
        return value;
    }
    return '';
}

function getSelectedCheckBoxListIndex(checkBoxList) {
    if (checkBoxList) {
        var value = '';
        var checkboxes = checkBoxList.getElementsByTagName('input');
        for (var i = 0; i <= checkboxes.length - 1; i++) {
            var checkBox = checkboxes[i];
            if (checkBox) {
                if (checkBox.checked) {
                    if (value === '') {
                        value = i;
                    }
                    else {
                        value += ',' + i;
                    }
                }
            }
        }
        return value;
    }
    return '';
}

function getCheckBoxText(checkBox,html) {
    if (checkBox) {
        if (html) {
            return checkBox.nextSibling.innerHTML;
        }
        else {
            return checkBox.nextSibling.innerText;
        }
    }
}

function clearCheckBoxList(list) {
    if (list) {
        for (var i = list.rows.length - 1; i >= 0; i--) {
            list.deleteRow(i);
        }
    }
}

addEventHandler(document, 'click', function () { closeMainMenu('tdFRMMenu') });
addEventHandler(document, 'click', function () { closeMainMenu('tdFRMTools') });

//Bubble message popup

function bubbleMessage(srcElement, html) {
    this.open = false;
    this.container = document.createElement('div');
    this.contentContainer = null;
    this.arrow = '';
    this.innerHTML = html;
    this.textAlign = 'left';
    this.verticalAlign = '';
    this.horizontalAlign = '';
    this.srcElement = srcElement || document.getElementById('srcElement');
    this.show = function (duration) {
        if (this.srcElement && this.container) {
            this.container.style.display = 'block';
            this.position();

            if (duration) {
                setTimeout(closeActiveBubbleMessagesFromPopup(), duration);
            }
        }
    };
    this.hide = function () {
        if (this.container) {
            this.container.style.display = 'none';
        }
    };
    this.setInnerText = function (value) {
        this.contentContainer.innerText = value;
    };
    this.setInnerHTML = function (value) {
        this.contentContainer.innerHTML = value;
    };
    this.position = function () {
        if (this.srcElement && this.container) {
            var top = 0;
            var bottom = 0;

            switch (this.verticalAlign.toUpperCase()) {
                case 'TOP': default:
                    top = this.getSourceTop() - (this.container.offsetHeight / 2);
                    break;
                case 'MIDDLE':
                    top = this.getSourceTop() + (this.srcElement.offsetHeight / 2) - (this.container.offsetHeight / 2);
                    break;
                case 'BOTTOM':
                    top = this.getSourceTop() + this.srcElement.offsetHeight - (this.container.offsetHeight / 2);
                    break;
            }
            switch (this.horizontalAlign.toUpperCase()) {
                case 'LEFT': default:
                    left = this.getSourceLeft() - (this.container.offsetWidth / 2);
                    break;
                case 'CENTER':
                    left = this.getSourceLeft() + (this.srcElement.offsetWidth / 2) - (this.container.offsetWidth / 2);
                    break;
                case 'RIGHT':
                    left = this.getSourceLeft() + this.srcElement.offsetWidth - 8;
                    break;
            }

            //this.arrow.style.top = '20px';
            //this.arrow.style.left = '4px';

            this.container.style.top = top + 'px';
            this.container.style.left = left + 'px';
        }
    };

    this.getSourceTop = function () {
        var iReturnValue = 0;
        var el = this.srcElement;
        while (el != null) {
            iReturnValue += el.offsetTop;
            el = el.offsetParent;
        }
        return iReturnValue;
    };

    this.getSourceLeft = function () {
        var iReturnValue = 0;
        var el = this.srcElement;
        while (el != null) {
            iReturnValue += el.offsetLeft;
            el = el.offsetParent;
        }
        return iReturnValue;
    };

    if (this.container) {
        var table = document.createElement('table');
        var topRow = table.insertRow(0);
        var middleRow = table.insertRow(1);
        var bottomRow = table.insertRow(2);
        var divContents = document.createElement('div');
        //var imgArrow = document.createElement('img');

        for (var i = 0; i <= 3; i++) {
            topRow.insertCell(i);
            middleRow.insertCell(i);
            bottomRow.insertCell(i);
        }

        this.container.appendChild(table);
        this.contentContainer = divContents;
        middleRow.cells[1].appendChild(divContents);

        table.cellPadding = 0;
        table.cellSpacing = 0;

        topRow.cells[0].style.width = '20px';
        topRow.cells[0].style.height = '20px';
        topRow.cells[2].style.width = '20px';
        topRow.cells[2].style.height = '20px';
        topRow.cells[1].style.height = '20px';

        middleRow.cells[0].style.width = '20px';
        middleRow.cells[1].style.height = '20px';

        bottomRow.cells[0].style.width = '20px';
        bottomRow.cells[0].style.height = '20px';
        bottomRow.cells[2].style.width = '20px';
        bottomRow.cells[2].style.height = '20px';
        bottomRow.cells[1].style.height = '20px';

        //imgArrow.style.width = '20px';
        //imgArrow.style.height = '20px';

        //imgArrow.src = 'images/bubble/bubble_arrow_left_yellow.png';


        topRow.cells[0].innerHTML = '&nbsp;';
        topRow.cells[1].innerHTML = '&nbsp;';
        topRow.cells[2].innerHTML = '&nbsp;';

        middleRow.cells[0].innerHTML = '&nbsp;';
        middleRow.cells[2].innerHTML = '&nbsp;';

        bottomRow.cells[0].innerHTML = '&nbsp;';
        bottomRow.cells[1].innerHTML = '&nbsp;';
        bottomRow.cells[2].innerHTML = '&nbsp;';

        if (this.innerHTML) {
            divContents.innerHTML = this.innerHTML;
        }
        else {
            divContents.innerHTML = '&nbsp;';
        }

        this.container.style.display = 'none';
        this.container.style.position = 'absolute';
        this.container.style.left = '0px';
        this.container.style.top = '0px';
        this.container.style.cursor = 'default';

        divContents.style.paddingLeft = '4px';
        divContents.style.paddingRight = '4px';
        divContents.style.textAlign = this.textAlign;

        //imgArrow.style.position = 'absolute';

        //this.arrow = imgArrow;

        //this.container.appendChild(imgArrow);

        var bubbleM = this;
        this.container.onclick = function () { bubbleM.hide(); };
        $(this.container).attr('id', 'divBubbleMessage');

        if (srcElement != null) {
            srcElement.appendChild(this.container);
        }
        else {
            //document.body.appendChild(this.container);
        }
        srcElement.bubbleMessage = this;

        srcElement.setAttribute('bubbleMessage', this);
    }
}

function closeActiveBubbleMessagesFromPopup() {
    var allElements = document.getElementsByTagName('*');
    activeBubbleMessages = [];
    for (var i = 0; i <= allElements.length - 1; i++) {
        var bubbleMessage = allElements[i].bubbleMessage;
        if (!bubbleMessage && allElements[i].getAttribute) {
            bubbleMessage = allElements[i].getAttribute('bubbleMessage');
        }
        if (bubbleMessage) {
            if (bubbleMessage.container.style.display == 'block') {
                activeBubbleMessages[activeBubbleMessages.length] = bubbleMessage;
                bubbleMessage.hide();
            }
        }
    }
}

function showActiveBubbleMessagesFromPopup() {
    for (var i = 0; i <= activeBubbleMessages.length - 1; i++) {
        activeBubbleMessages[i].show();
    }
    activeBubbleMessages = [];
}

// simple messaging (shows popups that fade after 3 seconds)
function successMessage(html, pos, autoClear, appendTo, customID) {
    simpleMessage(html, 'success', pos, autoClear, appendTo, customID);
}

function warningMessage(html, pos, autoClear, appendTo, customID) {
    simpleMessage(html, 'warning', pos, autoClear, appendTo, customID);
}
function dangerMessage(html, pos, autoClear, appendTo, customID) {
    simpleMessage(html, 'danger', pos, autoClear, appendTo, customID);
}

function infoMessage(html, pos, autoClear, appendTo, customID) {
    simpleMessage(html, 'info', pos, autoClear, appendTo, customID);
}

function plainMessage(html, pos, autoClear, appendTo, customID) {
    simpleMessage(html, 'plain', pos, autoClear, appendTo, customID);
}

function simpleMessage(html, type, pos, autoClear, appendTo, customID) {
    if (type == null) type = 'success';
    if (pos == null) pos = 'centered';
    if (autoClear == null) autoClear = true;
    if (appendTo == null) appendTo = document.body;

    var d = document.createElement('div');
    d.style.display = 'none';
    d.style.zIndex = '10000';
    var id = 'simplemessage_' + generateUniqueID();
    if (customID != null) {
        id = customID;
        if ($(appendTo).find('#' + customID).length > 0) {
            return; // we already have a popup msg with the same customID (they need to be unique)
        }
    }

    d.id = id;

    appendTo.appendChild(d);
    smpMsgDiv = $(d);

    smpMsgDiv.addClass('simplemessage');

    if (pos.indexOf(',') != -1) {
        smpMsgDiv.css('position', 'absolute');

        var coords = pos.trim().split(',');
        if (coords.length >= 2) {
            smpMsgDiv.css('left', coords[0].trim());
            smpMsgDiv.css('top', coords[1].trim());
        }
    }
    else {
        if (pos.indexOf('left') != -1) {
            smpMsgDiv.addClass('left');
        }
        else if (pos.indexOf('right') != -1) {
            smpMsgDiv.addClass('right');
        }
        else {
            smpMsgDiv.addClass('hcenter');
        }

        if (pos.indexOf('top') != -1) {
            smpMsgDiv.addClass('top');
        }
        else if (pos.indexOf('bottom') != -1) {
            smpMsgDiv.addClass('bottom');
        }
        else {
            smpMsgDiv.addClass('vcenter');
        }
    }

    if (type != 'success' && type != 'info' && type != 'warning' && type != 'danger' && type != 'plain') {
        type = 'success';
    }

    smpMsgDiv.addClass(type);

    smpMsgDiv.html(html);
    smpMsgDiv.show();

    if (autoClear) {
        setTimeout(function () { $('#' + id, appendTo).fadeOut(1000, function () { $('#' + id, appendTo).remove(); }); }, 1750);
    }

    return id;
}

// notification alert overlay window (slides up from bottom right)
function showNotificationOverlay(title, msg, icon, width, height, pos) {
    if (msg == null || msg.length == 0) {
        return;
    }

    if (title == null || title.length == 0) {
        title = 'System Notification';
    }

    if (icon == null || icon.length == 0) {
        icon = 'images/icons/exclamation.png';
    }

    if (width == null) width = 350;
    if (height == null) height = 200;
    if (pos == null) pos = 'bottom right';

    var div = $(document.createElement('div'));
    var id = 'notificationoverlay_' + generateUniqueID();

    div.prop('id', id);
    div.css('position', 'absolute');

    if (pos.indexOf('right') != -1) {
        div.css('right', getScrollBarWidth() + 'px');
    }

    div.css('width', width + 'px');
    if (height != null) {
        div.css('height', height + 'px');
        div.css('overflow-y', 'auto');
        div.css('overflow-x', 'hidden');
    }
    div.css('background-color', 'white');
    div.css('border', '1px solid #aaaaaa');

    var html = '';
    html += '<div style="background-color:#eeeeee;padding:5px;border-bottom:1px solid #aaaaaa;text-align:left;position:relative;cursor:pointer;" onclick="destroyNotificationOverlay(\'' + id + '\');">';
    if (icon != null) {
        html += '<img src="' + icon + '" style="position:absolute;width:16px;height:16px;top:3px;left:3px;"><div style="position:relative;left:19px;"><b>' + title + '</b></div>';
    }
    else {
        html += '<b>' + title + '</b>';
    }
    html += '<img src="images/fatarrowdown.png" style="position:absolute;top:3px;right:3px;width:16px;height:16px;">';
    html += '</div>';
    html += '<div style="padding:5px;">' + msg + '</div>';

    div.html(html);
    div.appendTo(document.body);

    if (pos.indexOf('bottom') != -1) {
        div.animate({ 'bottom': '0px' });
    }

    return id;
}

function destroyNotificationOverlay(id) {
    if (id == null) {
        id = '[id*=notificationoverlay_]';
    }
    else {
        id = '#' + id;
    }

    $(id).animate({ 'bottom' : (-1 * $(id).height()) + 'px'}, 300, function () { $(id).remove(); });
}



function generateUniqueID(len, allowNumbers, allowLetters, special) {
    var id = '';

    if (len == null) {
        len = 10;
    }

    if (allowNumbers == null) {
        allowNumbers = true;
    }

    if (allowLetters == null) {
        allowLetters = true;
    }

    if (special == null) {
        special = '_';
    }

    var chars = (allowNumbers ? '0123456789' : '') + (allowLetters ? 'abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ' : '') + special;
    var charsLen = chars.length;

    for (var i = 0; i < len; i++) {
        var pos = getRandomInt(charsLen);
        id += chars.substring(pos, pos + 1);
    }

    return id;
}

function getRandomInt(max) {
    return Math.floor(Math.random() * Math.floor(max));
}


function StripHTML(str)
{
    if (str != null) {
        // we add surrounding tags because without surrounding tags, plain text is interpreted as a tag
        // for example, "test" is stripped, but "<span>test</span>" returns "test"
        return $('<span>' + str + '</span>').text();
    }
    else {
        return null;
    }
}

function StrongEscape(str)
{
    if (str != null) {
        str = str.split('[').join('|LB|');
        str = str.split(']').join('|RB|');

        str = str.split('\\').join('[BS]');
        str = str.split('/').join('[FS]');
        str = str.split('\"').join('[QUOTE]');
        str = str.split('&').join('[AMP]');
        str = str.split('\'').join('[APOS]');
        str = str.split('<').join('[LT]');
        str = str.split('>').join('[GT]');
        str = str.split('+').join('[PL]');
        str = str.split('\t').join('[TAB]');
        str = str.split('\r\n').join('[NEWLINERN]');
        str = str.split('\n').join('[NEWLINE]');

        return str;
    }
    else {
        return null;
    }
}

function UndoStrongEscape(str)
{
    if (str != null) {
        str = str.split('[BS]').join('\\');
        str = str.split('[FS]').join('/');
        str = str.split('[QUOTE]').join('"');
        str = str.split('[AMP]').join('&');
        str = str.split('[APOS]').join('\'');
        str = str.split('[LT]').join('<');
        str = str.split('[GT]').join('>');
        str = str.split('[PL]').join('+');
        str = str.split('[TAB]').join('\t');
        str = str.split('[NEWLINERN]').join('\r\n');
        str = str.split('[NEWLINE]').join('\n');

        str = str.split('|LB|').join('[');
        str = str.split('|RB|').join(']');

        return str;
    }
    else {
        return null;
    }
}

///////////////////////////////////////////////
///// PDF SUPPORT
///////////////////////////////////////////////

function showPDFViewer(pdfURL, title, width, height, customButtonLabels, customButtonFunctions, customButtonData) {
    //debugger;
    if (pdfURL == null || pdfURL.length == 0) {
        dangerMessage('Invalid PDF URL');
    }

    this.pdfjsLib = pdfjsLib;
    pdfjsLib.pdfURL = pdfURL;
    pdfjsLib.currentZoom = 100;
    pdfjsLib.currentScale = 1.25;

    if (title == null) title = 'WTS - PDF Viewer';
    if (width == null) width = 800;
    if (height == null) height = parseInt((new WindowManager()).getHeight() * .9);
    var icon = 'images/icons/pdf_16.png';

    var div = $(document.createElement('div'));
    var id = 'pdfviewer';

    div.prop('id', id);
    div.css('position', 'absolute');
    div.css('display', 'none');

    div.css('overflow-y', 'hidden');
    div.css('overflow-x', 'hidden');
    div.css('background-color', 'white');
    div.css('border', '1px solid #aaaaaa');

    var html = '';
    html += '<div style="border-bottom:1px solid #000000;padding:3px;">';
    html += '<img id="arrowleftallpdfviewer" src="images/icons/arrowleftallgrey.png" style="cursor:pointer;position:relative;top:2px;" onclick="popupManager.GetPopupByName(\'PDFVIEWER\').Opener.pdfjsLib.goToPage(1)" alt="First Page" title="First Page">&nbsp;';
    html += '<img id="arrowleftpdfviewer" src="images/icons/arrowleftgrey.png" style="cursor:pointer;opacity:.3;position:relative;top:2px;" onclick="popupManager.GetPopupByName(\'PDFVIEWER\').Opener.pdfjsLib.showPreviousPage(false)" alt="Previous Page" title="Previous Page">&nbsp;';
    html += '<img id="arrowrightpdfviewer" src="images/icons/arrowrightgrey.png" style="cursor:pointer;opacity:.3;position:relative;top:2px;" onclick="popupManager.GetPopupByName(\'PDFVIEWER\').Opener.pdfjsLib.showNextPage()" alt="Next Page" title="Next Page">&nbsp;';
    html += '<img id="arrowrightallpdfviewer" src="images/icons/arrowrightallgrey.png" style="cursor:pointer;position:relative;top:2px;" onclick="popupManager.GetPopupByName(\'PDFVIEWER\').Opener.pdfjsLib.goToPage(-1)" alt="Last Page" title="Last Page">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;';
    html += '<input id="txtgotopagepdfviewer" type="text" value="1" style="text-align:center;width:25px;background-color:white;position:relative;" maxlength="3" onkeyup="popupManager.GetPopupByName(\'PDFVIEWER\').Opener.pdfjsLib.goToPage(-2)"> of <span id="spantotalpagespdfviewer">1</span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;';

    html += '<img id="zoomoutpdfviewer" src="images/icons/zoom_out.png" style="cursor:pointer;width:16px;height:16px;position:relative;top:2px;" onclick="popupManager.GetPopupByName(\'PDFVIEWER\').Opener.pdfjsLib.zoom(-25)" alt="Zoom Out" title="Zoom Out">&nbsp;&nbsp;';
    html += '<img id="zoominpdfviewer" src="images/icons/zoom_in.png" style="cursor:pointer;width:16px;height:16px;position:relative;top:2px;" onclick="popupManager.GetPopupByName(\'PDFVIEWER\').Opener.pdfjsLib.zoom(25)" alt="Zoom In" title="Zoom In">&nbsp;&nbsp;';
    html += '<img id="zoomresetpdfviewer" src="images/icons/zoom_reset.png" style="cursor:pointer;width:16px;height:16px;position:relative;top:2px;" onclick="popupManager.GetPopupByName(\'PDFVIEWER\').Opener.pdfjsLib.zoom(0)" alt="Reset Zoom" title="Reset Zoom">&nbsp;&nbsp;&nbsp;';
    html += '<span id="spanzoompctpdfviewer">100%</span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;';

    html += '<img id="savepdfviewer" src="images/icons/disk.png" style="cursor:pointer;width:16px;height:16px;position:relative;top:3px;" onclick="popupManager.GetPopupByName(\'PDFVIEWER\').Opener.pdfjsLib.savePDF()" alt="Save PDF" title="Save PDF">';

    if (customButtonLabels != null && customButtonLabels.length > 0) {
        html += '<div id="custombuttonspdfviewer" style="position:absolute;right:3px;top:3px;">';

        for (var i = 0; i < customButtonLabels.length; i++) {
            var label = customButtonLabels[i];
            var fnc = customButtonFunctions[i];
            var data = customButtonData != null ? customButtonData[i] : '';

            if (i > 0) {
                html += '&nbsp;';
            }

            html += '<input type="button" value="' + label + '" onclick="popupManager.GetPopupByName(\'PDFVIEWER\').Opener.' + fnc + '(\'' + data + '\');">';
        }

        html += '</div>';
    }

    html += '</div > ';
    html += '<div id="divcanvas" style="padding:5px;width:99%;height:93%;overflow-y:auto;">';
    html += '<canvas id="pdfcanvas"></canvas>';
    html += '</div > ';

    div.html(html);
    //debugger;
    div.appendTo(document.body);

    var divCanvas = div.find('#divcanvas');

    divCanvas.bind('mousewheel DOMMouseScroll', function (event) {
        var scrollTop = divCanvas.scrollTop();
        var scrollBottom = scrollTop + divCanvas.innerHeight();
        var scrollHeight = divCanvas[0].scrollHeight;
        var currentPage = parseInt(div.attr('currentpage')); // 0-based idx
        var pageCount = parseInt(div.attr('pagecount')); // count is 1-based

        if (!pdfjsLib.lastScrollTop) {
            pdfjsLib.lastScrollTop = scrollTop;
        }

        if (event.originalEvent.wheelDelta > 0 || event.originalEvent.detail < 0) {
            // scroll up
            if (scrollTop == 0 && (scrollTop < pdfjsLib.lastScrollTop || scrollBottom == scrollHeight) && currentPage > 0) {
                pdfjsLib.showPreviousPage(true);
                return false;
            }
        }
        else if (event.originalEvent.wheelDelta < 0) {
            // scroll down
            if (((scrollBottom >= scrollHeight && scrollTop > pdfjsLib.lastScrollTop) || scrollBottom == scrollHeight) && currentPage < (pageCount - 1)) {
                pdfjsLib.showNextPage(true);
                return false;
            }
        }

        pdfjsLib.lastScrollTop = scrollTop;
    });

    if (!pdfjsLib.renderPage) {
        pdfjsLib.renderPage = function (page) {
            var div = pdfjsLib.getViewerDiv();
            if (div == null) {
                return;
            }

            var pageCount = parseInt(div.attr('pagecount')); // count is 1-based
            div.attr('currentpage', page.pageIndex); // attr is 0-based

            // Set scale (zoom) level
            if (pdfjsLib.currentScale == null) {
                pdfjsLib.currentZoom = 100;
                pdfjsLib.currentScale = 1.25;
            }

            // Get viewport (dimensions)
            var viewport = page.getViewport(pdfjsLib.currentScale);

            // Get canvas#the-canvas
            var canvas = div.find('#pdfcanvas')[0];

            // Fetch canvas' 2d context
            var context = canvas.getContext('2d');

            // Set dimensions to Canvas
            canvas.height = viewport.height;
            canvas.width = viewport.width;

            // Prepare object needed by render method
            var renderContext = {
                canvasContext: context,
                viewport: viewport
            };

            var arrowLeft = div.find('#arrowleftpdfviewer');
            arrowLeft.css('opacity', page.pageIndex > 0 ? 1.0 : .3);
            arrowLeft.css('cursor', page.pageIndex > 0 ? 'pointer' : 'default');

            var arrowRight = div.find('#arrowrightpdfviewer');
            arrowRight.css('opacity', page.pageIndex < pageCount - 1 ? 1.0 : .3);
            arrowRight.css('cursor', page.pageIndex < pageCount - 1 ? 'pointer' : 'cursor');

            var spanTotalPages = div.find('#spantotalpagespdfviewer');
            spanTotalPages.html(pageCount);

            // Render PDF page
            page.render(renderContext);
        }

        pdfjsLib.zoom = function (zoomPct) {
            var div = pdfjsLib.getViewerDiv();
            if (div == null) {
                return;
            }

            if (zoomPct == 0) {
                pdfjsLib.currentZoom = 100;
                pdfjsLib.currentScale = 1.25;
            }
            else {
                pdfjsLib.currentZoom += zoomPct;

                if (pdfjsLib.currentZoom < 25) {
                    pdfjsLib.currentZoom = 25;
                }

                if (pdfjsLib.currentZoom > 400) {
                    pdfjsLib.currentZoom = 400;
                }

                pdfjsLib.currentScale = 1.25 * (pdfjsLib.currentZoom / 100);
            }

            var currentPage = parseInt(div.attr('currentpage')); // 0-based idx
            div.find('#spanzoompctpdfviewer').html(pdfjsLib.currentZoom + '%');

            pdfjsLib.goToPage(currentPage + 1, true);
        }

        pdfjsLib.goToPage = function (pageNumber, forceReload) { // by default, the page doesn't re-render if it's the same as the current page; force reload overrides the behavior
            if (forceReload == null) forceReload = false;

            var div = pdfjsLib.getViewerDiv();
            if (div == null) {
                return;
            }

            var totalPages = parseInt(div.attr('pagecount'));
            var currentPage = parseInt(div.attr('currentpage')); // 0-based idx

            if (pageNumber == -1) {
                pageNumber = totalPages;
            }
            else if (pageNumber == -2) {
                var pg = div.find('#txtgotopagepdfviewer').val();
                var pgStripped = stripAlpha(pg);

                if (pg != pgStripped) {
                    div.find('#txtgotopagepdfviewer').val(pgStripped);
                    pg = pgStripped;
                }

                if (pg == null || pg.length == 0 || isNaN(pg)) {
                    return;
                }

                pg = parseInt(pg);

                if (pg < 1) {
                    return;
                }
                else if (pg > totalPages) {
                    return;
                }
                else {
                    pageNumber = pg;
                }
            }

            if (pageNumber != (currentPage + 1) || forceReload) { // page number is 1-based since it comes from user input, currentPage is 0-based
                pdfjsLib.PDFDOC.getPage(pageNumber) // the getPage call expects a 1-based index
                    .then(function (page) {
                        pdfjsLib.renderPage(page);
                    });

                div.find('#txtgotopagepdfviewer').val(pageNumber);
                div.find('#divcanvas').scrollTop(0);
                pdfjsLib.lastScrollTop = 0;
            }
        }

        pdfjsLib.showPreviousPage = function (scrollMode) {
            var div = pdfjsLib.getViewerDiv();
            if (div == null) {
                return;
            }

            var totalPages = parseInt(div.attr('pagecount'));
            var currentPage = parseInt(div.attr('currentpage')); // 0-based idx

            if (currentPage > 0) {
                pdfjsLib.PDFDOC.getPage(currentPage) // the getPage call expects a 1-based index, so we are technically doing (currentPage + 1) - 1
                    .then(function (page) {
                        pdfjsLib.renderPage(page);
                    });
                div.find('#txtgotopagepdfviewer').val(currentPage);

                if (scrollMode) {
                    var divCanvas = div.find('#divcanvas');

                    var scrollTop = divCanvas.scrollTop();
                    var scrollBottom = scrollTop + divCanvas.innerHeight();
                    var scrollHeight = divCanvas[0].scrollHeight;

                    divCanvas.scrollTop(scrollHeight - divCanvas.innerHeight());
                    pdfjsLib.lastScrollTop = divCanvas.scrollTop();
                }
                else {
                    div.find('#divcanvas').scrollTop(0);
                    pdfjsLib.lastScrollTop = 0;
                }
            }
        }

        pdfjsLib.showNextPage = function () {
            var div = pdfjsLib.getViewerDiv();
            if (div == null) {
                return;
            }

            var totalPages = parseInt(div.attr('pagecount'));
            var currentPage = parseInt(div.attr('currentpage')); // 0-based idx

            if (currentPage < totalPages - 1) {
                pdfjsLib.PDFDOC.getPage(currentPage + 2) // the getPage call expects a 1-based index, so we are technically doing (currentPage + 1) + 1
                    .then(function (page) {
                        pdfjsLib.renderPage(page);
                    });
                div.find('#txtgotopagepdfviewer').val(currentPage + 2);

                div.find('#divcanvas').scrollTop(0);
                pdfjsLib.lastScrollTop = 0;
            }
        }

        pdfjsLib.getViewerDiv = function () {
            var div = $('#pdfviewer');

            if (div.length == 0) {
                var thePopup = popupManager.GetPopupByName('PDFVIEWER');
                div = thePopup != null ? $('#pdfviewer', thePopup.Body) : null;
            }

            return div;
        }

        pdfjsLib.savePDF = function () {
            var frm = $('#frmDownload');
            if (frm.length == 0) {
                var thePopup = popupManager.GetPopupByName('PDFVIEWER');
                if (thePopup != null) {
                    frm = $('#frmDownload', thePopup.Body);
                }

                if (frm.length == 0 && thePopup != null && thePopup.Opener != null) {
                    frm = $('#frmDownload', thePopup.Opener);
                }
            }

            if (frm.length > 0) {
                frm.attr('src', pdfjsLib.pdfURL);
            }
        }
    }

    // Asynchronous download PDF
    pdfjsLib.getDocument(pdfURL)
        .then(function (pdf) {
            var div = pdfjsLib.getViewerDiv();
            if (div == null) {
                return;
            }

            pdfjsLib.PDFDOC = pdf;

            div.attr('pagecount', pdf.numPages);

            return pdf.getPage(1);
        })
        .then(function (page) {
            pdfjsLib.renderPage(page);
        });

    // show the popup
    var openPopup = popupManager.AddPopupWindow('PDFVIEWER', title, null, height, width, 'PopupWindow', this, false, id);
    if (openPopup) openPopup.Open();

    openPopup.pdfjsLib = pdfjsLib;

    return id;
}


///////////////////////////////////////////////
///// MULTI-LEVEL GRID SUPPORT
///////////////////////////////////////////////


// the initial call to this function should leave parentIframe, x, and y NULL
// popupType can be info, danger, warning, and success
// specify x/y, or specify a pos
function showTopLevelToolTip(html, el, customid, autoClear, popupType, parentIframe, x, y, pos) {
    if (x == null) x = 0;
    if (y == null) y = 0;
    if (popupType == null) popupType = 'info';
    if (autoClear == null) autoClear = true;

    if (el != null) {
        var elpos = $(el).position();

        x += elpos.left;
        y += elpos.top;

        if (_currentLevel > 1) {
            parentIframe = window.frameElement;
        }
    }
    else {
        var frmpos = $(parentIframe).position();

        x += frmpos.left;
        y += frmpos.top;

        parentIframe = window.frameElement;
    }

    if (_currentLevel == 1) {
        y += 15;

        var pos = pos != null ? pos : (x + 'px,' + y + 'px');
        simpleMessage(html, popupType, pos, autoClear, null, customid);
    }
    else {
        if (parent.showTopLevelToolTip) {
            parent.showTopLevelToolTip(html, null, customid, autoClear, popupType, parentIframe, x, y, pos);
        }
    }
}

function hideTopLevelToolTip(customid) {
    if (_currentLevel == 1) {
        $('[id=' + customid + ']').remove(); // we use the [id=] syntax instead of the # syntax so we can clear ALL elements with the custom id rather than just 1
    }
    else {
        parent.hideTopLevelToolTip(customid);
    }
}

function isTopLevelToolTipVisible(customid) {
    if (_currentLevel == 1) {
        return $('[id=' + customid + ']').is(':visible');
    }
    else {
        return parent.isTopLevelToolTipVisible(customid);
    }
}

function getTopLevelGridPageIndex() {
    if (_currentLevel == 1) {
        return _gridPageIndex != null ? _gridPageIndex : 0;
    }
    else {
        return parent.getTopLevelGridPageIndex();
    }
}

function getTopLevelVariable(vname) {
    if (_currentLevel == 1) {
        return this[vname] != null ? this[vname] : null;
    }
    else {
        return parent.getTopLevelVariable(vname);
    }
}

function setTopLevelVariable(vname, val) {
    if (_currentLevel == 1) {
        this[vname] = val;
    }
    else {
        parent.setTopLevelVariable(vname, val);
    }
}






///////////////////////////////////////////////
///// CHART SUPPORT
///////////////////////////////////////////////

// supported options
// opt['displaylegend']
// opt['displayxaxisticks']
// opt['displayyaxisticks']
// opt['maxbarthickness'] // defaults to 50
function createDefaultChartOptions(opt) {
    if (opt == null) opt = {};

    var options = {};
    options.legend = {};
    options.legend.display = (opt['displaylegend'] == null || opt['displaylegend'] == 'true');

    options.scales = {};

    var yAxisScaleOption = {};
    yAxisScaleOption.ticks = {
        beginAtZero: true,
        display: (opt['displayyaxisticks'] == null || opt['displayyaxisticks'] == 'true')
    };
    yAxisScaleOption.maxBarThickness = opt['maxbarthickness'] == null ? '50' : opt['maxbarthickness'];
    options.scales.yAxes = [ yAxisScaleOption ];

    var xAxisScaleOption = {};
    xAxisScaleOption.ticks = {
        beginAtZero: true,
        display: (opt['displayxaxisticks'] == null || opt['displayxaxisticks'] == 'true')
    };
    xAxisScaleOption.maxBarThickness = opt['maxbarthickness'] == null ? '50' : opt['maxbarthickness'];
    options.scales.xAxes = [ xAxisScaleOption ];

    return options;
}

// data set options
// opt['datasetname'] REQ // single string name or array of strings (one per data set)
// opt['values'] REQ // single value array [1,3,3], OR for multiple data sets [ [1,2,3], [4,5,6] ]
// opt['valuelabels'] // label array ['a', 'b', 'c'] OR for multiple data sets [ ['a', 'b', 'c'], ['d', 'e', 'f'] ] // if not supplied, blanks will be inserted
// opt['backgroundcolor'] : (border color uses same format)
//      1: '#abcdef' => one color for all data
//      2: ['#abcdef', '#abcdef'] => colors for each item in a single data set
//      3: [ ['#abcdef'], ['#abcdef'] ] => one color for each data set
//      4: [ ['#abcdef', '#abcdef'], ['#abcdef', '#abcdef'] ] => multiple colors for each data set
// opt['bordercolor'] // single color or an array of colors (one per data set)
//      1: '#abcdef' => one color for all data
//      2: ['#abcdef', '#abcdef'] => colors for each item in a single data set
//      3: [ ['#abcdef'], ['#abcdef'] ] => one color for each data set
//      4: [ ['#abcdef', '#abcdef'], ['#abcdef', '#abcdef'] ] => multiple colors for each data set

function createBarChartDataSet(opt) {
    var data = {}

    var theData = opt['values'];
    if (theData == null || theData.constructor != Array) {
        return;
    }

    var firstValue = theData[0];
    var multipleDataSets = firstValue.constructor == Array; // if true, we have an array of arrays, meaning multiple data sets

    backgroundColor = opt['backgroundcolor'] == null ? 'rgba(255, 99, 132, 0.2)' : opt['backgroundcolor'];
    borderColor = opt['bordercolor'] == null ? 'rgba(255, 99, 132, 1.0)' : opt['bordercolor'];

    data.labels = opt['valuelabels'];

    data.datasets = [];

    if (!multipleDataSets) {
        theData = [theData]; // create a multi-data set so we can loop easier
    }

    for (var i = 0; i < theData.length; i++) {
        var dataset = {};

        var dsn = opt['datasetname'];
        if (dsn.constructor == String) {
            dataset.label = dsn;
        }
        else if (dsn.constructor == Array) {
            dataset.label = dsn[i];
        }

        dataset.data = theData[i];

        dataset.backgroundColor = [];
        dataset.borderColor = [];

        for (var x = 0; x < dataset.data.length; x++) {
            if (backgroundColor.constructor == String) { // 1: one color for everything
                dataset.backgroundColor.push(backgroundColor);
            }
            else if (backgroundColor.constructor == Array) {
                firstValue = backgroundColor[0];

                if (firstValue.constructor == String) { // 2: array of string colors for each value in the data set
                    dataset.backgroundColor.push(backgroundColor[x]);
                }
                else if (firstValue.constructor == Array) {
                    firstValue = firstValue[i];

                    if (firstValue.constructor == String) { // 3
                        dataset.backgroundColor.push(firstValue);
                    }
                    else if (firstValue.constructor == Array) { // 4
                        dataset.backgroundColor.push(firstValue[x]);
                    }
                }
            }

            if (borderColor.constructor == String) { // 1: one color for everything
                dataset.borderColor.push(borderColor);
            }
            else if (borderColor.constructor == Array) {
                firstValue = borderColor[0];

                if (firstValue.constructor == String) { // 2: array of string colors for each value in the data set
                    dataset.borderColor.push(borderColor[x]);
                }
                else if (firstValue.constructor == Array) {
                    firstValue = firstValue[i];

                    if (firstValue.constructor == String) { // 3
                        dataset.borderColor.push(firstValue);
                    }
                    else if (firstValue.constructor == Array) { // 4
                        dataset.borderColor.push(firstValue[x]);
                    }
                }
            }
        }

        if (data.labels == null || data.labels.length == 0) {
            data.labels = [];
            for (var x = 0; x < dataset.data.length; x++) {
                data.labels.push('');
            }
        }

        dataset.borderWidth = 1;

        data.datasets.push(dataset);
    }

    return data;
}

function addChartToCanvas(
    canvasID,
    chartType,
    data,
    options)
{
    if (data == null) {
        return;
    }

    var cnv = $('#' + canvasID);
    if (cnv.length == 0) {
        return;
    }

    var ctx = cnv[0].getContext('2d');

    if (options == null) {
        options = createDefaultChartOptions();
    }

    var theChart = new Chart(ctx, {
        type: chartType,
        data: data,
        options: options
    });
}

///////////////////////////////////////////////
///// RQMT SUPPORT - RQMTS SEEM TO BE SPREADING OUTSIDE OF THEIR MAINTENANCE CONTAINER, SO WE ARE ADDING A FEW GLOBAL RQMT FUNCTIONS HERE
///////////////////////////////////////////////

function openRQMTPopup(RQMTID, openSections, RQMTSetID, ParentRQMTID, closeFn, hideNonOpenSections, itemID, itemSubSection) {
    if (openSections == null || openSections.length == 0) openSections = '';

    var nURL = 'RQMT_Edit.aspx?RQMTID=' + RQMTID + '&RQMTSetID=' + (RQMTSetID != null ? RQMTSetID : 0) + '&ParentRQMTID=' + (ParentRQMTID != null ? ParentRQMTID : 0);
    nURL = editQueryStringValue(nURL, 'rnd', (new Date()).getTime());
    nURL = editQueryStringValue(nURL, 'OpenSections', openSections);

    if (hideNonOpenSections) {
        nURL = editQueryStringValue(nURL, 'HideNonOpenSections', '1');
    }

    if (itemID) {
        nURL = editQueryStringValue(nURL, 'ItemID', itemID);
    }

    if (itemSubSection != null) {
        nURL = editQueryStringValue(nURL, 'ItemSubSection', itemSubSection);
    }

    var title = RQMTID > 0 ? 'RQMT EDIT - #' + RQMTID : 'RQMT EDIT - NEW RQMT';

    var openPopup = popupManager.AddPopupWindow('RQMTEdit', title, 'Loading.aspx?Page=' + nURL, 700, 1250, 'PopupWindow', this, false);
    if (closeFn) {
        openPopup.onClose = closeFn;
    }
    if (openPopup) openPopup.Open();
}