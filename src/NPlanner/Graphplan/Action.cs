using System.Collections.Generic;
using System.Text;

namespace NPlanner.Graphplan
{
    /// <summary>
    /// foamliu, 2008/01/05.
    /// 
    /// Action 是完全实例化了的 Operator. 
    ///     
    /// By an action, we mean a fully-instantiated operator. For example, the operator 'put ?x into ?y' may
    ///     instantiate to the specific action 'put Object1 into Container2'. An action taken at time t 
    ///     adds to the world all the propositions which are among its Add-Effects and deletes all the 
    ///     propositions which are among its Delete-Effects.
    ///     
    /// It will be convenient to think of "doing nothing" to a proposition in a time step as a special kind 
    ///     of action we call a no-op or frame action.
    /// </summary>
    public class Action
    {
        #region Fields
        private string m_head;
        private Conjunction m_preCondition;
        private Conjunction m_addEffects;
        private Conjunction m_delEffects;

        private List<Proposition> m_preEdges;
        private List<Proposition> m_addProps;
        private List<Proposition> m_delProps;

        // foamliu, 2009/01/22, 与本 action 互斥的 actions.
        private List<Action> m_mutexActions;
        #endregion

        #region Properties

        /// <summary>
        /// 实例化了的头, 例如:
        /// Move (b1, b2, b3)
        /// 
        /// </summary>
        public string Head
        {
            get { return this.m_head; }
        }

        /// <summary>
        /// 前提
        /// </summary>
        public Conjunction PreCondition
        {
            get { return this.m_preCondition; }
        }
        
        /// <summary>
        /// 也就是Effect+
        /// 执行某个 行为 后加入的命题.
        /// </summary>
        public Conjunction AddEffects
        {
            get { return this.m_addEffects; }
        }

        /// <summary>
        /// 也就是Effect-
        /// 执行某个 行为 后移除的命题.
        /// </summary>
        public Conjunction DelEffects
        {
            get { return this.m_delEffects; }
        }

        /// <summary>
        /// 论文中的 "precondition-edges".
        /// </summary>
        public List<Proposition> PreEdges
        {
            get { return this.m_preEdges; }
        }

        /// <summary>
        /// 论文中的 "add-edges".
        /// </summary>
        public List<Proposition> AddEdges
        {
            get { return this.m_addProps; }
        }

        /// <summary>
        /// 论文中的 "delete-edges".
        /// </summary>
        public List<Proposition> DelEdges
        {
            get { return this.m_delProps; }
        }

        /// <summary>
        /// foamliu, 2009/01/22, 与本 action 互斥的 actions.
        /// 
        /// </summary>
        public List<Action> MutexActions
        {
            get { return this.m_mutexActions; }
        }
        #endregion

        #region Constructors
        //public Action()
        //{
        //    this.m_preCondition = new Conjunction();
        //    this.m_addEffects = new Conjunction();
        //    this.m_delEffects = new Conjunction();

        //    this.m_preEdges = new List<Proposition>();
        //    this.m_addProps = new List<Proposition>();
        //    this.m_delProps = new List<Proposition>();
        //}

        public Action(string h, string pre, string add, string del)
        {
            this.m_head = h;
            this.m_preCondition = new Conjunction(pre);
            this.m_addEffects = new Conjunction(add);
            this.m_delEffects = new Conjunction(del);

            this.m_preEdges = new List<Proposition>();
            this.m_addProps = new List<Proposition>();
            this.m_delProps = new List<Proposition>();

            // foamliu, 2009/01/22, 与本 action 互斥的 actions.
            this.m_mutexActions = new List<Action>();

        }

        public Action(string theName)
            : this(theName, 
            "", 
            "", 
            "")
        {  
 
        }
        
        #endregion

        #region Methods

        public void AddPreProp(Proposition prop)
        {
            this.m_preEdges.Add(prop);
        }

        public void AddAddProp(Proposition prop)
        {
            this.m_addProps.Add(prop);
        }

        public void AddDelProp(Proposition prop)
        {
            this.m_delProps.Add(prop);
        }

        public bool IsMutex(Action otherAction)
        {
            return m_mutexActions.Contains(otherAction);
        }

        /// <summary>
        /// foamliu, 2009/01/22, for debug use.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(m_head);
            return sb.ToString();
        }

        public bool IsNoop()
        {
            return this.Head.StartsWith("NOOP");
        }



        #endregion
    }
}
