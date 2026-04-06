using UnityEngine;
using System.Collections.Generic;

public enum BuildingType { None, Taman, Warung, PosRonda, SemuaKecualiKoperasi }
public enum StatType { Ekonomi, Lingkungan, Sosial }

[System.Serializable]
public struct StatEffect
{
    public StatType statType;
    public int amount;
}

[CreateAssetMenu(fileName = "NewCard", menuName = "DesaGame/Card Data")]
public class CardData : ScriptableObject
{
    public string cardName;
    public int costEconomy;
    public int durationDays;
    public BuildingType compatibleBuilding;
    public List<StatEffect> effects;
}