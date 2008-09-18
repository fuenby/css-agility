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
                HtmlNode p = d.GetElementbyId("test");
                CSSStyleDeclaration s = p.Style;
                Console.WriteLine(s.CssText);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
