using UnityEngine;
using System;

public class Inventory : MonoBehaviour
{
    [SerializeField]
    private Item[] _items;
    private int _index;

    public Item Current => _items[_index];

    void Start()
    {
        _index = 0;
        Array.ForEach(_items, i => i.SetEnable(false));
        Current.SetEnable(true);
    }

    public void MoveNext()
    {
        Current.SetEnable(false);
        _index = (_index + 1) % _items.Length;
        Current.SetEnable(true);
    }

    public void MovePrev()
    {
        Current.SetEnable(false);
        _index = (_index == 0) ? _items.Length - 1 : _index - 1;
        Current.SetEnable(true);
    }
}
