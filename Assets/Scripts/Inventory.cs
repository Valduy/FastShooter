using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField]
    private Item[] _items;
    private int _index;

    public Item Current => _items[_index];

    void Start()
    {
        _index = 0;
    }

    public void MoveNext()
    {
        _index = (_index + 1) % _items.Length;
    }

    public void MovePrev()
    {
        _index -= 1;

        if (_index < 0)
        {
            _index = _items.Length - 1;
        }
    }
}
