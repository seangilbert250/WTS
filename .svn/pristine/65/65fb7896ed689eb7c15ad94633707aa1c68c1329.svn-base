using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.IO;
using System.Text;

using WTS;

public partial class Controls_SimpleTreeViewControl : WTSControl
{
    private string _ClientID = null;

    public string ClientID
    {
        get
        {
            return _ClientID;
        }

        set
        {
            _ClientID = value;
            TreeView.ClientID = value;
        }
    }

    private WTS.SimpleTreeView _TreeView = null;

    public SimpleTreeView TreeView
    {
        get
        {
            if (_TreeView == null)
            {
                _TreeView = new WTS.SimpleTreeView(ClientID);
            }

            return _TreeView;
        }
    }

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

    protected void Page_Load(object sender, EventArgs e)
    {
        _ClientID = base.ClientID;
        TreeView.ClientID = _ClientID;


        ConfigureTreeView();
        divTreeViewContainer.InnerHtml = TreeView.RenderTreeView();
    }

    public void ConfigureTreeView()
    {
        TreeView.Indent = Indent;
        TreeView.Indents = Indents;
        TreeView.RowSpacing = RowSpacing;
        TreeView.RowSpacings = RowSpacings;
        TreeView.ImagePathPrefix = ImagePathPrefix;
        TreeView.LastClickedNodeColor = LastClickedNodeColor;
        TreeView.LastClickedNodeColors = LastClickedNodeColors;
        TreeView.LastClickedNodeBackgroundColor = LastClickedNodeBackgroundColor;
        TreeView.LastClickedNodeBackgroundColors = LastClickedNodeBackgroundColors;
        TreeView.OnClick = OnClick;
        TreeView.OnClicks = OnClicks;
        TreeView.ExpandNodeAltText = ExpandNodeAltText;
        TreeView.CollapseNodeAltText = CollapseNodeAltText;
        TreeView.ClickCallbackFunction = ClickCallbackFunction;
        TreeView.ClickCallbackFunctions = ClickCallbackFunctions;
        TreeView.Style = Style;
        TreeView.Styles = Styles;
    }

    public TreeViewNode AddTreeViewNode(TreeViewNode node)
    {
        return TreeView.AddTreeViewNode(node);
    }

    public TreeViewNode AddTreeViewNode(string text, string key = null, List<TreeViewNode> nodes = null)
    {
        return TreeView.AddTreeViewNode(text, key, nodes);
    }
}