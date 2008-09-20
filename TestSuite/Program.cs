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
            CSSStyleDeclaration s = CSSParser.ParseStyleDeclaration("color: red; font-family: 'Helvetica'; font-style: italic;");

            Assert.AreEqual(3, s.Count);
            Assert.AreEqual("italic", s.GetPropertyValue("font-style"));
        }

        [Test]
        public void SpaceSeparatedValues()
        {
            CSSStyleDeclaration s = CSSParser.ParseStyleDeclaration("border: 1px solid gold;color: blue");

            Assert.AreEqual(2, s.Count);
            Assert.AreEqual("blue", s.GetPropertyValue("color"));
        }

        [Test]
        public void Whitespace()
        {
            CSSStyleDeclaration s = CSSParser.ParseStyleDeclaration(" border : 1px solid ; color : \r\tblue ");

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

        public static void Main(string[] args)
        {
            CSSStyleDeclaration s = CSSParser.ParseStyleDeclaration("font-size: x-large; height: 80%; line-height: 12.7em; ");
            for (int i = 0; i < s.Count; ++i)
                Console.WriteLine("{0}: {1}", s[i], s.GetPropertyValue(s[i]));

            Console.WriteLine(s.Count);
        }
    }
}
