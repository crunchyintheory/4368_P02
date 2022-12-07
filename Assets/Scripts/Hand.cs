using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Hand : MonoBehaviour
{
    public bool isPlayerHand = false;
    public List<Card> Cards { get; } = new();
    [SerializeField] private Deck _deck;
    [SerializeField] private float _angleDelta = 10f;
    [SerializeField] private float _angleDeltaDelta = 1f;
    [SerializeField] private float _angleDeltaMin = 3f;
    [SerializeField] private int _angleDeltaScaleThreshold = 8;

    public int Size => this.Cards.Count;

    public bool HasValidPlay()
    {
        Debug.Log(this.Cards.Any(x => x.CanBePlayedOn(DiscardPile.TopCard)));
        return this.Cards.Any(x => x.CanBePlayedOn(DiscardPile.TopCard));
    }

    public Card[] Draw(Deck deck, int count, float time = 0.5f, bool isInitialDraw = false)
    {
        Card[] drawn = new Card[count];
        for (int i = 0; i < count; i++)
        {
            Card card = deck.Draw(isInitialDraw);
            card.transform.SetParent(this.transform);
            this.Cards.Add(card);
            card.RegisterMouseEventsAfter = Time.time + time;
            card.OnPlayed += OnPlay;
            drawn[i] = card;
        }
        
        Rearrange(time);
        return drawn;
    }

    private void Rearrange(float time = 1f)
    {
        float angleDelta = this._angleDelta;
        if (this.Cards.Count > this._angleDeltaScaleThreshold)
        {
            angleDelta = Mathf.Max(this._angleDeltaMin, this._angleDelta - (this.Cards.Count - this._angleDeltaScaleThreshold) * this._angleDeltaDelta);
        }
        float angle = angleDelta * (this.Cards.Count - 1) / -2;
        foreach (Card card in this.Cards)
        {
            Vector3 pos = Vector3.RotateTowards(this.transform.up * 0.04f, this.transform.right * 0.04f, Mathf.Deg2Rad * angle, 1);
            pos -= this.transform.up * 0.04f;
            pos += this.transform.forward * 0.0001f * angle * -1;
            pos += this.transform.position;

            Quaternion rot = this.transform.rotation;
            rot *= Quaternion.Euler(0, 0, angle * -1);
            card.SetHandPosition(pos, rot, time);
            card.ShouldRegisterMouseEvents = this.isPlayerHand;

            angle += angleDelta;
        }
    }

    public void PopulateWithPlaceholder()
    {
        Card card = this._deck.Draw();
        card.transform.SetParent(this.transform);
        this.Cards.Add(card);
        
        RearrangeInstant();
    }

    private void RearrangeInstant()
    {
        float angleDelta = this._angleDelta;
        if (this.Cards.Count > this._angleDeltaScaleThreshold)
        {
            angleDelta = Mathf.Max(this._angleDeltaMin, this._angleDelta - (this.Cards.Count - this._angleDeltaScaleThreshold) * this._angleDeltaDelta);
        }
        float angle = angleDelta * (this.Cards.Count - 1) / -2;
        foreach (Card card in this.Cards)
        {
            Vector3 pos = Vector3.RotateTowards(this.transform.up * 0.04f, this.transform.right * 0.04f, Mathf.Deg2Rad * angle, 1);
            pos -= this.transform.up * 0.04f;
            pos += this.transform.forward * 0.0001f * angle * -1;
            pos += this.transform.position;

            Quaternion rot = this.transform.rotation;
            rot *= Quaternion.Euler(0, 0, angle * -1);
            card.transform.SetPositionAndRotation(pos, rot);
            card.ShouldRegisterMouseEvents = this.isPlayerHand;

            angle += angleDelta;
        }
    }

    public void ResetHand()
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            DestroyImmediate(this.transform.GetChild(i).gameObject);
        }
        this.Cards.Clear();
    }

    public void OnPlay(Card card)
    {
        this.Cards.Remove(card);
        card.OnPlayed -= OnPlay;
        Rearrange();
    }
}
