using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CribbageEngine
{
    public static class Evaluation
    {
        //If there was a HAND class or something, these could be methods
        //But they should probably be static, logically
        //The rules of cribbage don't require cards to exist?

        const int FifteenValue = 2;
        const int PairValue = 2;
        const int KnobsValue = 1;

        public static int EvaluateFullHand(Card[] cards, int cutIndex)
        {
            //Evaluates the 5 cards according to the rules of Cribbage

            //The cut card is treated differently than the rest for knobs and flush purposes
            //Could assume that the last card is always the cut card but that isn't very cohesive

            //return 29;

            if (cutIndex < cards.Count())
            {
                var faces = new Card.FaceType[cards.Count()];
                var faceVals = new int[(int)Card.FaceType.King + 1];
                var suits = new Card.SuitType[cards.Count()];
                var values = new int[cards.Count()];

                for (int i = 0; i < cards.Count(); i++)
                {
                    faces[i] = cards[i].Face;
                    faceVals[(int)cards[i].Face]++;
                    suits[i] = cards[i].Suit;
                    values[i] = cards[i].Value;
                }

                int fifteens = FindFifteens(values);
                int pairs = FindPairs(faces);
                int runs = FindRuns(faceVals);
                int knobs = FindKnobs(cards, cutIndex);
                int flush = IsFlush(suits, cutIndex);

                //foreach(var card in cards)
                //{
                //    Console.WriteLine(card);
                //}

                //Console.WriteLine("Fifteens: {0}", fifteens);
                //Console.WriteLine("Pairs   : {0}", pairs);
                //Console.WriteLine("Runs    : {0}", runs);
                //Console.WriteLine("Knobs   : {0}", knobs);
                //Console.WriteLine("Flush   : {0}", flush);
                //Console.ReadKey();

                int total = fifteens + pairs + runs + flush + knobs;
                
                //Console.WriteLine("Total: {0}", total);
                //Console.ReadKey();

                return total;
            }
            else
            {
                throw new ArgumentException("CutIndex out of range!");
            }
        }

        static int FindFifteens(int[] values)
        {
            //HOYLE RULES:
            //Every way to make 15 in your hand is worth 2 points each

            //It might be worth changing this to support other values than 15
            //It wouldn't be Cribbage anymore, but supporting more than 5 cards isn't Cribbage either
            int sum = 0;
            int total = 0;

            //Looks at every possible combo you can make from cards in hand
            foreach (var combo in HelperFunctions.GetPowerSet(values.Count()))
            {
                foreach(int i in combo)
                {
                    //Sums all values
                    sum += values[i];
                    //No point checking any more if we already went over
                    if (sum > 15) break;
                }
                if (sum == 15)
                {
                    //This combo was indeed a 15
                    total += FifteenValue;
                }
                sum = 0;
            }
            return total;
        }

        static int FindPairs(Card.FaceType[] faces)
        {
            //HOYLE RULES:
            //Pair: 2 points, Triple: 6 points, Quad: 12 points
            //Although that's just the rule of thumb. Technically all pairs are just worth 2 points, 3 of a kind is 3 pairs
            int total = 0;

            //Handshake all of the cards together - pretty simple
            for (int i = 0; i < faces.Count(); i++)
            {
                for (int j = i + 1; j < faces.Count(); j++)
                {
                    if (faces[i] == faces[j])
                    {
                        total += PairValue;
                    }
                }
            }
            return total;
        }

        static int FindRuns(int[] faceVals)
        {
            int total = 0;
            int multiplier = 0;
            int consec = 0;

            for (int i = 0; i < faceVals.Count(); i++)
            {
                //Run is over - or hasn't begin
                if (faceVals[i] == 0)
                {
                    //When leaving run, generate point report
                    if (consec >= 3)
                    {
                        total += multiplier * consec;
                        consec = 0;
                        multiplier = 0;

                        //It's not possible to have two disconnected runs in (5 card) Cribbage
                        //TODO: Remove this break for > 5 card cribbage
                        break;
                    }
                    //No more consecutiveness 
                    consec = 0;
                    //No more multiplicativeness
                    multiplier = 0;
                }
                else
                {
                    if (multiplier == 0)
                    {
                        multiplier = 1;
                    }
                    //Consecutive multiplier allows double double run to be counted as 4
                    multiplier *= faceVals[i];
                    consec++;
                }
            }
            //Store end-of-hand run
            if (consec >= 3)
            {
                total += multiplier * consec;
            }
            return total;
        }

        static int FindKnobs(Card[] cards, int cutIndex)
        {
            //HOYLE RULES:
            //If a Jack in your hand has a suit matching that of the 
            //card that gets cut up, you have knobs

            //Knobs can't exist if the cut card is a Jack
            if (cards[cutIndex].Face != Card.FaceType.Jack)
            {
                //Stores the suit of the cut card to check later
                var cutSuit = cards[cutIndex].Suit;
                
                foreach (Card card in cards)
                {
                    //Check all cards for knobs
                    if (card.Face == Card.FaceType.Jack && card.Suit == cutSuit)
                    {
                        return KnobsValue;
                    }
                }
            }
            return 0;
        }

        static int IsFlush(Card.SuitType[] suits, int cutIndex)
        {
            //HOYLE RULES:
            //If all the cards in the hand are the same suit: 4 points (1 per card?)
            //If the cut card IS ALSO the same suit as the rest: 5 points (1 extra point)

            //Choose the first suit that isn't the cut card
            var suit = suits[cutIndex != 0 ? 0 : 1];
            
            for (int i = 0; i < suits.Count(); i++)
            {
                //Check all suits in the hand
                if (i != cutIndex && suits[i] != suit)
                {
                    //There was a card with a different suit
                    return 0;
                }
            }
            if (suits[cutIndex] != suit)
            {
                return suits.Count() - 1;
            }
            else
            {
                return suits.Count();
            }
        }
    }
}
