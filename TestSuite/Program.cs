using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using CSSAgilityPack;

namespace TestSuite
{
    class Program
    {
        static void Main(string[] args)
        {
            HtmlDocument d = new HtmlDocument();

            try
            {
                d.Load("test.html");
                HtmlNode n = d.DocumentNode;
                Console.WriteLine(n.InnerHtml);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
