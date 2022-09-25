using MoreMountains.CorgiEngine;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TrainGameManager : MonoBehaviour
{
    private float _currentDistance;
    private bool _isEndFlag;

    public string GameWinScene;
    public float Speed;
    public float MaxDistance;
    public Slider TrainSlider;
    public Health[] Engines;

    public void Start()
    {
        _currentDistance = 0;
        _isEndFlag = false;
    }

    private void FixedUpdate()
    {
        if (_isEndFlag) 
        {
            return;
        }

        var multiplier = Engines
            .Select(h => h.CurrentHealth / h.MaximumHealth)
            .Sum();
        
        if (multiplier <= 0)
        {
            OnLose();
            _isEndFlag = true;
        }

        _currentDistance += multiplier * Speed * Time.fixedDeltaTime;
        TrainSlider.value = _currentDistance / MaxDistance;

        if (_currentDistance >= MaxDistance)
        {
            OnWin();
            _isEndFlag = true;
        }
    }

    private void OnLose()
    {
        foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
        {
            Debug.Log($"Lives: {GameManager.Instance.MaximumLives}");
            LevelManager.Instance.PlayerDead(player.GetComponent<Character>());
        }
    }

    private void OnWin()
    {
        foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
        {
            var health = player.GetComponent<Health>();
            health?.DamageDisabled();
        }

        foreach (var engine in Engines)
        {
            engine.DamageDisabled();
        }

        foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            var health = enemy.GetComponent<Health>();
            health.Damage(health.MaximumHealth, gameObject, 0, 0, Vector3.up);
        }

        LevelManager.Instance.GotoLevel(GameWinScene);
    }
}
