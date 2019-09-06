<%@ Page Title="" Language="C#" MasterPageFile="~/Grids.master" AutoEventWireup="true" CodeFile="WorkItem_Comments.aspx.cs" Inherits="WorkItem_Comments" Theme="Default" %>

<asp:Content ID="cpHeadTitle" ContentPlaceHolderID="headTitle" runat="Server">Comments</asp:Content>
<asp:Content ID="cpHead" ContentPlaceHolderID="head" runat="Server">
	<link rel="stylesheet" href="Scripts/cleditor/jquery.cleditor.css" />
</asp:Content>
<asp:Content ID="cpHeaderText" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="Server">Comments</asp:Content>
<asp:Content ID="cpGridSetting" ContentPlaceHolderID="cphGridSettings" runat="Server">
	<span id="labelMessage" style="color: red; padding-left: 15px; display:none;"></span>
</asp:Content>
<asp:Content ID="cpPageContentInfo" ContentPlaceHolderID="cphPageContentInfo" runat="Server">
	<table cellpadding="0" cellspacing="0" style="float: right;">
		<tr>
			<td style="padding-right:3px;">
				<input type="button" id="btnAddComment" value="Add Comment" />
				<input type="button" id="btnSaveComments" value="Save" />
			</td>
		</tr>
	</table>
