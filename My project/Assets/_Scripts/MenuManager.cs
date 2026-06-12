using UnityEngine;
using TMPro;

public class CutsceneManager : MonoBehaviour
{
    [System.Serializable]
    public class CutsceneStep
    {
        public GameObject gambarCutscene; // Gambar untuk cutscene ini (misal: Gambar1)
        [TextArea(3, 5)]
        public string teksDialog;         // Teks dialog yang sesuai dengan gambar ini
    }

    [Header("UI Components")]
    public TextMeshProUGUI dialogText;   // Tarik 'Text (TMP)' ke sini
    public GameObject wadahDialog;       // Tarik objek 'Wadah/Panel Dialog' Anda ke sini

    [Header("Cutscene Sequence")]
    public CutsceneStep[] cutsceneSequence; // List urutan gambar dan dialog

    private int currentIndex = 0;

    void Start()
    {
        // Pastikan wadah dialog aktif saat mulai
        if (wadahDialog != null) wadahDialog.SetActive(true);

        // Sembunyikan semua gambar dulu untuk keamanan
        SembunyikanSemuaGambar();

        // Tampilkan cutscene pertama
        TampilkanStep(currentIndex);
    }

    public void AdvanceDialog()
    {
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

    void TampilkanStep(int index)
    {
        SembunyikanSemuaGambar();

        // Aktifkan gambar yang aktif sekarang
        if (cutsceneSequence[index].gambarCutscene != null)
        {
            cutsceneSequence[index].gambarCutscene.SetActive(true);
        }

        // Ubah teks dialognya
        if (dialogText != null)
        {
            dialogText.text = cutsceneSequence[index].teksDialog;
        }
    }

    void SembunyikanSemuaGambar()
    {
        // Menonaktifkan semua gambar yang didaftarkan agar tidak tumpang tindih
        foreach (var step in cutsceneSequence)
        {
            if (step.gambarCutscene != null)
            {
                step.gambarCutscene.SetActive(false);
            }
        }
    }

    void SelesaiCutscene()
    {
        Debug.Log("Komik selesai, masuk ke game utama!");
        if (wadahDialog != null) wadahDialog.SetActive(false);
        // Tambahkan fungsi pindah scene Anda di sini jika diperlukan
    }
}