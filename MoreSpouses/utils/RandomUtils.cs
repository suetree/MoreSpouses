using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SueMoreSpouses
{
    class RandomUtils
    {

        static List<int> RandomNumbers(int n, int min, int max)
        {
            List<int> numbers = new List<int>();
            long tick = DateTime.Now.Ticks;
            Random ran = new Random((int)(tick & 0xffffffffL) | (int)(tick >> 32));

            int[] a = new int[n];
            for (int i = 0; i < n; i++)
            {
                a[i] = ran.Next(min, max);
            }
            Boolean bol = true;
            while (bol)
            {
                Array.Sort(a);
                int num = 0;
                for (int i = 0; i < 9; i++)
                {
                    if (a[i] != a[i + 1])
                    {
                        num++;
                    }
                    else
                    {
                        a[i + 1] = ran.Next(min, max);
                    }
                    if (num == 9)
                    {
                        bol = false;
                        foreach (int Ele in a)
                        {
                            //Console.Write(Ele + "");
                            numbers.Add(Ele);
                        }

                    }
                }
            }

            return numbers;
        }
    }
}
