using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI txtCardName;
    public TextMeshProUGUI txtCost;
    public TextMeshProUGUI txtDuration;
    public Image imgCardIcon;

    public CardData myData;

    [HideInInspector] public int currentDuration;

    public void Setup(CardData data)
    {
        myData = data;
        currentDuration = data.durationDays;
        txtCardName.text = data.cardName;
        txtCost.text = "Harga Eko: " + data.costEconomy;

        if (imgCardIcon != null && data.cardImage != null)
        {
            imgCardIcon.sprite = data.cardImage;
        }

        UpdateDurationText();
    }

    [Header("Visual Settings")]
    public GameObject textContainer;

    public void SetTextVisibility(bool isVisible)
    {
        if (textContainer != null)
        {
            textContainer.SetActive(isVisible);
        }
    }

    public void UpdateDurationText()
    {
        txtDuration.text = "Durasi: " + currentDuration + " Hari";
    }

    public void OnClickTake()
    {
        DraftingManager.Instance.TryBuyCard(myData);
    }
}