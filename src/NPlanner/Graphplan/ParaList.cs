using System.Collections.Generic;
using System.Text;

namespace NPlanner.Graphplan
{
    /// <summary>
    /// foamliu, 2009/01/19, 操作符模板中的参数列表.
    /// </summary>
    public class ParaList
    {
        #region Fields
        private List<string> m_types;
        private List<string> m_vars;
        #endregion

        #region Properties
        public List<string> Types
        {
            get { return this.m_types; }
        }

        public List<string> Vars
        {
            get { return this.m_vars; }
        }
        #endregion

        #region Constructors

        public ParaList()
        {
        }

        public ParaList(List<string> types, List<string> vars)
        {
            this.m_types = types;
            this.m_vars = vars;
        }

        public ParaList(string para)
        {
            this.m_types = new List<string>();
            this.m_vars = new List<string>();

            if (para.IndexOf(",") > 0)
            {
                string[] paras = para.Split(new char[] { ',' });

                foreach (string p in paras)
                {
                    AddPara(p.Trim());
                }
            }
            else
            {
                AddPara(para.Trim());
            }
        }
        #endregion

        #region Methods

        private void AddPara(string p)
        {
            string[] tokens = p.Split();
            this.m_types.Add(tokens[0]);
            this.m_vars.Add(tokens[1]);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            int count = this.m_vars.Count;
            for (int i = 0; i < count; i++)
            {
                sb.Append(this.m_types[i] + " " + this.m_vars[i] + ", ");
            }
            string s = sb.ToString();
            return s.Substring(0,s.Length-2);
        }



        #endregion
    }
}
