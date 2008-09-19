using System;
using System.Collections.Generic;
using System.Text;

namespace CSSAgilityPack
{
    public class CSSParser
    {
        public static CSSStyleSheet Parse(string what)
        {
            int pos = 0, end = what.Length;
            return Parse(what, ref pos, ref end);
        }

        public static CSSStyleSheet Parse(string what, ref int pos, ref int end)
        {
            var result = new List<CSSRule>();

            pos = 0;
            while (pos < end)
            {
                CSSRule r = ParseRule(what, ref pos, ref end);
                if (r != null)
                {
                    result.Add(r);
                    continue;
                }

                if (AcceptCDO(what, ref pos, ref end))
                    continue;

                if (AcceptCDC(what, ref pos, ref end))
                    continue;

                AcceptWhitespace(what, ref pos, ref end);
            }

            return new CSSStyleSheet(result);
        }

        public static CSSRule ParseRule(string what, ref int pos, ref int end)
        {
            int mark = pos;

            List<string> selectors = new List<string>();

            string s = ParseSelector(what, ref pos, ref end);
            while (s != null)
            {
                selectors.Add(s);
                s = ParseSelector(what, ref pos, ref end);
            }
            if (selectors.Count == 0)
                return null;

            AcceptWhitespace(what, ref pos, ref end);

            if (!AcceptChar('{', what, ref pos, ref end))
            {
                pos = mark;
                return null;
            }

            CSSStyleDeclaration b = ParseStyleDeclaration(what, ref pos, ref end);
            if (b == null)
            {
                pos = mark;
                return null;
            }

            AcceptWhitespace(what, ref pos, ref end);
            if (!AcceptChar('}', what, ref pos, ref end))
            {
                pos = mark;
                return null;
            }

            return new CSSStyleRule(selectors, b);
        }

        public static string ParseSelector(string what, ref int pos, ref int end)
        {
            return ParseIdent(what, ref pos, ref end);
        }

        public static CSSStyleDeclaration ParseStyleDeclaration(string what)
        {
            int pos = 0;
            int end = what.Length;
            return ParseStyleDeclaration(what, ref pos, ref end);
        }

        public static CSSStyleDeclaration ParseStyleDeclaration(string what, ref int pos, ref int end)
        {
            int mark = pos;

            var decls = new List<CSSProp>();

            while (true)
            {
                var decl = ParseDeclaration(what, ref pos, ref end);
                if (decl != null)
                    decls.Add(decl);

                if (!AcceptChar(';', what, ref pos, ref end))
                    break;
            }

            return new CSSStyleDeclaration(what.Substring(mark, pos - mark), decls);
        }

        public static CSSProp ParseDeclaration(string what, ref int pos, ref int end)
        {
            int mark = pos;

            AcceptWhitespace(what, ref pos, ref end);

            string attr = ParseProperty(what, ref pos, ref end);
            if (attr == null)
            {
                pos = mark;
                return null;
            }

            AcceptWhitespace(what, ref pos, ref end);

            if (!AcceptChar(':', what, ref pos, ref end))
            {
                pos = mark;
                return null;
            }

            AcceptWhitespace(what, ref pos, ref end);

            string value = ParseValue(what, ref pos, ref end);
            if (value == null)
            {
                pos = mark;
                return null;
            }

            return new CSSProp(attr, value);
        }

        static string ParseProperty(string what, ref int pos, ref int end)
        {
            return ParseIdent(what, ref pos, ref end);
        }

        static string ParseValue(string what, ref int pos, ref int end)
        {
            return ParseIdent(what, ref pos, ref end);
        }

        public static string ParseIdent(string what, ref int pos, ref int end)
        {
            int mark = pos;
            StringBuilder result = new StringBuilder();

            while (pos < end)
            {
                if (!char.IsLetterOrDigit(what[pos]) && what[pos] != '-' && what[pos] != '_')
                    break;

                result.Append(what[pos++]);
            }

            if (result.Length == 0)
            {
                pos = mark;
                return null;
            }

            return result.ToString();
        }

        static public bool AcceptCDO(string what, ref int pos, ref int end)
        {
            return AcceptString("<!--", what, ref pos, ref end);
        }

        static public bool AcceptCDC(string what, ref int pos, ref int end)
        {
            return AcceptString("-->", what, ref pos, ref end);
        }

        static public bool AcceptString(string s, string what, ref int pos, ref int end)
        {
            int mark = pos;

            int i = 0;
            while (pos < end && i < s.Length && what[pos] == s[i])
            {
                pos++; i++;
            }

            if (i == s.Length)
                return true;

            pos = mark;
            return false;
        }

        public static bool AcceptChar(char ch, string what, ref int pos, ref int end)
        {
            if (pos < end && what[pos] == ch)
            {
                pos++;
                return true;
            }

            return false;
        }

        public static void AcceptWhitespace(string what, ref int pos, ref int end)
        {
            while (pos < end && char.IsWhiteSpace(what[pos]))
                pos++;
        }
    }
}
