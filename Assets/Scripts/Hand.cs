using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    [SerializeField] public List<Card> Cards;

    public void Draw(Deck deck, int count)
    {
        for (int i = 0; i < count; i++)
        {
            Card card = deck.CardInstances.Dequeue();
            card.transform.SetParent(this.transform);
            this.Cards.Add(card);
            card.AnimateTo(this.transform.position, this.transform.rotation, 1);
        }
    }
}
