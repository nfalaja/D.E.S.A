using UnityEngine;

public class Efek_Umum : MonoBehaviour
{
    public AudioSource audioSource;

    public void PlaySound()
    {
        audioSource.Play();
    }
}