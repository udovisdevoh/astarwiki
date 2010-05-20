using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tek.Graphics;
using System.Diagnostics;
using Tek.Text;
using System.Net;
using System.Xml;

namespace GridAStar
{
    public sealed class Query : IPathfindingQuery<string>
    {
        const int maxCommonLinkCount = 500;

        private readonly string sourcePageName;
        private readonly string destinationPageName;
        private readonly HashSet<string> destinationPageLinks;

        public Query(string sourcePageName, string destinationPageName)
        {
            this.sourcePageName = sourcePageName;
            this.destinationPageName = destinationPageName;
            this.destinationPageLinks = new HashSet<string>(GetPageLinks(destinationPageName));
        }

        public string Source
        {
            get { return sourcePageName; }
        }

        public string Destination
        {
            get { return destinationPageName; }
        }

        public void GetAdjacentStates(PathNode<string> node, List<AdjacentState<string>> adjacentStates)
        {
            foreach (string pageLink in GetPageLinks(node.State))
                adjacentStates.Add(new AdjacentState<string>(pageLink, 100));
        }

        public float EstimateCostToDestination(string pageName)
        {
            HashSet<string> pageLinks = new HashSet<string>(GetPageLinks(pageName));
            if (pageLinks.Contains(destinationPageName)) return 0;

            pageLinks.IntersectWith(destinationPageLinks);
            int commonLinkCount = pageLinks.Count;
            int cost = maxCommonLinkCount - commonLinkCount;

            if (commonLinkCount >= 10) Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("{0:D3}: {1}".FormatInvariant(commonLinkCount, pageName));
            if (commonLinkCount >= 10) Console.ResetColor();

            return cost;
        }

        public static IEnumerable<string> GetPageLinks(string pageName)
        {
            string url = "http://en.wikipedia.org/w/api.php?action=query&titles={0}&prop=links&format=xml&pllimit=500"
                .FormatInvariant(pageName.Replace(" ", "%20"));

            List<string> pageLinks = new List<string>();
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.UserAgent = "Venujord/1.0";
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    XmlDocument document = new XmlDocument();
                    document.Load(response.GetResponseStream());
                    foreach (XmlAttribute attribute in document.SelectNodes("/api/query/pages/page/links/pl/@title"))
                        pageLinks.Add(attribute.Value);
                }
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Exception: " + e.Message);
                Console.ResetColor();
            }

            pageLinks.RemoveAll(s => s.Contains(":"));

            return pageLinks;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Pathfinder<string> pathfinder = new Pathfinder<string>();

            Console.Write("Enter source page name: ");
            string sourcePageName = Console.ReadLine();
            Console.Write("Enter destination page name: ");
            string destinationPageName = Console.ReadLine();

            Query query = new Query(sourcePageName, destinationPageName);
            string[] path = pathfinder.Find(query);

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Path found!");
            foreach (string pageName in path)
                Console.WriteLine(pageName);
            Console.WriteLine(query.Destination);

            Console.Read();
        }
    }
}
