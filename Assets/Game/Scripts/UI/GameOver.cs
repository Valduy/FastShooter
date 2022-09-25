using UnityEngine;

public class GameOver : MonoBehaviour
{
    private AudioSource _audioSource;

    public AudioClip SelectSound;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlaySelect()
    {
        _audioSource.PlayOneShot(SelectSound);
    }
}
