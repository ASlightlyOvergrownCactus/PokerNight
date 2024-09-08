using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PokerNight.Properties;

public class Card
{
    public int number;
    public string suit;
    
    public Card(string suit, int number)
    {
        this.suit = suit;
        // 1 is Ace. Keep in mind, an Ace counts as a 1 and a 14
        // 11 is Jack, 12 is Queen, 13 is King
        this.number = number;
        
    }
}

public class Deck
{
    public List<Card> cardDeck;

    public Deck()
    {
        cardDeck = new List<Card>();
        // Adds 54 cards; 13 for each suit.
        for (int i = 1; i < 14; i++)
        {
            this.cardDeck.Add(new Card("Club", i));
            this.cardDeck.Add(new Card("Spade", i));
            this.cardDeck.Add(new Card("Heart", i));
            this.cardDeck.Add(new Card("Diamond", i));
        }
    }
}

public class Hand
{
    public List<Card> cardHand;
    // Data Sheet length needs to be the amount of rows within the CSV file
    private string[][] pointDataSheet = new string[14][];
    public int points;

    // ok i know this looks insane but i had to translate this from an excel spreadsheet
    // first array shows the categories
    // number is the card number
    // basePoints is the base points of the card number
    // Multiplier is name of combo multiplier
    // multiplier is value of combo multiplier
    // Information contains info on the combo
    public void GeneratePointDataSheet()
    {
        pointDataSheet[0] = new[]
        {
            "number", "basePoints", "Multiplier", "multiplier", "Information"
        };
        
        pointDataSheet[1] = new[]
        {
            "1", "2500", "2Kind", "2",
            "For a hand with two of the same card numbers. Multiply their value added together by 2"
        };
        
        pointDataSheet[2] = new[]
        {
            "2", "1200", "3Kind", "3",
            "For a hand with three of the same card numbers. Multiply their value added together by 3"
        };
        
        pointDataSheet[3] = new[]
        {
            "3", "1300", "4Kind", "4",
            "For a hand with four of the same card numbers. Multiply their value added together by 4"
        };
        
        pointDataSheet[4] = new[]
        {
            "4", "1400", "straight", "2",
            "For a hand with a hand that increments its numbers by one (ex: 12345). Add all together and multiply by 3"
        };
        
        pointDataSheet[5] = new[]
        {
            "5", "1500", "fullSColor", "1.2",
            "For a hand with all cards being the same color. Multiple total points by 1.2"
        };
        
        pointDataSheet[6] = new[]
        {
            "6", "1600", "fHSColor", "1.3",
            "For a hand with a hand of three black/red and 2 red/black cards respectively. Multiply total points by 1.3"
        };
        
        pointDataSheet[7] = new[]
        {
            "7", "1700", "flush", "2",
            "For a hand with all cards belonging to the same suit. Multiple total points by 2"
        };
        
        pointDataSheet[8] = new[]
        {
            "8", "1800", "2Pair", "1.5",
            "For a hand with 2 separate 2Kind combos. Multiple total points by 1.5 "
        };
        
        pointDataSheet[9] = new[]
        {
            "9", "1900", "fullHouse", "2.5",
            "For a hand with a 2Kind and 3Kind. Multiply total points by 2.5"
        };
        
        pointDataSheet[10] = new[]
        {
            "10", "2000", "royalFlush", "100",
            "For a hand with a a straight that starts from a 10 to go to an Ace with all cards being of the same suit. Multiply total points by 100 (this is an instant win)"
        };
        
        pointDataSheet[11] = new[]
        {
            "11", "2100"
        };
        
        pointDataSheet[12] = new[]
        {
            "12", "2200"
        };
        
        pointDataSheet[13] = new[]
        {
            "13", "2300"
        };
    }

    public Hand()
    {
        cardHand = new List<Card>();
        points = 0;
        GeneratePointDataSheet();
    }

    // Adds a random card from the deck, and removes it from the deck
    public void AddCardFromDeck(Deck deck)
    {
        if (deck.cardDeck.Count <= 0)
        {
            Debug.LogError("Empty Deck! Wth!!1!");
            return;
        }

        int deckNum = Random.Range(0, deck.cardDeck.Count);
        this.cardHand.Add(deck.cardDeck[deckNum]);
        deck.cardDeck.RemoveAt(deckNum);
    }
    
