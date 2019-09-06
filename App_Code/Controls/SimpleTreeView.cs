using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace WTS
{
    /// <summary>
    /// Summary description for SimpleTreeView
    /// </summary>
    public class SimpleTreeView
    {
        public string ClientID { get; set; }

        public List<TreeViewNode> Nodes { get; set; }

        /// <summary>
        /// The indent value (in pixels) to use for all child nodes. Use Indents to specify indent values for specific levels.
        /// </summary>
        public int Indent = 25;

        /// <summary>
        /// Indents to use for children at each level (for children at first level: Indents[1] = 25). These values will
        /// take precedence over the Indent value. If not specified, default Indent level is used instead.
        /// NOTE: Top level items cannot be indented. Use outer div styling instead.
        /// </summary>
        public Dictionary<int, int> Indents = new Dictionary<int, int>();

        /// <summary>
        /// The spacing between rows. Use RowSpacings to specify spacings for specific levels.
        /// </summary>
        public int RowSpacing = 7;

        /// <summary>
        /// Row spacing to use at specific levels. These values will take precedence over the RowSpacing value.
        /// </summary>
        public Dictionary<int, int> RowSpacings = new Dictionary<int, int>();

        /// <summary>
        /// By default, images are loaded from "images/icons/..."; use this property to prepend paths "../images.....";
        /// </summary>
        public string ImagePathPrefix = ""; // 

        /// <summary>
        /// If specified, the last clicked node will be highlighted with this color, and all other nodes deselected. Use
        /// LastClickedNodeColors to specify colors per level.
        /// </summary>
        public string LastClickedNodeColor = "";

        /// <summary>
        /// Colors to apply to the last clicked node per level. Colors specified take precedence over LastClickedNodeColor.
        /// </summary>
        public Dictionary<int, string> LastClickedNodeColors = new Dictionary<int, string>();

        /// <summary>
        /// If specified, the last clicked node will be highlighted with this background color, and all other nodes deselected. Use
        /// LastClickedNodeBackgroundColors to specify colors per level.
        /// </summary>
        public string LastClickedNodeBackgroundColor = "";

        /// <summary>
        /// Background colors to apply to the last clicked node per level. Colors specified take precedence over LastClickedNodeBackgroundColor.
        /// </summary>
        public Dictionary<int, string> LastClickedNodeBackgroundColors = new Dictionary<int, string>();

        /// <summary>
        /// Global onclick to apply to all nodes. It is inserted PRIOR to the local node onclicks. Use OnClicks to apply
        /// onclick behavior globally, by level.
        /// </summary>
        public string OnClick = "";

        /// <summary>
        /// Onclick behavior to apply to all nodes, by level. OnClick behavior specified here take precedence over the OnClick value.
        /// </summary>
        public Dictionary<int, string> OnClicks = new Dictionary<int, string>();

        public string ExpandNodeAltText = "Show All";
        public string CollapseNodeAltText = "Hide All";


        /// <summary>
        /// If present, when a node is clicked, this function will be called with FNCNAME(level, idx, key, node). The node will
        /// be the NodeContainer containing the node (so you can access other attributes, if needed).
        /// </summary>
        public string ClickCallbackFunction = "";
        public Dictionary<int, string> ClickCallbackFunctions = new Dictionary<int, string>();

        /// <summary>
        /// Styles for all nodes. Can be overridden by Styles dictionary, or by individual node styles.
        /// </summary>
        public string Style = "";

        /// <summary>
        /// Styles for text at each node level. Overridden by node-specific styles.
        /// </summary>
        public Dictionary<int, string> Styles = new Dictionary<int, string>();

        public SimpleTreeView(string clientID)
        {
            ClientID = clientID;
        }

        public TreeViewNode AddTreeViewNode(TreeViewNode node)
        {
            if (Nodes == null)
            {
                Nodes = new List<TreeViewNode>();
            }

            if (string.IsNullOrWhiteSpace(node.Key))
            {
                throw new ArgumentException("Invalid node. Node key cannot be blank.");
            }

            if (Nodes.Find(n => n.Key == node.Key) != null)
            {
                throw new ArgumentException("Invalid node. Node key ('" + node.Key + "') already exists.");
            }

            Nodes.Add(node);

            return node;

        }

        public TreeViewNode AddTreeViewNode(string text, string key = null, List<TreeViewNode> nodes = null)
        {
            if (Nodes == null)
            {
                Nodes = new List<TreeViewNode>();
            }

            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Invalid node. Node key cannot be blank.");
            }

            if (Nodes.Find(n => n.Key == key) != null)
            {
                throw new ArgumentException("Invalid node. Node key ('" + key + "') already exists.");
            }

            TreeViewNode node = new TreeViewNode(text, key, nodes);

            Nodes.Add(node);

            return node;
        }

        public string RenderTreeView()
        {
            return RenderTreeView(Nodes, ClientID + "_TreeViewNode", true, 1, -1, -1, "");
        }

        public string RenderTreeView(List<TreeViewNode> nodes, string id, bool open, int level, int parentLevel, int parentIdx, string parentKey)
        {
            StringBuilder str = new StringBuilder();

            int rowSpacing = RowSpacing;

            if (RowSpacings != null && RowSpacings.ContainsKey(level))
            {
                rowSpacing = RowSpacings[level];
            }

            for (int i = 0; nodes != null && i < nodes.Count; i++)
            {
                TreeViewNode node = nodes[i];
                bool hasChildren = node.Nodes != null && node.Nodes.Count > 0;
                string key = node.Key;

                string expandAltText = string.IsNullOrWhiteSpace(node.ExpandNodeAltText) ? ExpandNodeAltText : node.ExpandNodeAltText;
                expandAltText = string.IsNullOrWhiteSpace(expandAltText) ? "" : expandAltText.Replace("'", "");

                string collapseAltText = string.IsNullOrWhiteSpace(node.CollapseNodeAltText) ? CollapseNodeAltText : node.CollapseNodeAltText;
                collapseAltText = string.IsNullOrWhiteSpace(collapseAltText) ? "" : collapseAltText.Replace("'", "");

                // onclick
                string onClick = "";

                if (!string.IsNullOrWhiteSpace(OnClick))
                {
                    onClick = OnClick;
                }

                if (OnClicks != null && OnClicks.ContainsKey(level) && !string.IsNullOrWhiteSpace(OnClicks[level]))
                {
                    onClick = OnClicks[level];
                }

                if (!string.IsNullOrWhiteSpace(LastClickedNodeColor) ||
                    !string.IsNullOrWhiteSpace(LastClickedNodeBackgroundColor) ||
                    (LastClickedNodeColors != null && LastClickedNodeColors.ContainsKey(level)) ||
                    (LastClickedNodeBackgroundColors != null && LastClickedNodeBackgroundColors.ContainsKey(level))
                    )
                {
                    string color = LastClickedNodeColor;
                    if (LastClickedNodeColors != null && LastClickedNodeColors.ContainsKey(level))
                    {
                        color = LastClickedNodeColors[level];
                    }

                    string backgroundColor = LastClickedNodeBackgroundColor;
                    if (LastClickedNodeBackgroundColors != null && LastClickedNodeBackgroundColors.ContainsKey(level))
                    {
                        backgroundColor = LastClickedNodeBackgroundColors[level];
                    }

                    if (color == null) color = "";
                    if (backgroundColor == null) backgroundColor = "";

                    color = color.Replace("'", "");
                    backgroundColor = backgroundColor.Replace("'", "");

                    onClick += ClientID + "_SetNodeColor(" + level + ", " + i + ", '" + key + "', '" + color + "', '" + backgroundColor + "', true);";
                }

                if (hasChildren)
                {
                    onClick += ClientID + "_ToggleNode(" + level + ", " + i + ", '" + key + "');";
                }

                string clickCallback = ClickCallbackFunction;
                if (ClickCallbackFunctions != null && ClickCallbackFunctions.ContainsKey(level) && !string.IsNullOrWhiteSpace(ClickCallbackFunctions[level]))
                {
                    clickCallback = ClickCallbackFunctions[level];
                }

                if (!string.IsNullOrWhiteSpace(clickCallback))
                {
                    onClick += clickCallback + "(" + level + ", " + i + ", '" + key + "', " + ClientID + "_GetNodeByKey('" + key + "', " + level + ", " + i + "));";
                }

                if (!string.IsNullOrWhiteSpace(node.OnClick))
                {
                    onClick += node.OnClick;
                }

                onClick = onClick.Trim().Replace("[LEVEL]", level + "").Replace("[IDX]", i + "").Replace("[KEY]", key);

                if (!string.IsNullOrWhiteSpace(onClick) && !onClick.ToLower().EndsWith("return false;") && !onClick.ToLower().EndsWith("return false"))
                {
                    onClick += " return false;";
                }

                // style
                string style = node.Style;

                if (string.IsNullOrWhiteSpace(style))
                {
                    if (Styles != null && Styles.ContainsKey(level) && !string.IsNullOrWhiteSpace(Styles[level]))
                    {
                        style = Styles[level];
                    }
                }

                if (string.IsNullOrWhiteSpace(style))
                {
                    style = Style;
                }

                if (style == null)
                {
                    style = "";
                }

                /*
                 * NODE STRUCTURE:
                 * <div id="nodecontainer">
                 *   <div id="textcontainer">
                 *     <img selectorimage />
                 *     <div selectortext></div>
                 *   </div>
                 *   <div id="childnodecontainer">
                 *     <div id="nodecontainer">...
                 *     <div id="nodecontainer">...
                 *   </div>
                 * </div>
                 * <div id="nodecontainer">
                 *   <div id="textcontainer">
                 *     <img selectorimage />
                 *     <div selectortext></div>
                 *   </div>
                 *   <div id="childnodecontainer">
                 *     <div id="nodecontainer">...
                 *     <div id="nodecontainer">...
                 *   </div>
                 * </div>             
                 * 
                 */

                str.Append("<div id=\"" + id + "_NodeContainer\" style=\"position:relative;width:100%;margin-top:" + rowSpacing + "px;\" level=\"" + level + "\" idx=\"" + i + "\" key=\"" + key + "\" parentlevel=\"" + parentLevel + "\" parentidx=\"" + parentIdx + "\" parentkey=\"" + parentKey + "\" " + node.AttributeString + ">");
                str.Append("  <div id=\"" + id + "_TextContainer\" level=\"" + level + "\" idx=\"" + i + "\" key=\"" + key + "\" style=\"white-space:" + (node.Wrap ? "wrap" : "nowrap") + ";\">");
                if (hasChildren)
                {
                    str.Append("    <img src=\"" + ImagePathPrefix + "Images/Icons/expand.gif\" selectorimage=\"1\" level=\"" + level + "\" idx=\"" + i + "\" key=\"" + key + "\" style=\"cursor:pointer; margin-right:10px;position:absolute;left:0px;top:2px;\" onclick=\"" + ClientID + "_ToggleNode(" + level + ", " + i + ", '" + key + "')\" alt=\"" + expandAltText + "\" title=\"" + expandAltText + "\" expandalt=\"" + expandAltText + "\" collapsealt=\"" + collapseAltText + "\">");
                }
                str.Append("    <div selectortext=\"1\" level=\"" + level + "\" idx=\"" + i + "\" key=\"" + key + "\" style=\"position:relative;" + (hasChildren ? "left:15px;margin-right:15px;" : "") + (!string.IsNullOrWhiteSpace(onClick) ? "cursor:pointer;" : "") + style + "\" " + (!string.IsNullOrWhiteSpace(onClick) ? "onclick=\"" + onClick + "\"" : "") + ">" + node.Text + "</div>");
                str.Append("  </div>");

                if (hasChildren)
                {
                    int indent = Indent;

                    if (Indents != null && Indents.ContainsKey(level))
                    {
                        indent = Indents[level];
                    }

                    str.Append("  <div id=\"" + id + "_ChildNodeContainer\" level=\"" + level + "\" idx=\"" + i + "\" key=\"" + key + "\" style=\"display:" + (node.Open ? "block" : "none") + ";margin-left:" + indent + "px;\">");
                    str.Append(RenderTreeView(node.Nodes, id, false, level + 1, level, i, key));
                    str.Append("  </div>");
                }

                str.Append("</div>");
            }

            return str.ToString();
        }
    }
}