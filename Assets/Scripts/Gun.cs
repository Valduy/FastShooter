using UnityEngine;
using System;
using Unity.VisualScripting;

public class Gun : Item
{
    public GameObject bulletStarter;
    public GameObject bulletPrefab;
    
    protected override void Use()
    {
        var bullet = Instantiate(bulletPrefab, bulletStarter.transform.position, Quaternion.identity);
        Variables.Object(bullet).Set("Direction", new Vector2(MathF.Sign(transform.lossyScale.x), 0));
    }
}
