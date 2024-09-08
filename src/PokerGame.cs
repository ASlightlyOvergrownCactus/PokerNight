using System.Collections.Generic;
using PokerNight.Properties;
using UnityEngine;

namespace PokerNight;

public class PokerGame
{
    public List<Creature> pokerPlayers;
    
    public PokerGame()
    {
        pokerPlayers = new List<Creature>();
    }

    public void DebugHand()
    {
        Hand hand = new Hand();
        Deck deck = new Deck();
        
        for (int i = 0; i < 5; i++)
            hand.AddCardFromDeck(deck);

        for (int i = 0; i < hand.cardHand.Count; i++)
        {
            Debug.LogWarning("Card " + i + " is a " + hand.cardHand[i].number + " of " + hand.cardHand[i].suit);
        }
        Debug.LogWarning("Points: " + hand.DeterminePoints(hand));
    }
}