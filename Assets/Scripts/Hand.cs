using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Hand : MonoBehaviour
{
    public bool isPlayerHand = false;
    public List<Card> Cards { get; } = new();
    [SerializeField] private Deck _deck;

    public void Draw(Deck deck, int count, float time)
    {
        for (int i = 0; i < count; i++)
        {
            Card card = deck.CardInstances.Dequeue();
            card.transform.SetParent(this.transform);
            this.Cards.Add(card);
            card.RegisterMouseEventsAfter = Time.time + time;
        }
        
        Rearrange(time);
    }

    private void Rearrange(float time = 1f)
    {
        float angle = 0;
        float angleDelta = 10f;
        int direction = -1;
        foreach (Card card in this.Cards)
        {
            Vector3 pos = Vector3.RotateTowards(this.transform.up * 0.04f, this.transform.right * 0.04f, Mathf.Deg2Rad * angle * direction, 1);
            pos -= this.transform.up * 0.04f;
            pos += this.transform.forward * 0.0001f * angle * direction * -1;
            pos += this.transform.position;

            Quaternion rot = this.transform.rotation;
            rot *= Quaternion.Euler(0, 0, angle * direction * -1);
            card.SetHandPosition(pos, rot, time);
            card.ShouldRegisterMouseEvents = this.isPlayerHand;

            if (direction == -1)
                angle += angleDelta;

            direction *= -1;
        }
    }

    public void PopulateWithPlaceholder()
    {
        Card card = this._deck.CardInstances.Dequeue();
        card.transform.SetParent(this.transform);
        this.Cards.Add(card);
        
        RearrangeInstant();
    }

    private void RearrangeInstant()
    {
        float angle = 0;
        float angleDelta = 10f;
        int direction = -1;
        foreach (Card card in this.Cards)
        {
            Vector3 pos = Vector3.RotateTowards(this.transform.up * 0.04f, this.transform.right * 0.04f, Mathf.Deg2Rad * angle * direction, 1);
            pos -= this.transform.up * 0.04f;
            pos += this.transform.forward * 0.0001f * angle * direction * -1;
            card.transform.SetPositionAndRotation(this.transform.position + pos, this.transform.rotation * Quaternion.Euler(0, 0, angle * direction * -1));

            if (direction == -1)
                angle += angleDelta;

            direction *= -1;
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
}
