using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    private float _length;
    private float _startPosition;

    public float Speed;

    // Start is called before the first frame update
    void Start()
    {
        _length = GetComponent<SpriteRenderer>().bounds.size.x;
        _startPosition = transform.position.x;        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float distance = Time.fixedDeltaTime * Speed;
        transform.position = new Vector3(transform.position.x + distance, transform.position.y, transform.position.z);

        if (Mathf.Abs(transform.position.x - _startPosition) >= _length)
        {
            transform.position = new Vector3(_startPosition, transform.position.y, transform.position.z);
        }
    }
}
