using System;
using System.Collections.Generic;
using System.Text;

namespace NPlanner.Graphplan
{
    /// <summary>
    /// foamliu, 2008/01/05, 文字合取.
    /// </summary>
    public class Conjunction
    {
        #region Fields
        private List<string> m_literals;
        #endregion

        #region Properties
        public List<string> Literals
        {
            get { return this.m_literals; }
        }
        #endregion

        #region Constructors

        public Conjunction()
        {
            this.m_literals = new List<string>();
        }

        public Conjunction(string literal)
        {
            this.m_literals = new List<string>();

            if (literal.IndexOf("&") > 0)
            {
                string[] tokens = literal.Split(new char[] {'&'}, StringSplitOptions.RemoveEmptyEntries );

                foreach (string token in tokens)
                {
                    this.m_literals.Add(token.Trim());
                }
            }
            else if (String.IsNullOrEmpty(literal.Trim()) == false)
            {
                this.m_literals.Add(literal.Trim());
            }
        }
        #endregion

        #region Methods

        public void AddConjunction(Conjunction theConj)
        {
            foreach (string literal in theConj.Literals)
            {
                // foamliu, 2009/01/22, 避免重复.
                if (m_literals.Contains(literal) == false && !String.IsNullOrEmpty(literal))
                {
                    m_literals.Add(literal);
                }
            }
        }

        /// <summary>
        /// foamliu, 2009/01/20.
        /// 是否相同的合取, 顺序不重要.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Conjunction))
                return false;
            Conjunction theCnj = (Conjunction)obj;
            if (theCnj.Literals.Count != this.Literals.Count)
                return false;

            foreach (string item in theCnj.Literals)
            {
                if (!this.Literals.Contains(item))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// foamliu, 2009/01/21.
        /// 测试是否包含一个文字.
        /// </summary>
        /// <param name="conj"></param>
        /// <returns></returns>
        public bool Contains(string literal)
        {
            return m_literals.Contains(literal);
        }

        /// <summary>
        /// foamliu, 2009/01/21.
        /// 
        /// 测试是否包含另一个子合取式.
        /// </summary>
        /// <param name="conj"></param>
        /// <returns></returns>
        public bool Contains(Conjunction subConj)
        {
            foreach (string lit in subConj.Literals)
            {
                if (!this.Contains(lit))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// foamliu, 2009/01/22.
        /// 
        /// 测试跟另外一个合取式是否有交集.
        /// </summary>
        /// <param name="otherConj"></param>
        /// <returns></returns>
        public bool Intersect(Conjunction otherConj)
        {
            foreach (string literal in m_literals)
            {
                if (otherConj.Contains(literal))
                    return true;
            }
            return false;
        }


        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// 用于调试.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (this.Literals.Count == 0)
                return string.Empty;

            StringBuilder sb = new StringBuilder();
            foreach (string item in this.Literals)
            {
                sb.Append(item + " & ");
            }
            string s = sb.ToString();
            return s.Substring(0, s.Length-3);
        }
       
        #endregion

        
    }
}
