using UnityEngine;
using System.Collections.Generic;

public class Building : MonoBehaviour
{
    public BuildingType buildingType;
    public int maxSlots = 3;

    // Sekarang menyimpan UI fisiknya, bukan cuma datanya
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
            ApplyCardEffects(card.myData, 1); // Tambah stat
            return true;
        }
        return false;
    }

    private void ProcessDayEnd()
    {
        // Looping terbalik (dari belakang) karena kita akan menghancurkan data di tengah jalan
        for (int i = placedCards.Count - 1; i >= 0; i--)
        {
            CardUI card = placedCards[i];
            card.currentDuration--;
            card.UpdateDurationText(); // Update visual teks di layar

            if (card.currentDuration <= 0)
            {
                ApplyCardEffects(card.myData, -1); // Tarik kembali statnya
                placedCards.RemoveAt(i);
                Destroy(card.gameObject); // Hancurkan raga kartunya dari layar
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