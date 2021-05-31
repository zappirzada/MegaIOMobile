using CG.Web.MegaApiClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MegaIOMobile
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            Task.Run(Main);
        }
        void Main()
        {
            var client = new MegaApiClient();
            client.Login("zap_pirzada@hotmail.com", "***********");

            // GetNodes retrieves all files/folders metadata from Mega
            // so this method can be time consuming
            IEnumerable<INode> nodes = client.GetNodes();

            INode parent = nodes.Single(n => n.Type == NodeType.Root);
            DisplayNodesRecursive(nodes, parent);

            client.Logout();
        }

        void DisplayNodesRecursive(IEnumerable<INode> nodes, INode parent, int level = 0)
        {
            IEnumerable<INode> children = nodes.Where(x => x.ParentId == parent.Id);
            foreach (INode child in children)
            {
                string infos = $"- {child.Name} - {FileSizeFormatter.FormatSize(child.Size)} - {child.CreationDate}";
                Console.WriteLine(infos.PadLeft(infos.Length + level, '\t'));
                Device.BeginInvokeOnMainThread(() =>
                {
                    M.Text += infos.PadLeft(infos.Length + level, '\t') + Environment.NewLine;
                });
                if (child.Type == NodeType.Directory)
                {
                    DisplayNodesRecursive(nodes, child, level + 1);
                }
            }
        }
        public static class FileSizeFormatter
        {
            // Load all suffixes in an array  
            static readonly string[] suffixes =
            { "Bytes", "KB", "MB", "GB", "TB", "PB" };
            public static string FormatSize(Int64 bytes)
            {
                int counter = 0;
                decimal number = (decimal)bytes;
                while (Math.Round(number / 1024) >= 1)
                {
                    number = number / 1024;
                    counter++;
                }
                return string.Format("{0:n1}{1}", number, suffixes[counter]);
            }
        }
    }
}
