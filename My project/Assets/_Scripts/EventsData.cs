using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewEvent", menuName = "ProyekDesa/Random Event")]
public class EventData : ScriptableObject
{
    [Header("Informasi Bencana")]
    public string eventName;
    [TextArea(2, 4)]
    public string eventDescription;

    [Header("Efek Hukuman")]
    public List<StatModifier> penalties;
}

[System.Serializable]
public class StatModifier
{
    public StatType statType;
    public int amount; // Berikan nilai MINUS (contoh: -20)
}