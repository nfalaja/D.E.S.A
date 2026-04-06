using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Player Stats")]
    public int statEkonomi = 0;
    public int statLingkungan = 0;
    public int statSosial = 0;

    [Header("Game State")]
    public int currentDay = 1;
    public int currentWeek = 1;
    public int currentObjectiveTarget;

    public event Action OnDayChanged;
    public event Action OnGameOver;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        CalculateObjective();
        // Berikan 3 kartu awal di sini (Panggil fungsi dari HandManager)
    }

    public void NextDay()
    {
        // Koperasi Desa pasif income
        statEkonomi += 10;

        // Cek objektif jika hari ke-7, 14, 21, dst.
        if (currentDay % 7 == 0)
        {
            if (!IsObjectiveMet())
            {
                TriggerGameOver();
                return;
            }
            else
            {
                currentWeek++;
                CalculateObjective();
            }
        }

        currentDay++;
        OnDayChanged?.Invoke(); // Trigger semua gedung untuk mengurangi durasi kartu

        Debug.Log($"[SISTEM] Hari berganti ke-{currentDay} | Uang Kas Koperasi: {statEkonomi}");

        // Panggil sistem Drafting UI di sini
    }

    private void CalculateObjective()
    {
        // Target n = 30 + (10 x n^2)
        currentObjectiveTarget = 30 + (10 * (int)Mathf.Pow(currentWeek, 2));
        UIManager.Instance.UpdateObjectiveUI(currentObjectiveTarget);
    }

    private bool IsObjectiveMet()
    {
        // Asumsi objektif mengharuskan ketiga stat mencapai target. 
        // Sesuaikan jika logika objektifmu berbeda.
        return statEkonomi >= currentObjectiveTarget &&
               statLingkungan >= currentObjectiveTarget &&
               statSosial >= currentObjectiveTarget;
    }

    private void TriggerGameOver()
    {
        OnGameOver?.Invoke();
        UIManager.Instance.ShowGameOver(currentDay, statEkonomi + statLingkungan + statSosial);
    }

    public void ModifyStats(StatType type, int amount)
    {
        switch (type)
        {
            case StatType.Ekonomi: statEkonomi += amount; break;
            case StatType.Lingkungan: statLingkungan += amount; break;
            case StatType.Sosial: statSosial += amount; break;
        }
        UIManager.Instance.UpdateStatsUI();
    }
}