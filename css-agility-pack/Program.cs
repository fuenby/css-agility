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
            CSSStyleSheet c = new CSSStyleSheet();

            Debug.Assert(c.Parse("     \t\n\t   ") != null);
            Debug.Assert(c.Parse("<!-- -->") != null);
            Debug.Assert(c.Parse("body {color: red; }") != null);
            Debug.Assert(c.Parse("body {;;}") != null);
        }
    }
}
