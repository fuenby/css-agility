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

            Parse();
        }

        public string text_;
        public int pos_;

        public List<CSSRule> Parse(string what)
        {
            text_ = what; pos_ = 0;
            return Parse();
        }

        public List<CSSRule> Parse()
        {
            var result = new List<CSSRule>();

            pos_ = 0;
            while (pos_ < text_.Length)
            {
                CSSRule r = ParseRule();
                if (r != null)
                {
                    result.Add(r);
                    continue;
                }
                
                if (AcceptCDO())
                    continue;

                if (AcceptCDC())
                    continue;

                AcceptWhitespace();
            }

            return result;
        }

        public CSSRule ParseRule()
        {
            int mark = pos_;

            List<string> selectors = new List<string>();

            string s = ParseSelector();
            while (s != null)
            {
                selectors.Add(s);
                s = ParseSelector();
            }
            if (selectors.Count == 0)
                return null;

            AcceptWhitespace();

            if (!AcceptChar('{'))
            {
                pos_ = mark;
                return null;
            }

            CSSStyleDeclaration b = ParseStyleDeclaration();
            if (b == null)
            {
                pos_ = mark;
                return null;
            }

            AcceptWhitespace();
            if (!AcceptChar('}'))
            {
                pos_ = mark;
                return null;
            }
            
            return new CSSRule(selectors, b);
        }

        public string ParseSelector()
        {
            return ParseWord();
        }

        public string ParseWord()
        {
            int mark = pos_;
            StringBuilder result = new StringBuilder();

            AcceptWhitespace();
            while (pos_ < text_.Length)
            {
                if (!char.IsLetterOrDigit(text_[pos_]))
                    break;

                result.Append(text_[pos_++]);
            }
            AcceptWhitespace();

            if (result.Length == 0)
            {
                pos_ = mark;
                return null;
            }

            return result.ToString();
        }

        public CSSStyleDeclaration ParseStyleDeclaration()
        {
            int mark = pos_;

            var decls = new List<CSSDeclaration>();

            while (true)
            {
                var decl = ParseDeclaration();
                if (decl != null)
                    decls.Add(decl);

                if (!AcceptChar(';'))
                    break;
            }

            return new CSSStyleDeclaration(text_.Substring(mark, pos_ - mark), decls);
        }

        CSSDeclaration ParseDeclaration()
        {
            int mark = pos_;

            string attr = ParseAttribute();
            if (attr == null)
            {
                pos_ = mark;
                return null;
            }

            if (!AcceptChar(':'))
            {
                pos_ = mark;
                return null;
            }

            string value = ParseValue();
            if (value == null)
            {
                pos_ = mark;
                return null;
            }

            return new CSSDeclaration(attr, value);
        }

        string ParseAttribute()
        {
            return ParseWord();
        }

        string ParseValue()
        {
            return ParseWord();
        }

        public bool AcceptCDO()
        {
            return AcceptString("<!--");
        }

        public bool AcceptCDC()
        {
            return AcceptString("-->");
        }

        public bool AcceptString(string what)
        {
            int mark = pos_;

            int i = 0;
            while (pos_ < text_.Length && i < what.Length && text_[pos_] == what[i])
            {
                pos_++; i++;
            }

            if (i == what.Length)
                return true;

            pos_ = mark;
            return false;
        }

        public bool AcceptChar(char ch)
        {
            if (pos_ < text_.Length && text_[pos_] == ch)
            {
                pos_++;
                return true;
            }

            return false;
        }

        public void AcceptWhitespace()
        {
            while (pos_ < text_.Length && char.IsWhiteSpace(text_[pos_]))
            {
               pos_++;
            }
        }
    }

    public class Token
    {
        public static Token Valid = new Token();
    };

    public class CSSStyleDeclaration
    {
        List<CSSDeclaration> decls_;
        string cssText_;

        public CSSStyleDeclaration(string cssText, List<CSSDeclaration> decls)
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
            get
            {
                throw new NotImplementedException();
            }
            set { throw new NotImplementedException(); }
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

    public class CSSDeclaration
    {
        string attr_, value_;

        public CSSDeclaration(string attr, string value)
        {
            attr_ = attr;
            value_ = value;
        }
    }
}
