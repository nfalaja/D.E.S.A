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
    public GameObject pausePanel; // Tarik Panel_Pause ke sini nanti

    [Header("Game State")]
    public int currentDay = 1;
    public int currentWeek = 1;
    //public int currentObjectiveTarget;

    [Header("Visual Transisi")]
    public GameObject dayOverlay; // Panel hitam transparan untuk transisi
    public TextMeshProUGUI txtDayNumber; // Teks "Day 2", "Day 3", dst.

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

    //[HideInInspector] public int currentTarget;

    private void CalculateObjective()
    {
        // 1. Hitung angkanya berdasarkan rumusmu: 30 + (10 * n^2)
        currentObjectiveTarget = 30 + (10 * (currentWeek * currentWeek));

        // 2. Tentukan Fase Kesulitan Berdasarkan Minggu
        if (currentWeek <= 2)
        {
            // MINGGU 1 & 2: Fase Pengenalan (Ekonomi Saja)
            reqEkonomi = true;
            reqSosial = false;
            reqLingkungan = false;
            Debug.Log($"[SISTEM] Minggu {currentWeek}: Syarat lulus hanya EKONOMI ({currentObjectiveTarget})");
        }
        else if (currentWeek <= 4)
        {
            // MINGGU 3 & 4: Fase Pertumbuhan (Ekonomi + Sosial)
            reqEkonomi = true;
            reqSosial = true;
            reqLingkungan = false;
            Debug.Log($"[SISTEM] Minggu {currentWeek}: Syarat lulus EKONOMI & SOSIAL (Masing-masing {currentObjectiveTarget})");
        }
        else
        {
            // MINGGU 5+: Fase Bertahan Hidup (Ketiga Pilar)
            reqEkonomi = true;
            reqSosial = true;
            reqLingkungan = true;
            Debug.Log($"[SISTEM] Minggu {currentWeek}: Syarat lulus SEMUA STAT (Masing-masing {currentObjectiveTarget})");
        }

        UIManager.Instance.UpdateObjectiveUI(currentObjectiveTarget, reqEkonomi, reqSosial, reqLingkungan);

        // 3. (Opsional) Update UI. Kamu harus menyesuaikan skrip UIManager-mu 
        // agar hanya menampilkan target stat yang variabel 'req'-nya bernilai true.
        // UIManager.Instance.UpdateObjectiveUI(currentObjectiveTarget, reqEkonomi, reqSosial, reqLingkungan);
    }

    private bool IsObjectiveMet()
    {
        // Asumsi awal: Semua dianggap lulus
        bool passEko = true;
        bool passSos = true;
        bool passLing = true;

        // Jika stat tersebut diwajibkan minggu ini, cek apakah nilainya mencukupi
        if (reqEkonomi)
            passEko = statEkonomi >= currentObjectiveTarget;

        if (reqSosial)
            passSos = statSosial >= currentObjectiveTarget;

        if (reqLingkungan)
            passLing = statLingkungan >= currentObjectiveTarget;

        // Hakim hanya akan mengetuk palu "Lulus" jika semua stat yang diwajibkan terpenuhi
        return passEko && passSos && passLing;
    }

    private void TriggerGameOver()
    {
        Debug.Log("[SISTEM] GAME OVER Dicu!");

        // 1. Kunci status game
        isGameOver = true;

        // 2. Hentikan waktu
        Time.timeScale = 0f;
        int rekorMinggu = PlayerPrefs.GetInt("HighscoreWeek", 0);
        // 2. Bandingkan dengan pencapaian sekarang
        if (currentWeek > rekorMinggu)
        {
            // Jika pecah rekor, simpan ke memori permanen
            PlayerPrefs.SetInt("HighscoreWeek", currentWeek);
            PlayerPrefs.Save(); // Memastikan data tertulis ke storage
            Debug.Log($"[REKOR] Baru! Minggu {currentWeek} berhasil disimpan.");
        }

        // 3. Bersihkan sisa-sisa UI yang tertinggal
        if (DraftingManager.Instance != null && DraftingManager.Instance.draftingPanel != null)
        {
            DraftingManager.Instance.draftingPanel.SetActive(false);
        }

        // MATIKAN PAKSA PANEL TRANSISI HARI (Sesuaikan nama variabel 'dayOverlay' dengan milikmu)
        if (dayOverlay != null)
        {
            dayOverlay.SetActive(false);
        }

        // 4. Munculkan Vonis
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

        ExecuteDailyLogic();

        if (isGameOver)
        {
            yield break;
        }

        txtDayNumber.text = "Day " + currentDay;


        yield return new WaitForSeconds(1.5f); // Layar redup selama 1.5 detik

        dayOverlay.SetActive(false);

        DraftingManager.Instance.ShowDrafting(); // Baru munculkan drafting setelah transisi selesai

        isTransitioning = false; // Buka pintu kembali setelah semua selesai
    }

    private void ExecuteDailyLogic()
    {
        // 1. PANEN HARIAN DULU
        // Koperasi dan Bangunan menyetorkan poin mereka untuk hari ini
        statEkonomi += 10;
        OnDayChanged?.Invoke();

        // 2. EVALUASI AKHIR MINGGU
        // Cek apakah hari ini adalah hari ke-7, 14, 21, dst.
        if (currentDay % 7 == 0)
        {
            // Apakah poin setoran tadi sudah mencapai target?
            if (!IsObjectiveMet())
            {
                Debug.Log("[SISTEM] Target Gagal. Game Over dipicu.");
                TriggerGameOver();
                return; // Hentikan semuanya, jangan izinkan masuk ke hari berikutnya
            }
            else
            {
                Debug.Log("[SISTEM] Target Tercapai! Lanjut ke minggu berikutnya.");
                currentWeek++;
                CalculateObjective();
            }
        }

        // 3. MASUKI HARI ESOK
        currentDay++;
        UIManager.Instance.UpdateStatsUI();
    }

    // --- PROTOKOL GAME OVER NAVIGATION ---

    public void RetryGame()
    {
        Debug.Log("[SISTEM] Memulai ulang permainan...");
        isGameOver = false;
        Time.timeScale = 1f; // WAJIB: Cairkan waktu yang beku

        // Memuat ulang Scene yang sedang aktif saat ini secara otomatis
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToMainMenu()
    {
        Debug.Log("[SISTEM] Kembali ke Main Menu...");
        Time.timeScale = 1f; // WAJIB: Cairkan waktu

        // Ganti "MainMenu" dengan nama Scene main menu buatan temanmu yang presisi
        SceneManager.LoadScene("MainMenu");
    }

    public void ExitGame()
    {
        Debug.Log("[SISTEM] Keluar dari aplikasi.");
        Application.Quit();
    }

    void Update()
    {
        // Cek apakah ada keyboard, lalu cek apakah tombol Escape ditekan frame ini
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            // JANGAN izinkan pause jika sedang Game Over atau sedang Transisi Hari
            if (!isGameOver && !isTransitioning)
            {
                TogglePause();
            }
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f; // Bekukan waktu
            pausePanel.SetActive(true);
            Debug.Log("[SISTEM] Game Dipause.");
        }
        else
        {
            Time.timeScale = 1f; // Jalankan waktu kembali
            pausePanel.SetActive(false);
            Debug.Log("[SISTEM] Game Berlanjut.");
        }
    }

    public void ResumeGame()
    {
        // Pastikan status kembali normal sebelum lanjut
        isPaused = false;
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
    }
}

