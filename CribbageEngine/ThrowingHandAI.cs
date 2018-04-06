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
        protected enum CribType
        {
            Optimize, Deoptimize, Ignore
        }

        //CONSTANTS
        readonly int Iterations;
        const int FullHandSize = 5;

        //CONSTRUCTOR
        public ThrowingHandAI(Deck deck, int DefaultStartingHandSize = 6) : base(deck, DefaultStartingHandSize)
        {
            //COMBINATORICS
            // ( n k ) combinations, where n is _size and k is FinalHandSize
            // (n!/(k!*(n-k)!)
            Iterations = HelperFunctions.Combinations(_size, FinalHandSize);
            
            //Automatically creates ideal hand
            //ThrowAway(CribType.Ignore);
        }

        //METHODS
        protected int[] ThrowAway(CribType optimizeCrib)
        {
            //This is the AI, automatically chooses which cards to throw away
            //DESCRIPTION:
            //THERE ARE TWO THINGS THE AI NEEDS TO CONSIDER:
            //1) Having the statistically best hand possible (given random cut-up card)
            //2) Giving the opponent the statistically best/worst cards possible (given 3 random cards)

            //Stores all possible combos
            var combos = HelperFunctions.GetKCombination(_size, FinalHandSize);

            //This array will hold the best 4 indices to keep
            var best = new int[4];
            double score = 0, bestScore = 0;

            foreach (var combo in combos)
            {
                score = AnalyzeThrow(combo, optimizeCrib);
                {
                    if (score > bestScore)
                    {
                        bestScore = score;
                        best = combo;
                    }
                }
            }
            foreach (var throwIndex in GetIndexComplement(best))
            {
                MarkThrown(throwIndex);
            }
            return best;
        }

        protected double AnalyzeThrow(int[] keep, CribType optimize)
        {
            //Get weighted average of both sides of this option
            double handScore = AnalyzeHand(keep);
            double cribScore;
            double score;

            switch (optimize)
            {
                case CribType.Optimize:
                    cribScore = AnalyzeCrib(keep);
                    //Optimize hand AND crib at the same time
                    score = cribScore + handScore;
                    break;
                case CribType.Deoptimize:
                    cribScore = AnalyzeCrib(keep);
                    //Otherwise, deoptimize crib
                    score = handScore - cribScore;
                    break;
                case CribType.Ignore:
                    score = handScore;
                    break;
                default:
                    score = 0.0;
                    break;
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

        protected Card[] HandComplement
        {
            //Returns an array of all cards not in hand
            get
            {
                var cards = new Card[Deck.NumCards - _size];
                int top = 0;
                //Return all existing cards NOT in the _cards array
                foreach (var card in Deck.All)
                {
                    if (!_cards.Contains(card))
                    {
                        cards[top++] = card;
                    }
                }
                return cards;
            }
        }
    }
}
