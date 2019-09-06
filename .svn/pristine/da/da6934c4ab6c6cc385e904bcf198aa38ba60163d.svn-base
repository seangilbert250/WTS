//Javascript -- PopupWindow

//Get the frame that contains this page
var parentFrame = null;
var popupManager = parent.popupManager;
var pageWindowName = null;
var myPopupWindow = null;
var parentPopup = null;

var arrFrames = parent.document.getElementsByTagName("IFRAME");
for (var i = 0; i <= arrFrames.length-1; i++) {
    if (arrFrames[i].contentWindow == window){
        parentFrame = arrFrames[i];
        if (!popupManager) {
            popupManager = arrFrames[i].popupManager;
        }
        pageWindowName = arrFrames[i].pageWindowName;
        parentPopup = arrFrames[i].parentPopup;
        if (popupManager && pageWindowName) {
            myPopupWindow = popupManager.GetPopupByName(pageWindowName);
        }
        break;
    }
}

if (parentFrame) {
    window.opener = parentFrame.opener;
}

if (popupManager){
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

function closeWindow(popupWindowName) {
    if (pageWindowName == null) {
        pageWindowName = popupWindowName;
    }
    popupManager.RemovePopupWindow(pageWindowName);
    //if (popupManager) {
    //    for (var i = 0; i <= popupManager.PopupCount - 1; i++) {
    //        if (!popupManager.PopupWindows[i].isOpen) {
    //            popupManager.RemovePopupWindow(popupManager.PopupWindows[i].Name);
    //        }
    //    }
    //}
}

function cancelCurrentPopup() {
    try {
        if (window.event) {
            var key = window.event.keyCode;
            if (window.event.keyCode == 27) {
                // Escape
                QuestionBox('Close Popup', 'Are you sure you want to close the current popup window?', 'No,Yes', 'PopupWindowEscape', 200, 200, window.self, popupManager.ActivePopup.Name);
            }
        }
    }
    catch (e) {
    }
}

function closeCurrentPopup(answer) {
    try {
        if (answer == 'Yes') {
            if (popupManager) {
                var currentPopup = popupManager.PopupWindows[popupManager.PopupWindows.length - 2];
                if (currentPopup) {
                    currentPopup.Close();
                }
            }
        }
    }
    catch (e) {
    }
}

addEventHandler(document, 'keyup', cancelCurrentPopup);
