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
    
    public Stack<Card> CardInstances;

    [SerializeField] private int _numInstances = 0;

    [HideInInspector] public Hand PlayerHand;
    
    [SerializeField] private MeshRenderer _glow;
    [SerializeField] private float _glowIntensity = 1;
    [SerializeField] private float _glowSwapDuration = 2;
    
    public static event Action PlayerDrewCard;
    
    private float _currentIntensity = 0;
    private Coroutine _glowCoroutine;
    private bool _glowing = false;

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
            this.CardInstances = new Stack<Card>();
            /*for (int i = 0; i < this.transform.childCount; i++)
            {
                DestroyImmediate(this.transform.GetChild(i).gameObject);
            }*/
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

        int number = 1;
        foreach ((_, CardData data) in _shuffled)
        {
            Card template = data.IsWild ? this._wildTemplate : this._template;

            Vector3 position = this.transform.position + new Vector3(0, -heightOffset, 0);
            Quaternion rotation = Quaternion.Euler(-90, 0, 0);
            
            Card card = Instantiate(template, position, rotation, this.transform);
            card.name = $"{template.name} {number++}";
            this.CardInstances.Push(card);
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
        if (!PlayerTurnCardGameState.CanPlayerDraw) return;
        this.PlayerHand.Draw(this, 1);
        PlayerTurnCardGameState.HasDrawn = true;
        if (!this.PlayerHand.HasValidPlay())
        {
            PlayerTurnCardGameState.CanPlayerPlay = false;
            (CardGameSM.CurrentTurn as PlayerTurnCardGameState)!.OnCardPlayed();
        }
        PlayerDrewCard?.Invoke();
    }

    public Card Peek()
    {
        return this.CardInstances.Peek();
    }

    public Card Draw()
    {
        bool hasCard = this.CardInstances.TryPop(out Card card);
        
        if (hasCard)
            return card;

        Card top = DiscardPile.Instance.Cards.Pop();
        float heightOffset = 0;
        while(DiscardPile.Instance.Cards.TryPop(out card))
        {
            this.CardInstances.Push(card);

            Vector3 position = this.transform.position + new Vector3(0, heightOffset, 0);
            Quaternion rotation = Quaternion.Euler(0, 0, 0);
        
            card.AnimateTo(position, rotation, 0.5f);

            heightOffset += 0.0002f;
        }
        
        DiscardPile.Instance.Discard(top);
        return this.CardInstances.Pop();
    }

    private void OnEnable()
    {
        PlayerTurnCardGameState.PlayerTurnBegan += RenderGlow;
        PlayerTurnCardGameState.PlayerTurnEnded += RenderGlow;
        PlayerDrewCard += RenderGlow;
    }

    private void OnDisable()
    {
        PlayerTurnCardGameState.PlayerTurnBegan -= RenderGlow;
        PlayerTurnCardGameState.PlayerTurnEnded -= RenderGlow;
        PlayerDrewCard -= RenderGlow;
    }

    private void RenderGlow()
    {
        bool shouldGlow = PlayerTurnCardGameState.CanPlayerDraw;
        if (this._glowing == shouldGlow) return;

        this._glowing = shouldGlow;
        
        if(this._glowCoroutine != null)
            StopCoroutine(this._glowCoroutine);
        
        this._glowCoroutine = StartCoroutine(RenderGlowCoroutine(shouldGlow));
    }

    private IEnumerator RenderGlowCoroutine(bool glow)
    {
        float intensity = glow ? this._glowIntensity : 0;
        float startIntensity = glow ? 0 : this._glowIntensity;

        this._glow.transform.parent.transform.localPosition = Vector3.up * (this.CardInstances.Count - 1) * 0.0002f;

        for (float i = 0; i < this._glowSwapDuration; i += Time.fixedDeltaTime)
        {
            this._currentIntensity = Mathf.Lerp(startIntensity, intensity, i / this._glowSwapDuration);
            this._glow.material.SetFloat("_Intensity", this._currentIntensity);
            yield return new WaitForFixedUpdate();
        }
        
        this._currentIntensity = Mathf.Lerp(startIntensity, intensity, 1);
        this._glow.material.SetFloat("_Intensity", this._currentIntensity);
    }
}
