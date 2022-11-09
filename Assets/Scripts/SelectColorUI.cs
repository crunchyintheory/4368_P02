using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectColorUI : MonoBehaviour
{
    public void ColorSelected(int color)
    {
        PlayerTurnUISelectColorState.Instance.ColorSelected(Card.Colors[color]);
    }
}
