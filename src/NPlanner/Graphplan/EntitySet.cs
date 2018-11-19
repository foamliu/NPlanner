using System;
using System.Collections.Generic;

namespace NPlanner.Graphplan
{
    /// <summary>
    /// foamliu, 2009/01/19.
    /// 出现过的对象集合.
    /// </summary>
    public class EntitySet
    {
        #region Fields
        private List<Entity> m_objects;
        #endregion

        #region Constructors
        public EntitySet()
        {
            this.m_objects = new List<Entity>();
        }
        #endregion

        #region Methods
        public void addObject(string objType, string objName)
        {            
            this.m_objects.Add(new Entity(objType, objName));
        }

        public void addObject(Entity obj)
        {
            this.m_objects.Add(obj);
        }

        /// <summary>
        /// foamliu, 2009/01/20, 取得所有某个类型的对象.
        /// </summary>
        /// <param name="theType"></param>
        /// <returns></returns>
        public List<string> GetObjects(String theType)
        {
            List<string> objs = new List<string>();

            foreach (Entity obj in m_objects)
            {                
                if (theType.Equals(obj.Type))
                    objs.Add(obj.Name);
            }
            return objs;
        }
        #endregion
    }
}
