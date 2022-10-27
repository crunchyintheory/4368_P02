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

    [SerializeField] private UnoFlag _flag = UnoFlag.None;
    [SerializeField] public int DrawAmount = 0;
    [SerializeField] private char _value;

    public virtual char Value => this._value;
    public virtual UnoFlag Flag => this._flag;

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
        switch (this.Flag)
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
    
    protected virtual void Start()
    {
        Render();
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
        return this._value == value;
    }

    public virtual bool CanBePlayedOn(Card card)
    {
        if (card.ColorIs(this.Color))
            return true;
        
        else if (this.Flag == UnoFlag.None || card.FlagIs(UnoFlag.None))
            return card.ValueIs(this._value);

        else
            return card.FlagIs(this._flag);
    }
}