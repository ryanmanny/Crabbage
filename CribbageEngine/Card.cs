using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CribbageEngine
{
    public class Card
    {
        //ENUMS
        public enum SuitType
        {
            Spades, Hearts, Clubs, Diamonds
        }
        public enum FaceType
        {
            Ace = 1, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King
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
            //Used for debugging
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

        //OPERATOR OVERLOADS
        public override bool Equals(object obj)
        {
            return this == (Card) obj;
        }

        public static bool operator ==(Card l, Card r)
        {
            if (ReferenceEquals(l, null) && ReferenceEquals(r, null))
            {
                return true;
            }
            else if (ReferenceEquals(l, null) || ReferenceEquals(r, null))
            {
                return false;
            }
            else
            {
                if (l.Suit == r.Suit && l.Face == r.Face)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static bool operator !=(Card l, Card r)
        {
            return !(l == r);
        }

    }
}