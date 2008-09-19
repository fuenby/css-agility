using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace CSSAgilityPack
{
    class Program
    {
        static void Main(string[] args)
        {
            Debug.Assert(CSSParser.Parse("     \t\n\t   ") != null);
            Debug.Assert(CSSParser.Parse("<!-- -->") != null);
            Debug.Assert(CSSParser.Parse("body {color: red; }") != null);
            Debug.Assert(CSSParser.Parse("body {;;}") != null);
        }
    }
}
