using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace CompareXE
{
    public class CompareXElements
    {
        /// <summary>
        /// Deep comparison of two xml in form of string.
        /// Throws CompareXElementException if any difference found according to options.
        /// </summary>
        public static void CompareDeep(string xml1, string xml2, CompareXElementOptions opt = null)
        {
            var e1 = XElement.Parse(xml1);
            var e2 = XElement.Parse(xml2);
            CompareDeep(e1, e2, opt);
        }

        /// <summary>
        /// Deep comparison of two XElements.
        /// Throws CompareXElementException if any difference found according to options.
        /// </summary>
        public static void CompareDeep(XElement e1, XElement e2, CompareXElementOptions opt = null)
        {
            if (opt == null)
                opt = new CompareXElementOptions(); //default values

            //compare elements
            CompareShallow(e1, e2, opt);

            //compare attributes
            //2-step algorithm to find not only absent but extra attributes in e2
            for (int j = 1; j < 3; j++)
            {
                var o1 = e1;
                var o2 = e2;
                if (j == 2)
                {
                    o1 = e2;
                    o2 = e1;
                }

                var a1List = o1.Attributes().Where(a => a.Name.NamespaceName != XNamespace.Xmlns.NamespaceName && a.Name.LocalName != "xmlns");
                var a2List = o2.Attributes().Where(a => a.Name.NamespaceName != XNamespace.Xmlns.NamespaceName && a.Name.LocalName != "xmlns");
                var a1Cnt = a1List.Count();
                var a2Cnt = a2List.Count();
                for (int i = 0; i < a1Cnt; i++)
                {
                    var a1 = a1List.ElementAt(i);

                    XAttribute a2 = null;
                    if (i < a2Cnt)
                        a2 = a2List.ElementAt(i);

                    if (opt.IgnoreChildOrder)
                    {
                        a2 = a2List.FirstOrDefault(a =>
                        {
                            return (opt.IgnoreNamespaces ? true : a.Name.Namespace == a1.Name.Namespace) &&
                                    (opt.IgnoreCaseInNames ? a.Name.LocalName.ToLower() == a1.Name.LocalName.ToLower() : a.Name.LocalName == a1.Name.LocalName);
                        });
                    }

                    if (a2 == null)
                        throw new CompareXElementException(string.Format(
                            j == 1 ? "Attribute '{0}' not found in element '{1}'" : "Unexpected attribute '{0}' in element '{1}'", 
                            a1.Name.LocalName, o1.Name.LocalName), o1);

                    if (j == 1) //compare only once
                        CompareShallow(a1, a2, opt);
                }
            }

            //check children
            for (int j = 1; j < 3; j++)
            {
                var o1 = e1;
                var o2 = e2;
                if (j == 2)
                {
                    o1 = e2;
                    o2 = e1;
                }

                var c1Cnt = o1.Elements().Count();
                var c2Cnt = o2.Elements().Count();

                for (int i = 0; i < c1Cnt; i++)
                {
                    var c1 = o1.Elements().ElementAt(i);
                    XElement c2 = null;
                    if (i < c2Cnt)
                        c2 = o2.Elements().ElementAt(i);

                    if (opt.IgnoreChildOrder)
                    {
                        c2 = o2.Elements().FirstOrDefault(e =>
                        {
                            return (opt.IgnoreNamespaces ? true : e.Name.Namespace == c1.Name.Namespace) &&
                                    (opt.IgnoreCaseInNames ? e.Name.LocalName.ToLower() == c1.Name.LocalName.ToLower() : e.Name.LocalName == c1.Name.LocalName);
                        });
                    }

                    if (c2 == null)
                        throw new CompareXElementException(string.Format(
                            j == 1 ? "Element '{0}' not found inside element '{1}'" : "Unexpected element '{0}' inside element '{1}'", 
                            c1.Name.LocalName, o1.Name.LocalName), c1);

                    if (j == 1)
                        CompareDeep(c1, c2, opt);
                }
            }
        }

        /// <summary>
        /// Shallow comparison of elements or attributes (only names and values without childs).
        /// Throws CompareXElementException if any difference found according to options.
        /// </summary>
        public static void CompareShallow(XObject o1, XObject o2, CompareXElementOptions opt)
        {
            if (o1 is XElement)
            {
                var e1 = (XElement)o1;
                var e2 = (XElement)o2;

                if (opt.IgnoreCaseInNames ? e1.Name.LocalName.ToLower() != e2.Name.LocalName.ToLower() : e1.Name.LocalName != e2.Name.LocalName)
                    throw new CompareXElementException(string.Format("Invalid element name '{0}', expect '{1}' {2}", e2.Name.LocalName, e1.Name.LocalName,
                        e1.Parent != null ? "in element '" + e1.Parent.Name.LocalName + "'" : "for root"), e2);

                if (!opt.IgnoreNamespaces && e1.Name.Namespace != e2.Name.Namespace)
                    throw new CompareXElementException(string.Format("Invalid namespace '{0}' (expect '{1}') for element '{2}'", e2.Name.Namespace, e1.Name.Namespace, e1.Name.LocalName), e2);

                var e1Cnt = e1.Elements().Count();
                var e2Cnt = e2.Elements().Count();
                var t1 = (XText)e1.Nodes().Where(n => n.NodeType == XmlNodeType.Text).FirstOrDefault();
                var t2 = (XText)e2.Nodes().Where(n => n.NodeType == XmlNodeType.Text).FirstOrDefault();
                if (t1 != null && t2 == null)
                    throw new CompareXElementException(string.Format("Expect text value '{0}' for element '{1}'", t1.Value, e1.Name.LocalName), e2);
                if (t1 == null && t2 != null && e1Cnt > 0)
                    throw new CompareXElementException(string.Format("Element '{0}' must be container, not text '{1}'", e1.Name.LocalName, t2.Value), e2);
                if (t1 == null && t2 != null && e1Cnt == 0)
                    throw new CompareXElementException(string.Format("Element '{0}' must be empty, not text '{1}'", e1.Name.LocalName, t2.Value), e2);
                if (t1 != null && t2 != null && t1.Value != t2.Value)
                    throw new CompareXElementException(string.Format("Invalid element value '{0}', expect '{1}' for element '{2}'", e2.Value, e1.Value, e1.Name.LocalName), e2);
            }
            else if (o1 is XAttribute)
            {
                var e1 = (XAttribute)o1;
                var e2 = (XAttribute)o2;

                if (opt.IgnoreCaseInNames ? e1.Name.LocalName.ToLower() != e2.Name.LocalName.ToLower() : e1.Name.LocalName != e2.Name.LocalName)
                    throw new CompareXElementException(string.Format("Invalid attribute name '{0}', expect '{1}' for element '{2}'", e2.Name.LocalName, e1.Name.LocalName, e1.Parent.Name.LocalName), e2);

                if (!opt.IgnoreNamespaces && e1.Name.Namespace != e2.Name.Namespace)
                    throw new CompareXElementException(string.Format("Invalid attribute '{0}' namespace '{1}' (expect '{2}') for element '{3}'", e1.Name.LocalName, e2.Name.Namespace, e1.Name.Namespace, e1.Parent.Name.LocalName), e2);

                if (e1.Value != e2.Value)
                    throw new CompareXElementException(string.Format("Invalid value '{0}' (expect '{1}') in attribute '{2}' for element '{3}'", e2.Value, e1.Value, e1.Name.LocalName, e1.Parent.Name.LocalName), e2);
            }
            else
            {
                throw new Exception("Only XElement and XAttribute can be compared");
            }
        }
    }

    public class CompareXElementOptions
    {
        public bool IgnoreChildOrder = false;
        public bool IgnoreNamespaces = false;
        public bool IgnoreCaseInNames = false;
    }

    /// <summary>
    /// Compare difference exception, see Message to get human readable detail in form 'what:where'
    /// </summary>
    public class CompareXElementException : Exception
    {
        //where
        public XObject O { get; set; }
        //what different
        public string Msg { get; set; }

        public CompareXElementException(string msg, XObject o)
        {
            O = o;
            Msg = msg;
        }

        public override string Message
        {
            get
            {
                //return string.Format("{0}. Full path to invalid node: '{1}'", Msg, Path(O));
                return Msg;
            }
        }

        private string Path(XObject o)
        {
            string fullName = null;
            while (o != null)
            {
                string name = "";
                if (o is XAttribute)
                    name = "@" + (o as XAttribute).Name.LocalName;
                else if (o is XElement)
                    name = (o as XElement).Name.LocalName;
                else
                    name = "[" + o.NodeType.ToString() + "]";

                if (fullName == null)
                    fullName = name;
                else
                    fullName = name + "/" + fullName;

                o = o.Parent;
            }
            return fullName;
        }
    }
}
