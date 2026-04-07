using UnityEngine;
using TMPro;

public class CardUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI txtCardName;
    public TextMeshProUGUI txtCost;
    public TextMeshProUGUI txtDuration;

    // Menyimpan memori tentang jati diri kartu ini
    public CardData myData;

    [HideInInspector] public int currentDuration; // Tambahkan baris ini di bawah myData

    public void Setup(CardData data)
    {
        myData = data;
        currentDuration = data.durationDays; // Set umur awal
        txtCardName.text = data.cardName;
        txtCost.text = "Harga Eko: " + data.costEconomy;
        UpdateDurationText();
    }

    // Fungsi baru untuk dipanggil setiap ganti hari
    public void UpdateDurationText()
    {
        txtDuration.text = "Durasi: " + currentDuration + " Hari";
    }

    // Sambungkan fungsi ini ke Button di dalam UI Kartu
    public void OnClickTake()
    {
        DraftingManager.Instance.TryBuyCard(myData);
    }
}