    // Runs through each combo type
    public int DeterminePoints(Hand hand)
    {
        if (hand.cardHand.Count <= 0)
        {
            Debug.LogError("DeterminePoints Method\n Empty Hand!!! Wth!!!");
            return -1;
        }
        
        int tempPoints = 0;
        tempPoints = FullAndPair(hand);
        if (tempPoints == 0)
        {
            tempPoints += Straight(hand);
            if (tempPoints == 0)
            {
                tempPoints += OfKind(hand);
            }
        }
        tempPoints += HighCard(hand);
        tempPoints = Flush(hand, (double)tempPoints);
        
        return tempPoints;
    }
    
    // Combo Methods
    // Checks for Flush
    public int Flush(Hand hand, double points)
    {
        if (hand.cardHand.Count <= 0)
        {
            Debug.LogError("Flush Method\nCard Hand empty!! Wth!!!");
            return -1;
        }
        string tempSuit = hand.cardHand[0].suit;
        List<Card> tempArry = hand.cardHand.FindAll(card => card.suit.Equals(tempSuit));
        if (tempArry.Count == hand.cardHand.Count)
        {
            // Royal Flush checks only if the hand is already a flush.
            if (RoyalFlush(hand))
            {
                Debug.Log("Royal Flush!!!");
                points *= 1000;
            }
            else
            {
                Debug.Log("Flush!");
                points *= 4;
            }
        }

        return (int)points;
    }

