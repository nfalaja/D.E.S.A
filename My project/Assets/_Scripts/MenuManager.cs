using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // Ditambahkan agar bisa pindah scene

public class CutsceneManager : MonoBehaviour
{
    [System.Serializable]
    public class CutsceneStep
    {
        public GameObject gambarCutscene; // Gambar untuk cutscene ini
        public string namaPembicara;      // BARU: Nama karakter yang berbicara di slide ini
        [TextArea(3, 5)]
        public string teksDialog;         // Teks dialog yang sesuai dengan gambar ini
    }

    [Header("UI Components")]
    public TextMeshProUGUI dialogText;       // Tempat teks dialog utama (Objek 'Dialog')
    public TextMeshProUGUI namaPembicaraText; // BARU: Tempat nama karakter (Objek 'Pembicara')
    public GameObject wadahDialog;           // Tempat background dialog (Objek 'Wadah')

    [Header("Cutscene Sequence")]
    public CutsceneStep[] cutsceneSequence; // List urutan gambar dan dialog

    private int currentIndex = 0;

    void Start()
    {
        if (wadahDialog != null) wadahDialog.SetActive(true);

        SembunyikanSemuaGambar();
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

        // 1. Aktifkan gambar yang aktif sekarang
        if (cutsceneSequence[index].gambarCutscene != null)
        {
            cutsceneSequence[index].gambarCutscene.SetActive(true);
        }

        // 2. Ubah teks nama pembicara
        if (namaPembicaraText != null)
        {
            namaPembicaraText.text = cutsceneSequence[index].namaPembicara;
        }

        // 3. Ubah teks isi dialognya
        if (dialogText != null)
        {
            dialogText.text = cutsceneSequence[index].teksDialog;
        }
    }

    void SembunyikanSemuaGambar()
    {
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

        // Menggunakan nama scene target dari input di editor kamu
        SceneManager.LoadScene("ToturialScane");
    }
}