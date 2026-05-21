using UnityEngine;
using UnityEngine.SceneManagement; // Wajib untuk mengatur perpindahan scene

public class MainMenuController : MonoBehaviour
{
    // Masukkan nama Scene Cutscene kamu secara persis di Inspector nanti
    [SerializeField] private string namaSceneCutscene;

    public void TekanTombolStart()
    {
        // Berpindah ke scene cutscene
        SceneManager.LoadScene(namaSceneCutscene);
    }
}