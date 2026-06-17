using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Event Panel")]
    public GameObject eventPanel;
    public TextMeshProUGUI txtEventTitle;
    public TextMeshProUGUI txtEventDescription;
    public TextMeshProUGUI txtEventEffects;

    [Header("Stats Text")]
    public TextMeshProUGUI txtEcon;
    public TextMeshProUGUI txtEnv;
    public TextMeshProUGUI txtSoc;

    [Header("Boards & Panels")]
    public TextMeshProUGUI txtObjective;
    public TextMeshProUGUI txtNotification;
    public GameObject gameOverPanel;
    public TextMeshProUGUI txtGameOverDetails;
    public GameObject papanOBJ; // Panel yang show/hide

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        papanOBJ.SetActive(false); // Sembunyikan saat game mulai
    }

    // Dipanggil oleh Btn_OBJ
    public void TogglePapanOBJ()
    {
        bool sedangAktif = papanOBJ.activeSelf;
        papanOBJ.SetActive(!sedangAktif);
    }

    public void ShowEventNotification(EventData krisis)
    {
        txtEventTitle.text = "INFO DESA: " + krisis.eventName;
        txtEventDescription.text = krisis.eventDescription;

        // --- PROTOKOL BEDAH EFEK ---
        string efekTeks = "<b>Dampak:</b>\n";

        // Loop melalui semua penalti/bonus yang ada di event ini
        if (krisis.penalties != null && krisis.penalties.Count > 0)
        {
            foreach (var modifier in krisis.penalties)
            {
                // Format: Jika minus tampilkan merah, jika plus (event baik) tampilkan hijau
                string tanda = modifier.amount > 0 ? "+" : "";
                string warnaHex = modifier.amount > 0 ? "#00FF00" : "#FF0000"; // Hijau atau Merah

                efekTeks += $"<color={warnaHex}>{modifier.statType}: {tanda}{modifier.amount}</color>\n";
            }
        }
        else
        {
            efekTeks += "Tidak ada dampak signifikan.";
        }

        // Tembakkan hasil rakitan teks ke UI
        if (txtEventEffects != null)
        {
            txtEventEffects.text = efekTeks;
        }
        // ---------------------------

        eventPanel.SetActive(true);
    }

    public void UpdateStatsUI()
    {
        txtEcon.text = ": " + GameManager.Instance.statEkonomi;
        txtEnv.text = ": " + GameManager.Instance.statLingkungan;
        txtSoc.text = ": " + GameManager.Instance.statSosial;
        CheckNotifications();
    }

    public void UpdateObjectiveUI(int targetScore, bool reqEko, bool reqSos, bool reqLing)
    {
        string teksBaru = "<b>Target Minggu Ini:</b>\n";

        if (reqEko) teksBaru += $"- Ekonomi: {targetScore}\n";
        if (reqSos) teksBaru += $"- Sosial: {targetScore}\n";
        if (reqLing) teksBaru += $"- Lingkungan: {targetScore}\n";

        txtObjective.text = teksBaru;
        papanOBJ.SetActive(false); // Auto-tutup saat minggu baru
    }

    private void CheckNotifications()
    {
        int target = GameManager.Instance.currentObjectiveTarget;
        if (GameManager.Instance.statLingkungan < target)
            txtNotification.text = "Kampung Kurang Asri!";
        else if (GameManager.Instance.statSosial < target)
            txtNotification.text = "Warga Kurang Berinteraksi!";
        else if (GameManager.Instance.statEkonomi < target)
            txtNotification.text = "Kas Desa Menipis!";
        else
            txtNotification.text = "Semua Aman Terkendali.";
    }

    public void ShowGameOver(int totalDays, int totalStats)
    {
        gameOverPanel.SetActive(true);
        int rekorTertinggi = PlayerPrefs.GetInt("HighscoreWeek", 0);

        txtGameOverDetails.text = $"Kamu bertahan selama {totalDays} Hari.\n" +
                                  $"Total Stat Terakhir: {totalStats}\n" +
                                  $"<b>Rekor Minggu Tertinggi: Minggu {rekorTertinggi}</b>";
    }

    public void CloseEventNotification()
    {
        eventPanel.SetActive(false);
        DraftingManager.Instance.ShowDrafting();
    }
}