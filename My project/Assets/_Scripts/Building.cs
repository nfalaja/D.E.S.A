using UnityEngine;
using System.Collections.Generic;

public class Building : MonoBehaviour
{
    public BuildingType buildingType;
    public int maxSlots = 3;

    // Simpan kartu beserta sisa durasinya
    private Dictionary<CardData, int> activeCards = new Dictionary<CardData, int>();

    private void Start()
    {
        GameManager.Instance.OnDayChanged += ProcessDayEnd;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnDayChanged -= ProcessDayEnd;
    }

    public bool TryPlaceCard(CardData card)
    {
        if (activeCards.Count >= maxSlots) return false;

        if (card.compatibleBuilding == buildingType || card.compatibleBuilding == BuildingType.SemuaKecualiKoperasi)
        {
            activeCards.Add(card, card.durationDays);
            ApplyCardEffects(card, 1); // Tambah stat
            return true;
        }
        return false;
    }

    private void ProcessDayEnd()
    {
        List<CardData> cardsToRemove = new List<CardData>();
        List<CardData> keys = new List<CardData>(activeCards.Keys);

        foreach (var card in keys)
        {
            activeCards[card]--;
            if (activeCards[card] <= 0)
            {
                cardsToRemove.Add(card);
            }
        }

        foreach (var card in cardsToRemove)
        {
            RemoveCard(card);
        }
    }

    private void RemoveCard(CardData card)
    {
        ApplyCardEffects(card, -1); // Tarik kembali stat saat kartu hancur
        activeCards.Remove(card);
        // Hapus representasi visual UI kartu di sini
    }

    private void ApplyCardEffects(CardData card, int multiplier)
    {
        foreach (var effect in card.effects)
        {
            GameManager.Instance.ModifyStats(effect.statType, effect.amount * multiplier);
        }
    }
}
