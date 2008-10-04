using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace CSSAgilityPack {
	public class CSSStyleSheet {
		List<CSSRule> rules_;

		public List<CSSRule> CssRules {
			get { return rules_; }
		}

		public CSSRule OwnerRule {
			get { return null; }
		}

		public CSSStyleSheet(List<CSSRule> rules) {
			rules_ = rules;
		}

		CSSStyleSheet Load(string fileName) {
			return Load(new StreamReader(fileName));
		}

		CSSStyleSheet Load(StreamReader r) {
			string text = r.ReadToEnd();
			int pos = 0, len = text.Length;

			return CSSParser.Parse(text, ref pos, ref len);
		}
	}

	public class CSSProp {
		string attr_, value_;

		public CSSProp(string attr, string value) {
			attr_ = attr;
			value_ = value;
		}

		public string Name { get { return attr_; } }
		public string Value { get { return value_; } }
	}
}
