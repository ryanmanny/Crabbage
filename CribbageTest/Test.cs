using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CribbageEngine;

namespace CribbageTest
{
    class Test
    {
        static void Main(string[] args)
        {
            Console.WriteLine(TestCribStrategies(5));
        }

        public static double TestCribStrategies(int n)
        {
            //This function compares the GREEDY throwing strategy with the much slower PERFECT PLAY strategy
            //The strategies are compared n times and the percentage of success is returned

            var deck = new Deck();
            ThrowingHandAITest ai;

            //Tracks trial successes vs. failures
            int same = 0, different = 0;

            for (int i = 0; i < n; i++)
            {
                //Gives AI new input
                deck.Shuffle();
                ai = new ThrowingHandAITest(deck, ThrowingHandAI.ThrowingStrategy.Invalid);

                bool success = ai.CompareCribStrategies();

                if (success) same++;
                else different++;
            }

            //Returns percentage of trials that gave same result
            return 100 * ((double) same / (same + different));
        }

        public static void PrintCards(Card[] cards)
        {
            foreach (var card in cards)
            {
                Console.WriteLine(card);
            }
        }

        internal struct Result
        {
            public Result(string testName)
            {
                success = true;
                this.testName = testName;
            }
            public bool? success;
            public string testName;
        }

        internal delegate Result TestFunction();

        internal static string GetTestName()
        {
            //Returns the Name of the Method who calls this function
            return new System.Diagnostics.StackFrame(1).GetMethod().Name;
        }
    }

    class DeckTest : Deck
    {

    }

    class ThrowingHandTest : ThrowingHand
    {
        //CONSTRUCTOR
        public ThrowingHandTest(Deck deck, int size = 6) : base(deck, size)
        {

        }

        //RUN TEST BATTERY
        public void RunTests()
        {
            var tests = new List<Test.TestFunction>();
            tests.Add(TestValid);
            tests.Add(TestUnmarkAll);
            tests.Add(TestMark);

            foreach (var test in tests)
            {
                var res = test();
                var output = new StringBuilder();
                output.Append(res.testName);
                switch (res.success)
                {
                    case null:
                        output.Append(" was inconclusive!");
                        break;
                    case true:
                        output.Append(" passed!");
                        break;
                    case false:
                        output.Append(" failed!");
                        break;
                }
                Console.WriteLine(output.ToString());
            }
        }

        //INDIVIDUAL TESTS

        //TODO: THERE NEEDS TO BE A CHECK THROWN METHOD
        private Test.Result TestValid()
        {
            var res = new Test.Result(Test.GetTestName());

            if (_size == 6)
            {
                UnmarkAll();

                if (Valid) res.success = false;
                MarkThrown(0);

                if (Valid) res.success = false;
                MarkThrown(1);

                //Should only be valid if you have thrown two cards away
            }
            else
            {
                res.success = null;
            }
            return res;
        }
        private Test.Result TestUnmarkAll()
        {
            var res = new Test.Result(Test.GetTestName());

            if (_size == 6)
            {
                //Assume MarkThrown works
                MarkThrown(0);
                if (_numThrown != 1) res.success = null;
                if (_thrown[0] != true) res.success = null;
                MarkThrown(1);

                UnmarkAll();

                if (_numThrown != 0) res.success = false;

                if (_thrown[0] != false) res.success = false;
                if (_thrown[1] != false) res.success = false;
            }
            else
            {
                res.success = null;
            }
            return res;
        }
        private Test.Result TestMark()
        {
            var res = new Test.Result(Test.GetTestName());

            if (_size == 6)
            {
                try
                {
                    //This should throw an exception, if it doesn't things are busted
                    res.success = false;
                    MarkThrown(100);
                }
                catch (IndexOutOfRangeException e)
                {
                    res.success = true;
                }

                UnmarkAll();

                MarkThrown(0);
                if (_numThrown != 1) res.success = false;
                if (_thrown[0] != true) res.success = false;

                MarkThrown(0);
                if (_numThrown != 1) res.success = false;

                MarkThrown(1);
                if (_numThrown != 2) res.success = false;
                if (_thrown[1] != true) res.success = false;
            }
            else
            {
                res.success = null;
            }
            return res;
        }
    }

    class ThrowingHandAITest : ThrowingHandAI
    {
        public ThrowingHandAITest(Deck deck, ThrowingStrategy strategy) : base(deck, strategy)
        {

        }

        public bool CompareCribStrategies()
        {
            //Overrides usual strategy with new one
            var ignore = GetOptimalThrow(ThrowingStrategy.IgnoreCrib);
            var optimize = GetOptimalThrow(ThrowingStrategy.DeoptimizeCrib);

            if (Enumerable.SequenceEqual(ignore, optimize))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
