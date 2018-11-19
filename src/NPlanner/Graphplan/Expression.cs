using System;

namespace NPlanner.Graphplan
{
    public class Expression
    {        
        private string m_left;
        private string m_right;
        private string m_op;

        public Expression(string exp)
        {
            string[] tokens = exp.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            m_left = tokens[0];
            m_op = tokens[1];
            m_right = tokens[2];
        }

        public bool Evaluate(Unifier u)
        {
            bool result = false;

            string leftValue;
            string rightValue;

            leftValue = GetValue(m_left, u);
            rightValue = GetValue(m_right, u);

            if (m_op.Equals("=="))
            {
                result = leftValue.Equals(rightValue);
            }
            else if (m_op.Equals("!="))
            {
                result = !(leftValue.Equals(rightValue));
            }

            return result;
        }

        private string GetValue(string var, Unifier u)
        {
            if (Util.IsVariable(var))
                return u.Get(var);
            else
                return var;
        }
    }
}
