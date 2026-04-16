using UnityEngine;
using TMPro;

public class CardUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI txtCardName;
    public TextMeshProUGUI txtCost;
    public TextMeshProUGUI txtDuration;

    public CardData myData;

    [HideInInspector] public int currentDuration;

    public void Setup(CardData data)
    {
        myData = data;
        currentDuration = data.durationDays;
        txtCardName.text = data.cardName;
        txtCost.text = "Harga Eko: " + data.costEconomy;
        UpdateDurationText();
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