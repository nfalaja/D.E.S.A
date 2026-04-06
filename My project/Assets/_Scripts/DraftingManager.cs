using UnityEngine;
using System.Collections.Generic;

public class DraftingManager : MonoBehaviour
{
    public static DraftingManager Instance { get; private set; }

    [Header("UI & Spawning")]
    public GameObject draftingPanel; // Layar redup tempat kartu muncul
    public Transform cardContainer;  // Tempat kartu dijejer (gunakan HorizontalLayoutGroup)
    public GameObject cardUIPrefab;  // Template "Raga" kartu

    [Header("Data")]
    public List<CardData> allAvailableCards; // Masukkan 20 aset kartu ke sini dari Inspector

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Berlangganan (Subscribe) ke event pergantian hari
        GameManager.Instance.OnDayChanged += ShowDrafting;
    }

    public void ShowDrafting()
    {
        draftingPanel.SetActive(true);

        // 1. Sapu bersih kartu sisa kemarin yang tidak dipilih
        foreach (Transform child in cardContainer)
        {
            Destroy(child.gameObject);
        }

        // 2. Munculkan 3 kartu baru
        for (int i = 0; i < 3; i++)
        {
            // Ambil data acak dari 20 kartu
            int randomIndex = Random.Range(0, allAvailableCards.Count);
            CardData randomCard = allAvailableCards[randomIndex];

            // Cetak raga kartunya ke layar
            GameObject newCard = Instantiate(cardUIPrefab, cardContainer);

            // Suntikkan roh/datanya ke dalam raga tersebut
            newCard.GetComponent<CardUI>().Setup(randomCard);
        }
    }

    public void TryBuyCard(CardData selectedCard)
    {
        // 1. CEK DULU: Apakah tangan sudah penuh (Batas 4)?
        if (!HandManager.Instance.CanAddCard())
        {
            Debug.LogWarning("[DRAFTING] Gagal beli! Tangan sudah penuh.");
            // (Opsional) Munculkan notifikasi ke layar
            UIManager.Instance.txtNotification.text = "Tangan Penuh! Maksimal 4 Kartu.";
            return;
        }

        // 2. CEK UANG: Apakah kas cukup?
        if (GameManager.Instance.statEkonomi >= selectedCard.costEconomy)
        {
            // Potong uang
            GameManager.Instance.ModifyStats(StatType.Ekonomi, -selectedCard.costEconomy);

            // MASUKKAN KARTU KE TANGAN
            HandManager.Instance.AddCardToHand(selectedCard);

            // Tutup layar drafting
            draftingPanel.SetActive(false);
            Debug.Log($"[DRAFTING] Berhasil membeli {selectedCard.cardName}!");
        }
        else
        {
            Debug.LogWarning("[DRAFTING] Kas Ekonomi tidak cukup!");
            UIManager.Instance.txtNotification.text = "Uang Kas Tidak Cukup!";
        }
    }
}