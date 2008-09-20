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
        public void Whitespace()
        {
            CSSStyleDeclaration s = CSSParser.ParseStyleDeclaration(" border : 1px solid ; \n\rcolor : \r\tblue ");

            Assert.AreEqual(2, s.Count);
            Assert.AreEqual("blue", s.GetPropertyValue("color"));
            Assert.AreEqual("1px solid", s.GetPropertyValue("border"));
        }

        [Test]
        public void NonStandardUnits()
        {
            CSSStyleDeclaration s = CSSParser.ParseStyleDeclaration("font-size: x-large; height: 80%; line-height: 12.7em; ");

            Assert.AreEqual(3, s.Count);
        }

        [Test]
        public void StyleRule()
        {
            CSSRule r = CSSParser.ParseRule("body { font-size: 2em; color: red; }");

            Assert.AreEqual(CSSRule.RuleType.STYLE_RULE, r.Type);

            CSSStyleRule sr = r as CSSStyleRule;
            Assert.IsNotNull(sr);
            Assert.AreEqual("body", sr.SelectorText);
        }

        [Test]
        public void StyleSheet()
        {
            CSSStyleSheet s = CSSParser.Parse("body { color: black; } b { font-family: sans-serif; }");

            Assert.IsNotNull(s.CssRules);
            Assert.AreEqual(2, s.CssRules.Count);

            CSSRule r = s.CssRules[0];
            Assert.AreEqual(CSSRule.RuleType.STYLE_RULE, r.Type);
            
            CSSStyleRule sr = r as CSSStyleRule;
            Assert.AreEqual("body", sr.SelectorText);
        }

        [Test]
        public void HtmlStyles()
        {
            HtmlDocument d = new HtmlDocument();
            d.Load("test.html");
            HtmlNode n = d.GetElementbyId("test");
            CSSStyleDeclaration s = n.Style;

            Assert.AreEqual(3, s.Count);
            Assert.AreEqual("font-style", s[2]);
        }

        public static void Main(string[] args)
        {
            CSSStyleDeclaration s = CSSParser.ParseStyleDeclaration("font-size: x-large; height: 80%; line-height: 12.7em; ");
            for (int i = 0; i < s.Count; ++i)
                Console.WriteLine("{0}: {1}", s[i], s.GetPropertyValue(s[i]));

            Console.WriteLine(s.Count);
        }
    }
}
