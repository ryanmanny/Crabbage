using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CribbageEngine
{
    public class Deck
    {
        //CONSTANTS
        const int NumCards = 52;
       
        //CONSTRUCTOR
        public Deck()
        {
            //Allocates card array (the box of cards I guess)
            _cards = new Card[NumCards];
            //Adds all 52 cards to the deck
            for (int i = 0; i < NumCards; i++)
            {
                _cards[i] = new Card(i);
            }
            //Initially shuffles the deck
            Shuffle();
        }

        //METHODS
        public void Shuffle()
        {
            //It might be better to refactor this random object
            Random r = new Random();
            //Swaps every card with a random position (can be itself)
            //Ensures virtually infinite entropy I think
            for(int i = 0; i < NumCards; i++)
            {
                //Stores swapcard because random
                int swap = r.Next(NumCards);

                //Swap
                Card temp = _cards[i];
                _cards[i] = _cards[swap];
                _cards[swap] = temp;
            }
            //Resets top of the deck to 0
            _top = 0;
        }
        
        public Card Draw()
        {
            //Returns top card, moves top position down one
            return _cards[_top++];
        }

        public override string ToString()
        {
            string allCards = "";
            foreach (Card card in _cards)
            {
                allCards += card.ToString() + Environment.NewLine;
            }
            return allCards;
        }

        //PRIVATE FIELDS
        //The array of cards
        private Card[] _cards;
        //Keeps track of the top of the deck, like a stack
        private int _top;
    }

    public class Card
    {
        //ENUMS
        public enum SuitType
        {
            Spades, Hearts, Clubs, Diamonds
        }
        public enum FaceType
        {
            Ace=1, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King
        }

        //CONSTANTS
        public const int NumSuits = 4;
        public const int NumFaces = 13;

        //The numerical value assigned to the face cards
        internal const int FaceCardValue = 10;

        //This must be readonly because the values assigned to Faces aren't known at compiletime
        internal readonly FaceType[] FaceCards = { FaceType.Jack, FaceType.Queen, FaceType.King };
        
        //CONSTRUCTOR
        public Card(int card)
        {
            //Assigns all 13 faces for each suit
            _face = (FaceType)(card % NumFaces + 1);
            //Every 13 cards, suit changes
            _suit = (SuitType)(card / NumFaces);
        }

        //METHODS
        public override string ToString()
        {
            return Face.ToString() + " of " + Suit.ToString();
        }

        //PROPERTIES
        public SuitType Suit { get { return _suit; } }
        public FaceType Face { get { return _face; } }
        public int Value
        {
            get
            {
                if (FaceCards.Contains(Face))
                {
                    //If the card is a Facecard its value is always 10
                    return FaceCardValue;
                }
                else
                {
                    //If a card is not a Facecard its value is its face
                    return (int)(Face);
                }            
            }
        }

        //INTERNAL FIELDS
        private readonly SuitType _suit;
        private readonly FaceType _face;
    }
}
