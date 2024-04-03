using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    [SerializeField] private RectTransform staminaFill;

    private float staminaFillSize;

    private void Awake()
    {
        staminaFillSize = staminaFill.rect.height;
    }

    public void SetStaminaFill(float ratio)
    {
        staminaFill.SetSizeWithCurrentAnchors( RectTransform.Axis.Vertical, staminaFillSize * ratio);
    }
}
