using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CribbageEngine
{
    public class ThrowingHandAI : ThrowingHand
    {
        //TYPES
        public enum ThrowingStrategy
        {
            //This ENUM field pattern should be replaced with Strategy Pattern (almost too obvious)

            //Perfect play, BRUTEFORCE algorithms
            OptimizeCrib,
            DeoptimizeCrib,
            //GREEDY algorithm, always takes best hand (much faster)
            IgnoreCrib,
            //STUPID algorith, always takes random hand
            Random,
            Invalid
        }
        
        //CONSTRUCTOR
        public ThrowingHandAI(Deck deck, ThrowingStrategy strategy, int DefaultStartingHandSize = 6) : base(deck, DefaultStartingHandSize)
        {
            _strategy = strategy;
            //Automatically creates (somewhat) ideal hand
            //ThrowAway(_strategy);
        }

        //METHODS
        protected void ThrowAway(ThrowingStrategy strategy)
        {
            //Figures out the best way to throw away
            var best = GetOptimalThrow(strategy);

            foreach (var throwIndex in HelperFunctions.GetIndexComplement(_size, best))
            {
                //Mark the indices from the optimal throw as thrown
                MarkThrown(throwIndex);
            }
        }

        protected int[] GetOptimalThrow(ThrowingStrategy strategy)
        {
            //This is the AI, automatically chooses which cards to throw away
            //DESCRIPTION:
            //THERE ARE TWO THINGS THE AI NEEDS TO CONSIDER:
            //1) Having the statistically best hand possible (given random cut-up card)
            //2) Giving the opponent the statistically best/worst cards possible (given 3 random cards)

            //What Optimal means depends on your strategy

            //This array will hold the best 4 indices to keep
            var best = new int[4];

            //Stores the best score so far to find max with
            double score = double.MinValue, bestScore = 0;

            //Looks at every possible throwing possible (there should be 15, since 6C4 is 15)
            foreach (var combo in HelperFunctions.GetKCombination(_size, FinalHandSize))
            {
                //Finds hand that best satisfies chosen strategy
                score = AnalyzeThrow(combo, strategy);
                {
                    //Better hand found, replace original
                    if (score > bestScore)
                    {
                        bestScore = score;
                        best = combo;
                    }
                }
            }
            return best;
        }

        protected double AnalyzeThrow(int[] keep, ThrowingStrategy strategy)
        {
            //Get weighted average of both sides of this option
            double handScore = AnalyzeHand(keep);
            double cribScore;
            double score;

            //STRATEGY PATTERN - CHANGE THIS LATER!
            //score = strategy(keep)
            //There is no point to have all these algorithms here

            //Switch statement takes the AI's strategy
            switch (strategy)
            {
                case ThrowingStrategy.OptimizeCrib:
                    cribScore = AnalyzeCrib(keep);
                    //Optimize hand AND crib at the same time
                    score = cribScore + handScore;
                    break;

                case ThrowingStrategy.DeoptimizeCrib:
                    cribScore = AnalyzeCrib(keep);
                    //Otherwise, deoptimize crib. The better the crib, the worse this throw
                    score = handScore - cribScore;
                    break;

                case ThrowingStrategy.IgnoreCrib:
                    //There are only 15 ways to throw away, ignoring will only give a bad throw every once in a while
                    //It's MUCH MUCH faster than optimize and deoptimize
                    score = handScore;
                    break;

                case ThrowingStrategy.Random:
                    //Randomly assigns quality values to each throw
                    var r = new Random();
                    score = 29 * r.NextDouble();
                    break;

                default:
                    //This strategy doesn't exist
                    throw new ArgumentException("No strategy provided!");
            }
            return score;
        }

        protected double AnalyzeHand(int[] keep)
        {
            //Gets the value of hand by walking all paths and finding weighted average

            //AI counts cards, divines which cards can still be left in deck
            var deck = HandComplement;

            //value is the weighted average of all possible hands
            double value = 0.0;
            
            var fullHand = new Card[FullHandSize];
            int lastCardIndex = fullHand.Count() - 1;

            //Generates first four cards of hand from keep array
            GetCards(keep).CopyTo(fullHand, 0);
            
            //Walks all paths
            foreach (var card in deck)
            {
                //Swaps out fifth card for all possible cut cards
                fullHand.SetValue(card, lastCardIndex);
                //Adds outcome of cut card to weighted average
                value += (Evaluation.EvaluateFullHand(fullHand, lastCardIndex) * ((double)1/(Deck.NumCards - _size)));
            }
            return value;
        }

        protected double AnalyzeCrib(int[] keep)
        {
            var deck = HandComplement;
            
            //value is the weighted average of all possible hands
            double value = 0.0;
            
            var fullCrib = new Card[FullHandSize];
            //Holds the 0-2 cards that the user has thrown away
            var thrownCards = GetRest(keep);
            
            //Generates first initial crib cards from keep array
            thrownCards.CopyTo(fullCrib, 0);

            //How many ways are there to finish the crib?
            //In other words, how many ways are there to choose 3 cards from a deck of 46?
            //In other words, a lot!
            var combos = HelperFunctions.GetKCombination(deck.Count() - _size, FullHandSize - thrownCards.Count());
            int possibilities = combos.Count();

            foreach (var combo in combos)
            {
                //There are cases where the cutIndex is NOT needed (no Jacks, two different suits in hand)
                //For now we will ignore that, but it may be able to squeeze out a little performance
                for (int cutCard = thrownCards.Count(); cutCard < FullHandSize; cutCard++)
                {
                    //For each possible combo, finish the crib and evaluate
                    for (int i = 0; i < combo.Count(); i++)
                    {
                        fullCrib[i + thrownCards.Count()] = HandComplement[combo[i]];
                    }
                    value += ((Evaluation.EvaluateFullHand(fullCrib, cutCard) / (possibilities)));
                }
            }

            return value;
        }

        //PRIVATE FIELDS
        private ThrowingStrategy _strategy;
    }
}
