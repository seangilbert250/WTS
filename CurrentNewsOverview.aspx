<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CurrentNewsOverview.aspx.cs" Inherits="CurrentNewsOverview" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript" src="Scripts/Chart.min.js"></script>
    <script type="text/javascript" src="Scripts/shell.js"></script>
    <script type="text/javascript" src="Scripts/jquery-1.11.2.min.js"></script>
    <script type="text/javascript" src="Scripts/jquery-ui.min.js"></script>
    <script type="text/javascript" src="Scripts/common.js"></script>
    <script type="text/javascript" src="Scripts/pdf.js"></script>
    <script type="text/javascript" src="Scripts/pdf.worker.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        <%--<div>
            <canvas id="canvas"></canvas>
        </div>--%>
        <div>
            Loaded

        </div>

        <div id="pdfviewer" style="border-bottom: 1px solid #000000; padding: 3px;">
            <img id="arrowleftallpdfviewer" src="images/icons/arrowleftallgrey.png" style="cursor: pointer; position: relative; top: 2px;" onclick="pdfjsLib.goToPage(1)" alt="First Page" title="First Page" />
            <img id="arrowleftpdfviewer" src="images/icons/arrowleftgrey.png" style="cursor: pointer; opacity: .3; position: relative; top: 2px;" onclick="pdfjsLib.showPreviousPage(false)" alt="Previous Page" title="Previous Page" />
            <img id="arrowrightpdfviewer" src="images/icons/arrowrightgrey.png" style="cursor: pointer; opacity: .3; position: relative; top: 2px;" onclick="pdfjsLib.showNextPage()" alt="Next Page" title="Next Page" />
            <img id="arrowrightallpdfviewer" src="images/icons/arrowrightallgrey.png" style="cursor: pointer; position: relative; top: 2px;" onclick="pdfjsLib.goToPage(-1)" alt="Last Page" title="Last Page" />
            <input id="txtgotopagepdfviewer" type="text" value="1" style="text-align: center; width: 25px; background-color: white; position: relative;" maxlength="3" onkeyup="pdfjsLib.goToPage(-2)" />
            of <span id="spantotalpagespdfviewer">1</span>
            <img id="zoomoutpdfviewer" src="images/icons/zoom_out.png" style="cursor: pointer; width: 16px; height: 16px; position: relative; top: 2px;" onclick="pdfjsLib.zoom(-25)" alt="Zoom Out" title="Zoom Out" />
            <img id="zoominpdfviewer" src="images/icons/zoom_in.png" style="cursor: pointer; width: 16px; height: 16px; position: relative; top: 2px;" onclick="pdfjsLib.zoom(25)" alt="Zoom In" title="Zoom In" />
            <img id="zoomresetpdfviewer" src="images/icons/zoom_reset.png" style="cursor: pointer; width: 16px; height: 16px; position: relative; top: 2px;" onclick="pdfjsLib.zoom(0)" alt="Reset Zoom" title="Reset Zoom" />
            <span id="spanzoompctpdfviewer">100%</span>
            <img id="savepdfviewer" src="images/icons/disk.png" style="cursor: pointer; width: 16px; height: 16px; position: relative; top: 3px;" onclick="pdfjsLib.savePDF()" alt="Save PDF" title="Save PDF" />
            <div id="divcanvas" style="padding: 5px; width: 99%; height: 93%; overflow-y: auto;">
                <canvas id="pdfcanvas" />
            </div>
        </div>
        

        <script type="text/javascript">
            //debugger;
            //function setUpPDFView() {
            //    try {
            //        if (!window.requestAnimationFrame) {
            //            window.requestAnimationFrame = (function () {
            //                return window.webkitRequestAnimationFrame ||
            //                    window.mozRequestAnimationFrame ||
            //                    window.oRequestAnimationFrame ||
            //                    window.msRequestAnimationFrame ||
            //                    function (callback, element) {
            //                        window.setTimeout(callback, 1000 / 60);
            //                    };
            //            })();
            //        }

            //        document.addEventListener('tizenhwkey', function (e) {
            //            if (e.keyName === 'back') {
            //                try {
            //                    tizen.application.getCurrentApplication().exit();
            //                } catch (error) { }
            //            }
            //        });

            //        var canvas = document.getElementById('canvas');
            //        var context = canvas.getContext('2d');
            //        var pageElement = document.getElementById('page');

            //        var reachedEdge = false;
            //        var touchStart = null;
            //        var touchDown = false;

            //        var lastTouchTime = 0;
            //        pageElement.addEventListener('touchstart', function (e) {
            //            touchDown = true;

            //            if (e.timeStamp - lastTouchTime < 500) {
            //                lastTouchTime = 0;
            //                toggleZoom();
            //            } else {
            //                lastTouchTime = e.timeStamp;
            //            }
            //        });

            //        pageElement.addEventListener('touchmove', function (e) {
            //            if (pageElement.scrollLeft === 0 ||
            //                pageElement.scrollLeft === pageElement.scrollWidth - page.clientWidth) {
            //                reachedEdge = true;
            //            } else {
            //                reachedEdge = false;
            //                touchStart = null;
            //            }

            //            if (reachedEdge && touchDown) {
            //                if (touchStart === null) {
            //                    touchStart = e.changedTouches[0].clientX;
            //                } else {
            //                    var distance = e.changedTouches[0].clientX - touchStart;
            //                    if (distance < -100) {
            //                        touchStart = null;
            //                        reachedEdge = false;
            //                        touchDown = false;
            //                        openNextPage();
            //                    } else if (distance > 100) {
            //                        touchStart = null;
            //                        reachedEdge = false;
            //                        touchDown = false;
            //                        openPrevPage();
            //                    }
            //                }
            //            }
            //        });

            //        pageElement.addEventListener('touchend', function (e) {
            //            touchStart = null;
            //            touchDown = false;
            //        });

            //        var pdfFile;
            //        var currPageNumber = 1;

            //        var openNextPage = function () {
            //            var pageNumber = Math.min(pdfFile.numPages, currPageNumber + 1);
            //            if (pageNumber !== currPageNumber) {
            //                currPageNumber = pageNumber;
            //                openPage(pdfFile, currPageNumber);
            //            }
            //        };

            //        var openPrevPage = function () {
            //            var pageNumber = Math.max(1, currPageNumber - 1);
            //            if (pageNumber !== currPageNumber) {
            //                currPageNumber = pageNumber;
            //                openPage(pdfFile, currPageNumber);
            //            }
            //        };

            //        var zoomed = false;
            //        var toggleZoom = function () {
            //            zoomed = !zoomed;
            //            openPage(pdfFile, currPageNumber);
            //        };

            //        var fitScale = 1;
            //        var openPage = function (pdfFile, pageNumber) {
            //            var scale = zoomed ? fitScale : 1;

            //            pdfFile.getPage(pageNumber).then(function (page) {
            //                viewport = page.getViewport(1);

            //                if (zoomed) {
            //                    var scale = pageElement.clientWidth / viewport.width;
            //                    viewport = page.getViewport(scale);
            //                }

            //                canvas.height = viewport.height;
            //                canvas.width = viewport.width;

            //                var renderContext = {
            //                    canvasContext: context,
            //                    viewport: viewport
            //                };

            //                page.render(renderContext);
            //            });
            //        };

            //        PDFJS.disableStream = true;
            //        PDFJS.getDocument('../FHP Weekly Developer Meeting 9102018 1200 PM.pdf').then(function (pdf) {
            //            pdfFile = pdf;

            //            openPage(pdf, currPageNumber, 1);
            //        });
            //    }
            //    catch (e) {

            //        var a = e;
            //        debugger;
            //    }

            //}

            function foo() {

                try {

                    var div = $('#pdfviewer');
                    var divCanvas = div.find('#divcanvas');
                    var pdfURL = 'Download_Attachment.aspx?attachmentID=14929';

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

                            /*if (div.length == 0) {
                                var thePopup = popupManager.GetPopupByName('PDFVIEWER');
                                div = thePopup != null ? $('#pdfviewer', thePopup.Body) : null;
                            }*/

                            return div;
                        }

                        pdfjsLib.savePDF = function () {
                            alert('TODO');
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
                            //debugger;
                            var div = pdfjsLib.getViewerDiv();
                            if (div == null) {
                                return;
                            }

                            pdfjsLib.PDFDOC = pdf;

                            div.attr('pagecount', pdf.numPages);

                            return pdf.getPage(1);
                        })
                        .then(function (page) {
                            //debugger;
                            pdfjsLib.renderPage(page);
                        });



                }
                catch (e) {

                    var a = e;
                    debugger;
                }


            }




            $(document).ready(function () {

                try {
                    debugger;
                    this.pdfjsLib = pdfjsLib;
                    pdfjsLib.pdfURL = 'Download_Attachment.aspx?attachmentID=14929';
                    pdfjsLib.currentZoom = 100;
                    pdfjsLib.currentScale = 1.25;
                    //setUpPDFView();
                    foo();
                    debugger;
                }
                catch (e) {

                    var a = e;
                    //debugger;
                }

            });
        </script>
    </form>
</body>
</html>
