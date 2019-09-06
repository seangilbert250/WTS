using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace WTS
{
    public class TreeViewNode
    {
        public string Text { get; set; }
        public string Key { get; set; }
        public List<TreeViewNode> Nodes { get; set; }
        public Dictionary<string, string> Attributes { get; set; }
        public string OnClick { get; set; }
        public string OnOpen { get; set; }
        public string OnClose { get; set; }
        public bool Wrap { get; set; }
        public bool Open { get; set; }
        public string ExpandNodeAltText { get; set; }
        public string CollapseNodeAltText { get; set; }
        public string Style { get; set; }

        public TreeViewNode(string text, string key = null, List<TreeViewNode> nodes = null)
        {
            Text = text;
            Key = key;
            Nodes = nodes;
        }

        public TreeViewNode AddChildNode(string text, string key = null, List<TreeViewNode> nodes = null)
        {
            if (Nodes == null)
            {
                Nodes = new List<TreeViewNode>();
            }

            TreeViewNode node = new TreeViewNode(text, key, nodes);

            if (Nodes.Find(n => n.Key == key) != null)
            {
                throw new ArgumentException("Invalid child node entry. Node key ('" + key + "') already exists.");
            }

            Nodes.Add(node);

            return node;
        }

        public void AddAttribute(string key, string value)
        {
            if (Attributes == null)
            {
                Attributes = new Dictionary<string, string>();
            }

            Attributes[key] = value;
        }

        public void RemoveAttribute(string key)
        {
            if (Attributes != null && Attributes.ContainsKey(key))
            {
                Attributes.Remove(key);
            }
        }

        public string AttributeString
        {
            get
            {
                StringBuilder str = new StringBuilder();

                if (Attributes != null && Attributes.Count > 0)
                {
                    foreach (string key in Attributes.Keys)
                    {
                        if (str.Length > 0) str.Append(" ");
                        str.Append(key + "=\"" + Attributes[key] + "\"");
                    }
                }

                return str.ToString();
            }
        }
    }
}