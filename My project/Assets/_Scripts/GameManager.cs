using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System;
using TMPro;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Random Events")]
    public List<EventData> possibleEvents;
    [Range(0, 100)] public int eventChancePerWeek = 30; // 30% kemungkinan terjadi

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
        // 1. TETAPKAN MODAL AWAL SECARA ABSOLUT
        statEkonomi = 100;
        statLingkungan = 0;
        statSosial = 0;

        CalculateObjective();
        GiveInitialCards();

        isPaused = false;
        Time.timeScale = 1f;
        pausePanel.SetActive(false);

        // 2. PAKSA UI BERSINKRONISASI DI FRAME PERTAMA
        // Tanpa baris ini, pemain hanya akan melihat UI kosong/default prefab.
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateStatsUI();
        }
        else
        {
            Debug.LogError("[GameManager] UIManager belum siap! Pastikan Script Execution Order UIManager lebih dulu dari GameManager.");
        }
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



    private void CalculateObjective()
    {
        // KODE PENYELAMATAN: Pertumbuhan Linear (Bukan Kuadratik)
        currentObjectiveTarget = 50 + (30 * currentWeek) + (15 * currentWeek * currentWeek);

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

        // Catat minggu sekarang sebelum logika berjalan
        int previousWeek = currentWeek;

        // Hitung stat dan cek kematian di balik layar gelap
        ExecuteDailyLogic();

        if (isGameOver)
        {
            yield break;
        }

        txtDayNumber.text = "Hari ke " + currentDay;

        // Tunggu sebentar agar pemain bisa membaca "Hari ke X"
        yield return new WaitForSeconds(1.5f);

        // Hilangkan layar hitam transisi
        dayOverlay.SetActive(false);

        // --- CEK EVENT MINGGUAN SEBELUM DRAFTING ---
        // Jika minggu bertambah (artinya baru saja ganti minggu) DAN minimal minggu ke-3
        bool isNewWeek = currentWeek > previousWeek;

        if (isNewWeek && currentWeek >= 3)
        {
            // Coba picu event. Fungsi ini akan mengembalikan TRUE jika event terjadi.
            bool eventTerjadi = TriggerRandomEvent();

            // Jika event terjadi, HENTIKAN coroutine ini, JANGAN tampilkan drafting.
            // Pemain harus menutup panel event dulu. Nanti tombol tutup event yang akan
            // memanggil panel drafting.
            if (eventTerjadi)
            {
                isTransitioning = false;
                yield break; // Berhenti di sini!
            }
        }

        // Jika tidak ada ganti minggu ATAU ganti minggu tapi tidak ada event yang terpicu,
        // lanjutkan ke panel drafting seperti biasa.
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

                // --- PEMICU EVENT (Hanya muncul mulai Minggu ke-3) ---
                // Beri pemain waktu bernapas di 2 minggu awal untuk menata ekonomi.
                //if (currentWeek >= 3)
                //{
                //    TriggerRandomEvent();
                //}
            }
        }

        currentDay++;
        UIManager.Instance.UpdateStatsUI();
    }

    private bool TriggerRandomEvent()
    {
        if (possibleEvents == null || possibleEvents.Count == 0) return false;

        int roll = UnityEngine.Random.Range(0, 100);
        int currentChance = Mathf.Min(eventChancePerWeek + (currentWeek * 5), 60);

        if (roll < currentChance)
        {
            int randomIndex = UnityEngine.Random.Range(0, possibleEvents.Count);
            EventData eventAcak = possibleEvents[randomIndex];

            Debug.Log($"[EVENT MINGGUAN] {eventAcak.eventName} Terjadi!");

            foreach (var modifier in eventAcak.penalties)
            {
                ModifyStats(modifier.statType, modifier.amount);
            }

            UIManager.Instance.ShowEventNotification(eventAcak);
            return true;
        }
        return false;
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
            // Jika game over atau sedang transisi, BERHENTI (Gunakan return kosong)
            if (isGameOver || isTransitioning) return;

            // Langsung panggil fungsinya, JANGAN pakai kata return di depannya
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