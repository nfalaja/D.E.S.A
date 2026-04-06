using UnityEngine;
using TMPro;

public class CardUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI txtCardName;
    public TextMeshProUGUI txtCost;
    public TextMeshProUGUI txtDuration;

    // Menyimpan memori tentang jati diri kartu ini
    private CardData myData;

    // Fungsi ini dipanggil oleh sistem saat memunculkan kartu
    public void Setup(CardData data)
    {
        myData = data;
        txtCardName.text = data.cardName;
        txtCost.text = "Harga Eko: " + data.costEconomy;
        txtDuration.text = "Durasi: " + data.durationDays + " Hari";
    }

    // Sambungkan fungsi ini ke Button di dalam UI Kartu
    public void OnClickTake()
    {
        DraftingManager.Instance.TryBuyCard(myData);
    }
}