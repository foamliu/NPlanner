using System.Collections.Generic;
using System.Text;

namespace NPlanner.Graphplan
{
    /// <summary>
    /// foamliu, 2009/01/19.
    /// 
    /// P0 就是初始状态 sj.
    /// Pj 包括满足下列条件的命题p:
    /// p 属于 P(j-1) 或者 存在 a 属于 Aj, p 属于 effects+(a).
    /// </summary>
    public class PropositionLayer
    {
        #region Fields

        // foamliu, 2009/01/22, 这是第几层, 参见 Graphplan 类头的注释.
        private int m_number;

        private ActionLayer m_prev;
        private ActionLayer m_next;

        private Dictionary<string, Proposition> m_dict;
        private List<Proposition> m_props;
        private Conjunction m_conj;
        #endregion

        #region Properties

        public int Number
        {
            get { return m_number; }
            set { m_number = value; }
        }

        public ActionLayer PrevLayer
        {
            get { return m_prev; }
            set { m_prev = value; }
        }

        public ActionLayer NextLayer
        {
            get { return m_next; }
            set { m_next = value; }
        }

        public Conjunction Conjunction
        {
            get { return m_conj; }            
        }
        #endregion

        #region Constructors
        public PropositionLayer()
        {
            this.m_props = new List<Proposition>();
            this.m_dict = new Dictionary<string, Proposition>();
            this.m_conj = new Conjunction();
        }

        public PropositionLayer(ActionLayer prev)
        {
            this.m_prev = prev;
            this.m_props = new List<Proposition>();
            this.m_dict = new Dictionary<string, Proposition>();
            this.m_conj = new Conjunction();
        }
        #endregion

        #region Methods
        public void SetInitLayer(Conjunction conjunct)
        {
            foreach (string literal in conjunct.Literals)
            {
                AddProposition(literal);
            }
        }

        public Proposition AddProposition(string theName)
        {
            // foamliu, 2009/01/22, 为简洁先注掉, 为效率可能以后加上.
            //if (m_dict.ContainsKey(theName))
            //    return m_dict[theName];

            return AddProposition(new Proposition(theName));
        }

        public Proposition AddProposition(Proposition thePro)
        {
            // foamliu, 2009/01/22, fix a bug: 
            // {"An item with the same key has already been added."}.
            if (m_dict.ContainsKey(thePro.Name))
                return m_dict[thePro.Name];

            m_props.Add(thePro);
            m_dict.Add(thePro.Name, thePro);
            m_conj.Literals.Add(thePro.Name);
            return thePro;
        }

        public Proposition GetProposition(string literal)
        {
            return m_dict[literal];
        }

        /// <summary>
        /// foamliu, 2009/01/22, 检测互斥的命题.
        /// 
        /// 下面这段拷贝自如下论文的 2.2 节:
        /// "Fast Planning Through Planning Graph Analysis"
        /// Two propositions p and q in a proposition-level are marked as exclusive if all ways of
        ///     creating proposition p are exclusive of all ways of creating proposition q. Specifically, they
        ///     are marked as exclusive if each action a having an add-edge to proposition p is marked as
        ///     exclusive of each action b having an add-edge to proposition q.
        /// </summary>
        public void TestMutex()
        {
            for (int i = 0; i < m_props.Count; i++)
            {
                for (int j = i + 1; j < m_props.Count; j++)
                {
                    Proposition theProp1 = m_props[i];
                    Proposition theProp2 = m_props[j];

                    if (TestActions(theProp1, theProp2))
                    {
                        theProp1.MutexProps.Add(theProp2);
                        theProp2.MutexProps.Add(theProp1);
                    }
                }
            }
        }

        /// <summary>
        /// foamliu, 2009/01/22, 检测命题互斥.
        /// 
        /// 要求所有相关的 action 都彼此互斥.
        /// </summary>
        /// <param name="theProp1"></param>
        /// <param name="theProp2"></param>
        /// <returns></returns>
        private bool TestActions(Proposition theProp1, Proposition theProp2)
        {
            foreach (Action theAct1 in theProp1.AddEdges)
            {
                foreach (Action theAct2 in theProp2.AddEdges)
                {
                    if (theAct1.IsMutex(theAct2) == false)
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        ///  foamliu, 2009/01/22, for debug use.
        ///  
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(this.GetType().Name + " " + m_number + ":");
            foreach (Proposition prop in m_props)
            {
                sb.AppendLine(prop.ToString());
            }
            return sb.ToString();
        }
        #endregion


    }
}
