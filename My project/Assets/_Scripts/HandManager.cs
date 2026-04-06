using UnityEngine;
using System.Collections.Generic;

public class HandManager : MonoBehaviour
{
    public static HandManager Instance { get; private set; }

    [Header("Pengaturan Tangan")]
    public int maxHandSize = 4;

    [Header("Referensi UI")]
    public Transform handContainer; // Meja fisik di bawah layar
    public GameObject handCardPrefab; // Kita bisa pakai prefab kartu yang sama dengan drafting

    // Memori penyimpanan data kartu di tangan
    private List<CardData> cardsInHand = new List<CardData>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Fungsi untuk mengecek apakah tangan masih muat
    public bool CanAddCard()
    {
        return cardsInHand.Count < maxHandSize;
    }

    // Fungsi untuk memasukkan kartu ke tangan
    public void AddCardToHand(CardData cardData)
    {
        if (!CanAddCard()) return;

        cardsInHand.Add(cardData);

        // Cetak raga kartunya di UI Tangan
        GameObject newCard = Instantiate(handCardPrefab, handContainer);

        // Suntikkan datanya
        newCard.GetComponent<CardUI>().Setup(cardData);

        Debug.Log($"[HAND] {cardData.cardName} masuk ke tangan. Total: {cardsInHand.Count}/{maxHandSize}");
    }
}