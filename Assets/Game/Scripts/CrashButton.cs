using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrashButton : MonoBehaviour
{
    public void Crash()
    {
        throw new System.Exception("Test");
    }
}
