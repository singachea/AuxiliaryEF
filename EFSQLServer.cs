using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;

namespace AuxiliaryEF
{
    public static class EFSQLServer
    {
        public static void DeleteAllChildrenInSQLServerSelfReference<T>(this DbSet<T> dbSet, T entity, Func<T, List<T>> getChildren) where T : class
        {
            var children = GetAllChildren<T>(entity, getChildren);
            var enm = children.GetEnumerator();
            while (enm.MoveNext() && enm.Current != null)
            {
                dbSet.Remove(enm.Current);
            }
        }

        public static List<T> GetAllChildren<T>(T parent, Func<T, List<T>> getChildren)
        {
            var flatList = new List<T>();
            flatList.Add(parent);
            var children = getChildren(parent);
            if (children != null && children.Count > 0)
            {
                foreach (var child in children)
                {
                    flatList.AddRange(GetAllChildren<T>(child, getChildren));
                }
            }
            return flatList;
        }
    }
}
