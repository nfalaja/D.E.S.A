using UnityEngine;
using System;
using TMPro;

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

    [Header("Visual Transisi")]
    public GameObject dayOverlay; // Panel hitam transparan untuk transisi
    public TextMeshProUGUI txtDayNumber; // Teks "Day 2", "Day 3", dst.

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
        GiveInitialCards();
        // Berikan 3 kartu awal di sini (Panggil fungsi dari HandManager)
    }

    private void GiveInitialCards()
    {
        // Cari 3 kartu awal dari list di DraftingManager
        // Idealnya kamu punya list khusus atau menandai kartu mana yang untuk awal
        for (int i = 0; i < 3; i++)
        {
            if (DraftingManager.Instance.allAvailableCards.Count > i)
            {
                HandManager.Instance.AddCardToHand(DraftingManager.Instance.allAvailableCards[i]);
            }
        }
    }

    public void NextDay()
    {
        statEkonomi += 10;

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
        StartCoroutine(ShowDayTransition()); // Picu visual transisi
        OnDayChanged?.Invoke();
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

    private bool isTransitioning = false; // Kunci pengaman

    public void TryStartNextDay()
    {
        // Jika sedang transisi, blokir semua klik!
        if (isTransitioning)
        {
            Debug.LogWarning("[SISTEM] Sabar! Hari sedang berganti...");
            return;
        }

        StartCoroutine(ShowDayTransition());
    }

    public void OnClickNextDay()
    {
        if (isTransitioning) return; // Jika sedang transisi, abaikan klik pemain

        StartCoroutine(ShowDayTransition());
    }

    private System.Collections.IEnumerator ShowDayTransition()
    {
        isTransitioning = true; // Kunci pintu

        dayOverlay.SetActive(true);
        txtDayNumber.text = "Day " + currentDay;
        
        ExecuteDailyLogic();

        yield return new WaitForSeconds(1.5f); // Layar redup selama 1.5 detik

        dayOverlay.SetActive(false);
        DraftingManager.Instance.ShowDrafting(); // Baru munculkan drafting setelah transisi selesai

        isTransitioning = false; // Buka pintu kembali setelah semua selesai
    }

    private void ExecuteDailyLogic()
    {
        statEkonomi += 10; // Income Koperasi

        // Cek Objektif Mingguan
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
        OnDayChanged?.Invoke(); // Ini untuk update durasi kartu di bangunan
        UIManager.Instance.UpdateStatsUI();
    }
}