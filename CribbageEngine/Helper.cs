using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace CribbageEngine
{
    public class HelperFunctions
    {
        public static BigInteger Factorial(int n)
        {
            if (n < 0)
            {
                throw new ArgumentException("Factorial isn't defined for n < 0, at least not by me");
            }
            if (n == 1 || n == 0)
            {
                return 1;
            }
            else
            {
                return n * Factorial(n - 1);
            }
        }

        //Combinatorics hell

        public static int Combinations(int n, int k)
        {
            //Returns nCk
            return (int) (Factorial(n) / (Factorial(k) * Factorial(n - k)));
        }

        public static int[][] GetPowerSet(int n)
        {
            //There is no reason to generate a new version of a power set everytime we need one
            //They're just a description of the laws of the universe
            if (PowerSets.ContainsKey(n))
            {
                return PowerSets[n];
            }
            else
            {
                return PowerSets[n] = EnumeratePowerSet(n);
            }
        }

        public static int[][] GetKCombination(int n, int k)
        {
            var tup = new Tuple<int, int>(n, k);
            if (KCombinations.ContainsKey(tup))
            {
                return KCombinations[tup];
            }
            else
            {
                return KCombinations[tup] = EnumerateKCombination(n, k);
            }
        }

        private static int[][] EnumeratePowerSet(int n)
        {
            //Enumerates a new version of the power set
            //Enumerates all K Combinations for k <= n
            var list = new List<int[]>();

            for (int i = 1; i <= n; i++)
            {
                list.AddRange(EnumerateKCombination(n, i));
            }

            return list.ToArray();
        }

        public static int[][] EnumerateKCombination(int n, int k)
        {
            //THE COMBINATORIC METHOD
            //POSSIBLE COMBINATIONS: 6C4 = 6C2 = (6!)/(2!*4!) = (720/48) = 15
            //https://en.wikipedia.org/wiki/Combination#Enumerating_k-combinations

            //There will be k combinations
            var iterations = Combinations(n, k);

            if (iterations > 0)
            {
                //There will be Iterations possible hands, each size k
                var combos = new int[iterations][];
                for (int i = 0; i < iterations; i++)
                {
                    combos[i] = new int[k];
                }

                //This is the max index that can be recorded in the array
                var max = n - 1;
                //Array of the indexes
                var indices = new int[k];
                for (int i = 0; i < k; i++)
                {
                    indices[i] = i;
                }

                //Store the first combo
                indices.CopyTo(combos[0], 0);

                for (int i = 1; i < iterations; i++)
                {
                    //Always try to increment the last number first
                    int x = k-1;

                    if (indices[x] < max)
                    {
                        //Try to increment the last number
                        indices[x]++;
                    }
                    else
                    {
                        //Find the first incrementable number
                        //The max number containable in any slot is (n - (k-x)) ... (6 - (4-3) 
                        
                        while (!(indices[x] < n + x - (k))) { x--; }

                        //Indices
                        indices[x++]++;

                        //The numbers after x need to be set to (x) + 1, etc. No overlap allowed!
                        while (x < k)
                        {
                            indices[x] = indices[x - 1] + 1;
                            x++;
                        }
                    }
                    //Store the current combination
                    indices.CopyTo(combos[i], 0);
                }
                return combos;
            }
            else
            {
                throw new ArgumentException("No possible combinations!");
            }
        }

        public static int[] GetIndexComplement(int n, int[] indices)
        {
            var complement = new int[n - indices.Count()];
            int top = 0;
            for (int i = 0; i < n; i++)
            {
                if (!indices.Contains(i))
                {
                    complement[top++] = i;
                }
            }
            return complement;
        }

        //Members
        private static Dictionary<Tuple<int, int>, int[][]> KCombinations = new Dictionary<Tuple<int, int>, int[][]>();
        private static Dictionary<int, int[][]> PowerSets = new Dictionary<int, int[][]>();
    }
}
