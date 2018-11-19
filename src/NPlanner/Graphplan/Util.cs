using System;
using System.Text;

namespace NPlanner.Graphplan
{
    public class Util
    {
        /// <summary>
        /// foamliu, 2009/01/21, 替换合取式中的变量.
        /// 比如, conj为:
        ///     ON (?obj, ?from) & Clear (?obj) & Clear (?to)
        /// un为:
        ///  * -------------------------- 
        ///  *   var      |  value
        ///  * --------------------------
        ///  *   ?obj	  |  b1
        ///  *   ?from	  |  b2
        ///  *   ?to      |  b3
        ///  * --------------------------
        ///  则返回:
        ///     ON (b1, b2) & Clear (b1) & Clear (b3)
        ///  
        /// </summary>
        /// <param name="conj"></param>
        /// <param name="un"></param>
        /// <returns></returns>
        public static string Substitute(Conjunction conj, Unifier un)
        {
            //StringBuilder sb = new StringBuilder();

            //foreach (string literal in conj.Literals)
            //{
            //    string[] tokens = Util.Split(literal);
            //    foreach (string token in tokens)
            //    {
            //        if (token.StartsWith("?"))
            //        {
            //            // 是变量, 取对应值.
            //            sb.Append(un.Get(token));
            //        }
            //        else
            //        {
            //            // 不是变量, 直接加上去.
            //            sb.Append(token);
            //        }
            //    }

            //    sb.Append(" & ");
            //}

            //// 去掉结尾的 " & ".
            //string s = sb.ToString();
            //return s.Substring(0, s.Length - 3);

            string s = conj.ToString();

            foreach(string var in un.Table.Keys)
            {
                string val = un.Table[var];
                s = s.Replace(var, val);
            }

            return s;
        }

        /// <summary>
        /// foamliu, 2009/01/21, 替换并构建 Action 的签名.
        /// 
        /// 例如:
        /// 下面这个 Op 的头:
        /// Move (Block ?obj, Block ?from, Block ?to)
        /// 
        /// 在 un 为如下的情况下:
        ///  * -------------------------- 
        ///  *   var      |  value
        ///  * --------------------------
        ///  *   ?obj	  |  b1
        ///  *   ?from	  |  b2
        ///  *   ?to      |  b3
        ///  * --------------------------
        ///  
        /// 替换的结果是:
        /// Move ( b1, Block b2, Block b3 )
        /// </summary>
        /// <param name="m_opHead"></param>
        /// <param name="un"></param>
        /// <returns></returns>
        public static string Substitute(OperatorHead m_opHead, Unifier un)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(m_opHead.OpName + " ( ");

            if (m_opHead.ParaList.Vars.Count == 0)
            {
                // 没有参数
                sb.Append(" )");

                // 这里不需要后处理.
                return sb.ToString();
            }
            else
            {
                foreach (string var in m_opHead.ParaList.Vars)
                {
                    if (Util.IsVariable(var))
                    {
                        // 是变量, 取对应值.
                        sb.Append(un.Get(var) + ", ");
                    }
                    else
                    {
                        // 不是变量, 直接加上去.
                        sb.Append(var + ", ");
                    }
                    //sb.Append(var + ", ");
                }
            }

            // 去掉最后的 ", ", 再加上 " )"
            string s = sb.ToString();
            return s.Substring(0, s.Length - 2) + " )";

        }

        public static string[] Split(string list)
        {            
            return list.Split(
                " (),\t\n\r\f".ToCharArray(),
                StringSplitOptions.RemoveEmptyEntries);
        }

        public static bool IsVariable(string para)
        {
            return para.StartsWith("?");
        }

        
    }
}
