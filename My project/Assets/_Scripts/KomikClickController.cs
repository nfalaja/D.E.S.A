using UnityEngine;
using UnityEngine.SceneManagement;

public class KomikClickController : MonoBehaviour
{
    [Header("Susunan Gambar Komik")]
    // Masukkan semua objek Gambar kamu ke sini sesuai urutan muncul
    [SerializeField] private GameObject[] semuaGambar;

    [Header("Pengaturan Scene")]
    [SerializeField] private string namaSceneGameplayUtama;

    private int indeksSekarang = 0;

    void Start()
    {
        // Memastikan di awal game hanya gambar pertama yang aktif
        for (int i = 0; i < semuaGambar.Length; i++)
        {
            semuaGambar[i].SetActive(i == 0);
        }
    }

    // Fungsi ini yang akan dipanggil setiap kali layar diklik
    public void TampilkanGambarBerikutnya()
    {
        indeksSekarang++;

        // Jika gambar masih ada, nyalakan gambar berikutnya
        if (indeksSekarang < semuaGambar.Length)
        {
            semuaGambar[indeksSekarang].SetActive(true);
        }
        else
        {
            // Jika gambar sudah habis, langsung pindah ke game utama
            Debug.Log("Komik selesai, masuk ke game utama!");
            SceneManager.LoadScene(namaSceneGameplayUtama);
        }
    }
}