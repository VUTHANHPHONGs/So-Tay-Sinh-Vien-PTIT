using UnityEngine;

public class ToggleAudioSources : MonoBehaviour
{
    private bool isMuted = false;
    public AudioSource[] audioSources; // G?n c�c AudioSource qua Inspector

    public void ToggleMute()
    {
        // ??i tr?ng th�i gi?a mute v� unmute
        isMuted = !isMuted;

        // Duy?t qua t?t c? c�c AudioSource v� thay ??i tr?ng th�i mute
        foreach (AudioSource audioSource in audioSources)
        {
            if (audioSource.isPlaying || isMuted)
            {
                // N?u ?ang ph�t ho?c c?n mute, thay ??i tr?ng th�i mute
                audioSource.mute = isMuted;
            }
            // N?u kh�ng ph�t nh?c, gi? nguy�n tr?ng th�i mute
        }
    }
}
