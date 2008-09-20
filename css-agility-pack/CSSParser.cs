﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

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

            if (!AcceptChar('{', what, ref pos))
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
            if (!AcceptChar('}', what, ref pos))
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

                if (!AcceptChar(';', what, ref pos))
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

            if (!AcceptChar(':', what, ref pos))
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
            StringBuilder result = new StringBuilder();

            while (true)
            {
                string t = ParseAny(what, ref pos, ref end);
                if (t != null)
                {
                    if (result.Length != 0)
                        result.Append(' ');
                    result.Append(t);
                }
                else
                    break;
            }

            return result.ToString();
        }

        static string ParseAny(string what, ref int pos, ref int end)
        {
            string result = ParseIdent(what, ref pos, ref end)
                ?? ParseString(what, ref pos, ref end)
                ?? ParsePercentage(what, ref pos, ref end)
                ?? ParseDimension(what, ref pos)
                ?? ParseNumber(what, ref pos);
            if (result != null)
                AcceptWhitespace(what, ref pos, ref end);

            return result;
        }

        public static string ParseIdent(string what, ref int pos, ref int end)
        {
            int ps = pos;
            StringBuilder result = new StringBuilder();

            if (AcceptChar('-', what, ref ps))
                result.Append('-');

            if (!AcceptNmstart(what, ref ps, ref end))
                return null;
            result.Append(what[ps - 1]);

            while (AcceptNmchar(what, ref ps, ref end))
                result.Append(what[ps - 1]);

            pos = ps;
            return result.ToString();
        }

        public static string ParseString(string what, ref int pos, ref int end)
        {
            int mark = pos;
            StringBuilder result = new StringBuilder();

            if (pos >= end)
                return null;

            char sep = what[pos];
            if (sep != '\'' && sep != '"')
                return null;

            AcceptChar(sep, what, ref pos);
            
            result.Append(sep);
            while (pos < end && "\n\t\r".IndexOf(what[pos]) < 0 && what[pos] != sep)
            {
                result.Append(what[pos++]);
            }

            if (!AcceptChar(sep, what, ref pos))
            {
                pos = mark;
                return null;
            }

            result.Append(sep);
            return result.ToString();
        }

        static string ParseDimension(string what, ref int pos)
        {
            int ps = pos;
            int en = what.Length;
            StringBuilder result = new StringBuilder();

            string num = ParseNumber(what, ref ps);
            if (num == null)
                return null;
            result.Append(num);

            string dim = ParseIdent(what, ref ps, ref en);
            if (dim == null)
                return null;
            result.Append(dim);

            pos = ps;
            return result.ToString();
        }

        static string ParsePercentage(string what, ref int pos, ref int end)
        {
            int ps = pos;
            StringBuilder result = new StringBuilder();

            while (ps < end && char.IsDigit(what[ps]))
                result.Append(what[ps++]);

            if (result.Length == 0)
                return null;

            if (!AcceptChar('%', what, ref ps))
                return null;

            result.Append('%');
            pos = ps;
            return result.ToString();
        }

        static string ParseNumber(string what, ref int pos)
        {
            int ps = pos;
            StringBuilder result = new StringBuilder();
            
            while (AcceptEither("0-9", what, ref ps))
                result.Append(what[ps - 1]);

            if (AcceptChar('.', what, ref ps))
            {
                result.Append('.');
                if (!AcceptEither("0-9", what, ref ps))
                    return null;
                while (AcceptEither("0-9", what, ref ps))
                    result.Append(what[ps - 1]);
            }

            if (result.Length == 0)
                return null;

            pos = ps;
            return result.ToString();
        }

        //static string ParseDimension(string what, ref int pos)
        //{
        //    int ps = pos;

        //}

        static bool AcceptNmstart(string what, ref int pos, ref int end)
        {
            return AcceptEither("_a-z", what, ref pos);
        }

        static bool AcceptNmchar(string what, ref int pos, ref int end)
        {
            return AcceptEither("-_a-z", what, ref pos);
        }

        static bool AcceptEither(string e, string what, ref int pos)
        {
            if (pos >= what.Length)
                return false;

            for (int i = 0; i < e.Length; ++i)
            {
                if (e[i] == '-' && i != 0)
                {
                    if (e[i-1] <= what[pos] && what[pos] <= e[i+1])
                    {
                        pos++;
                        return true;
                    }
                }
                else 
                {
                    if (what[pos] == e[i])
                    {
                        pos++;
                        return true;
                    }
                }
            }

            return false;
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

        public static bool AcceptChar(char ch, string what, ref int pos)
        {
            if (pos < what.Length && what[pos] == ch)
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
