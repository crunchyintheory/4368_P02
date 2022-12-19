using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
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

    [SerializeField] private AudioClip _shuffleSound;
    [SerializeField] private AudioClip _drawSound;
    
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

    public static List<KeyValuePair<int, T>> Shuffle<T>([NotNull] IEnumerable<T> list, int iterations = 1)
    {
        if (iterations <= 0) throw new Exception("Invalid iterations");
        
        List<KeyValuePair<int, T>> shuffled = list.Select(item =>
        {
            int random = Random.Range(int.MinValue, int.MaxValue);
            return new KeyValuePair<int, T>(random, item);
        }).ToList();
        shuffled.Sort((x, y) => x.Key.CompareTo(y.Key));

        for (int i = 1; i < iterations; i++)
        {
            shuffled = shuffled.Select(t => new KeyValuePair<int, T>(Random.Range(int.MinValue, int.MaxValue), t.Value)).ToList();
            shuffled.Sort((x, y) => x.Key.CompareTo(y.Key));
        }
        
        return shuffled;
    }

    public static void ShuffleToList<T>([NotNull] IEnumerable<T> list, [CanBeNull] out List<T> shuffled, int iterations = 1)
    {
        shuffled = Shuffle(list, iterations).Select(x => x.Value).ToList();
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
        
        List<CardData> cards = new();
        foreach (DeckPreset data in this._initialCards)
        {
            for (int i = 0; i < data.Count; i++)
            {
                cards.Add(data.Card);
            }
            for (int i = 0; i < data.CountPerColor; i++)
            {
                foreach(Card.UnoColor color in Card.Colors)
                {
                    CardData card = data.Card;
                    card.Color = color;
                    cards.Add(card);
                }
            }
        }

        List<KeyValuePair<int, CardData>> shuffled = Shuffle(cards, 3);

        int number = 1;
        foreach ((_, CardData data) in shuffled)
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

        if(this._shuffleSound) AudioHelper.PlayClip2D(this._shuffleSound, 1);
    }

    private async void OnMouseUp()
    {
        if (!PlayerTurnCardGameState.CanPlayerDraw) return;
        await this.PlayerHand.Draw(this, 1);
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

    private Task _shufflingTask;

    public async Task<Card> Draw(bool isInitialDraw = false)
    {
        if (this._drawSound && !isInitialDraw) AudioHelper.PlayClip2D(this._drawSound, 1);

        if(this._shufflingTask is {IsCompleted: false})
            Task.WaitAll(this._shufflingTask);

        this.CardInstances.TryPop(out Card card);

        if(this.CardInstances.Count == 0) ShuffleDiscardIntoDeck();

        return card;
    }

    private async Task ShuffleDiscardIntoDeck()
    {
        Card top = DiscardPile.Instance.Cards.Pop();
        float heightOffset = 0;
        
        // shuffle the following cards
        List<Card> cards = new();
        
        while(DiscardPile.Instance.Cards.TryPop(out Card card))
        {
            card.ResetInstance();
            cards.Add(card);
        }

        List<KeyValuePair<int, Card>> shuffled = Shuffle(cards, 3);
        
        foreach((int _, Card card) in shuffled)
        {
            this.CardInstances.Push(card);
            card.transform.SetParent(this.transform);
            Vector3 position = this.transform.position + new Vector3(0, heightOffset, 0);
            Quaternion rotation = Quaternion.Euler(-90, 0, 0);
        
            card.AnimateTo(position, rotation, 0.5f);
            heightOffset += 0.0002f;
        }

        // put the discarded card back
        DiscardPile.Instance.Discard(top);
    }

    private void OnEnable()
    {
        PlayerTurnCardGameState.PlayerTurnBegan += RenderGlow;
        PlayerTurnCardGameState.PlayerTurnEnded += RenderGlow;
        DiscardPile.OnChange += RenderGlow;
        PlayerDrewCard += RenderGlow;
    }

    private void OnDisable()
    {
        PlayerTurnCardGameState.PlayerTurnBegan -= RenderGlow;
        PlayerTurnCardGameState.PlayerTurnEnded -= RenderGlow;
        DiscardPile.OnChange -= RenderGlow;
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
