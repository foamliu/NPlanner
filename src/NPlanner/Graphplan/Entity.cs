
namespace NPlanner.Graphplan
{
    /// <summary>
    /// foamliu, 2009/01/19, 对象.
    /// 
    /// 比如在 block world 这个经典的例子里面: B1,B2,B3 都是类型为 Block 的对象.
    /// 所以它的主要属性就是名字和类型.
    /// </summary>
    public class Entity
    {
        #region Fields
        private string m_objName;
        private string m_objType;
        #endregion

        #region Properties
        public string Name
        {
            get { return this.m_objName; }
        }
        public string Type
        {
            get { return this.m_objType; }
        }
        #endregion

        #region Constructors
        public Entity(string objName, string objType)
        {
            this.m_objName = objName;
            this.m_objType = objType;
        }
        #endregion

        #region Methods

        /// <summary>
        /// 调式用.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string s = "(" + m_objType + "," + m_objName + ")";
            return s;
        }
        #endregion
    }

}
