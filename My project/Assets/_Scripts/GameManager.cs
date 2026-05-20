using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Player Stats")]
    public int statEkonomi = 0;
    public int statLingkungan = 0;
    public int statSosial = 0;

    [Header("Pause Settings")]
    public bool isPaused = false;
    public GameObject pausePanel;

    [Header("Game State")]
    public int currentDay = 1;
    public int currentWeek = 1;

    [Header("Visual Transisi")]
    public GameObject dayOverlay;
    public TextMeshProUGUI txtDayNumber;

    [Header("Saklar Objektif Aktif")]
    public bool reqEkonomi;
    public bool reqSosial;
    public bool reqLingkungan;

    public event Action OnDayChanged;
    public event Action OnGameOver;

    [HideInInspector] public bool isGameOver = false;
    [HideInInspector] public int currentObjectiveTarget;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        CalculateObjective();
        GiveInitialCards();

        isPaused = false;
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
    }

    private void GiveInitialCards()
    {
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
        StartCoroutine(ShowDayTransition());
        OnDayChanged?.Invoke();
    }

    private void CalculateObjective()
    {
        currentObjectiveTarget = 30 + (10 * (currentWeek * currentWeek));

        if (currentWeek <= 2)
        {
            reqEkonomi = true;
            reqSosial = false;
            reqLingkungan = false;
            Debug.Log($"[SISTEM] Minggu {currentWeek}: Syarat lulus hanya EKONOMI ({currentObjectiveTarget})");
        }
        else if (currentWeek <= 4)
        {
            reqEkonomi = true;
            reqSosial = true;
            reqLingkungan = false;
            Debug.Log($"[SISTEM] Minggu {currentWeek}: Syarat lulus EKONOMI & SOSIAL (Masing-masing {currentObjectiveTarget})");
        }
        else
        {
            reqEkonomi = true;
            reqSosial = true;
            reqLingkungan = true;
            Debug.Log($"[SISTEM] Minggu {currentWeek}: Syarat lulus SEMUA STAT (Masing-masing {currentObjectiveTarget})");
        }

        UIManager.Instance.UpdateObjectiveUI(currentObjectiveTarget, reqEkonomi, reqSosial, reqLingkungan);
    }

    private bool IsObjectiveMet()
    {
        bool passEko = true;
        bool passSos = true;
        bool passLing = true;

        if (reqEkonomi) passEko = statEkonomi >= currentObjectiveTarget;
        if (reqSosial) passSos = statSosial >= currentObjectiveTarget;
        if (reqLingkungan) passLing = statLingkungan >= currentObjectiveTarget;

        return passEko && passSos && passLing;
    }

    private void TriggerGameOver()
    {
        Debug.Log("[SISTEM] GAME OVER Dipicu!");

        isGameOver = true;
        Time.timeScale = 0f;

        int rekorMinggu = PlayerPrefs.GetInt("HighscoreWeek", 0);
        if (currentWeek > rekorMinggu)
        {
            PlayerPrefs.SetInt("HighscoreWeek", currentWeek);
            PlayerPrefs.Save();
            Debug.Log($"[REKOR] Baru! Minggu {currentWeek} berhasil disimpan.");
        }

        if (DraftingManager.Instance != null && DraftingManager.Instance.draftingPanel != null)
        {
            DraftingManager.Instance.draftingPanel.SetActive(false);
        }

        if (dayOverlay != null)
        {
            dayOverlay.SetActive(false);
        }

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

    private bool isTransitioning = false;

    public void TryStartNextDay()
    {
        if (isTransitioning)
        {
            Debug.LogWarning("[SISTEM] Sabar! Hari sedang berganti...");
            return;
        }

        StartCoroutine(ShowDayTransition());
    }

    public void OnClickNextDay()
    {
        if (isTransitioning) return;
        StartCoroutine(ShowDayTransition());
    }

    private System.Collections.IEnumerator ShowDayTransition()
    {
        isTransitioning = true;
        dayOverlay.SetActive(true);

        ExecuteDailyLogic();

        if (isGameOver)
        {
            yield break;
        }

        txtDayNumber.text = "Day " + currentDay;

        yield return new WaitForSeconds(1.5f);

        dayOverlay.SetActive(false);

        DraftingManager.Instance.ShowDrafting();

        isTransitioning = false;
    }

    private void ExecuteDailyLogic()
    {
        statEkonomi += 10;
        OnDayChanged?.Invoke();

        if (currentDay % 7 == 0)
        {
            if (!IsObjectiveMet())
            {
                Debug.Log("[SISTEM] Target Gagal. Game Over dipicu.");
                TriggerGameOver();
                return;
            }
            else
            {
                Debug.Log("[SISTEM] Target Tercapai! Lanjut ke minggu berikutnya.");
                currentWeek++;
                CalculateObjective();
            }
        }

        currentDay++;
        UIManager.Instance.UpdateStatsUI();
    }

    public void RetryGame()
    {
        Debug.Log("[SISTEM] Memulai ulang permainan...");
        isGameOver = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToMainMenu()
    {
        Debug.Log("[SISTEM] Kembali ke Main Menu...");
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void ExitGame()
    {
        Debug.Log("[SISTEM] Keluar dari aplikasi.");
        Application.Quit();
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            // Jangan izinkan apapun saat Game Over atau Transisi
            if (isGameOver || isTransitioning) return;

            // PRIORITAS 1: Kalau Papan OBJ sedang terbuka, tutup dulu
            if (UIManager.Instance.papanOBJ.activeSelf)
            {
                UIManager.Instance.papanOBJ.SetActive(false);
                return; // Berhenti di sini, jangan lanjut ke Pause
            }

            // PRIORITAS 2: Baru toggle Pause seperti biasa
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f;
            pausePanel.SetActive(true);
            Debug.Log("[SISTEM] Game Dipause.");
        }
        else
        {
            Time.timeScale = 1f;
            pausePanel.SetActive(false);
            Debug.Log("[SISTEM] Game Berlanjut");
        }
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
    }
}