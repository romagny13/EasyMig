using System.Collections.Generic;

namespace EasyMigLib
{
    public class Util
    {
        public static void Concat(List<string> list, List<string> toAdd)
        {
            foreach (var item in toAdd)
            {
                list.Add(item);
            }
        }
    }
}
