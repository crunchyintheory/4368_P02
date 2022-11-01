using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public struct DeckPreset
{
    [SerializeField] public int Count;
    [SerializeField] public int CountPerColor;
    [SerializeField] public CardData Card;
}

[SelectionBase]
public class Deck : MonoBehaviour
{
    [SerializeField] private Card _template;
    [SerializeField] private WildCard _wildTemplate;
    [SerializeField] private DeckPreset[] _initialCards;
    
    public Queue<Card> CardInstances;

    [SerializeField] private int _numInstances = 0;

    private void Start()
    {
        this.CardInstances = new Queue<Card>();
    }

    public void ResetDeck()
    {
        if (this.CardInstances == null)
        {
            this.CardInstances = new Queue<Card>();
        }
        else
        {
            foreach (Card card in this.CardInstances)
            {
                if (card == null)
                    continue;
                Destroy(card.gameObject);
            }
            this.CardInstances.Clear();
        }
        
        this._numInstances = 0;
        
        float heightOffset = 0;
        
        List<KeyValuePair<int, CardData>> _shuffled = new();
        foreach (DeckPreset data in this._initialCards)
        {
            for (int i = 0; i < data.Count; i++)
            {
                _shuffled.Add(new KeyValuePair<int, CardData>(Random.Range(1, 108), data.Card));                
            }
            for (int i = 0; i < data.CountPerColor; i++)
            {
                foreach(Card.UnoColor color in Card.Colors)
                {
                    CardData card = data.Card;
                    card.Color = color;
                    _shuffled.Add(new KeyValuePair<int, CardData>(Random.Range(1, 108), card));
                }
            }
        }

        _shuffled.Sort((x, y) => x.Key.CompareTo(y.Key));

        foreach ((_, CardData data) in _shuffled)
        {
            Card template = data.IsWild ? this._wildTemplate : this._template;
            
            Card card = Instantiate(template, this.transform.position + new Vector3(0, heightOffset, 0), Quaternion.Euler(-90, 0, 0), this.transform);
            this.CardInstances.Enqueue(card);
            card.Import(data);
            card.Render();

            this._numInstances++;
            heightOffset += 0.0002f;
        }
    }
}
