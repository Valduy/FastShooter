using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FBLevelController : MonoBehaviour
{
    void Start()
    {
        Firebase.Analytics.FirebaseAnalytics.LogEvent(Firebase.Analytics.FirebaseAnalytics.EventLevelStart);
        Debug.Log("Start");
    }

    void OnDestroy()
    {
        Firebase.Analytics.FirebaseAnalytics.LogEvent(Firebase.Analytics.FirebaseAnalytics.EventLevelEnd);
        Debug.Log("End");

    }
}
