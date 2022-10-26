using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WildCard : Card
{
    private bool _colorChosen = false;
    
    public override void Render()
    {
        foreach (TextMeshProUGUI text in this._texts)
            text.text = this.Flag == UnoFlag.Draw ? $"+{this.DrawAmount}" : "";

        if (this._colorChosen)
            this._meshRenderer.material.color = ColorTable[this.Color];
    }

    public override bool CanBePlayedOn(Card card)
    {
        return true;
    }

    public override bool ValueIs(char value)
    {
        return true;
    }
    
    public override bool FlagIs(UnoFlag flag)
    {
        return true;
    }
}
