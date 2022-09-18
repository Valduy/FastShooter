using MoreMountains.CorgiEngine;
using System.Linq;
using UnityEngine;

public class TrainGameManager : MonoBehaviour
{
    private float _currentDistance;

    public string GameWinScene;
    public float Speed;
    public float MaxDistance;
    public Health[] Engines;

    public void Start()
    {
        _currentDistance = 0;
    }

    private void FixedUpdate()
    {
        var multiplier = Engines
            .Select(h => h.CurrentHealth / h.MaximumHealth)
            .Sum();
        
        _currentDistance += multiplier * Speed * Time.fixedDeltaTime;
        Debug.Log($"{_currentDistance} / {MaxDistance}");

        if (_currentDistance >= MaxDistance)
        {
            LevelManager.Instance.GotoLevel(GameWinScene);
        }
    }
}
