using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ARZVPRewrite.Models.REAPER
{
    /// <summary>
    /// Represents a REAPER project.
    /// </summary>
    public class ReaperProject
    {
        /// <summary>
        /// Gets or sets the list of nodes.
        /// </summary>
        public List<ReaperNode> Nodes { get; set; } = new List<ReaperNode>();

        /// <summary>
        /// Gets or sets the current project's file path.
        /// </summary>
        public string FilePath { get; set; }

        private ReaperNode _currentNode = null;
        private int _lineNumber = 0;

        public ReaperProject(string path)
        {
            FilePath = path;
            ParseLines();
        }

        private void ParseLines()
        {
            var lines = File.ReadAllLines(FilePath);
            var root = new ReaperNode("ROOT");
            Nodes.Add(root);
            _currentNode = root;

            foreach (var line in lines)
            {
                _lineNumber++;
                var trimmed = line.Trim();

                if (string.IsNullOrWhiteSpace(trimmed))
                    continue;

                try
                {
                    if (trimmed.StartsWith("<") && !trimmed.EndsWith(">"))
                    {
                        ParseOpeningNode(trimmed);
                    }
                    else if (trimmed.StartsWith("<") && trimmed.EndsWith(">"))
                    {
                        ParseSelfClosingNode(trimmed);
                    }
                    else if (trimmed == ">")
                    {
                        if (_currentNode?.Parent != null)
                            _currentNode = _currentNode.Parent;
                    }
                    else
                    {
                        ParseProperty(trimmed);
                    }
                }
                catch
                {
                    if (_currentNode != null && _currentNode.Parent != null)
                        _currentNode = _currentNode.Parent;
                }
            }

            if (root.Children.Count > 0)
            {
                Nodes.Clear();
                Nodes.AddRange(root.Children);
            }
        }

        private void ParseOpeningNode(string trimmed)
        {
            var firstSpace = trimmed.IndexOf(' ');
            string name;
            string paramString;

            if (firstSpace == -1)
            {
                name = trimmed.Substring(1);
                paramString = "";
            }
            else
            {
                name = trimmed.Substring(1, firstSpace - 1);
                paramString = trimmed.Substring(firstSpace + 1);
            }

            var parameters = ParseParameters(paramString);
            var node = new ReaperNode(name, parameters.ToArray());

            if (_currentNode != null)
            {
                node.Parent = _currentNode;
                _currentNode.Children.Add(node);
            }

            _currentNode = node;
        }

        private void ParseSelfClosingNode(string trimmed)
        {
            var content = trimmed.Substring(1, trimmed.Length - 2).Trim();

            var firstSpace = content.IndexOf(' ');
            string name;
            string paramString;

            if (firstSpace == -1)
            {
                name = content;
                paramString = "";
            }
            else
            {
                name = content.Substring(0, firstSpace);
                paramString = content.Substring(firstSpace + 1);
            }

            var parameters = ParseParameters(paramString);
            var node = new ReaperNode(name, parameters.ToArray());

            if (_currentNode != null)
            {
                node.Parent = _currentNode;
                _currentNode.Children.Add(node);
            }
        }

        private void ParseProperty(string trimmed)
        {
            if (_currentNode == null) return;

            var parts = ParseParameters(trimmed);
            if (parts.Count >= 2)
            {
                var key = parts[0];
                var value = string.Join(" ", parts.Skip(1));
                _currentNode.Properties[key] = value;
            }
            else if (parts.Count == 1)
            {
                _currentNode.Properties[parts[0]] = "1";
            }
        }

        private List<string> ParseParameters(string input)
        {
            var result = new List<string>();
            if (string.IsNullOrWhiteSpace(input))
                return result;

            var current = new StringBuilder();
            var inQuotes = false;
            var escaped = false;

            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];

                if (escaped)
                {
                    current.Append(c);
                    escaped = false;
                    continue;
                }

                if (c == '\\')
                {
                    escaped = true;
                    current.Append(c);
                    continue;
                }

                if (c == '"')
                {
                    inQuotes = !inQuotes;
                    current.Append(c);
                    continue;
                }

                if (c == ' ' && !inQuotes)
                {
                    if (current.Length > 0)
                    {
                        result.Add(current.ToString());
                        current.Clear();
                    }
                    continue;
                }

                current.Append(c);
            }

            if (current.Length > 0)
            {
                result.Add(current.ToString());
            }

            return result;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var node in Nodes)
            {
                sb.Append(node.ToString());
            }
            return sb.ToString();
        }
    }
}