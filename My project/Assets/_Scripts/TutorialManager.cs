using UnityEngine;
using UnityEngine.SceneManagement; // Wajib ditambahkan untuk fungsi pindah Scene

public class TutorialManager : MonoBehaviour
{
    public GameObject[] slides;

    [Header("Tulis nama Scene game utamamu di sini")]
    public string namaSceneTujuan; // Variabel baru untuk nama scene

    private int slideSekarang = 0;

    void Start()
    {
        // Matikan semua slide saat game mulai
        for (int i = 0; i < slides.Length; i++)
        {
            slides[i].SetActive(false);
        }

        // Nyalakan slide pertama saja
        if (slides.Length > 0)
        {
            slides[0].SetActive(true);
        }
    }

    public void KlikSelanjutnya()
    {
        // Matikan slide yang sedang tampil
        slides[slideSekarang].SetActive(false);

        // Pindah ke slide berikutnya
        slideSekarang++;

        // Cek apakah masih ada slide yang tersisa
        if (slideSekarang < slides.Length)
        {
            // Nyalakan slide berikutnya
            slides[slideSekarang].SetActive(true);
        }
        else
        {
            // Jika slide habis, muat Scene utama
            Debug.Log("Memuat Scene: " + namaSceneTujuan);
            SceneManager.LoadScene(namaSceneTujuan);
        }
    }
}
