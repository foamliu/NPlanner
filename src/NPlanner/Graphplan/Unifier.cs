using System.Collections.Generic;

namespace NPlanner.Graphplan
{
    /// <summary>
    /// foamliu, 2009/01/20.
    /// 变量名和值的映射表, 比如:
    ///  * -------------------------- 
    ///  *   var      |  value
    ///  * --------------------------
    ///  *   ?x	      |  A
    ///  *   ?y	      |  B
    ///  *   ?z       |  C
    ///  * --------------------------
    /// </summary>
    public class Unifier
    {
        #region Fields
        private Dictionary<string, string> m_table;
        #endregion

        #region Properties
        public Dictionary<string, string> Table
        {
            get { return m_table; }
        }
        #endregion

        #region Constructors
        public Unifier()
        {
            m_table = new Dictionary<string, string>();
        }

        public Unifier(string varList, string valList)
        {
            m_table = new Dictionary<string, string>();

            //string[] varTokens = varList.Split(
            //    new string[]{" ","(",")",",","\t","\n","\r","\f"}, 
            //    StringSplitOptions.RemoveEmptyEntries);
            //string[] valTokens = valList.Split(
            //    new string[] { " ", "(", ")", ",", "\t", "\n", "\r", "\f" },
            //    StringSplitOptions.RemoveEmptyEntries);

            string[] varTokens = Util.Split(varList);
            string[] valTokens = Util.Split(valList);

            for (int i = 0; i < varTokens.Length; i++)
            {
                m_table.Add(varTokens[i], valTokens[i]);
            }
        }

        public Unifier(List<string> varList, List<string> valList)
        {
            m_table = new Dictionary<string, string>();

            for (int i = 0; i < varList.Count; i++)
            {
                m_table.Add(varList[i], valList[i]);
            }
        }

        public Unifier(List<string> varList, string valList)
        {
            m_table = new Dictionary<string, string>();

            string[] valTokens = Util.Split(valList);

            for (int i = 0; i < varList.Count; i++)
            {
                m_table.Add(varList[i], valTokens[i]);
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// foamliu, 2009/01/22.
        /// </summary>
        /// <param name="var"></param>
        /// <returns></returns>
        public string Get(string var)
        {
            string val = m_table[var];
            return val;
        }
        #endregion
    }
}
