using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CribbageEngine
{
    //DESCRIPTION:      This class handles the logic between dealing and pegging
    //                  By MVC, View should be able to mark cards as thrown before actually throwing
    //PRECONDITIONS:    Deck must be initialized and contain at least enough cards to populate
    //                  a starting hand (usually 6)
    //POSTCONDITIONS:   This class will contain two things:
    //                  FinalHand   - An array of 4 Cards used for pegging and conuting
    //                  ThrownCards - An array of 0-2 (usually 2) Cards that will be sent to the crib

    public class ThrowingHand
    {
        ////DATA TYPES
        //protected internal struct HandCard
        //{
        //    public Card card;
        //    public bool thrown;
        //}
        
        //CONSTANTS
        protected const int DefaultStartingHandSize = 6;
        protected const int FinalHandSize = 4;

        //CONSTRUCTOR
        public ThrowingHand(Deck deck, int size = DefaultStartingHandSize)
        {
            //Hand has knowledge of deck
            //Hand is populated from the top cards on the deck (sort of breaks the rules of Cribbage)
            _size = size;
            _cards = new Card[_size];
            _thrown = new bool[_size];

            for (int i = 0; i < _size; i++)
            {
                //Populates hand
                _cards[i]= deck.Draw();
                //Sets thrown array to false
                _thrown[i] = false;
            }

            _numThrown = 0;
        }

        //PROPERTIES
        public Card[] Cards
        {
            //Will probably be used for display
            get
            {
                //Currently returns a copy, may change this in the future based on design
                Card[] cards = new Card[_size];

                //Generates array of normal cards from hand cards
                for (int i = 0; i < _size; i++)
                {
                    cards[i] = _cards[i];
                }
                return cards;
            }
        }

        public Card[] FinalHand
        {
            get
            {
                //Returns the array of 4 cards that will make up the final hand
                if (Valid)
                {
                    Card[] nonThrown = new Card[FinalHandSize];
                    int top = 0;

                    for (int i = 0; i < _size; i++)
                    {
                        if (!_thrown[i])
                        {
                            //Dang I wanted to use yield
                            //yield return _cards[i];
                            nonThrown[top++] = _cards[i];
                        }
                        //Final hand has been populated
                        if (top >= FinalHandSize)
                        {
                            break;
                        }
                    }
                    return nonThrown;
                }
                else
                {
                    return null;
                }
            }
        }

        public Card[] ThrownCards
        {
            get
            {
                //Will return the array of cards that are being thrown away
                //These cards will be used to populate Crib

                //size is the number of cards that must be thrown away to reach 4 cards. 0-2 possible
                int size = _size - FinalHandSize;

                //If the right number of cards have been thrown away, we're good
                if (Valid)
                {
                    Card[] thrownCards = new Card[size]; //Could theoretically be zero
                    int top = 0;

                    for (int i = 0; i < _size; i++)
                    {
                        if (_thrown[i])
                        {
                            thrownCards[top++] = _cards[i];
                        }
                        if (top >= size)
                        {
                            break;
                        }
                    }
                    return thrownCards;
                }
                else
                {
                    return null;
                }
            }
        }

        public bool Valid
        {
            //When this is true, the two subHand properties will return valid input, we can finish
            get
            {
                //If there are two cards thrown away from a 6-card hand, this will be true
                //              2 == (6 - 4)
                return _numThrown == (_size - FinalHandSize);
            }
        }

        //METHODS
        private Card GetCard(int index)
        {
            //Private for now because I don't think there is any reason for random access like this
            //Finds a card based on index in hand. Not exactly sure what this will end up being used for
            if (index < _size)
            {
                return _cards[index];
            }
            else
            {
                throw new IndexOutOfRangeException("Looking for card outside hand!");
            }
        }

        protected Card[] GetCards(int[] indices)
        {
            var cards = new Card[indices.Count()];
            int top = 0;

            foreach (int i in indices)
            {
                cards[top++] = _cards[i];
            }
            return cards;
        }

        protected Card[] GetRest(int[] indices)
        {
            //Returns all cards not in the list of indices
            return GetCards(GetIndexComplement(indices));
        }

        public int[] GetIndexComplement(int[] indices)
        {
            var complement = new int[_size - indices.Count()];
            int top = 0;
            for (int i = 0; i < _size; i++)
            {
                if (!indices.Contains(i))
                {
                    complement[top++] = i;
                }
            }
            return complement;
        }
        
        public void MarkThrown(int index)
        {
            if (!(index < _size))
            {
                throw new IndexOutOfRangeException("Throwing nonexistent card");
            }
            else if (!_thrown[index])
            {
                _thrown[index] = true;
                _numThrown++;
            }
        }

        public void UnmarkThrown(int index)
        {
            if (!(index < _size))
            {
                throw new IndexOutOfRangeException("Unthrowing nonexistent card!");
            }
            else if (_thrown[index])
            {
                _thrown[index] = false;
                _numThrown--;
            }
        }

        public void UnmarkAll()
        {
            for (int i = 0; i < _size; i++)
            {
                _thrown[i] = false;
            }
        }
        
        //PRIVATE FIELDS
        protected Card[] _cards;
        protected bool[] _thrown;
        protected int _size;

        //Keeps track of number of cards thrown. Used by Valid Property
        protected int _numThrown;
    }
}
