using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace CSSAgilityPack
{
    public class CSSStyleSheet
    {
        List<CSSRule> rules_;

        public List<CSSRule> CssRules
        {
            get { return rules_; }
        }

        public CSSRule OwnerRule
        {
            get { return null; }
        }

        public CSSStyleSheet(List<CSSRule> rules)
        {
            rules_ = rules;
        }

        CSSStyleSheet Load(string fileName)
        {
            return Load(new StreamReader(fileName));
        }

        CSSStyleSheet Load(StreamReader r)
        {
            string text = r.ReadToEnd();
            int pos = 0, len = text.Length;

            return CSSParser.Parse(text, ref pos, ref len);
        }
    }

    public class CSSRule
    {
        public enum RuleType { UNKNOWN_RULE = 0, STYLE_RULE, CHARSET_RULE, IMPORT_RULE, MEDIA_RULE, FONT_FACE_RULE, PAGE_RULE };

        public CSSRule()
        {
            type_ = RuleType.UNKNOWN_RULE;
        }

        public RuleType Type
        {
            get { return type_; }
        }

        public string CssText { get { throw new NotImplementedException(); } }

        public CSSRule OwnerRule { get { return null; } }

        public CSSStyleSheet ParentStyleSheet { get { return null; } }

        protected RuleType type_;
    }

    public class CSSStyleRule : CSSRule
    {
        public CSSStyleRule(List<string> selectors, CSSStyleDeclaration style)
        {
            selectors_ = selectors;
            style_ = style;
            type_ = RuleType.STYLE_RULE;
        }

        public string SelectorText { get { return String.Join(" ", selectors_.ToArray()); } }

        public CSSStyleDeclaration Style { get { return style_; } }

        List<string> selectors_;
        CSSStyleDeclaration style_;
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