    // Checks for Royal Flush (called within the Flush Method)\
    private bool RoyalFlush(Hand hand)
    {
        if (hand.cardHand.Any(card => card.number == 10) && cardHand.Any(card => card.number == 11) && cardHand.Any(card => card.number == 12) && cardHand.Any(card => card.number == 13) && cardHand.Any(card => card.number == 1))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // Checks for Straight
    public int Straight(Hand hand)
    {
        if (hand.cardHand.Count <= 0)
        {
            Debug.Log("Straight Method\nHand is empty!!! Wth!!!");
            return -1;
        }
        
        double points = 0;

        Card[] tempArry = hand.cardHand.OrderBy(x => x.number).ToArray();
        Card[] tempArry2;
        int check = 1;
        bool skip = false;

        // Sets the final card in the temp arry to an Ace if the deck contains an Ace. (This is for straights with the final card being an Ace).
        if (tempArry[0].number == 1)
        {
            tempArry2 = new Card[tempArry.Length + 1];
            int x = 0;
            // Checks to make sure the straight is not bugged
            if (StraightBugCheck(hand))
            {
                skip = true;
                Debug.LogError("StraightBugCheck failed! Wth!!!");
            }
            else
            {
                foreach (Card card in tempArry)
                {
                    tempArry2[x] = card;
                    x++;
                }
                tempArry2[tempArry2.Length - 1] = tempArry2[0];
            }
        }

        else
        {
            tempArry2 = tempArry;
        }

        if (!skip)
        {
            for (int i = 0; i < tempArry2.Length - 1; i++)
            {
                // -12 happens when the Ace is acting as a 14
                if (1 == tempArry2[i + 1].number - tempArry[i].number || -12 == tempArry2[i + 1].number - tempArry[i].number)
                {
                    check++;
                }
            }
            // For when there is no Ace in the hand
            if (tempArry2[0].number != 1)
            {
                if (check == tempArry2.Length)
                {
                    Debug.Log("Straight!");
                    for (int i = 0; i < tempArry2.Length; i++)
                    {
                        points += int.Parse(pointDataSheet[tempArry2[i].number][1]);
                    }
                    points *= 2;


                }
            }
            // For when there is an Ace in the hand
            else
            {
                if (check == tempArry2.Length - 1)
                {
                    Debug.Log("Straight!");
                    for (int i = 0; i < tempArry2.Length; i++)
                    {
                        points += int.Parse(pointDataSheet[tempArry2[i].number][1]);
                    }
                    points -= int.Parse(pointDataSheet[1][1]);
                    points *= 2;
                }
            }
        }

        return (int)points;
    }

    // Method for checking to make sure there is not a fake Straight, a bug which happened with how the Straight check works. 
    // The bug causes a hand of Ace, 2, King, Queen, and Jack to be a Straight due to the counter system
    // Also causes a hand of an Ace, 2, 3, 4, King to be a Straight due to the counter system.
    // Method introduces a hard code check for these.
    public bool StraightBugCheck(Hand hand)
    {

        if (hand.cardHand.Any(card => card.number == 1) && hand.cardHand.Any(card => card.number == 2) && hand.cardHand.Any(card => card.number == 3) && hand.cardHand.Any(card => card.number == 4) && hand.cardHand.Any(card => card.number == 13))
        {
            return true;
        }
        else if (hand.cardHand.Any(card => card.number == 2) && hand.cardHand.Any(card => card.number == 11) && hand.cardHand.Any(card => card.number == 12) && hand.cardHand.Any(card => card.number == 13) && hand.cardHand.Any(card => card.number == 1))
        {
            return true;
        }
        else if (hand.cardHand.Any(card => card.number == 1) && hand.cardHand.Any(card => card.number == 2) && hand.cardHand.Any(card => card.number == 3) && hand.cardHand.Any(card => card.number == 12) && hand.cardHand.Any(card => card.number == 13))
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    // Checks for Full House and Two Pair
    public int FullAndPair(Hand hand)
    {
        bool notFull = true;
        int points = 0;
        int tempNumb;
        int i;
        int j = 0;
        for (i = 0; i < hand.cardHand.Count; i++)
        {
            tempNumb = hand.cardHand[i].number;
            List<Card> tempArry = hand.cardHand.FindAll(
                card => card.number == tempNumb);
            // Full House Check
            if (tempArry.Count == 3)
            {
                j = 3 * 3 * int.Parse(pointDataSheet[tempNumb][1]);
                for (i = 0; i < hand.cardHand.Count; i++)
                {
                    tempNumb = hand.cardHand[i].number;
                    tempArry = hand.cardHand.FindAll(
                        card => card.number == tempNumb);
                    if (tempArry.Count == 2)
                    {
                        Debug.Log("Full House!");
                        points = j + (points = 2 * 2 * int.Parse(pointDataSheet[tempNumb][1]));
                        notFull = false;
                        break;
                    }
                }
                break;
            }

            // Two Pair Check
            else if (tempArry.Count == 2 && notFull)
            {
               j = j = 2 * 2 * int.Parse(pointDataSheet[tempNumb][1]);
                int x = tempArry[0].number;
                for (i = 0; i < hand.cardHand.Count; i++)
                {
                    tempNumb = hand.cardHand[i].number;
                    tempArry = hand.cardHand.FindAll(
                        card => card.number == tempNumb);
                    if (tempArry.Count == 2 && tempArry[0].number != x)
                    {
                        Debug.Log("Two Pair!");
                        points = j + (points = 2 * 2 * int.Parse(pointDataSheet[tempNumb][1]));
                        break;
                    }
                }
                break;
            }
        }
        return points;
    }

    // Checks for two, three, and four of a Kind and calculates points if found
    public int OfKind(Hand hand)
    {
        int points = 0;
        int tempNumb;
        for (int i = 0; i < hand.cardHand.Count; i++)
        {
            tempNumb = hand.cardHand[i].number;
            List<Card> tempArry = hand.cardHand.FindAll( 
                card => card.number == tempNumb);
            // Four of a kind check
            if (tempArry.Count == 4)
            {
                Debug.Log("Four of a Kind!");
                points = 4 * 3 * int.Parse(pointDataSheet[tempNumb][1]);
                break;
            }
            // Three of a kind check
            else if (tempArry.Count == 3)
            {
                Debug.Log("Three of a Kind!");
                points = 3 * 2 * int.Parse(pointDataSheet[tempNumb][1]);
                break;
            }
            // Two of a kind check
            else if (tempArry.Count == 2)
            {
                Debug.Log("Two of a Kind!");
                points = 2 * 2 * int.Parse(pointDataSheet[tempNumb][1]);
                break;
            }
        }
        return points;
    }

    // Calculates total points for just the cards, no combos involved
    public int HighCard(Hand hand)
    {
        int points = 0;

        foreach (Card card in hand.cardHand)
        {
            points += int.Parse(pointDataSheet[card.number][1]);
        }

        return points;
    }
}