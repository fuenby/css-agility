using System;
using System.Collections.Generic;
using System.Text;

namespace CSSAgilityPack {
	public class CSSValue {
		public enum Type { CSS_INHERIT = 0, CSS_PRIMITIVE_VALUE = 1, CSS_VALUE_LIST = 2, CSS_CUSTOM = 3 }

		public string CssText { get { throw new NotImplementedException(); } }

		public Type CssValueType { get { throw new NotImplementedException(); } }
	}

	public class CSSStyleDeclaration {
		List<CSSProp> decls_;
		string cssText_;

		public CSSStyleDeclaration(string cssText, List<CSSProp> decls) {
			cssText_ = cssText;
			decls_ = decls;
		}

		public string CssText {
			get {
				if (cssText_ == null) {
					List<string> texts = new List<string>();
					foreach (CSSProp decl in decls_) {
						texts.Add(decl.Name + ": " + decl.Value);
					}
					cssText_ = String.Join("; ", texts.ToArray());
				}

				return cssText_;
			}
			set {
				throw new NotImplementedException();
			}
		}

		public int Count {
			get { return decls_.Count; }
		}

		public CSSRule ParentRule {
			get { throw new NotImplementedException(); }
		}

		public string this[int index] {
			get { return decls_[index].Name; }
		}

		public string GetPropertyValue(string propertyName) {
			foreach (CSSProp decl in decls_)
				if (decl.Name == propertyName)
					return decl.Value;

			return string.Empty;
		}

		public CSSValue GetPropertyCSSValue(string propertyName) {
			throw new NotImplementedException();
		}

		public string RemoveProperty(string propertyName) {
			foreach (CSSProp decl in decls_) {
				if (decl.Name == propertyName) {
					decls_.Remove(decl);
					cssText_ = null;
					return decl.Value;
				}
			}

			return String.Empty;
		}

		public void SetProperty(string propertyName, string value, string priority) {
			cssText_ = null;

			for (int i = 0; i < decls_.Count; ++i) {
				if (decls_[i].Name == propertyName) {
					decls_[i] = new CSSProp(propertyName, value);
					return;
				}
			}

			decls_.Add(new CSSProp(propertyName, value));
		}
	}
}
