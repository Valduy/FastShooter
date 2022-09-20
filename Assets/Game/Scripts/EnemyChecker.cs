using MoreMountains.CorgiEngine;
using UnityEngine;

public class EnemyChecker : MonoBehaviour
{
    public Vector3 Offset = new Vector3(0.0f, 0.3f, 0.0f);
    public float Distance = 15;

    void Update()
    {
        var lefHit = Physics2D.Raycast(transform.position + Offset, Vector2.left, Distance, LayerMask.GetMask("Enemies"));
        GUIManager.Instance.Indicator.SetLeftVisibility(lefHit.collider != null);

        var rightHit = Physics2D.Raycast(transform.position + Offset, Vector2.right, Distance, LayerMask.GetMask("Enemies"));
        GUIManager.Instance.Indicator.SetRightVisibility(rightHit.collider != null);
    }
}
