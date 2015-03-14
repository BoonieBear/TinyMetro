using System;
using System.Collections.Generic;

namespace TinyMetroWpfLibrary.Utility
{
    public class MergeListUtility
    {
        public static void MergeList<T>(IList<T> oldValue, IList<T> newValue, Comparison<T> compareFun)
        {
            if ((oldValue == null) || (newValue == null))
            {
                return;
            }
            if ((oldValue.Count == 0) && (newValue.Count == 0))
            {
                return;
            }

            if (newValue.Count == 0)
            {
                oldValue.Clear();
                return;
            }

            //bool IsModeChanged = false;
            int posI = 0;
            int posJ = 0;
            int i, j, k, m, n, p;

            for (i = 0; i < newValue.Count; i++)
            {
                for (j = posJ; j < oldValue.Count; j++)
                {
                    if (0 == compareFun(oldValue[j], newValue[i]))
                    {
                        for (k = j - 1, n = 0; k >= posJ; k--, n++)
                        {
                            oldValue.RemoveAt(k);
                        }
                        posJ = j - n;

                        for (m = posI, p = 0; m < i; m++, p++)
                        {
                            oldValue.Insert(posJ + p, newValue[m]);
                        }

                        posJ += p + 1;
                        posI = posJ;

                        break;
                    }
                }
            }

            if (posI < newValue.Count || posJ < oldValue.Count)
            {
                int delcount = oldValue.Count;
                for (j = posJ; j < delcount; j++)
                {
                    oldValue.RemoveAt(posJ);
                }
                for (i = posI; i < newValue.Count; i++)
                {
                    oldValue.Add(newValue[i]);
                }
            }

            return;
        }
    }
}
