using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WildCard : Card
{
    private bool _colorChosen = false;

    public override void Render()
    {
        this.IsWild = true;
        foreach (TextMeshProUGUI text in this._texts)
            text.text = this._flag == UnoFlag.Draw ? $"+{this.DrawAmount}" : "";

        if (this._colorChosen)
            this._meshRenderer.material.color = ColorTable[this.Color];
    }

    public void SetColor(UnoColor color)
    {
        this._colorChosen = true;
        this.Color = color;
        Render();
    }

    public override bool CanBePlayedOn(Card card)
    {
        return true;
    }

    public override bool ValueIs(char value)
    {
        return false;
    }
    
    public override bool FlagIs(UnoFlag flag)
    {
        return false;
    }

    protected override void OnMouseUpAsButton()
    {
        if (!this.ShouldRegisterMouseEvents || this.RegisterMouseEventsAfter > Time.time || !PlayerTurnCardGameState.CanPlayerPlay) return;
        PlayerTurnUIPlayState.Instance.SelectColor();
        PlayerTurnUISelectColorState.OnColorSelected += OnColorChosen;
        PlayerTurnCardGameState.DisableDrawing = true;
        PlayerTurnCardGameState.CanPlayerPlay = false;
        PlayerTurnCardGameState.HasDrawn = true;
    }

    public void OnColorChosen(UnoColor color)
    {
        PlayerTurnUISelectColorState.OnColorSelected -= OnColorChosen;
        SetColor(color);
        Play();
        PlayerTurnCardGameState.DisableDrawing = false;
        PlayerTurnCardGameState.Instance.OnCardPlayed();
    }

    public override void ResetInstance()
    {
        this._colorChosen = false;
        base.ResetInstance();
    }
}
