using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class CutsceneManager : MonoBehaviour
{
    [System.Serializable]
    public class CutsceneStep
    {
        public GameObject gambarCutscene;      // Gambar untuk cutscene ini
        public string namaPembicara;           // Nama karakter yang berbicara
        [TextArea(3, 5)]
        public string teksDialog;              // Teks dialog
    }

    [Header("═══ UI Components ═══")]
    public TextMeshProUGUI dialogText;         // Tempat teks dialog utama
    public TextMeshProUGUI namaPembicaraText;  // Tempat nama karakter
    public GameObject wadahDialog;             // Background dialog

    [Header("═══ Cutscene Sequence ═══")]
    public CutsceneStep[] cutsceneSequence;    // List urutan gambar dan dialog

    [Header("═══ Scene Settings ═══")]
    public string namaSceneTujuan = "GameScene"; // Nama scene tujuan

    private int currentIndex = 0;
    private bool isInitialized = false;

    void Start()
    {
        // ✅ Validasi array
        if (cutsceneSequence == null || cutsceneSequence.Length == 0)
        {
            Debug.LogError("❌ cutsceneSequence KOSONG! Isi di Inspector!");
            enabled = false;
            return;
        }

        Debug.Log($"✅ CutsceneManager initialized dengan {cutsceneSequence.Length} steps");

        if (wadahDialog != null) wadahDialog.SetActive(true);
        SembunyikanSemuaGambar();
        TampilkanStep(currentIndex);

        isInitialized = true;
    }

    void Update()
    {
        if (!isInitialized) return;

        // ✅ Input Mouse Click
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            AdvanceDialog();
        }

        // ✅ Input Space Key
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            AdvanceDialog();
        }
    }

    /// <summary>
    /// Lanjut ke step berikutnya
    /// </summary>
    public void AdvanceDialog()
    {
        if (!isInitialized) return;

        currentIndex++;

        if (currentIndex < cutsceneSequence.Length)
        {
            TampilkanStep(currentIndex);
        }
        else
        {
            SelesaiCutscene();
        }
    }

    /// <summary>
    /// Kembali ke step sebelumnya
    /// </summary>
    public void PreviousDialog()
    {
        if (!isInitialized) return;

        currentIndex--;

        if (currentIndex < 0)
        {
            currentIndex = 0;
            Debug.LogWarning("⚠️ Sudah di step pertama!");
            return;
        }

        TampilkanStep(currentIndex);
    }

    /// <summary>
    /// Tampilkan step tertentu
    /// </summary>
    void TampilkanStep(int index)
    {
        // ✅ Validasi index
        if (index < 0 || index >= cutsceneSequence.Length)
        {
            Debug.LogError($"❌ Index {index} tidak valid! Length: {cutsceneSequence.Length}");
            return;
        }

        // ✅ Cek apakah element null
        if (cutsceneSequence[index] == null)
        {
            Debug.LogError($"❌ cutsceneSequence[{index}] adalah NULL!");
            return;
        }

        CutsceneStep step = cutsceneSequence[index];

        SembunyikanSemuaGambar();

        // ✅ 1. Tampilkan gambar
        if (step.gambarCutscene != null)
        {
            step.gambarCutscene.SetActive(true);
            Debug.Log($"✅ Gambar {index} ditampilkan");
        }
        else
        {
            Debug.LogWarning($"⚠️ Gambar step {index} adalah NULL!");
        }

        // ✅ 2. Update nama pembicara
        if (namaPembicaraText != null)
        {
            namaPembicaraText.text = step.namaPembicara;
            Debug.Log($"✅ Nama pembicara: {step.namaPembicara}");
        }
        else
        {
            Debug.LogWarning("⚠️ namaPembicaraText tidak ter-assign!");
        }

        // ✅ 3. Update teks dialog
        if (dialogText != null)
        {
            dialogText.text = step.teksDialog;
            Debug.Log($"✅ Dialog: {step.teksDialog}");
        }
        else
        {
            Debug.LogWarning("⚠️ dialogText tidak ter-assign!");
        }

        Debug.Log($"📍 Step {index}/{cutsceneSequence.Length - 1}");
    }

    /// <summary>
    /// Sembunyikan semua gambar
    /// </summary>
    void SembunyikanSemuaGambar()
    {
        if (cutsceneSequence == null) return;

        foreach (var step in cutsceneSequence)
        {
            if (step != null && step.gambarCutscene != null)
            {
                step.gambarCutscene.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Cutscene selesai - load scene berikutnya
    /// </summary>
    void SelesaiCutscene()
    {
        Debug.Log($"🎬 Cutscene selesai! Loading scene: {namaSceneTujuan}");

        if (wadahDialog != null)
        {
            wadahDialog.SetActive(false);
        }

        if (!string.IsNullOrEmpty(namaSceneTujuan))
        {
            SceneManager.LoadScene(namaSceneTujuan);
        }
        else
        {
            Debug.LogError("❌ namaSceneTujuan kosong!");
        }
    }

    /// <summary>
    /// Reset ke step pertama
    /// </summary>
    public void ResetCutscene()
    {
        currentIndex = 0;
        SembunyikanSemuaGambar();
        TampilkanStep(currentIndex);
        Debug.Log("🔄 Cutscene di-reset");
    }

    /// <summary>
    /// Get current step index
    /// </summary>
    public int GetCurrentIndex()
    {
        return currentIndex;
    }

    /// <summary>
    /// Get total steps
    /// </summary>
    public int GetTotalSteps()
    {
        return cutsceneSequence != null ? cutsceneSequence.Length : 0;
    }

    /// <summary>
    /// Check if cutscene complete
    /// </summary>
    public bool IsCutsceneComplete()
    {
        return currentIndex >= cutsceneSequence.Length - 1;
    }
}