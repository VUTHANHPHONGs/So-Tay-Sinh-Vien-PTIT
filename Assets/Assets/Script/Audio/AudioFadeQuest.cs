using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AudioFadeQuest : MonoBehaviour
{
    public AudioSource audioSource1; // Audio source ??u ti�n
    public AudioSource audioSource2; // Audio source th? hai
    public Button fadeOutButton;
    public float fadeDuration = 2.0f;

    void Start()
    {
        fadeOutButton.onClick.AddListener(StartFadeOut);
    }

    void StartFadeOut()
    {
        StartCoroutine(FadeOutAudio());
    }

    IEnumerator FadeOutAudio()
    {
        float startVolume = audioSource1.volume;

        // Gi?m �m l??ng audioSource1 t? t?
        while (audioSource1.volume > 0)
        {
            audioSource1.volume -= startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }

        audioSource1.volume = 0;
        audioSource1.Stop(); // D?ng audioSource1 khi �m l??ng b?ng 0
        audioSource1.mute = true;

        // K�ch ho?t audioSource2 v� ph�t
        audioSource2.volume = 0.5f;
        audioSource2.Play();

        // Ch? cho audioSource2 ph�t h?t
        yield return new WaitUntil(() => !audioSource2.isPlaying);

        // Sau khi audioSource2 ph�t xong, m? l?i audioSource1
        audioSource1.volume = 0.5f;
        audioSource1.Play();
    }
}
