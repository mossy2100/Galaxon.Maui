using System.Text.RegularExpressions;
using Galaxon.Core.Exceptions;
using HtmlAgilityPack;
using Microsoft.Maui.Controls.Shapes;

namespace Galaxon.Maui.Utilities;

internal static class TextUtility
{
    private const int _DefaultMargin = 5;

    /// <summary>
    /// Easily construct a formatted string.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="bold">If it should be bold.</param>
    /// <param name="italic">If it should be italic.</param>
    /// <param name="color">What color it should be.</param>
    /// <param name="style">What style it should use.</param>
    /// <returns></returns>
    public static FormattedString CreateFormattedString(string text, bool bold = false,
        bool italic = false, Color? color = null, Style? style = null)
    {
        // Initialize the Span.
        var span = new Span { Text = text };

        // Set the font style if specified.
        if (bold || italic)
        {
            var attr = FontAttributes.None;
            if (bold)
            {
                attr |= FontAttributes.Bold;
            }
            if (italic)
            {
                attr |= FontAttributes.Italic;
            }
            span.FontAttributes = attr;
        }

        // Set the color if specified.
        if (color != null)
        {
            span.TextColor = color;
        }

        // Set the style if specified.
        if (style != null)
        {
            span.Style = style;
        }

        // Construct the FormattedString object.
        return new FormattedString { Spans = { span } };
    }

    public static FormattedString NormalText(string text)
    {
        return CreateFormattedString(text);
    }

    public static FormattedString BoldText(string text)
    {
        return CreateFormattedString(text, bold: true);
    }

    public static FormattedString ItalicText(string text)
    {
        return CreateFormattedString(text, italic: true);
    }

    public static FormattedString ColorText(string text, Color color)
    {
        return CreateFormattedString(text, color: color);
    }

    public static FormattedString StyleText(string text, Style style)
    {
        return CreateFormattedString(text, style: style);
    }

    public static FormattedString StyleText(string text, string styleName)
    {
        var style = ResourceUtility.LookupStyle(styleName);
        return CreateFormattedString(text, style: style);
    }

    public static string CollapseWhitespace(string text)
    {
        return Regex.Replace(text, @"\s{2,}", " ");
    }

    private static void AddSpan(string text, int fontSize, FontAttributes fontAttributes,
        Layout layout)
    {
        // Ignore empty spans.
        if (string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        // Construct a new span.
        var newSpan = new Span
        {
            Text = CollapseWhitespace(text),
            FontSize = fontSize,
            FontAttributes = fontAttributes,
        };

        // Get the last label in the layout.
        var lastLabel = (Label?)layout.LastOrDefault(child => child is Label);

        // If there are no labels in the layout, add one.
        if (lastLabel == null)
        {
            lastLabel = new Label();
            layout.Add(lastLabel);
        }

        // Create the FormattedString if necessary.
        lastLabel.FormattedText ??= new FormattedString();

        // Add the span to the label.
        lastLabel.FormattedText.Spans.Add(newSpan);
    }

    public static void ProcessHtmlDocument(HtmlNode node, int parentFontSize,
        FontAttributes parentFontAttributes, Layout layout)
    {
        switch (node.NodeType)
        {
            case HtmlNodeType.Text:
                AddSpan(node.InnerText, parentFontSize, parentFontAttributes, layout);
                break;

            case HtmlNodeType.Element:
                // Handle horizontal rules.
                if (node.Name is "hr")
                {
                    layout.Add(GetHorizontalRule(layout.Width));
                    break;
                }

                // Create a new Label if necessary.
                if (node.Name is "p" or "h1" or "h2" or "li" or "br")
                {
                    var topMargin = node.Name switch
                    {
                        "br" => 0,
                        "li" => node == node.ParentNode.ChildNodes[0] ? _DefaultMargin : 0,
                        _ => _DefaultMargin,
                    };
                    var bottomMargin = node.Name switch
                    {
                        "br" => 0,
                        "li" => node == node.ParentNode.ChildNodes[^1] ? _DefaultMargin : 0,
                        _ => _DefaultMargin,
                    };
                    layout.Add(new Label
                    {
                        Margin = new Thickness(0, topMargin, 0, bottomMargin),
                    });
                }

                // Determine the font attributes.
                var fontAttributes = parentFontAttributes;
                switch (node.Name)
                {
                    case "b" or "strong":
                        fontAttributes |= FontAttributes.Bold;
                        break;

                    case "i" or "em":
                        fontAttributes |= FontAttributes.Italic;
                        break;
                }

                // Determine the font size.
                var fontSize = node.Name switch
                {
                    "h1" => 24,
                    "h2" => 20,
                    _ => parentFontSize,
                };

                // Handle unordered lists.
                if (node.Name == "li" && node.ParentNode.Name == "ul")
                {
                    // Add a span for the bullet.
                    AddSpan("\u2022 ", fontSize, FontAttributes.None, layout);
                }

                if (node.HasChildNodes)
                {
                    // Add new labels and spans for each child node.
                    foreach (var childNode in node.ChildNodes)
                    {
                        ProcessHtmlDocument(childNode, fontSize, fontAttributes, layout);
                    }
                }
                else if (node.InnerText != "")
                {
                    AddSpan(node.InnerText, fontSize, fontAttributes, layout);
                }
                break;

            case HtmlNodeType.Document:
                // Add new spans for each child node.
                foreach (var childNode in node.ChildNodes)
                {
                    ProcessHtmlDocument(childNode, parentFontSize, parentFontAttributes, layout);
                }
                break;

            case HtmlNodeType.Comment:
                // Ignore.
                break;

            default:
                throw new MatchNotFoundException("Invalid node type.");
        }
    }

    public static async Task LoadHtmlIntoLayout(string filename, Layout layout)
    {
        // Load the HTML.
        await using var stream = await FileSystem.OpenAppPackageFileAsync(filename);
        var doc = new HtmlDocument();
        doc.Load(stream);

        // Recursively process the HTML document into the layout.
        ProcessHtmlDocument(doc.DocumentNode, 14, FontAttributes.None, layout);

        // Set the text color according to the theme.
        SetLayoutTextColor(layout);
    }

    public static void SetLayoutTextColor(Layout layout)
    {
        // Get the text color.
        var textColor = Application.Current!.RequestedTheme == AppTheme.Light
            ? Colors.Black
            : Colors.White;

        // Change the color of every label and span in the layout.
        foreach (var item in layout)
        {
            // Ignore non-labels.
            if (item is not Label label)
            {
                continue;
            }

            // Update the label color.
            label.TextColor = textColor;

            // Update the color of any spans in the label.
            if (label.FormattedText is { Spans.Count: > 0 })
            {
                foreach (var span in label.FormattedText.Spans)
                {
                    span.TextColor = textColor;
                }
            }
        }
    }

    public static Rectangle GetHorizontalRule(double width)
    {
        return new Rectangle
        {
            BackgroundColor = Colors.Grey,
            WidthRequest = width,
            HeightRequest = 1,
        };
    }
}
