using MoreMountains.InventoryEngine;
using UnityEngine;

public class InventoryCrutch : MonoBehaviour
{
    public InventoryItem[] Items;
    public MoreMountains.InventoryEngine.Inventory MainInventory;
    public MoreMountains.InventoryEngine.Inventory WeaponInventory;

    void Start()
    {
        foreach (var item in Items)
        {
            MainInventory.AddItem(item, 1);
        }

        if (Items.Length > 0)
        {
            MainInventory.EquipItem(Items[0], 0);
            MainInventory.MoveItemToInventory(0, WeaponInventory);
        }
    }
}
