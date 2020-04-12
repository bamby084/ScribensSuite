using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Navigation;
using HTMLConverter;
using PluginScribens.Common;

namespace PluginScribens.UI.Controls
{
    public class HtmlTextBlock : TextBlock
    {
        public static readonly DependencyProperty HtmlTextProperty = DependencyProperty.Register(
            "HtmlText",
            typeof(string),
            typeof(HtmlTextBlock),
            new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnHtmlTextChange)));

        public string HtmlText
        {
            get => (string)GetValue(HtmlTextProperty);
            set => SetValue(HtmlTextProperty, value);
        }

        private void ParseHtml(string html)
        {
            try
            {
                this.Inlines.Clear();
                if (string.IsNullOrEmpty(html))
                    return;

                string xaml = HtmlToXamlConverter.ConvertHtmlToXaml(html, true);
                var document = XamlReader.Parse(xaml) as FlowDocument;
                var inlines = GetInlines(document.Blocks);

                var hyperLinks = inlines.Where(l => l is Hyperlink).ToList();
                foreach (Hyperlink hyperLink in hyperLinks)
                {
                    hyperLink.TextDecorations = null;
                    hyperLink.FontWeight = FontWeights.Bold;
                    hyperLink.RequestNavigate += OnHyperLinkClicked;
                }

                this.Inlines.AddRange(inlines);
            }
            catch(Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private void OnHyperLinkClicked(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
                e.Handled = true;
            }
            catch(Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private IList<Inline> GetInlines(BlockCollection blocks)
        {
            try
            {
                var inlines = new List<Inline>();
                foreach (var block in blocks)
                {
                    if (block is Paragraph)
                    {
                        inlines.AddRange(((Paragraph)block).Inlines);
                    }
                    else if (block is Section)
                    {
                        inlines.AddRange(GetInlines(((Section)block).Blocks));
                    }
                }

                return inlines;
            }
            catch(Exception ex)
            {
                Logger.Error(ex);
                return new List<Inline>();
            }
        }

        private static void OnHtmlTextChange(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var textBlock = (HtmlTextBlock)sender;
            textBlock.ParseHtml(e.NewValue?.ToString());
        }
    }
}
