using UnityEngine;
using System.Collections;

public class PlayTwoSounds : MonoBehaviour
{
    private AudioSource audioSource;

    public AudioClip sound1;
    public AudioClip sound2;

    public float delay = 0.5f;

    private void Awake()
    {
        // Ambil AudioSource kalau ada, kalau tidak buat baru
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Setting dasar biar cocok untuk UI
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // 2D sound
    }

    public void PlaySounds()
    {
        StartCoroutine(PlaySequence());
    }

    private IEnumerator PlaySequence()
    {
        audioSource.PlayOneShot(sound1);
        yield return new WaitForSeconds(delay);
        audioSource.PlayOneShot(sound2);
    }
}