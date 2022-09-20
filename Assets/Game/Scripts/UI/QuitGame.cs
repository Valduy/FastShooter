using UnityEngine;

public class QuitGame : MonoBehaviour
{
    public void Exit()
    {
        Application.Quit();
        Debug.Log("Quit.");
    }
}
