using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DataContainerController : MonoBehaviour
{
    [Header("Container UI Elements")]
    [SerializeField] private TMP_Text symbolName;
    [SerializeField] private Image symbolImage;

    public void SetData(SymbolData sd)
    {
        symbolName.text = sd.type.ToString();
        symbolImage.sprite = sd.sprite;
    }
}
