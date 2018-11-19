using System.Collections.Generic;

namespace NPlanner.Graphplan
{
    /// <summary>
    /// foamliu, 2009/01/19.
    /// 
    /// In STRIPS-like planning domains, operators have preconditions, add-effects, and delete-effects, all
    ///     of which are conjuncts of propositions, and have parameters that can be instantiated to objects
    ///     in the world.
    /// </summary>
    public class Operator
    {
        #region Fields

        // head
        private OperatorHead m_opHead;
        // precondition
        private Conjunction m_pre;
        // add effects
        private Conjunction m_add;
        // del effects
        private Conjunction m_del;

        // foamliu, 2009/01/22, conditions, 输入参数(对象)需要满足的条件.
        // 例如: 
        // Move (Block ?obj, Block ?from, Block ?to)
        // 就需要:
        // ?obj != ?from & ?obj != ?to & ?from != ?to & ?to != @Table
        // 这个条件可能明显但不充分.
        private Condition m_cond;

        // 所有可能的组合
        private List<string> m_allUnifiers;
        // 符合条件的组合
        private List<string> m_validUnifiers;
        #endregion

        #region Properties
        public OperatorHead OpHead
        {
            get { return this.m_opHead; }
        }

        public Conjunction Precondition
        {
            get { return this.m_pre; }
        }

        public Conjunction AddEffects
        {
            get { return this.m_add; }
        }

        public Conjunction DelEffects
        {
            get { return this.m_del; }
        }

        // 调试用
        public List<string> AllUnifiers
        {
            get { return this.m_allUnifiers; }
        }

        public Condition Conditions
        {
            get { return this.m_cond; }
        }
        #endregion

        #region Constructors

        /// <summary>
        /// 创建一个新实例.
        /// </summary>
        //public Operator()
        //{
        //    this.m_cond = c;
        //    this.m_pre = new Conjunction();
        //    this.m_add = new Conjunction();
        //    this.m_del = new Conjunction();

        //    this.m_allUnifiers = new List<string>();
        //    this.m_validUnifiers = new List<string>();
        //}

        /// <summary>
        /// 创建一个新实例.
        /// </summary>
        public Operator(OperatorHead h, Condition c, Conjunction p, Conjunction a, Conjunction d)
        {
            this.m_opHead = h;
            this.m_cond = c;
            this.m_pre = p;
            this.m_add = a;
            this.m_del = d;

            this.m_allUnifiers = new List<string>();
            this.m_validUnifiers = new List<string>();
        }

        #endregion

        #region Methods
        /// <summary>
        /// foamliu, 2009/01/20.
        /// 找到所有这样的 actions: 它们的 Preconditions 在 thePre 里.
        /// 
        /// </summary>
        /// <param name="thePre"></param>
        /// <returns></returns>
        public List<Action> GenActions(Conjunction thePre)
        {
            List<Action> actions = new List<Action>();
            List<string> vars = m_opHead.ParaList.Vars;

            foreach (string s in m_validUnifiers)
            {
                Unifier un = new Unifier(vars, s);
                string aPre = Util.Substitute(m_pre, un);
                Conjunction conj = new Conjunction(aPre);

                if (thePre.Contains(conj))
                {
                    // 创建一个 action
                    string aHead = Util.Substitute(m_opHead, un);
                    string aAdd = Util.Substitute(m_add, un);
                    string aDel = Util.Substitute(m_del, un);

                    actions.Add(new Action(aHead, aPre, aAdd, aDel));
                }
            }

            return actions;
        }

        /// <summary>
        /// foamliu, 2009/01/20.
        /// 使用参数列表和对象列表, 得到所有可能取值.
        /// 
        /// 比如: 
        /// 对象包括: 
        /// Block b1, Block b2, Block b3, Block Table.
        /// Operator "Move" 的参数列表为:
        /// Block ?obj, Block ?from, Block ?to
        /// 则各参数的所有可能取值为:
        /// ?obj,  ?from,  ?to
        /// 
        /// </summary>
        /// <param name="os"></param>
        public void GetPossibleUnifiers(EntitySet os)
        {
            // Block, Block, Block
            List<string> types = m_opHead.ParaList.Types;

            for (int i = 0; i < types.Count; i++)
            {
                // b1, b2, b3, Table
                List<string> objs = os.GetObjects(types[i]);
                Join(objs);                
            }

            if (m_cond.Expressions.Count == 0)
            {
                m_validUnifiers = m_allUnifiers;
            }
            else
            {
                List<string> vars = m_opHead.ParaList.Vars;
                foreach (string s in m_allUnifiers)
                {
                    Unifier u = new Unifier(vars, s);
                    if (ValidateCond(u))
                    {
                        m_validUnifiers.Add(s);
                    }
                }
            }
        }

        private bool ValidateCond(Unifier u)
        {
            return m_cond.Evaluate(u);
        }

        /// <summary>
        /// foamliu, 2009/01/21.
        /// </summary>
        /// <param name="theOther"></param>
        private void Join(List<string> theOther)
        {
            // 第一次需要初始化
            if (m_allUnifiers.Count == 0)
            {
                m_allUnifiers.AddRange(theOther);
                return;
            }

            List<string> temp = new List<string>();
            temp.AddRange(m_allUnifiers);
            m_allUnifiers.Clear();
            foreach (string already in temp)
            {
                foreach (string newObj in theOther)
                {
                    m_allUnifiers.Add(already + " " + newObj);
                }
            }
        }

        #endregion




    }
}
