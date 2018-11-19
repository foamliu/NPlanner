using System.Collections.Generic;

namespace NPlanner.Graphplan
{
    /// <summary>
    /// Operator 的容器.
    /// </summary>
    public class OperatorSet
    {
        private List<Operator> m_ops;

        public List<Operator> Operators
        {
            get { return m_ops; }
        }

        public OperatorSet()
        {
            this.m_ops = new List<Operator>();
        }

        public List<Action> GenActions(Conjunction thePre)
        {
            List<Action> actions = new List<Action>();
            foreach (Operator op in m_ops)
            {
                List<Action> temp = op.GenActions(thePre);
                if (temp != null)
                    actions.AddRange(temp);
            }
            return actions;
        }

        public void InitUnifiers(EntitySet os)
        {
            foreach (Operator op in m_ops)
            {
                op.GetPossibleUnifiers(os);
            }
        }
    }
}
