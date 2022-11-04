using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[Serializable]
public struct CardData
{
    public Card.UnoColor Color;
    public Card.UnoFlag Flag;
    public char Value;
    public int DrawAmount;

    public bool IsWild;
}

[SelectionBase]
public class Card : MonoBehaviour
{
    public static readonly UnoColor[] Colors = new[]
    {
        UnoColor.Red,
        UnoColor.Green,
        UnoColor.Blue,
        UnoColor.Yellow
    };
    
    public static readonly Dictionary<UnoColor, Color> ColorTable = new ()
    {
        { UnoColor.Red, new Color(1f, 0f, 0) },
        { UnoColor.Green, new Color(0f, 0.8f, 0.2f) },
        { UnoColor.Blue, new Color(0f, 0f, 1f) },
        { UnoColor.Yellow, new Color(1f, 0.8f, 0f) }
    };

    [Serializable] public enum UnoColor : byte
    {
        Red,
        Green,
        Yellow,
        Blue
    }
    
    [Serializable] public enum UnoFlag : byte
    {
        None,
        Skip,
        Reverse,
        Draw
    }

    [SerializeField] public UnoColor Color;

    [SerializeField] protected TextMeshProUGUI[] _texts;
    [SerializeField] protected MeshRenderer _meshRenderer;

    [SerializeField] protected UnoFlag _flag = UnoFlag.None;
    [SerializeField] public int DrawAmount = 0;
    [SerializeField] protected char _value;

    public bool ShouldRegisterMouseEvents = false;
    public float RegisterMouseEventsAfter = 0;

    private Vector3 _positionInHand;
    private Quaternion _rotationInHand;

    public Vector3 OriginalPosition;
    public Quaternion OriginalRotation;
    private Vector3 _targetPosition;
    private Quaternion _targetRotation;
    private float _targetEndTime = -1;
    private float _targetStartTime;

    public delegate void OnPlayedEventHandler(Card sender);
    public event OnPlayedEventHandler OnPlayed;

    public virtual void Import(CardData card)
    {
        this.Color = card.Color;
        this._flag = card.Flag;
        this._value = card.Value;
        this.DrawAmount = card.DrawAmount;
        Render();
    }

    public virtual void Render()
    {
        this._meshRenderer.material.color = ColorTable[this.Color];
        this._texts[0].color = ColorTable[this.Color];
        string val = "";
        switch (this._flag)
        {
            case UnoFlag.Draw:
                val = $"+{this.DrawAmount}";
                break;
            case UnoFlag.Reverse:
                val = "<";
                break;
            case UnoFlag.Skip:
                val = "X";
                break;
            default:
                val = this._value.ToString();
                break;
        }
        foreach(TextMeshProUGUI text in this._texts)
        {
            text.text = val;
        }
    }

    public virtual bool ColorIs(UnoColor color)
    {
        return this.Color == color;
    }

    public virtual bool FlagIs(UnoFlag flag)
    {
        return this._flag == flag;
    }

    public virtual bool ValueIs(char value)
    {
        return this._value.Equals(value);
    }

    public virtual bool CanBePlayedOn(Card card)
    {
        if (card.ColorIs(this.Color))
            return true;
        
        else if (this._flag == UnoFlag.None || card.FlagIs(UnoFlag.None))
            return card.ValueIs(this._value);

        else
            return card.FlagIs(this._flag);
    }

    public void SetHandPosition(Vector3 positionWS, Quaternion rotationWS, float time = 1.0f)
    {
        this._positionInHand = positionWS;
        this._rotationInHand = rotationWS;
        AnimateTo(positionWS, rotationWS, time, false);
    }

    public void AnimateTo(Vector3 positionWS, Quaternion rotationWS, float time = 1.0f, bool autoCompleteCurrentAnimation = true)
    {
        if (autoCompleteCurrentAnimation)
        {
            this.OriginalPosition = this._targetPosition;
            this.OriginalRotation = this._targetRotation;
        }
        else
        {
            this.OriginalPosition = this.transform.position;
            this.OriginalRotation = this.transform.rotation;
        }

        
        this._targetPosition = positionWS;
        this._targetRotation = rotationWS;
        this._targetStartTime = Time.time;
        this._targetEndTime = Time.time + time;
    }

    protected virtual void Start()
    {
        Render();
    }

    private void Update()
    {
        float t = (Time.time - this._targetStartTime) / (this._targetEndTime - this._targetStartTime);
        
        if (t >= 0)
            this.transform.SetPositionAndRotation(Vector3.Slerp(this.OriginalPosition, this._targetPosition, t), Quaternion.Slerp(this.OriginalRotation, this._targetRotation, t));
    }

    private void OnMouseEnter()
    {
        if (!this.ShouldRegisterMouseEvents || this.RegisterMouseEventsAfter > Time.time) return;
        Vector3 pos = this._positionInHand + this.transform.up * 0.02f;
        AnimateTo(pos, this._rotationInHand, 0.1f, true);
    }

    private void OnMouseExit()
    {
        if (!this.ShouldRegisterMouseEvents || this.RegisterMouseEventsAfter > Time.time) return;
        AnimateTo(this._positionInHand, this._rotationInHand, 0.1f, false);
    }

    private void OnMouseUp()
    {
        if (!this.ShouldRegisterMouseEvents || this.RegisterMouseEventsAfter > Time.time) return;
        if (CanBePlayedOn(DiscardPile.Stack.Peek()))
        {
            this.OnPlayed?.Invoke(this);
            DiscardPile.Instance.Discard(this);
        }
    }
}