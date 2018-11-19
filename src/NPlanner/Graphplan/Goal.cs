
namespace NPlanner.Graphplan
{
    public class Goal
    {
        #region Fields
        private bool m_achieved;
        private Proposition m_prop;
        #endregion

        #region Properties
        public bool Achieved
        {
            get { return this.m_achieved; }
            set { this.m_achieved = value; }
        }
        public Proposition Proposition
        {
            get { return this.m_prop; }
        }
        #endregion

        #region Constructors

        public Goal()
        {
            this.m_achieved = false;            
        }
        /// <summary>
        /// foamliu, 2009/01/22.
        /// 
        /// </summary>
        public Goal(Proposition theProp)
        {
            this.m_prop = theProp;
            this.m_achieved = false;
        }
        #endregion

        #region Methods

        public override string ToString()
        {
            return m_prop.ToString();
        }

        #endregion
    }
}
