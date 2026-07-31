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
    public List<CardData> allAvailableCards; // Masukkan aset kartu ke sini dari Inspector

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Pastikan panel drafting tertutup saat game mulai
        if (draftingPanel != null)
        {
            draftingPanel.SetActive(false);
        }

        // Langsung bagikan kartu awal ke tangan pemain secara acak
        GenerateStartingHand();

        // (Opsional) Berlangganan ke event pergantian hari untuk memunculkan drafting nanti
        // GameManager.Instance.OnDayChanged += ShowDrafting;
    }

    // FUNGSI BARU: Membagikan kartu awal langsung ke tangan pemain
    public void GenerateStartingHand()
    {
        // 1. (Opsional tapi disarankan) Bersihkan tangan pemain terlebih dahulu 
        // jika kebetulan ada kartu nyangkut dari editor.
        // Asumsi variabel tempat menampung kartu di HandManager bernama "cardContainer" atau serupa.
        // Jika beda nama, sesuaikan dengan milikmu ya!
        /*
        foreach (Transform child in HandManager.Instance.cardContainer)
        {
            Destroy(child.gameObject);
        }
        */

        List<CardData> tempCardPool = new List<CardData>(allAvailableCards);

        // 2. Loop ini yang menentukan jumlah kartu awal (batasnya adalah 3)
        for (int i = 0; i < 3; i++)
        {
            if (tempCardPool.Count == 0) break;

            int randomIndex = Random.Range(0, tempCardPool.Count);
            CardData randomCard = tempCardPool[randomIndex];

            // Masukkan ke tangan
            HandManager.Instance.AddCardToHand(randomCard);

            // Hapus dari pool agar tidak kembar
            tempCardPool.RemoveAll(card => card == randomCard);
        }

        Debug.Log("[DRAFTING] 3 Kartu awal berhasil diacak dan masuk ke tangan.");
    }

    // Fungsi ini sekarang HANYA dipanggil saat pergantian hari / event tertentu
    public void ShowDrafting()
    {
        draftingPanel.SetActive(true);

        // 1. Sapu bersih kartu sisa kemarin yang tidak dipilih
        foreach (Transform child in cardContainer)
        {
            Destroy(child.gameObject);
        }

        // 2. Buat copy dari list utama ke temporary pool. 
        List<CardData> tempCardPool = new List<CardData>(allAvailableCards);

        // 3. Munculkan 3 kartu baru secara acak tanpa duplikat
        for (int i = 0; i < 3; i++)
        {
            if (tempCardPool.Count == 0) break;

            int randomIndex = Random.Range(0, tempCardPool.Count);
            CardData randomCard = tempCardPool[randomIndex];

            GameObject newCard = Instantiate(cardUIPrefab, cardContainer);
            newCard.GetComponent<CardUI>().Setup(randomCard);

            tempCardPool.RemoveAll(card => card == randomCard);
        }
    }

    public void TryBuyCard(CardData selectedCard)
    {
        // 1. CEK TANGAN PENUH
        if (!HandManager.Instance.CanAddCard())
        {
            Debug.LogWarning("[DRAFTING] Gagal beli! Tangan sudah penuh.");

            
            return;
        }

        // 2. CEK UANG
        if (GameManager.Instance.statEkonomi >= selectedCard.costEconomy)
        {
            GameManager.Instance.ModifyStats(StatType.Ekonomi, -selectedCard.costEconomy);
            HandManager.Instance.AddCardToHand(selectedCard);

            draftingPanel.SetActive(false);
        }
        else
        {
            Debug.LogWarning("[DRAFTING] Kas Ekonomi tidak cukup!");

            // Panggil Pop-Up dan Suara Error
            UIManager.Instance.ShowWarning("Uang Kas Tidak Cukup!");
        }
    }

    public void SkipDrafting()
    {
        Debug.Log("[DRAFTING] Pemain memilih untuk melewati pengambilan kartu.");
        draftingPanel.SetActive(false);
    }
}