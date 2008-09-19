using System;
using System.Collections.Generic;
using System.Text;

namespace CSSAgilityPack
{
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
}
