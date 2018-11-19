using System;
using System.Collections.Generic;
using System.Text;

namespace NPlanner.Graphplan
{
    public class OperatorHead
    {
        /// <summary>
        /// Operator 的名字.
        /// </summary>
        private string m_opName;

        /// <summary>
        /// Operator 的参数列表.
        /// </summary>
        private ParaList m_paraList;

        public string OpName
        {
            get { return this.m_opName; }
        }

        public ParaList ParaList
        {
            get { return this.m_paraList; }
        }

        public OperatorHead(String theName, ParaList theParaList)
        {
            this.m_opName = theName;
            this.m_paraList = theParaList;
        }
    }
}
