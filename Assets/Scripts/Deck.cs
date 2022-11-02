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

    public bool CanPlayerDraw = false;
    [HideInInspector] public Hand PlayerHand;

    public void RemoveChildren()
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            DestroyImmediate(this.transform.GetChild(i).gameObject);
        }
    }

    public void ResetDeck()
    {
        if (this.CardInstances == null)
        {
            this.CardInstances = new Queue<Card>();
            for (int i = 0; i < this.transform.childCount; i++)
            {
                DestroyImmediate(this.transform.GetChild(i).gameObject);
            }
        }
        else
        {
            foreach (Card card in this.CardInstances)
            {
                if (card == null)
                    continue;
                
                if (Application.isEditor)
                    DestroyImmediate(card.gameObject);
                else
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

            Vector3 position = this.transform.position + new Vector3(0, heightOffset, 0);
            Quaternion rotation = Quaternion.Euler(-90, 0, 0);
            
            Card card = Instantiate(template, position, rotation, this.transform);
            this.CardInstances.Enqueue(card);
            card.Import(data);
            card.Render();
            card.OriginalPosition = position;
            card.OriginalRotation = rotation;

            this._numInstances++;
            heightOffset -= 0.0002f;
        }
    }

    private void OnMouseUp()
    {
        if (!this.CanPlayerDraw) return;
        this.PlayerHand.Draw(this, 1, 0.5f);
        this.CanPlayerDraw = false;
    }
}
