using System;
using System.Collections.Generic;
using System.Text;

namespace NPlanner.Graphplan
{
    /// <summary>
    /// foamliu, 2009/01/19, 实现一个命题 (Proposition) 类.
    /// </summary>
    public class Proposition
    {
        #region Fields
        private string m_name;

        // 前提包含本命题的行为集合.
        private List<Action> m_preEdges;
        // Add-Effects包含本命题的行为集合.
        private List<Action> m_addEdges;
        // Del-Effects包含本命题的行为集合.
        private List<Action> m_delEdges;

        // foamliu, 2009/01/22, 与本 proposition 互斥的 propositions.
        private List<Proposition> m_mutexProps;
        #endregion

        #region Properties

        public string Name
        {
            get { return this.m_name; }
        }

        // 前提包含本命题的行为集合.
        public List<Action> PreEdges
        {
            get { return this.m_preEdges; }
        }

        // Add-Effects包含本命题的行为集合.
        public List<Action> AddEdges
        {
            get { return this.m_addEdges; }
        }

        // Del-Effects包含本命题的行为集合.
        public List<Action> DelEdges
        {
            get { return this.m_delEdges; }
        }

        // foamliu, 2009/01/22, 与本 proposition 互斥的 propositions.
        public List<Proposition> MutexProps
        {
            get { return this.m_mutexProps; }
        }
        #endregion

        #region Constructors

        public Proposition(string name)
        {
            this.m_name = name;

            this.m_preEdges = new List<Action>();
            this.m_addEdges = new List<Action>();
            this.m_delEdges = new List<Action>();

            this.m_mutexProps = new List<Proposition>();
        }
        #endregion

        #region Methods

        public void AddPreEdge(Action act)
        {
            this.m_preEdges.Add(act);
        }

        public void AddAddEdge(Action act)
        {
            this.m_addEdges.Add(act);
        }

        public void AddDelEdge(Action act)
        {
            this.m_delEdges.Add(act);
        }

        public bool IsMutex(Proposition otherProp)
        {
            return m_mutexProps.Contains(otherProp);
        }

        /// <summary>
        /// foamliu, 2009/01/22, for debug use.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(m_name);
            return sb.ToString();
        }
        #endregion
    }
}
