using System.Collections.Generic;
using System.Text;

namespace NPlanner.Graphplan
{
    /// <summary>
    /// foamliu, 2009/01/19.
    /// Action layers: A1, A2, ...
    /// 一个 action layer 包括所有使得 precond(a) 属于 P(j-1) 的 a.
    /// 
    /// 如果任何合法的规划都不可能同时包括行为层中两个行为时, 我们说它们是互斥的行为.
    /// 参见: "Fast Planning Through Planning Graph Analysis", 2.2节 (P5).
    /// 同样, 如果任何合法的规划都不可能同时包括命题层中两个命题时, 我们说它们也是互斥的命题.
    /// </summary>
    public class ActionLayer
    {
        #region Fields

        // foamliu, 2009/01/22, 这是第几层, 参见 Graphplan 类头的注释.
        private int m_number;

        private PropositionLayer m_prev;
        private PropositionLayer m_next;

        private List<Action> m_actions;

        // foamliu, 2009/01/22, 用于在规划图中搜索规划.        
        private List<Action> m_selectedActions;
        // foamliu, 2009/01/22, 当前层的子目标.
        private List<Goal> m_goalSet;
       
        #endregion

        #region Properties

        public int Number
        {
            get { return m_number; }
            set { m_number = value; }
        }

        public PropositionLayer PrevLayer
        {
            get { return m_prev; }
            set { m_prev = value; }
        }

        public PropositionLayer NextLayer
        {
            get { return m_next; }
            set { m_next = value; }
        }

        #endregion

        #region Constructors
        //public ActionLayer()
        //{
        //}

        /// <summary>
        /// foamliu, 2009/01/21.
        /// </summary>
        /// <param name="prev"></param>
        /// <param name="acts"></param>
        public ActionLayer(PropositionLayer prev, List<Action> acts)
        {
            this.m_actions = new List<Action>();
            this.m_selectedActions = new List<Action>();
            this.m_goalSet = new List<Goal>();

            this.m_prev = prev;
            this.m_next = new PropositionLayer(this);

            foreach (Action act in acts)
            {
                this.m_actions.Add(act);
                CreatePreEdges(act);
                CreateAddEdges(act);
                CreateDelEdges(act);
            }
            // add no-ops
            AddNoops();
            // 检测互斥的行为
            TestMutex();
            // 下一层检测互斥的命题
            m_next.TestMutex();
        }

        #endregion

        #region Methods

        /// <summary>
        /// foamliu, 2009/01/21, 添加 No-op actions.
        /// </summary>
        private void AddNoops()
        {
            Conjunction conj = m_prev.Conjunction;

            foreach (string s in conj.Literals)
            {
                // 创建 No-op action.
                Action theAct = new Action("NOOP: " + s);
                theAct.PreCondition.Literals.Add(s);
                theAct.AddEffects.Literals.Add(s);
                this.m_actions.Add(theAct);
                // 把这个 no-op 加入当前行为层.
                Proposition theProp = m_prev.GetProposition(s);
                theProp.AddPreEdge(theAct);
                theAct.AddPreProp(theProp);
                // 把这个命题加入下个命题层.
                theProp = m_next.AddProposition(s);
                theProp.AddAddEdge(theAct);
                theAct.AddAddProp(theProp);
            }
        }

        /// <summary>
        /// foamliu, 2009/01/21, 建立 PreCondition 边.
        /// 
        /// 例如:
        /// Move ( b1, b2, b3 )
        /// PreCondition 是:
        ///     ON (b1, b2) & Clear (b1) & Clear (b3)
        /// </summary>
        /// <param name="theAct"></param>
        private void CreatePreEdges(Action theAct)
        {
            Conjunction pre = theAct.PreCondition;            

            foreach (string s in pre.Literals)
            {
                Proposition theProp = m_prev.GetProposition(s);
                theProp.AddPreEdge(theAct);
                theAct.AddPreProp(theProp);
            }
        }

