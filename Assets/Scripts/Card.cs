using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;

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
    public static readonly UnoColor[] Colors = {
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
    
    [SerializeField] private MeshRenderer _glow;
    [SerializeField] private float _glowIntensity = 1;
    [SerializeField] private float _glowSwapDuration = 2;

    [SerializeField] private AudioClip _playSound;
    [SerializeField] private ParticleSystem _particles;

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

    private Deck deck;
    
    private float _currentIntensity = 0;
    private Coroutine _glowCoroutine;
    private bool _glowing = false;

    public delegate void OnPlayedEventHandler(Card sender);
    public event OnPlayedEventHandler OnPlayed;

    public bool IsWild;

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
        this.IsWild = false;
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
        
        else if (this._flag == UnoFlag.None && card.FlagIs(UnoFlag.None))
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

    public void Play()
    {
        this.OnPlayed?.Invoke(this);

        if (this._playSound) AudioHelper.PlayClip2D(this._playSound, 1.0f);
        if (this._particles && (this._flag != UnoFlag.None || this.IsWild)) StartCoroutine(PlayCoroutine());

        switch (this._flag)
        {
            case UnoFlag.Draw:
                CardGameSM.CurrentTurn.DrawEnemy(this.DrawAmount);
                break;
            case UnoFlag.Reverse:
            case UnoFlag.Skip:
                CardGameSM.CurrentTurn.SkipEnemyTurn();
                break;
        }
        
        DiscardPile.Instance.Discard(this);
    }

    private IEnumerator PlayCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        this._particles.Play();
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

    protected virtual void OnMouseUpAsButton()
    {
        if (!this.ShouldRegisterMouseEvents || this.RegisterMouseEventsAfter > Time.time || !PlayerTurnCardGameState.CanPlayerPlay) return;
        if (CanBePlayedOn(DiscardPile.Stack.Peek()))
        {
            PlayerTurnCardGameState.CanPlayerPlay = false;
            PlayerTurnCardGameState.HasDrawn = true;
            Play();
            PlayerTurnCardGameState.Instance.OnCardPlayed();
        }
    }

    private void OnEnable()
    {
        PlayerTurnCardGameState.PlayerTurnBegan += RenderGlow;
        PlayerTurnCardGameState.PlayerTurnEnded += RenderGlow;
        Deck.PlayerDrewCard += RenderGlow;
        DiscardPile.OnChange += RenderGlow;
        EnemyTurnCardGameState.EnemyTurnBegan += RenderGlow;
    }

    private void OnDisable()
    {
        PlayerTurnCardGameState.PlayerTurnBegan -= RenderGlow;
        PlayerTurnCardGameState.PlayerTurnEnded -= RenderGlow;
        Deck.PlayerDrewCard -= RenderGlow;
        DiscardPile.OnChange -= RenderGlow;
        EnemyTurnCardGameState.EnemyTurnBegan -= RenderGlow;
    }

    private void RenderGlow()
    {
        bool shouldGlow = this.ShouldRegisterMouseEvents && PlayerTurnCardGameState.CanPlayerPlay && CanBePlayedOn(DiscardPile.Stack.Peek());
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

        for (float i = 0; i < this._glowSwapDuration; i += Time.fixedDeltaTime)
        {
            this._currentIntensity = Mathf.Lerp(startIntensity, intensity, i / this._glowSwapDuration);
            this._glow.material.SetFloat("_Intensity", this._currentIntensity);
            yield return new WaitForFixedUpdate();
        }
        
        this._currentIntensity = Mathf.Lerp(startIntensity, intensity, 1);
        this._glow.material.SetFloat("_Intensity", this._currentIntensity);
    }

    public virtual void ResetInstance()
    {
        // No variables to reset in the base card, only wild cards, but we call render for compatibility
        Render();
    }
}