using UnityEngine;

public class Item : MonoBehaviour
{
    private CooldownTimer _cooldownTimer;

    [SerializeField]
    private float _cooldown;

    void Start()
    {
        _cooldownTimer = new CooldownTimer();
        _cooldownTimer.coolDownAmount = _cooldown;
    }

    public bool TryUse()
    {
        if (!_cooldownTimer.CoolDownComplete)
        {
            return false;
        }

        Use();
        _cooldownTimer.StartColldown();
        return true;
    }

    protected void Use()
    {
        Debug.Log("Use");
    }
}
