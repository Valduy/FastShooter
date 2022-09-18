using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public float MinRadiusToPlayer;

    public bool IsEnable
    {
        get
        {
            foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (Vector3.Distance(transform.position, player.transform.position) < MinRadiusToPlayer)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
