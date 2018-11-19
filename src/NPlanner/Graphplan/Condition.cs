using System;
using System.Collections.Generic;
using System.Text;

namespace NPlanner.Graphplan
{
    public class Condition
    {
        private List<Expression> m_exps;

        public List<Expression> Expressions
        {
            get { return m_exps; }
        }

        public Condition(string cond)
        {
            m_exps = new List<Expression>();

            string[] exps = cond.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string exp in exps)
            {
                m_exps.Add(new Expression(exp));
            }
        }

        public bool Evaluate(Unifier u)
        {
            foreach (Expression exp in m_exps)
            {
                if (exp.Evaluate(u) == false)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
