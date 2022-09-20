using System.Collections;
using UnityEngine;

public class ChooChoo : MonoBehaviour
{
    public float TimeBetweenChooChoos;
    public float TimeBetweenChooAndChoo;
    public float ChooChooTime;
    public float Offset;
    
    void Start()
    {
        StartCoroutine(ChooChooCoroutine());
    }

    private IEnumerator ChooChooCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(TimeBetweenChooChoos);
            transform.position -= new Vector3(0, Offset, 0);
            yield return new WaitForSeconds(ChooChooTime);
            transform.position += new Vector3(0, Offset, 0);
            yield return new WaitForSeconds(TimeBetweenChooAndChoo);
            transform.position -= new Vector3(0, Offset, 0);
            yield return new WaitForSeconds(ChooChooTime);
            transform.position += new Vector3(0, Offset, 0);
        }       
    }
}
