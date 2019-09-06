<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SimpleTreeViewControl.ascx.cs" Inherits="Controls_SimpleTreeViewControl" %>
<div id="divTreeViewContainer" runat="server">   
</div>
<script type="text/javascript">
    function <%=ClientID%>_GetContainer() {
        return $('#<%=ClientID%>_divTreeViewContainer');
    }

    function <%=ClientID%>_ToggleAllNodes(open) {
        if (open == undefined) return;

        var container = <%=ClientID%>_GetContainer();

        var imgs = container.find('img[selectorimage=1]');
        var childContainers = container.find('[id$=_ChildNodeContainer]');

        if (open) {
            $.each(imgs, function () {
                $(this).attr('src', $(this).attr('src').replace('expand.gif', 'collapse.gif'));
                var txt = $(this).attr('collapsealt');
                $(this).attr('alt', txt);
                $(this).attr('title', txt);
            });

            $.each(childContainers, function () {
                $(this).show();
            });
        }
        else {
            $.each(imgs, function () {
                $(this).attr('src', $(this).attr('src').replace('collapse.gif', 'expand.gif'));
                var txt = $(this).attr('expandalt');
                $(this).attr('alt', txt);
                $(this).attr('title', txt);
            });

            $.each(childContainers, function () {
                $(this).hide();
            });
        }
    }

    function <%=ClientID%>_ToggleNode(level, idx, key, open) {
        var container = <%=ClientID%>_GetContainer();
        var img = container.find('img[selectorimage=1][level=' + level + '][idx=' + idx + '][key=' + key + ']');

        if (img.length > 0) {
            var childContainer = container.find('[id$=_ChildNodeContainer][level=' + level + '][idx=' + idx + '][key=' + key + ']');

            if (childContainer.length > 0) {
                var isOpen = $(childContainer).is(':visible');

                var openNode = !isOpen;

                if (open != null) {
                    openNode = open;
                }

                if (openNode) {
                    if (!isOpen) {
                        $(img).attr('src', $(img).attr('src').replace('expand.gif', 'collapse.gif'));
                        var txt = $(img).attr('collapsealt');
                        $(img).attr('alt', txt);
                        $(img).attr('title', txt);

                        $(childContainer).show();
                    }

                    return true;
                }
                else {
                    if (isOpen) {
                        $(img).attr('src', $(img).attr('src').replace('collapse.gif', 'expand.gif'));
                        var txt = $(img).attr('expandalt');
                        $(img).attr('alt', txt);
                        $(img).attr('title', txt);

                        $(childContainer).hide();
                    }

                    return false;
                }
            }
        }

        return null;
    }

    function <%=ClientID%>_SetNodeColor(level, idx, key, textColor, backgroundColor, clearAll) {
        var container = <%=ClientID%>_GetContainer();

        if (clearAll) {
            var allNodes = container.find('div[selectortext=1]');

            for (var i = 0; i < allNodes.length; i++) {
                var node = allNodes[i];

                var defaultColor = $(node).closest('[id$=_NodeContainer]').attr('defaultcolor');
                if (defaultColor != null && defaultColor.length > 0) {
                    $(node).css('color', defaultColor);
                }
                else {
                    $(node).css('color', '');
                }

                var defaultBackgroundColor = $(node).closest('[id$=_NodeContainer]').attr('defaultbackgroundcolor');
                if (defaultBackgroundColor != null && defaultBackgroundColor.length > 0) {
                    $(node).css('background-color', defaultBackgroundColor);
                }
                else {
                    $(node).css('background-color', '');
                }
            }
        }

        var txt = container.find('div[selectortext=1][level=' + level + '][idx=' + idx + '][key=' + key + ']');

        if (txt.length > 0) {
            if (textColor != null && textColor.length > 0) {
                $(txt).css('color', textColor);
            }

            if (backgroundColor != null && backgroundColor.length > 0) {
                $(txt).css('background-color', backgroundColor);
            }
        }
    }

    function <%=ClientID%>_GetNodeByKey(key, level, idx) {
        var container = <%=ClientID%>_GetContainer();

        var selector = '[id$=_TreeViewNode_NodeContainer][key=' + key + ']';

        if (level != null) selector += '[level=' + level + ']';
        if (idx != null) selector += '[idx=' + idx + ']';

        return container.find(selector);
    }


    function <%=ClientID%>_GetNodeByAttribute(name, value) {
        var container = <%=ClientID%>_GetContainer();

        var selector = '[id$=_TreeViewNode_NodeContainer][' + name + '=' + value + ']';

        return container.find(selector);        
    }

    function <%=ClientID%>_NodeHasChildren(node, key, level, idx) {
        if (node == null) {
            node = <%=ClientID%>_GetNodeByKey(key, level, idx);
        }

        if (node != null) {
            var childContainer = $(node).find('[id$=_ChildNodeContainer]');

            return childContainer.length > 0;
        }
        else {
            return false;
        }
    }

    function <%=ClientID%>_GetChildNodes(node, key, level, idx) {
        if (node == null) {
            node = <%=ClientID%>_GetNodeByKey(key, level, idx);
        }

        if (node != null) {
            var childContainer = $(node).find('[id$=_ChildNodeContainer]');

            if (childContainer.length > 0) {
                return childContainer.find('[id$=_NodeContainer]');
            }
            else {
                return null;
            }
        }
        else {
            return null;
        }
    }

    function <%=ClientID%>_OpenNodeAndAllParents(level, idx, key) {
        var container = <%=ClientID%>_GetContainer();

        var img = container.find('img[selectorimage=1][level=' + level + '][idx=' + idx + '][key=' + key + ']');

        if (img.length > 0) {
            var childContainer = container.find('[id$=_ChildNodeContainer][level=' + level + '][idx=' + idx + '][key=' + key + ']');

            if (childContainer.length > 0) {
                $(img).attr('src', $(img).attr('src').replace('expand.gif', 'collapse.gif'));
                var txt = $(img).attr('collapsealt');
                $(img).attr('alt', txt);
                $(img).attr('title', txt);

                $(childContainer).show();
            }

            var parentChildContainer = $(img).closest('[id$=_ChildNodeContainer');
            if (parentChildContainer.length > 0) {
                var plevel = $(parentChildContainer).attr('level');
                var pidx = $(parentChildContainer).attr('idx');
                var pkey = $(parentChildContainer).attr('key');

                <%=ClientID%>_OpenNodeAndAllParents(plevel, pidx, pkey);
            }
        }
        else {
            var txt = container.find('div[selectortext=1][level=' + level + '][idx=' + idx + '][key=' + key + ']');

            if (txt.length > 0) {
                var parentChildContainer = $(txt).closest('[id$=_ChildNodeContainer');
                if (parentChildContainer.length > 0) {
                    var plevel = $(parentChildContainer).attr('level');
                    var pidx = $(parentChildContainer).attr('idx');
                    var pkey = $(parentChildContainer).attr('key');

                    <%=ClientID%>_OpenNodeAndAllParents(plevel, pidx, pkey);
                }
            }
        }
    }

    // returns a list of level|idx|key,level|idx|key containing all visible nodes
    function <%=ClientID%>_GetAllVisibleNodes(includeLevel, includeIdx, includeKey) {
        var container = <%=ClientID%>_GetContainer();    

        var allChildContainers = container.find('[id$=_ChildNodeContainer]');

        var visibleNodes = '';
        var allNodesClosed = true;
        var allNodesOpen = true;

        if (allChildContainers.length > 0) {
            $.each(allChildContainers, function () {
                if ($(this).is(':visible')) {
                    var level = $(this).attr('level');
                    var idx = $(this).attr('idx');
                    var key = $(this).attr('key');

                    var token = '';

                    if (includeLevel) {
                        token += level;
                    }

                    if (includeIdx) {
                        if (token.length > 0) token += '|';
                        token += idx;
                    }

                    if (includeKey) {
                        if (token.length > 0) token += '|';
                        token += key;
                    }

                    if (visibleNodes.length > 0) visibleNodes += ',';
                    visibleNodes += token;

                    allNodesClosed = false;
                }
                else {
                    allNodesOpen = false;
                }
            });
        }
        else {
            allNodesOpen = false;
        }

        if (allNodesOpen) visibleNodes += ',ano';
        if (allNodesClosed) visibleNodes = 'anc';

        return visibleNodes;
    }

    function <%=ClientID%>_SetTreeViewHTML(html) {
        var container = <%=ClientID%>_GetContainer();
        container.html(html);
    }

</script>