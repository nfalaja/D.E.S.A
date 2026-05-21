using UnityEngine;
using UnityEngine.Playables; // Wajib untuk mendeteksi Timeline
using UnityEngine.SceneManagement; // Wajib untuk berpindah scene

public class CutsceneAutoLoader : MonoBehaviour
{
    [SerializeField] private PlayableDirector playableDirector;
    [SerializeField] private string namaSceneGameplayUtama;

    private void OnEnable()
    {
        // Mendaftarkan fungsi ketika Timeline selesai berputar
        playableDirector.stopped += OnTimelineSelesai;
    }

    private void OnDisable()
    {
        // Membersihkan pendaftaran fungsi saat objek dinonaktifkan
        playableDirector.stopped -= OnTimelineSelesai;
    }

    private void OnTimelineSelesai(PlayableDirector director)
    {
        Debug.Log("Cutscene Selesai! Mengalihkan ke Game Utama...");
        // Berpindah ke scene gameplay utama secara otomatis
        SceneManager.LoadScene(namaSceneGameplayUtama);
    }
}