        /// <summary>
        /// foamliu, 2009/01/21, 建立 Add-Effects 边.
        /// </summary>
        private void CreateAddEdges(Action theAct)
        {
            Conjunction add = theAct.AddEffects;            

            foreach (string s in add.Literals)
            {
                Proposition theProp = m_next.AddProposition(s);
                theProp.AddAddEdge(theAct);
                theAct.AddAddProp(theProp);
            }
        }

        /// <summary>
        /// foamliu, 2009/01/21, 建立 Del-Effects 边.
        /// </summary>
        private void CreateDelEdges(Action theAct)
        {
            Conjunction add = theAct.DelEffects;            

            foreach (string s in add.Literals)
            {
                Proposition theProp = m_next.AddProposition(s);
                theProp.AddDelEdge(theAct);
                theAct.AddDelProp(theProp);
            }
        }

        /// <summary>
        /// foamliu, 2009/01/22.
        /// 
        /// 检测互斥的行为, 主要使用如下规则:
        /// 
        /// (1) 如果一个 action 删除了另一个 action 的前提或者 Add-Effect.
        /// (2) 如果一个 action 的前提跟另一个的互斥.
        /// 
        /// 这两条规则并不足以检测出所有在合法规划中无法并存的 actions, 但是常常可以检测出绝大部分, 因此可以大大缩短搜索工作量.
        /// 
        /// </summary>
        public void TestMutex()
        {
            for (int i = 0; i < m_actions.Count; i++)
            {
                for (int j = i + 1; j < m_actions.Count; j++)
                {
                    Action theAct1 = m_actions[i];
                    Action theAct2 = m_actions[j];

                    if (TestAddEffects(theAct1, theAct2)
                        || TestPreCond(theAct1, theAct2)
                        || TestPreMutex(theAct1, theAct2))
                    {
                        theAct1.MutexActions.Add(theAct2);
                        theAct2.MutexActions.Add(theAct1);
                    }
                }
            }
        }

        /// <summary>
        /// foamliu, 2009/01/22, 互斥则返回 true.
        /// 
        /// (1) 的上半部分, 一个 action 删除了另一个 action 的 add-effects.
        /// 也就是 一个 action 的 del-effects 与 另一个 action 的 add-effects 有交集.
        /// </summary>
        private bool TestAddEffects(Action theAct1, Action theAct2)
        {
            //return (theAct1.DelEffects.Intersect(theAct2.AddEffects)
            //    || theAct2.DelEffects.Intersect(theAct1.AddEffects));

            //string str1 = theAct1.Head.Substring(0, 4);
            //string str2 = theAct2.Head.Substring(0, 4);

            bool res1 = theAct1.DelEffects.Intersect(theAct2.AddEffects);
            bool res2 = theAct2.DelEffects.Intersect(theAct1.AddEffects);
            return (res1 || res2);
        }

        /// <summary>
        /// foamliu, 2009/01/22, 互斥则返回 true.
        /// 
        /// (1) 的上半部分, 一个 action 删除了另一个 action 的前提.
        /// 也就是 一个 action 的 del-effects 与 另一个 action 的 preconditions 有交集.
        /// </summary>
        private bool TestPreCond(Action theAct1, Action theAct2)
        {
            //return (theAct1.DelEffects.Intersect(theAct2.PreCondition)
            //    || theAct2.DelEffects.Intersect(theAct1.PreCondition));

            //string str1 = theAct1.Head.Substring(0, 4);
            //string str2 = theAct2.Head.Substring(0, 4);

            bool res1 = theAct1.DelEffects.Intersect(theAct2.PreCondition);
            bool res2 = theAct2.DelEffects.Intersect(theAct1.PreCondition);
            return (res1 || res2);
        }

        /// <summary>
        /// foamliu, 2009/01/22, 互斥则返回 true.
        /// 
        /// </summary>
        private bool TestPreMutex(Action theAct1, Action theAct2)
        {
            //if ((theAct1.Head.Equals("Move ( b2, Table, b3 )") && theAct2.Head.Equals("NOOP: ON (b1, b2)"))
            //    || theAct2.Head.Equals("Move ( b2, Table, b3 )") && theAct1.Head.Equals("NOOP: ON (b1, b2)"))
            //    System.Diagnostics.Debugger.Launch();

            //System.Console.WriteLine("{0} {1}", theAct1.Head, theAct2.Head);


            foreach (Proposition theProp1 in theAct1.PreEdges)
            {
                foreach (Proposition theProp2 in theAct2.PreEdges)
                {
                    bool res = theProp1.IsMutex(theProp2);
                    if (res)
                        return true;
                }
            }
            return false;
        }

