using System;
using System.Collections.Generic;
using System.Text;
using HtmlAgilityPack;
using CSSAgilityPack;
using System.Diagnostics;
using NUnit.Framework;

namespace TestSuite
{
    [TestFixture]
    public class Program
    {
        [Test]
        public void InlineStyle()
        {
            CSSStyleDeclaration s = CSSParser.ParseStyleDeclaration("color: red;");

            Assert.AreEqual(1, s.Count);
            Assert.AreEqual("red", s.GetPropertyValue("color"));
        }

        [Test]
        public void MultipleInlineStyles()
        {
            CSSStyleDeclaration s = CSSParser.ParseStyleDeclaration("color: red; font-family: 'Helvetica', \"Arial\", sans-serif;" + 
            "font-style: italic;");

            Assert.AreEqual(3, s.Count);
            Assert.AreEqual("italic", s.GetPropertyValue("font-style"));
        }

        public static void Main(string[] args)
        {
            HtmlDocument d = new HtmlDocument();

            d.Load("test.html");
            HtmlNode p = d.GetElementbyId("test");

            CSSStyleDeclaration s = p.Style;
            for (int i = 0; i < s.Count; ++i)
                Console.WriteLine(s[i]);

            Console.WriteLine(s.Count);
        }
    }
}
