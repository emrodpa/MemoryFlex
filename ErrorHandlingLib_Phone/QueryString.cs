using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrorHandlingLib_Phone
{
    public partial class QueryString
    {
        protected Dictionary<string, string> m_DicKeysAndValues;

        public static char Separator = '|';

        public QueryString()
        {
            m_DicKeysAndValues = new Dictionary<string, string>();

        }  // of QueryString()

        public QueryString(string p_strQueryString)
            : this()
        {
            List<string> l_Pairs = new List<string>(p_strQueryString.Split(QueryString.Separator));

            foreach (string pair in l_Pairs)
            {
                string[] l_Arr = pair.Split('=');

                m_DicKeysAndValues.Add(l_Arr[0], l_Arr[1]);

            }  // of foreach

        }  // of QueryString()

        public bool AddParam(string p_Name, string p_Value)
        {
            bool l_bSuccess = true;

            if (m_DicKeysAndValues.ContainsKey(p_Name))
                l_bSuccess = false;
            else
            {
                m_DicKeysAndValues.Add(p_Name, p_Value);
            }  // of if/else

            return l_bSuccess;

        }  // of AddParam()

        public bool UpdateParam(string p_Name, string p_Value)
        {
            bool l_bSuccess = true;

            if (!m_DicKeysAndValues.ContainsKey(p_Name))
                l_bSuccess = false;
            else
            {
                m_DicKeysAndValues[p_Name] = p_Value;
            }  // of if/else

            return l_bSuccess;

        }  // of AddParam()

        public override string ToString()
        {
            StringBuilder l_StringBuilder = new StringBuilder();

            foreach (string name in m_DicKeysAndValues.Keys)
            {
                l_StringBuilder.Append(string.Format("&{0}={1}", name, m_DicKeysAndValues[name]));

            }  // of foreach

            l_StringBuilder.Remove(0, 1);

            return l_StringBuilder.ToString();

        }  // of ToString()

    }  // of class QueryString

}  // of namespace ErrorHandlingLib_Phone
