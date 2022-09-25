using MoreMountains.Tools;
using System.Collections;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    private AudioSource _audioSource;

    public MMTouchButton[] MenuButtons;
    public AudioClip SelectSound;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void Exit()
    {
        SetButtonsInteractable(false);
        StartCoroutine(QuitCoroutine());
    }

    public void PlaySelect()
    {
        _audioSource.PlayOneShot(SelectSound);
    }

    private void SetButtonsInteractable(bool isInteractable)
    {
        foreach (MMTouchButton button in MenuButtons)
        {
            button.Interactable = isInteractable;
        }
    }

    private IEnumerator QuitCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        Application.Quit();
        Debug.Log("Quit.");
        SetButtonsInteractable(true);
    }
}
