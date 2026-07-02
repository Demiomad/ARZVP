using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Threading.Tasks;

namespace ARZVPRewrite.Models.REAPER
{
    /// <summary>
    /// Represents a REAPER node.
    /// </summary>
    public class ReaperNode
    {
        /// <summary>
        /// Gets or sets the node's name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the list of child notes.
        /// </summary>
        public List<ReaperNode> Children { get; set; } = new List<ReaperNode>();

        /// <summary>
        /// Gets or sets the list of parameters.
        /// </summary>
        public List<string> Parameters { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the list of properties.
        /// </summary>
        public Dictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Gets or sets the parent node.
        /// </summary>
        public ReaperNode Parent { get; set; } = null;

        public ReaperNode(string name)
        {
            Name = name;
        }

        public ReaperNode(string name, params string[] parameters) : this(name)
        {
            Parameters = parameters.ToList();
        }

        /// <summary>
        /// Gets a child node.
        /// </summary>
        /// <param name="name">The name of the target child node.</param>
        /// <returns>The target child node.</returns>
        public ReaperNode GetNode(string name)
            => Children.FirstOrDefault(n => n.Name == name);

        /// <summary>
        /// Retrieves a parameter.
        /// </summary>
        /// <param name="index">The index of the parmeter to retrieve.</param>
        /// <returns>The parameter's raw value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown whenever <paramref name="index"/> is less than zero, or greater than the length of <see cref="Parameters"/>.</exception>
        public string GetParam(int index)
        {
            if (index < 0 || index > Parameters.Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            return Parameters[index];
        }

        /// <summary>
        /// Retrieves a parameter as a string.
        /// </summary>
        /// <param name="index">The index of the parmeter to retrieve.</param>
        /// <returns>The parameter's string value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown whenever <paramref name="index"/> is less than zero, or greater than the length of <see cref="Parameters"/>.</exception>
        public string GetParamString(int index)
            => GetParam(index).Trim('"');

        /// <summary>
        /// Retrieves a parameter as an integer.
        /// </summary>
        /// <param name="index">The index of the parmeter to retrieve.</param>
        /// <returns>The parameter's integer value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown whenever <paramref name="index"/> is less than zero, or greater than the length of <see cref="Parameters"/>.</exception>
        public int GetParamInt(int index)
        {
            var paramStr = GetParamString(index);

            if (!int.TryParse(paramStr, out var value))
                throw new Exception();

            return value;
        }

        /// <summary>
        /// Retrieves a parameter as a floating-point value.
        /// </summary>
        /// <param name="index">The index of the parmeter to retrieve.</param>
        /// <returns>The parameter's floating-point value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown whenever <paramref name="index"/> is less than zero, or greater than the length of <see cref="Parameters"/>.</exception>
        public double GetParamFloat(int index)
        {
            var paramStr = GetParamString(index);

            if (!double.TryParse(paramStr, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var value))
                throw new Exception();

            return value;
        }

        /// <summary>
        /// Retrieves a property as a string.
        /// </summary>
        /// <param name="key">The key of the property.</param>
        /// <returns>The property's string value.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if <paramref name="key"/> was not found in <see cref="Properties"/>.</exception>
        public string GetPropertyString(string key)
        {
            if (!Properties.TryGetValue(key, out var value))
                throw new KeyNotFoundException($"The property \"{key}\" was not found.");

            return value;
        }

        /// <summary>
        /// Retrieves a parameter as an integer.
        /// </summary>
        /// <param name="key">The key of the property.</param>
        /// <returns>The property's integer value.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if <paramref name="key"/> was not found in <see cref="Properties"/>.</exception>
        public int GetParamInt(string key)
        {
            var paramStr = GetPropertyString(key);

            if (!int.TryParse(paramStr, out var value))
                throw new Exception();

            return value;
        }

        /// <summary>
        /// Retrieves a property as a floating-point value.
        /// </summary>
        /// <param name="key">The key of the property.</param>
        /// <returns>The property's floating-point value.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if <paramref name="key"/> was not found in <see cref="Properties"/>.</exception>
        public double GetPropertyFloat(string key)
        {
            var paramStr = GetPropertyString(key);

            if (!double.TryParse(paramStr, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var value))
                throw new Exception();

            return value;
        }

        /// <summary>
        /// Sets a property's value.
        /// </summary>
        public void SetProperty(string key, string value)
            => Properties[key] = value;

        /// <summary>
        /// Sets a parameter's value.
        /// </summary>
        public void SetParam(int index, string value)
            => Parameters[index] = value;

        public override string ToString()
            => ToString(0);

        private string ToString(int indent = 0)
        {
            var indentation = new string(' ', indent * 2);
            var paramsStr = Parameters.Count > 0 ? $" {string.Join(" ", Parameters)}" : string.Empty;
            var output = new StringBuilder();
            output.AppendLine($"{indentation}<{Name}{paramsStr}");

            foreach (var prop in Properties)
            {
                output.AppendLine($"{indentation}  {prop.Key} {prop.Value}");
            }

            foreach (var child in Children)
            {
                output.Append(child.ToString(indent + 1));
            }

            output.AppendLine($"{indentation}>");

            return output.ToString();
        }
    }
}
