using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Stats Text")]
    public TextMeshProUGUI txtEcon;
    public TextMeshProUGUI txtEnv;
    public TextMeshProUGUI txtSoc;

    [Header("Boards & Panels")]
    public TextMeshProUGUI txtObjective;
    public TextMeshProUGUI txtNotification;
    public GameObject gameOverPanel;
    public TextMeshProUGUI txtGameOverDetails;

    //[Header("UI Objektif")]
    //public TextMeshProUGUI txtObjective; // Slot untuk teks target di kanvas

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void UpdateStatsUI()
    {
        txtEcon.text = "Eko: " + GameManager.Instance.statEkonomi;
        txtEnv.text = "Ling: " + GameManager.Instance.statLingkungan;
        txtSoc.text = "Sos: " + GameManager.Instance.statSosial;
        CheckNotifications();
    }

    public void UpdateObjectiveUI(int targetScore, bool reqEko, bool reqSos, bool reqLing)
    {
        string teksBaru = "<b>Target Minggu Ini:</b>\n";

        if (reqEko) teksBaru += $"- Ekonomi: {targetScore}\n";
        if (reqSos) teksBaru += $"- Sosial: {targetScore}\n";
        if (reqLing) teksBaru += $"- Lingkungan: {targetScore}\n";

        txtObjective.text = teksBaru;
    }

    private void CheckNotifications()
    {
        // Logika sederhana papan pemberitahuan
        int target = GameManager.Instance.currentObjectiveTarget;
        if (GameManager.Instance.statLingkungan < target)
            txtNotification.text = "Kampung kita kurang asri!";
        else if (GameManager.Instance.statSosial < target)
            txtNotification.text = "Warga kurang berinteraksi!";
        else if (GameManager.Instance.statEkonomi < target)
            txtNotification.text = "Kas desa menipis!";
        else
            txtNotification.text = "Semua aman terkendali.";
    }

    public void ShowGameOver(int totalDays, int totalStats)
    {
        gameOverPanel.SetActive(true);
        // Ambil rekor tertinggi yang tersimpan
        int rekorTertinggi = PlayerPrefs.GetInt("HighscoreWeek", 0);

        // Rakit teks detail
        // Kamu bisa menggunakan \n untuk baris baru agar lebih rapi
        txtGameOverDetails.text = $"Kamu bertahan selama {totalDays} Hari.\n" +
                                  $"Total Stat Terakhir: {totalStats}\n" +
                                  $"<b>Rekor Minggu Tertinggi: Minggu {rekorTertinggi}</b>";
    }

    // Sambungkan ke Button di Inspector
    public void OnClickNextDay()
    {
        GameManager.Instance.TryStartNextDay();
    }


}