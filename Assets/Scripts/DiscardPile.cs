using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscardPile : MonoBehaviour
{
    public Stack<Card> Cards = new();

    public static DiscardPile Instance;
    public static Stack<Card> Stack => Instance.Cards;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
    }

    private void OnDestroy()
    {
        Instance = null;
    }
    
    public void Discard(Card card)
    {
        this.Cards.Push(card);
        AnimateCard(card, this.Cards.Count - 1);
        card.transform.SetParent(this.transform, true);
        card.ShouldRegisterMouseEvents = false;
    }

    public void AnimateCard(Card card, int index)
    {
        float heightOffset = 0.0002f * index;
        
        Vector3 position = this.transform.position + new Vector3(0, heightOffset, 0);
        Quaternion rotation = Quaternion.Euler(90, 0, 0);
        
        card.AnimateTo(position, rotation, 0.5f);
    }
}
