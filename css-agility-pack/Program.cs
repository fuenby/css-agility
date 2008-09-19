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
            Debug.Assert(CSSStyleSheet.Parse("     \t\n\t   ") != null);
            Debug.Assert(CSSStyleSheet.Parse("<!-- -->") != null);
            Debug.Assert(CSSStyleSheet.Parse("body {color: red; }") != null);
            Debug.Assert(CSSStyleSheet.Parse("body {;;}") != null);
        }
    }
}
