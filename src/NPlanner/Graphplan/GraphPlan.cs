using System;
using System.Collections.Generic;
using System.Text;

namespace NPlanner.Graphplan
{
    /// <summary>
    /// foamliu, 2008/01/05.
    /// 
    /// A planning problem:
    /// 
    /// - A STRIPS-like domain,
    /// - A set of objects,
    /// - A set of propositions (literals) called the Initial Conditions,
    /// - A set of Problem Goals which are propositions that are required to be true at the end 
    ///     of a plan.
    ///     
    /// 
    /// A Planning Graph is a directed, leveled graph with two kinds of nodes and three kinds of edges.
    /// The levels alternate between proposition levels containing proposition nodes 
    /// (each labeled with some proposition) and action levels containing action nodes (each labeled with some action).
    /// 
    /// 
    /// 
    /// foamliu, 2009/01/19.
    /// 
    /// 规划图是一个分层的有向图 G = (N, E). 由两类节点与三类边构成.
    /// N = P1 + A1 + P2 + A2 + P3 + ...
    /// 
    /// E 也就是边 (Arc) 包括:
    /// = 从 Pj 上的命题 p 到 Aj 上的行为 a, 如果 p 属于 precond(a).
    /// = 从 属于 Aj 的行为 a 到 属于 P(j+1) 的命题 p:
    ///     - 如果 p 属于 effects+(a), 则是正边.
    ///     - 如果 p 属于 effects-(a), 则是负边.
    /// 
    /// 不存在其他的边.
    /// 
    /// 可达性分析:
    ///     如果从如是状态 sj 到一个目标 g 是可达的, 则在规划图中有命题层 Pg 使得 g 属于 Pg.
    ///     
    /// 这是必要非充分条件.
    /// 
    /// </summary>
    public class GraphPlan
    {
        #region Fields
        // A set of objects
        private EntitySet m_objects;
        // A set of operators
        private OperatorSet m_operators;
        // The initial conditions
        private Conjunction m_initials;
        // The goal
        private Conjunction m_goal;
        // 
        //  
        /// <summary>
        /// the first level of a Planning Graph is a proposition level and consists of 
        ///     one node for each proposition in the Initial Conditions.
        /// </summary>
        private PropositionLayer m_firstProp;
        // The levels in a Planning Graph, from earliest to latest are: propositions true at time 1, possible actions at
        //  time 1, propositions possibly true at time 2, possible actions at time 2, propositions possibly
        //  true at time 3, and so forth.
        private PropositionLayer m_lastProp;

        public const int MaxLevel = 100;
        #endregion

        #region Properties

        public EntitySet ObjectSet
        {
            get { return m_objects; }
            set { m_objects = value; }
        }

        public Conjunction Initials
        {
            get { return m_initials; }
            set { m_initials = value; }
        }

        public Conjunction Goal
        {
            get { return m_goal; }
            set { m_goal = value; }
        }

        public PropositionLayer FirstProp
        {
            get { return m_firstProp; }
            set { m_firstProp = value; }
        }

        public PropositionLayer LastProp
        {
            get { return m_lastProp; }
            set { m_lastProp = value; }
        }
        #endregion

        #region Constructors
        public GraphPlan()
        {
            this.m_objects = new EntitySet();
            this.m_initials = new Conjunction();
            this.m_goal = new Conjunction();
           
        }

        public GraphPlan(EntitySet objects, Conjunction init, Conjunction goal, OperatorSet opSet)
        {
            this.m_objects = objects;
            this.m_initials = init;
            this.m_goal = goal;
            this.m_operators = opSet;
            
        }
        #endregion

        #region Methods
        /// <summary>
        /// foamliu, 2009/01/19, 初始化规划图.
        /// </summary>
        private void InitGraph()
        {
            // 初始条件构成第一个命题层: P1.
            this.m_firstProp = new PropositionLayer();
            this.m_firstProp.SetInitLayer(this.m_initials);
            // 参见类头注释.
            this.m_firstProp.Number = 1;
            this.m_lastProp = this.m_firstProp;
        }

