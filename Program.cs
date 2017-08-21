using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace CompareXE
{
    class Program
    {
        static void Main(string[] args)
        {
            //pairs of different xml (or equal)
            string[][] xml = {
                new string[] { "<a/>", "<b/>"},
                new string[] { "<a/>", "<a></a>"},
                new string[] { "<a></a>", "<a b='c'></a>" },
                new string[] { "<a b='c'></a>", "<a></a>" },
                new string[] { "<a><b></b></a>", "<a></a>" },
                new string[] { "<a></a>", "<a><b></b></a>" },
                new string[] { "<a><b>a</b></a>", "<a><b></b></a>"},
                new string[] { "<a><b></b></a>", "<a><b>a</b></a>"},
                new string[] { "<a b='10' d='20'></a>", "<a d='20' b='10'></a>" },
                new string[] { "<a b='10' d='20'></a>", "<a d='10' B='20'></a>" },
                new string[] { "<a><b><c></c></b></a>", "<a><b><C></C></b></a>" },
                new string[] { "<b><c:a xmlns:c='aaa' d='1'></c:a></b>", "<b><a d='1'></a></b>" },
                new string[] { "<a xmlns='aaa'></a>", "<A></A>" },
                new string[] { "<c xmlns:a='aaa' a:d='20'><a b='10'>aaa</a></c>", "<c d='20'><a b='10'>aaa</a></c>" },
                new string[] { "<a:c xmlns:a='aaa' d='20'><a1 b='10'>aaa</a1></a:c>", "<c d='20'><a b='10'>aaa</a></c>" },
                new string[] { "<a:c xmlns:a='aaa' a:d='20'><a1 b='10'>aaa</a1></a:c>", "<a:c xmlns:a='aaa' a:d='20'><a1 b='10'>aaa</a1></a:c>" }
            };

            for (int i = 0; i < xml.Length; i++)
            {
                try
                {
                    Console.WriteLine("============= Test #" + i);
                    Console.WriteLine("xml1: " + xml[i][0]);
                    Console.WriteLine("xml2: " + xml[i][1]);
                    Console.WriteLine();

                    XElement e1 = XElement.Parse(xml[i][0]);
                    XElement e2 = XElement.Parse(xml[i][1]);

                    Compare(e1, e2, "default");
                    Compare(e1, e2, "ignore namespaces", new CompareXElementOptions { IgnoreNamespaces = true });
                    Compare(e1, e2, "ignore order", new CompareXElementOptions { IgnoreChildOrder = true });
                    Compare(e1, e2, "ignore case", new CompareXElementOptions { IgnoreCaseInNames = true });
                    Compare(e1, e2, "ignore all", new CompareXElementOptions { IgnoreNamespaces = true, IgnoreChildOrder = true, IgnoreCaseInNames = true });
                    Console.WriteLine();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        private static void Compare(XElement e1, XElement e2, string title, CompareXElementOptions opt = null)
        {
            string msg = "xml are equal";
            try
            {
                CompareXElements.CompareDeep(e1, e2, opt);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            Console.WriteLine(title + ": " + msg);
        }
    }
}
