using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class KomikClickController : MonoBehaviour
{
    [Header("═══ Susunan Gambar Komik ═══")]
    [SerializeField] private GameObject[] semuaGambar;

    [Header("═══ Pengaturan Scene ═══")]
    [SerializeField] private string namaSceneGameplayUtama;

    private int indeksSekarang = 0;
    private bool isInitialized = false;

    void Start()
    {
        // ✅ Validasi array
        if (semuaGambar == null || semuaGambar.Length == 0)
        {
            Debug.LogError("❌ semuaGambar kosong!");
            enabled = false;
            return;
        }

        Debug.Log($"✅ Komik initialized dengan {semuaGambar.Length} gambar");

        // Matikan semua gambar dulu
        for (int i = 0; i < semuaGambar.Length; i++)
        {
            if (semuaGambar[i] != null)
            {
                semuaGambar[i].SetActive(false);
            }
        }

        // Aktifkan hanya gambar pertama
        if (semuaGambar[0] != null)
        {
            semuaGambar[0].SetActive(true);
            Debug.Log($"✅ Gambar 0 ditampilkan");
        }

        indeksSekarang = 0;
        isInitialized = true;
    }

    void Update()
    {
        if (!isInitialized) return;

        // ✅ Gunakan Input System
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            TampilkanGambarBerikutnya();
        }

        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            TampilkanGambarBerikutnya();
        }
    }

    /// <summary>
    /// Tampilkan gambar berikutnya dengan bounds checking yang ketat
    /// </summary>
    public void TampilkanGambarBerikutnya()
    {
        // ✅ Safety check: apakah indeks sekarang valid
        if (indeksSekarang < 0 || indeksSekarang >= semuaGambar.Length)
        {
            Debug.LogError($"❌ FATAL: indeksSekarang {indeksSekarang} out of bounds! Length: {semuaGambar.Length}");
            return;
        }

        // ✅ Matikan gambar sekarang
        if (semuaGambar[indeksSekarang] != null)
        {
            semuaGambar[indeksSekarang].SetActive(false);
            Debug.Log($"🔴 Gambar {indeksSekarang} dimatikan");
        }

        // ✅ Increment index
        indeksSekarang++;

        // ✅ Cek apakah masih ada gambar berikutnya
        if (indeksSekarang < semuaGambar.Length)
        {
            // Ada gambar berikutnya
            if (semuaGambar[indeksSekarang] != null)
            {
                semuaGambar[indeksSekarang].SetActive(true);
                Debug.Log($"✅ Gambar {indeksSekarang} ditampilkan");
            }
            else
            {
                Debug.LogWarning($"⚠️ Gambar {indeksSekarang} adalah NULL!");
            }
        }
        else
        {
            // Sudah mencapai akhir
            indeksSekarang--; // Reset ke gambar terakhir
            Debug.Log($"🎬 Komik selesai! Total gambar: {semuaGambar.Length}");
            LoadNextScene();
        }
    }

    /// <summary>
    /// Kembali ke gambar sebelumnya
    /// </summary>
    public void TampilkanGambarSebelumnya()
    {
        // ✅ Safety check
        if (indeksSekarang <= 0)
        {
            Debug.LogWarning("⚠️ Sudah di gambar pertama!");
            return;
        }

        // Matikan gambar sekarang
        if (semuaGambar[indeksSekarang] != null)
        {
            semuaGambar[indeksSekarang].SetActive(false);
        }

        // Mundur
        indeksSekarang--;

        // Tampilkan gambar sebelumnya
        if (semuaGambar[indeksSekarang] != null)
        {
            semuaGambar[indeksSekarang].SetActive(true);
            Debug.Log($"✅ Kembali ke gambar {indeksSekarang}");
        }
    }

    /// <summary>
    /// Reset ke gambar pertama
    /// </summary>
    public void ResetKomik()
    {
        // Matikan semua gambar
        for (int i = 0; i < semuaGambar.Length; i++)
        {
            if (semuaGambar[i] != null)
            {
                semuaGambar[i].SetActive(false);
            }
        }

        // Reset index dan tampilkan gambar pertama
        indeksSekarang = 0;
        if (semuaGambar[0] != null)
        {
            semuaGambar[0].SetActive(true);
        }

        Debug.Log("🔄 Komik di-reset ke gambar pertama");
    }

    /// <summary>
    /// Load scene berikutnya dengan validasi
    /// </summary>
    private void LoadNextScene()
    {
        if (string.IsNullOrEmpty(namaSceneGameplayUtama))
        {
            Debug.LogError("❌ namaSceneGameplayUtama kosong!");
            return;
        }

        Debug.Log($"🚀 Loading scene: {namaSceneGameplayUtama}");
        SceneManager.LoadScene(namaSceneGameplayUtama);
    }

    /// <summary>
    /// Get current image index
    /// </summary>
    public int GetCurrentIndex()
    {
        return indeksSekarang;
    }

    /// <summary>
    /// Get total images count
    /// </summary>
    public int GetTotalImages()
    {
        return semuaGambar != null ? semuaGambar.Length : 0;
    }

    /// <summary>
    /// Check if komik complete
    /// </summary>
    public bool IsKomikComplete()
    {
        return indeksSekarang >= semuaGambar.Length - 1;
    }
}