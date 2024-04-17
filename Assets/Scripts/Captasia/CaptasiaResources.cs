using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class with references to captasia resources for ease of access.
/// </summary>
public class CaptasiaResources
{
    /// <summary>
    /// This class will contain references to an instance of an object or prefabs.
    /// </summary>
    public class Instance
    {
        public static readonly GameObject INDICATOR = Resources.Load<GameObject>("Prefabs/Captivator/Indicator");

        public class UI
        {
            public static readonly GameObject EXPLORER_UI = Resources.Load<GameObject>("Prefabs/UI/InGame/ExplorerUI");
            public static readonly GameObject CAPTIVATOR_UI = Resources.Load<GameObject>("Prefabs/UI/InGame/CaptivatorUI");
            public static readonly GameObject ITEM_SLOT = Resources.Load<GameObject>("Prefabs/Inventory/ItemSlot");
        }

        public class Ritual
        {
            // The Ritual Object used to spawn a panel
            public static readonly GameObject RITUAL = Resources.Load<GameObject>("Prefabs/RitualPanels/Ritual");

            public static readonly GameObject CLICK_FAST_RITUAL = Resources.Load<GameObject>("Prefabs/RitualPanels/ClickFastPanel");
            public static readonly GameObject KEYPAD_RITUAL = Resources.Load<GameObject>("Prefabs/RitualPanels/KeyPadRitual");
            public static readonly GameObject SIMON_RITUAL = Resources.Load<GameObject>("Prefabs/RitualPanels/SimonRitual");
            public static readonly GameObject SHARK_RITUAL = Resources.Load<GameObject>("Prefabs/RitualPanels/SharkRitual");
            public static readonly GameObject PICTURE_SLIDER_RITUAL = Resources.Load<GameObject>("Prefabs/RitualPanels/PictureSliderRitual");
            public static readonly GameObject TYPE_SENTENCE_RITUAL = Resources.Load<GameObject>("Prefabs/RitualPanels/TypeSentenceRitual");
            public static readonly GameObject CARD_MATCH_RITUAL = Resources.Load<GameObject>("Prefabs/RitualPanels/CardMatchPanel");
            public static readonly GameObject SENSE_RITUAL = Resources.Load<GameObject>("Prefabs/RitualPanels/SensePanel");
            public static readonly GameObject CARD_SWIPE_RITUAL = Resources.Load<GameObject>("Prefabs/RitualPanels/CardSwipe");
        }

        public class Items
        {
            // Effects
            public static readonly GameObject BANANA_PEEL = Resources.Load<GameObject>("Prefabs/Items/Effect/BananaPeel");
            public static readonly GameObject MAP_POINTER = Resources.Load<GameObject>("Prefabs/Items/Effect/MapPointer");
        }
    }

    public class KeyObjectsPath
    {
        public const string Podium = "Prefabs/Captivator/Podium";
        public const string Ritual = "Prefabs/RitualPanels/Ritual";
        public const string Chest = "Prefabs/Chest/Chest";
        public const string SpawnPoint = "Prefabs/SpawnPoint";

        // Trees
        public const string TreePink = "Prefabs/World/Walls/tree-pink";
    }

    public class ItemPrefabPath
    {
        // Common Items
        public const string BANANA_ITEM = "Prefabs/Items/BananaItem";
        public const string LANTERN_ITEM = "Prefabs/Items/LanternItem";
        public const string CANDY_BAR = "Prefabs/Items/CandyBar";

        // Epic Items
        public const string FLASH_LIGHT_ITEM = "Prefabs/Items/FlashLightItem";
        
        // Unique Items
        public const string MAP_ITEM = "Prefabs/Items/MapItem";

        // Legendary Items
        public const string KEY_ITEM = "Prefabs/Items/KeyItem";

        // Item Effects
        public const string BANANA_PEEL_EFFECT = "Prefabs/Items/Effect/BananaPeel";
    }

    public class Sprites
    {
        // Items
        public static readonly Sprite KEY_SPRITE = Resources.Load<Sprite>("Textures/Items/Key");
        public static readonly Sprite BANANA_SPRITE = Resources.Load<Sprite>("Textures/Items/Banana");
        public static readonly Sprite FLASH_LIGHT_SPRITE = Resources.Load<Sprite>("Textures/Items/FlashLight");
        public static readonly Sprite CANDY_BAR_SPRITE = Resources.Load<Sprite>("Textures/Items/CandyBar");
        public static readonly Sprite LANTERN_SPRITE = Resources.Load<Sprite>("Textures/Items/Lantern");
        public static readonly Sprite MAP_SPRITE = Resources.Load<Sprite>("Textures/Items/Map");

        // Effects
        public static readonly Sprite BANANA_PEEL_SPRITE = Resources.Load<Sprite>("Textures/Items/Effects/BananaPeel");

        // Character Icons
        public static readonly Sprite MATT_ICON = Resources.Load<Sprite>("Textures/Explorer/Matt");
        public static readonly Sprite COOLGUY_ICON = Resources.Load<Sprite>("Textures/Explorer/CoolGuy");
        public static readonly Sprite PAM_ICON = Resources.Load<Sprite>("Textures/Explorer/Pam");
        public static readonly Sprite JEN_ICON = Resources.Load<Sprite>("Textures/Explorer/Jen");
    }
}