        /// <summary>
        /// foamliu, 2009/01/19, 扩展规划图.
        /// 
        /// Expand the planning graph, one action layer and one proposition layer at a time.
        /// 
        /// </summary>
        private bool ExpandGraph()
        {
            for (int i = 1; i <= MaxLevel; i++)
            {
                // 创建 Ai 和 Pi+1
                List<Action> actions = this.m_operators.GenActions(this.m_lastProp.Conjunction);

                ActionLayer aLayer = new ActionLayer(this.m_lastProp, actions);
                aLayer.Number = this.m_lastProp.Number;
                this.m_lastProp.NextLayer = aLayer;
                this.m_lastProp = aLayer.NextLayer;
                this.m_lastProp.Number = aLayer.Number + 1;

                if (GoalsReachable())
                    return true;

                // 测试是否 Level Off
                if (LevelOff())
                {
                    System.Console.WriteLine("Graph Levels Off at level" + m_lastProp.Number);
                    return false;
                }
            }
            System.Console.WriteLine("到达了预设的最大 Level:" + MaxLevel);
            return false;
        }

        public bool CreateGraph()
        {
            m_operators.InitUnifiers(m_objects);
            InitGraph();
            bool res = ExpandGraph();
            return res;
        }

        /// <summary>
        /// foamliu, 2009/01/20.
        /// 
        /// 目标可达且彼此不互斥.
        /// </summary>
        /// <returns></returns>
        private bool GoalsReachable()
        {
            // 首先是所有目标中的命题都在.
            foreach (string literal in this.m_goal.Literals)
            {
                if (!this.m_lastProp.Conjunction.Literals.Contains(literal))
                    return false;
            }

            // 其次目标中的命题不彼此互斥.
            for (int i = 0; i < m_goal.Literals.Count; i++)
            {
                Proposition theProp1 = m_lastProp.GetProposition(m_goal.Literals[i]);
                for (int j = i + 1; j < m_goal.Literals.Count; j++)
                {
                    Proposition theProp2 = m_lastProp.GetProposition(m_goal.Literals[j]);
                    if (theProp1.IsMutex(theProp2))
                        return false;
                }
            }
            return true;
        }

        public bool SearchGoal()
        {
            return m_lastProp.PrevLayer.SearchPlan(m_goal);
        }

        /// <summary>
        /// foamliu, 2009/01/22, 得到规划.
        /// 
        /// </summary>
        /// <returns></returns>
        public List<string> GetPlan()
        {
            List<string> plan = new List<string>();

            ActionLayer aLayer = this.m_firstProp.NextLayer;
            while (aLayer != null)
            {
                plan.AddRange(aLayer.GetSelectedActions());
                aLayer = aLayer.NextLayer.NextLayer;
            }

            List<string> temp = new List<string>();
            temp.AddRange(plan);
            plan.Clear();

            foreach (string step in temp)
            {
                if (step.StartsWith("NOOP") == false && plan.Contains(step) == false)
                {
                    plan.Add(step);
                }
            }

            return plan;
        }

        /// <summary>
        /// foamliu, 2009/01/19.
        /// 
        /// 如果一直找不到有效的规划, 则最后会到达一个命题层 P, 之后所有的命题层都与它完全相同.
        /// 原因: 由于 No-Op actions 的存在, 一个命题出现在某个命题层中, 则它也必然出现在后续的命题层中.
        /// 也就是命题层中命题数量是单调递增的, 而在命题有限的情况下又是有界的, 所以必然会有最大值.
        /// 
        /// 这个条件可以作为图扩展的结束测试.
        /// </summary>
        /// <returns></returns>
        private bool LevelOff()
        {
            PropositionLayer p = this.m_lastProp;
            ActionLayer act = p.PrevLayer;
            if (!p.Equals(act.PrevLayer))
                return false;
            return true;
        }


        /// <summary>
        /// foamliu, 2009/01/22, for debug use.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            PropositionLayer pLayer = m_firstProp;
            ActionLayer aLayer;
            while (pLayer != null)
            {
                sb.Append(pLayer.ToString());
                aLayer = pLayer.NextLayer;
                if (aLayer != null)
                {
                    sb.Append(aLayer.ToString());
                    pLayer = aLayer.NextLayer;
                }
                else
                {
                    pLayer = null;
                }
            }
            return sb.ToString();
        }

        #endregion

    }
}