        public List<string> GetSelectedActions()
        {
            List<string> acts = new List<string>();
            foreach (Action theAct in m_selectedActions)
            {
                acts.Add(theAct.Head);
            }
            return acts;
        }

        /// <summary>
        /// foamliu, 2009/01/22.
        /// 
        /// 根据搜索要求生成目标集合, 也就是下一层的命题集合.
        /// </summary>
        private void GenGoalSet(Conjunction theGoals)
        {
            m_goalSet.Clear();
            foreach (string literal in theGoals.Literals)
            {
                m_goalSet.Add(new Goal(this.NextLayer.GetProposition(literal)));
            }
        }

        /// <summary>
        /// foamliu, 2009/01/19.
        /// 
        /// 从后往前一层层的搜索, 比如假设 Pj 层包含目标中的所有命题, 首先在 Aj-1 层找到以这些目标命题为正效果的行为,
        /// 然后在 Pj-1 找这些行为的前提.
        /// </summary>
        public bool SearchPlan(Conjunction theGoals)
        {
            
            GenGoalSet(theGoals);

            
            if (SearchTheLayer(0))
            {
                Conjunction newGoals = new Conjunction();

                foreach (Action theAct in m_selectedActions)
                {
                    newGoals.AddConjunction(theAct.PreCondition);
                }

                ActionLayer aLayer = m_prev.PrevLayer;
                if (aLayer == null)	// no more layers we reached the init state
                    return true;

                return aLayer.SearchPlan(newGoals);

            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// foamliu, 2009/01/22.
        /// 
        /// </summary>
        /// <param name="theGoals"></param>
        /// <returns></returns>
        private bool SearchTheLayer(int pos)
        {
            //System.Console.WriteLine(pos);

            if (pos == m_goalSet.Count)
                return true;
            Goal theGoal = m_goalSet[pos];

            // 先试试是否选中的 action 已经可以满足它.
            //if (SatisfiedBySelectedActions(theGoal.Proposition))
            //{
            //    theGoal.Achieved = true;
            //    // 下一个
            //    m_pos++;
            //    return true;
            //}

            foreach (Action theAct in theGoal.Proposition.AddEdges)
            {
                // 与现在选中的行为不互斥
                if (!IsMutexSelectedActions(theAct))
                {
                    m_selectedActions.Add(theAct);
                    theGoal.Achieved = true;

                    // 下一个                   

                    if (SearchTheLayer(pos + 1) == true)
                    {
                        return true;
                    }                    

                    m_selectedActions.Remove(theAct);
                    theGoal.Achieved = false;                    
                }
            }

            // 一个有效的 action 都没找到, 返回 false.
            return false;
        }

        /// <summary>
        /// foamliu, 2009/01/22.
        /// 
        /// 是否选中的 action 已经可以满足它.
        /// </summary>
        /// <returns></returns>
        private bool SatisfiedBySelectedActions(Proposition theProp)
        {
            foreach (Action theAct in m_selectedActions)
            {
                if (theProp.AddEdges.Contains(theAct))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// foamliu, 2009/01/22.
        /// 
        /// 跟 m_selectedActions 其他 action 互斥
        /// </summary>
        /// <returns></returns>
        private bool IsMutexSelectedActions(Action theAct)
        {
            foreach (Action action in m_selectedActions)
            {
                if (theAct.IsMutex(action))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// foamliu, 2009/01/22, for debug use.
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(this.GetType().Name + " " + m_number + ":");
            foreach (Action act in m_actions)
            {
                sb.AppendLine(act.ToString());
            }
            return sb.ToString();
        }

        #endregion
    }
}
