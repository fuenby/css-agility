using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace CSSAgilityPack
{
    public class CSSStyleSheet
    {
        void Load(string fileName)
        {
            Load(new StreamReader(fileName));
        }

        void Load(StreamReader r)
        {
            text_ = r.ReadToEnd();
            int pos = 0, len = text_.Length;

            Parse(text_, ref pos, ref len);
        }

        public string text_;
        public int pos_;

        public static List<CSSRule> Parse(string what)
        {
            int pos = 0, end = what.Length;
            return Parse(what, ref pos, ref end);
        }

        public static List<CSSRule> Parse(string what, ref int pos, ref int end)
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

            return result;
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
            
            return new CSSRule(selectors, b);
        }

        public static string ParseSelector(string what, ref int pos, ref int end)
        {
            return ParseWord(what, ref pos, ref end);
        }

        public static string ParseWord(string what, ref int pos, ref int end)
        {
            int mark = pos;
            StringBuilder result = new StringBuilder();

            AcceptWhitespace(what, ref pos, ref end);
            while (pos < end)
            {
                if (!char.IsLetterOrDigit(what[pos]))
                    break;

                result.Append(what[pos++]);
            }
            AcceptWhitespace(what, ref pos, ref end);

            if (result.Length == 0)
            {
                pos = mark;
                return null;
            }

            return result.ToString();
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

            string attr = ParseAttribute(what, ref pos, ref end);
            if (attr == null)
            {
                pos = mark;
                return null;
            }

            if (!AcceptChar(':', what, ref pos, ref end))
            {
                pos = mark;
                return null;
            }

            string value = ParseValue(what, ref pos, ref end);
            if (value == null)
            {
                pos = mark;
                return null;
            }

            return new CSSProp(attr, value);
        }

        static string ParseAttribute(string what, ref int pos, ref int end)
        {
            return ParseWord(what, ref pos, ref end);
        }

        static string ParseValue(string what, ref int pos, ref int end)
        {
            return ParseWord(what, ref pos, ref end);
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

    public class Token
    {
        public static Token Valid = new Token();
    };

    public class CSSStyleDeclaration
    {
        List<CSSProp> decls_;
        string cssText_;

        public CSSStyleDeclaration(string cssText, List<CSSProp> decls)
        {
            cssText_ = cssText;
            decls_ = decls;
        }

        public string CssText
        {
            get
            {
                return cssText_;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public int Count
        {
            get { return decls_.Count; }
        }

        public string this[int index]
        {
            get { return decls_[index].Name; }
        }

        public string GetPropertyValue(string propertyName)
        {
            foreach (CSSProp decl in decls_)
                if (decl.Name == propertyName)
                    return decl.Value;
            return string.Empty;
        }
    }

    public class CSSRule
    {
        public CSSRule(List<string> selectors, CSSStyleDeclaration block)
        {
            selectors_ = selectors;
            block_ = block;
        }

        List<string> selectors_;
        CSSStyleDeclaration block_;
    }

    public class CSSProp
    {
        string attr_, value_;

        public CSSProp(string attr, string value)
        {
            attr_ = attr;
            value_ = value;
        }

        public string Name { get { return attr_; } }
        public string Value { get { return value_; } }
    }
}
