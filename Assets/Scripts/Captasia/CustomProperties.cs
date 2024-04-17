using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomProperties
{
    // World size property
    public const string WORLD_SIZE = "WORLD_SIZE";

    // World seed property
    public const string WORLD_SEED = "WORLD_SEED";

    // Player key for which team they are on
    public const string TEAM_KEY = "TEAM_KEY";

    // Room property for whether or not explorers have won
    public const string EXPLORER_WIN_KEY = "EXPLORER_WIN_KEY";

    // Room Property for whether or not captivator has won
    public const string CAPTIVATOR_WIN_KEY = "CAPTIVATOR_WIN_KEY";

    // Player Photon View ID
    public const string CHARACTER_VIEW_KEY = "CHARACTER_VIEW_KEY";

    // Player has been rescued
    public const string RESCUED_KEY = "RESCUED_KEY";

    // Player state for whether or not they are completely capsuled
    public const string EXPLORER_COMPLETELY_CAPSULED_KEY = "EXPLORER_COMPLETELY_CAPSULED_KEY";

    // Player state for whether or not they are on a podium
    public const string EXPLORER_PODIUM_KEY = "EXPLORER_PODIUM_KEY"; 

    // Captivator Captive Key
    public const string CAPTIVE_KEY = "CAPTIVE_KEY";

    // Player performing on ritual
    public const string PERFORMING_RITUAL_NUM_KEY = "PERFORMING_RITUAL_NUM_KEY";

    // Player has loaded level
    public const string PLAYER_HAS_LOADED_KEY = "PLAYER_HAS_LOADED_KEY";

    // Selected Character
    public const string SELECTED_CHARACTER_KEY = "SELECTED_CHARACTER_KEY";

    // Keeps track if the Game Started
    public const string GAME_STARTED_KEY = "GAME_STARTED_KEY";

    // Max ritual counter
    public const string MAX_RITUAL_COUNTER_KEY = "MAX_RITUAL_COUNTER_KEY";

    // Rituals required per player
    public const string RITUALS_REQUIRED_PER_PLAYER = "RITUALS_REQUIRED_PER_PLAYER";

    // Ritual State Counter
    public const string RITUAL_STATE_COUNTER_KEY = "RITUAL_STATE_COUNTER_KEY";

    // Counter for captured players
    public const string CAPTURE_COUNTER_KEY = "CAPTURE_COUNTER_KEY";

    // Map Selection Key
    public const string MAP_SELECTION_KEY = "MAP_SELECTION_KEY";

    public class Character
    {
        public const string CAPTIVATOR = "Captivator";
        public const string MATT = "Matt";
        public const string COOLGUY = "CoolGuy";
        public const string PAM = "Pam";
        public const string JEN = "Jen";
    }

    public class UI
    {
        public class AllPlayers
        {
            public const string RITUAL_COUNTER_KEY = "RITUAL_COUNTER_KEY";
        }

        public class Captivator
        {
            public const string PLAYER_CATCH_COUNTER_KEY = "PLAYER_CATCH_COUNTER_KEY";
        }

        public class Explorer
        {

        }
    }
}