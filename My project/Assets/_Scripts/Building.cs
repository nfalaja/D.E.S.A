using UnityEngine;
using System.Collections.Generic;

public class Building : MonoBehaviour
{
    public BuildingType buildingType;
    public int maxSlots = 3;

    private List<CardUI> placedCards = new List<CardUI>();

    private void Start()
    {
        GameManager.Instance.OnDayChanged += ProcessDayEnd;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnDayChanged -= ProcessDayEnd;
    }

    public bool TryPlaceCard(CardUI card)
    {
        if (placedCards.Count >= maxSlots) return false;

        if (card.myData.compatibleBuilding == buildingType || card.myData.compatibleBuilding == BuildingType.SemuaKecualiKoperasi)
        {
            placedCards.Add(card);

            // PERUBAHAN MUTLAK 1: 
            // Kartu TIDAK LAGI memberikan stat instan saat ditaruh.
            // Pemain harus menunggu pergantian hari untuk "panen".

            return true;
        }
        return false;
    }

    private void ProcessDayEnd()
    {
        // Looping terbalik karena kita berpotensi menghancurkan kartu di tengah jalan
        for (int i = placedCards.Count - 1; i >= 0; i--)
        {
            CardUI card = placedCards[i];

            // PERUBAHAN MUTLAK 2: KARTU BEKERJA (PANEN HARIAN)
            // Hasilkan poin stat SEBELUM umur kartu berkurang
            ApplyCardEffects(card.myData, 1);

            // Umur berkurang
            card.currentDuration--;
            card.UpdateDurationText();

            // PERUBAHAN MUTLAK 3: KARTU HANCUR (TANPA PENALTI)
            if (card.currentDuration <= 0)
            {
                // Tidak ada lagi kode penarikan stat ApplyCardEffects(-1). 
                // Poin yang sudah dicetak adalah milik pemain selamanya.

                placedCards.RemoveAt(i);
                Destroy(card.gameObject);
            }
        }
    }

    private void ApplyCardEffects(CardData cardData, int multiplier)
    {
        foreach (var effect in cardData.effects)
        {
            GameManager.Instance.ModifyStats(effect.statType, effect.amount * multiplier);
        }
    }
}