</asp:Content>
<asp:Content ID="cpBody" ContentPlaceHolderID="ContentPlaceHolderBody" runat="Server">
	<div id="divComments" style="width: 99%; padding: 5px; overflow: auto;">
	</div>
	<div id="templateContainer" style="display: none;">
		<div class="divCommentTemp" style="width: 100%; padding-bottom: 5px;">
			<table style="width: 100%; border: 1px solid grey;" cellpadding="0" cellspacing="0">
				<tr class="pageContentHeader">
					<td style="padding-left:5px;">
						<span id="spanCommentBy1"></span>
					</td>
					<td style="text-align: right; padding-right:2px;">
						<input type="button" class="Edit" value="Edit" style="display:none;" />
						<input type="button" class="Delete" value="Delete" />
						<input type="button" class="Reply" value="Reply" style="display:none;" />
					</td>
				</tr>
				<tr>
					<td class="pageContentInfo" colspan="2" style="padding: 3px 1px 3px 5px;">
						<%--<asp:TextBox ID="txtComment1" runat="server" TextMode="MultiLine" Height="125px" Width="99%"></asp:TextBox>--%>
						<textarea id="txtComment1" rows="10" style="width:99%;"></textarea>
					</td>
				</tr>
			</table>
		</div>
	</div>
	<script src="Scripts/cleditor/jquery.cleditor.js"></script>

	<asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>
	
	<script id="jsEditor" type="text/javascript">

		function addHtmlEditor(control) {
			var w = $(control).width();
			var h = 150;
			if (($(control).height() + 30) > 150) {
				h = $(control).height() + 30;
			};

			$(control).cleditor({ width: w, height: h });

			//$(control).cleditor()[0].change(function () { txt_change($(control)); });
			$($(control).cleditor()[0].doc).css('height', (h - 28) + 'px');
			$($(control).cleditor()[0].doc.body).css('height', (h - 35) + 'px');
			$($(control).cleditor()[0].doc.body).css('background-color', '#F5F6CE');

			return control;
		}

	</script>

	<script type="text/javascript">
		var _pageUrls;
		var _workItemID = 0;
		var _isAdmin = false;
		var username = "<%=HttpContext.Current.User.Identity.Name%>";

		function refresh(saved) {
			var url = document.location.href;
			if (!saved) {
				url = editQueryStringValue(url, 'Saved', '0');
			}
			else {
				url = editQueryStringValue(url, 'Saved', '1');
			}
        	document.location.href = 'Loading.aspx?Page=' + url;
        }

        function clearMessage() {
        	$('#labelMessage').text('');
        	$('#labelMessage').hide();
        }

        function exportToExcel() {
        	clearMessage();

        	var nLocation = 'WorkItem_Comments.aspx' + window.location.search;
        	window.open('Loading.aspx?Page=' + editQueryStringValue(nLocation, 'excel', '1'));
        }

        function SaveComments() {
        	clearMessage();

        	var newComments = $('#divComments').find("[COMMENTID='']");
        	var changedComments = [];

			//new comments
        	$.each($('#divComments').find("[COMMENTID='']"), function (Commentindex, Comment) {
        		var changedComment = [];
        		var $currentComment = $(Comment);
        		var currentCommentText = $currentComment.find('textarea').val();
        		if (currentCommentText != '') {
        			changedComment.push(''); //id
        			changedComment.push($currentComment.attr('PARENTID')); //parentid
        			changedComment.push(currentCommentText); //text
        			changedComments.push(changedComment);
        		}
        	});

        	//changed comments
        	if (arrComments != null && arrComments.length > 0
				&& arrComments[0] != null) {
        		$.each(arrComments[0], function (rowindex, row) {
        			var changedComment = [];
        			var $currentComment = $('#divComments').find("[COMMENTID='" + row.COMMENTID + "']");
        			var currentCommentText = $currentComment.find('textarea').val();
        			if (row.COMMENT_TEXT != currentCommentText) {
        				changedComment.push(row.COMMENTID); //id
        				changedComment.push(''); //parentid
        				changedComment.push(currentCommentText); //text
        				changedComments.push(changedComment);
        			}
        		});
        	}

        	PageMethods.SaveComments(_workItemID, changedComments, SaveComments_Done, OnError);
        }
        function SaveComments_Done(result) {
        	var saved = false;
        	var ids = '', errorMsg = '';

        	try {
        		var obj = jQuery.parseJSON(result);

        		if (obj) {
        			if (obj.saved && obj.saved.toUpperCase() == 'TRUE') {
        				saved = true;
        			}
        			if (obj.ids) {
        				ids = obj.ids;
        			}
        			if (obj.error) {
        				errorMsg = obj.error;
        			}
        		}

        		if (saved) {
        			$('#labelMessage').text('Comment(s) have been saved.');
        			$('#labelMessage').show();

        			refresh(true);
        		}
        		else {
        			MessageBox('Failed to save comment(s). \n' + errorMsg);
        		}

        	} catch (e) {

        	}
        }

        function highlightComments(arrComments) {
        	clearMessage();

			$.each(arrComments, function (rowindex, COMMENTID) {
				$('#divComments').find("[COMMENTID='" + COMMENTID + "']").find('textarea').css('background-color', 'yellow');
			});
		}

        function Delete(COMMENTID) {
        	clearMessage();

			var $deletedComment = $('#divComments').find("[COMMENTID='" + COMMENTID + "']");
			var $nextComment = $deletedComment.next();
			var DeletedCommentLevel = parseInt($deletedComment.attr('LEVEL'));
			var NextCommentLevel = parseInt($nextComment.attr('LEVEL'));
			var arrCOMMENTIDsToDelete = [];

			arrCOMMENTIDsToDelete.push($deletedComment.attr('COMMENTID'));

			while (DeletedCommentLevel < NextCommentLevel) {
				arrCOMMENTIDsToDelete.push($nextComment.attr('COMMENTID'));
				$nextComment = $nextComment.next();
				NextCommentLevel = parseInt($nextComment.attr('LEVEL'));
			}

			highlightComments(arrCOMMENTIDsToDelete);

			if (arrCOMMENTIDsToDelete.length == 1) {
				confirmMsg = 'Are you sure you want to delete this Comment?';
			}
			else {
				MessageBox('This Comment has replies, you are not allowed to delete it.');
				return false;
			}

			if (confirm(confirmMsg)) {
				arrCOMMENTIDsToDelete.reverse();
				PageMethods.DeleteComments(_workItemID, arrCOMMENTIDsToDelete, Delete_Done, OnError);
			} else {
				$('#divComments').find('textarea').css('background-color', '');
			}
		}
		function Delete_Done(result) {
			var deleted = false;
			var ids = '', errorMsg = '';

			try {
				var obj = jQuery.parseJSON(result);

				if (obj) {
					if (obj.deleted && obj.deleted.toUpperCase() == 'TRUE') {
						deleted = true;
					}
					if (obj.ids) {
						ids = obj.ids;
					}
					if (obj.error) {
						errorMsg = obj.error;
					}
				}

				if (deleted) {
					$('#labelMessage').text('Comment(s) have been deleted.');
					$('#labelMessage').show();

					refresh(true);
				}
				else {
					MessageBox('Failed to delete comment(s). \n' + errorMsg);
				}

			} catch (e) {

			}
		}

		function OnError(result) {
			var resultText = 'An error occurred when communicating with the server';/*\n' +
                    'readyState = ' + result.readyState + '\n' +
                    'responseText = ' + result.responseText + '\n' +
                    'status = ' + result.status + '\n' +
                    'statusText = ' + result.statusText;*/

			MessageBox('save error:  \n' + resultText);
		}

		function AddComment() {
			clearMessage();

			var $Temp = $('#templateContainer .divCommentTemp').clone()
			var $txt = $Temp.find('textarea');

			$Temp.attr('COMMENTID', '');
			$Temp.attr('PARENTID', '');
			$Temp.find('span').text(today() + ' ' + username);
			$txt.val('');

			$Temp.find('textarea').focus(function () {
				addHtmlEditor($Temp.find('textarea'));
				clearMessage();
			});
            $Temp.find('.Reply').click(function () { AddReply(-1, $Temp); });
            $Temp.find('.Reply').prop('disabled', true);
            $Temp.find('.Delete').click(function () {
            	var $divComment = $(this).closest('div');
            	$(this).closest('div').remove();
            });
            $('#divComments').prepend($Temp);

            $Temp.find('textarea').focus();

            document.getElementById('divComments').style.height = document.getElementById('divComments').offsetHeight + $Temp.find('textarea')[0].offsetHeight + 20 + 'px';

            //resizeComments();
		    resizeFrame();

		}
		function today() {
			var today = new Date();
			var dd = today.getDate();
			var mm = today.getMonth() + 1; //January is 0!
			var yyyy = today.getFullYear();
			if (dd < 10) {
				dd = '0' + dd
			}
			if (mm < 10) {
				mm = '0' + mm
			}
			return mm + '/' + dd + '/' + yyyy;
		}
		function AddReply(COMMENTID, obj) {
			clearMessage();

			var $pComment = $(obj);
			//Use id of container div and class of the actual div being cloned to avoid duplicate ids
			var $Temp = $('#templateContainer .divCommentTemp').clone().uniqueId();
			$Temp.attr('COMMENTID', '');
			$Temp.attr('PARENTID', COMMENTID);
			$Temp.find('span').text(today() + ' ' + username);
			var $txt = $Temp.find('textarea');
			$Temp.find('textarea').text('');

            $Temp.find('textarea').focus(function () {
            	addHtmlEditor($Temp.find('textarea'));
            	clearMessage();
            });
            $Temp.find('.Reply').click(function () { AddReply(-1, $Temp); });
            $Temp.find('.Delete').click(function () {
            	var $divComment = $(this).closest('div');
            	$(this).closest('div').remove();
            });

            var w = $pComment.width() - 25;
            $Temp.width(w);

            $Temp.css('float', 'right');
            $Temp.find('.Reply').prop('disabled', true);
            $pComment.find('.Reply').prop('disabled', true);
            $pComment.append($Temp);

            $Temp.find('textarea').focus();

            resizeFrame();
		}

		function BuildComments() {
			if (arrComments == null || arrComments.length == 0 || arrComments[0] == null) {
				return;
			}

			var commentType = 'Comment';
			$.each(arrComments[0], function (rowindex, row) {
				var $Temp = $('#templateContainer .divCommentTemp').clone();
				$Temp.attr('COMMENTID', row.COMMENTID);
				$Temp.attr('PARENTID', row.PARENTID);
				$Temp.attr('LEVEL', row.LVL);

				commentType = (row.PARENTID == null || row.PARENTID == 0) ? 'Comment' : 'Reply';

				$Temp.find('span').text(commentType + ' By ' + row.CREATEDBY + ' on ' + row.CREATEDDATE);
				$Temp.find('textarea').html(row.COMMENT_TEXT);
				$Temp.find('.Reply').click(function () { AddReply(row.COMMENTID, $Temp); });

				$Temp.find('.Delete').click(function () {
					Delete($Temp.attr('COMMENTID'))
				});

				if (_isAdmin || username == row.CREATEDBY) {
                	var nextComment = arrComments[0][rowindex + 1];
                	if (nextComment == undefined) {
                		$Temp.find('.Delete').show();
                		$Temp.find('.Edit').hide();
                	} else {
                		if (arrComments[0][rowindex + 1].LVL <= row.LVL) {
                			$Temp.find('.Delete').show();
                			//$Temp.find('textarea').prop('disabled', true);
                		} else {
                			$Temp.find('.Delete').hide();
                			//$Temp.find('textarea').prop('disabled', true);
                		}
                	}
                } else {
                	$Temp.find('.Delete').hide();
                	$Temp.find('.Edit').hide();
                	$Temp.find('textarea').prop('disabled', true);
                }
                $('#divComments').append($Temp);

                if (row.PARENTID != null) {
                	var lastCommentAdded = $('#divComments').find("[COMMENTID='" + row.COMMENTID + "']");
                	var w = 100 - (row.LVL * 2.5);
                	lastCommentAdded.css('width', w + '%');
                	lastCommentAdded.css('float', 'right');
                }

			});

			$('#divComments').find('textarea').each(function () {
				addHtmlEditor($(this));
				clearMessage();
			});

			resizeComments();
		}

		$(document).ready(function () {
			_pageUrls = new PageURLs();
			_workItemID = +'<%=this.WorkItemID %>';
			if ('<%=this.IsAdmin.ToString().ToUpper() %>' == 'TRUE') {
				_isAdmin = true;
			}
			$(window).resize(resizeComments);

			$('#pageContentHeader').hide();
			$('#imgReport').hide();
			$('#imgSort').hide();
			$('#imgExport').hide();//.click(function () { exportToExcel(); });
			$('#imgRefresh').click(function () { refresh(); });
			$('#btnAddComment').click(function (event) { AddComment(); });
			$('#btnSaveComments').click(function (event) { SaveComments(); });

			if (typeof arrComments !== 'undefined' && arrComments.length > 0) {
				BuildComments();
			}
			else {
				$('#divComments').text('No Comments have been saved.');
			}

			var qty = +'<%=this.Comment_Count %>';
			if (parent.SetCommentQty) {
				parent.SetCommentQty(qty);
			}

			if ('<%=this.SaveComplete %>' == '1') {
				$('#labelMessage').text('Comments have been saved.');
				$('#labelMessage').show();
			}
			else {
				clearMessage();
			}

			resizeFrame();
		});
		function resizeComments() {
			resizePageElement('divComments', 0);

		}
		function resizeFrame() {
			var frame = getMyFrameFromParent();
			resizeFrameFromContents(frame.id);
		}

	</script>
</asp:Content>

