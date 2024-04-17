using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Inventory : MonoBehaviour
{
    public const int maxItemSlots = 3;

    // Item Descriptions
    public const string keyDescription = "Key: Allows you to instantly complete a ritual that you are on top of.";
    public const string bananaDescription = "Banana: Drops a banana peel on the ground that can stun the captivator for a few seconds.";
    public const string mapDescription = "Map: Spawns a pointer that will point you to the nearest incomplete ritual.";
    public const string flashLightDescription = "Flash Light: On use, will activate a light that shines towards your mouse cursor for 5 seconds." +
        " This light can be shined at the captivator to blind them. Shining the captivator long enough will stun them for a few seconds.";
    public const string candyBarDescription = "Candy Bar: Consuming this item will recover 1 health point (hp).";
    public const string lanternDescription = "Lantern: On use, the lantern will increase your light radius for 15 seconds.";

    #region Public Variables

    [Header("Reference to the item slots panel")]
    public GameObject slotsPanel;

    [Header("List of items")]
    public List<GameObject> items;

    [Header("UI Components")]
    public Button pickUpButton;

    [Header("Player Reference Object")]
    public Explorer explorerRef;

    #endregion

    #region Unity Update / Start

    void Update()
    {
        cullNullItems();
    }

    #endregion

    public void addItem(Item item)
    {
        GameObject newItemSlot = null;
        if (items.Count < maxItemSlots)
        {
            switch (item.itemType)
            {
                case Item.ItemType.Key:
                    newItemSlot = instantiateAndSetIcon(CaptasiaResources.Sprites.KEY_SPRITE);
                    newItemSlot.GetComponent<ItemSlot>().ToolTipText.text = keyDescription;
                    newItemSlot.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        if (!explorerRef.capsuled)
                        {
                            if (explorerRef.foundRitual != null)
                            {
                                //RPC call
                                explorerRef.foundRitual.GetComponent<Ritual>().raiseKeyEvent();
                                Destroy(newItemSlot);
                            }
                        }
                    });
                    break;
                case Item.ItemType.Banana:
                    newItemSlot = instantiateAndSetIcon(CaptasiaResources.Sprites.BANANA_SPRITE);
                    newItemSlot.GetComponent<ItemSlot>().ToolTipText.text = bananaDescription;
                    newItemSlot.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        if (!explorerRef.capsuled)
                        {
                            PhotonNetwork.Instantiate(CaptasiaResources.ItemPrefabPath.BANANA_PEEL_EFFECT,
                                explorerRef.transform.position,
                                Quaternion.identity);
                            Destroy(newItemSlot);
                        }
                    });
                    break;
                case Item.ItemType.Map:
                    newItemSlot = instantiateAndSetIcon(CaptasiaResources.Sprites.MAP_SPRITE);
                    newItemSlot.GetComponent<ItemSlot>().ToolTipText.text = mapDescription;
                    newItemSlot.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        if (!explorerRef.capsuled)
                        {
                            Instantiate(CaptasiaResources.Instance.Items.MAP_POINTER, explorerRef.transform);
                            Destroy(newItemSlot);
                        }
                    });
                    break;
                case Item.ItemType.FlashLight:
                    newItemSlot = instantiateAndSetIcon(CaptasiaResources.Sprites.FLASH_LIGHT_SPRITE);
                    newItemSlot.GetComponent<ItemSlot>().ToolTipText.text = flashLightDescription;
                    newItemSlot.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        if (!explorerRef.capsuled)
                        {
                            explorerRef.flashLight.duration = 5;
                            Destroy(newItemSlot);
                        }
                    });
                    break;
                case Item.ItemType.CandyBar:
                    newItemSlot = instantiateAndSetIcon(CaptasiaResources.Sprites.CANDY_BAR_SPRITE);
                    newItemSlot.GetComponent<ItemSlot>().ToolTipText.text = candyBarDescription;
                    newItemSlot.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        if (!explorerRef.capsuled)
                        {
                            if (explorerRef.hp < explorerRef.MAX_HP)
                            {
                                explorerRef.hp++;
                            }
                            Destroy(newItemSlot);
                        }
                    });
                    break;
                case Item.ItemType.Lantern:
                    newItemSlot = instantiateAndSetIcon(CaptasiaResources.Sprites.LANTERN_SPRITE);
                    newItemSlot.GetComponent<ItemSlot>().ToolTipText.text = lanternDescription;
                    newItemSlot.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        if (!explorerRef.capsuled)
                        {
                            explorerRef.lanternDuration += 15;
                            explorerRef.lightRadius = explorerRef.MAX_LIGHT_RADIUS + 2;
                            Destroy(newItemSlot);
                        }
                    });
                    break;
            }
        }
    }

    /// <summary>
    /// Instantiates the item slot and sets the icon.
    /// </summary>
    /// <param name="sprite"></param>
    private GameObject instantiateAndSetIcon(Sprite sprite)
    {
        GameObject newItemSlot = Instantiate(CaptasiaResources.Instance.UI.ITEM_SLOT, slotsPanel.transform);
        Image itemIcon = newItemSlot.transform.Find("ItemIcon").GetComponent<Image>();
        itemIcon.sprite = sprite;
        items.Add(newItemSlot);

        return newItemSlot;
    }

    /// <summary>
    /// Culls empty item slots
    /// </summary>
    private void cullNullItems()
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == null)
            {
                items.RemoveAt(i);
            }
        }
    }
}
