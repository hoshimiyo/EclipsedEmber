using UnityEngine;
using UnityEngine.SceneManagement;

using System.Collections;

public class GameManager : MonoBehaviour
{
    public string transitionFromScene;
    public static GameManager instance { get; set; }
    public Vector2 respawnPoint;
    [SerializeField] private FirePitManager _firePitManager;

    // Reference to the player
    private GameObject player;

    // Save data reference
    private SaveData gameData;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            InitializeSaveData();
            // Register for scene loading events
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Find player at start
        FindPlayer();


    }

    private void OnDestroy()
    {
        // Unregister from scene loading events
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnScreenLoadedTriggered");
        // Find player reference after scene loads
        FindPlayer();

        // If we loaded a scene from saved data, position the player
        if (gameData != null && gameData.currentSceneBuildIndex == scene.buildIndex)
        {
            StartCoroutine(PositionPlayerAfterSceneLoad());
            Debug.Log("OnSceneLoadedWhatIsGoingOn");
        }
        else
        {
            Debug.Log("OnSceneLoadedFix");
        }
    }

    private void FindPlayer()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void InitializeSaveData()
    {
        if (SaveSystem.SaveExists())
        {
            gameData = SaveSystem.LoadGame();
        }
        else
        {
            gameData = new SaveData();
        }
    }

    public void SetRespawnPoint(FirePitManager firePit)
    {
        if (_firePitManager == null)
        {
            _firePitManager = firePit;
        }
        else if (_firePitManager != null && _firePitManager != firePit)
        {
            _firePitManager.Disable();
            _firePitManager = firePit;
        }
        respawnPoint = firePit.transform.position;

        // Save the game when setting a checkpoint
        SaveGame();
    }

    // Public method to save game
    public void SaveGame()
    {
        // Make sure gameData is initialized
        if (gameData == null)
        {
            gameData = new SaveData();
            Debug.Log("Null GameData");
        }


        // Update scene data
        SaveCurrentScene();

        // Save player data
        SavePlayerData();

        // TODO: Save any other game state

        // Save to disk
        SaveSystem.SaveGame(gameData);
    }

    // Save current scene information
    private void SaveCurrentScene()
    {
        Debug.Log("SaveCurrentScene triggered");
        Scene currentScene = SceneManager.GetActiveScene();
        gameData.currentSceneName = currentScene.name;
        gameData.currentSceneBuildIndex = currentScene.buildIndex;

        // If you have a player reference
        if (player != null)
        {
            gameData.playerSpawnPosition = new Vector3Data(player.transform.position);
        }
    }

    // Save player-specific data
    private void SavePlayerData()
    {
        if (player != null)
        {
            // Save position
            gameData.playerPosition = new Vector3Data(player.transform.position);

            // Save player stats
            PlayerStat playerStat = player.GetComponent<PlayerStat>();
            if (playerStat != null)
            {
                gameData.playerHealth = playerStat.Health;
                gameData.playerMana = playerStat.Mana;
                gameData.healthCap = PlayerStat.healthCap;
            }

            // Save player movement data
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                gameData.isFacingRight = playerMovement._isFacingRight;
                gameData.respawnPoint = new Vector3Data(playerMovement._respawnPoint);
                gameData.availableJumps = playerMovement._availableJump;
                gameData.canDash = playerMovement._canDash;
            }

            // Save player spell casting data
            PlayerSpellCasting spellCasting = player.GetComponent<PlayerSpellCasting>();
            if (spellCasting != null)
            {
                gameData.isCasting = spellCasting.isCasting;
            }
        }
    }

    // Load the saved game
    public void LoadGame()
    {
        gameData = SaveSystem.LoadGame();

        // If loading from main menu or different scene
        if (SceneManager.GetActiveScene().buildIndex != gameData.currentSceneBuildIndex &&
            !string.IsNullOrEmpty(gameData.currentSceneName))
        {
            LoadSavedScene();
            LoadPlayerData();
        }
        else
        {
            // If already in correct scene, just load the player data
            LoadPlayerData();
        }
    }

    // Load a specific scene from save data
    public void LoadSavedScene()
    {
        // Try loading by build index first (more reliable)
        if (gameData.currentSceneBuildIndex >= 0)
        {
            SceneManager.LoadScene(gameData.currentSceneBuildIndex);
        }
        // Fall back to loading by name
        else if (!string.IsNullOrEmpty(gameData.currentSceneName))
        {
            SceneManager.LoadScene(gameData.currentSceneName);
        }
    }

    private void LoadPlayerData()
    {
        if (player == null)
            FindPlayer();

        if (player != null)
        {
            // Apply position if not already set in PositionPlayerAfterSceneLoad
            player.transform.position = gameData.playerPosition.ToVector3();

            // Apply player stats
            PlayerStat playerStat = player.GetComponent<PlayerStat>();
            if (playerStat != null)
            {
                playerStat.Health = (int)gameData.playerHealth;
                playerStat.Mana = gameData.playerMana;
                PlayerStat.healthCap = gameData.healthCap;
            }

            // Apply player movement data
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                // Handle facing direction - this may require flipping the sprite
                if (playerMovement._isFacingRight != gameData.isFacingRight)
                {
                    Vector3 localScale = player.transform.localScale;
                    localScale.x *= -1f;
                    player.transform.localScale = localScale;
                    playerMovement._isFacingRight = gameData.isFacingRight;
                }

                // Set respawn point
                playerMovement.SetRespawnPoint(gameData.respawnPoint.ToVector2());

                // Set jump and dash state
                playerMovement._availableJump = gameData.availableJumps;
                playerMovement._canDash = gameData.canDash;
            }
        }
        else Debug.Log("ApllyStatFix");
    }
    // Apply saved player position after scene loads
    private IEnumerator PositionPlayerAfterSceneLoad()
    {
        // Wait for the scene to fully load
        yield return new WaitForEndOfFrame();

        // Find player in the scene if not already set
        if (player == null)
            FindPlayer();

        if (player != null)
        {
            // Apply position
            player.transform.position = gameData.playerSpawnPosition.ToVector3();

            // Now apply all other player data
            LoadPlayerData();
        }
    }

    public void SaveAndQuit()
    {
        SaveGame();
        // Application.Quit(); // Uncomment when ready
    }
    private void OnApplicationQuit()
    {
        // Save before quitting
        if(SceneManager.GetActiveScene().name != "MainMenuScene")
            SaveGame();
    }

    public void ChangeScene(string sceneName)
    {
        // Save before changing scenes
        SaveGame();
        SceneManager.LoadScene(sceneName);
    }

    public void ChangeScene(int buildIndex)
    {
        // Save before changing scenes
        SaveGame();
        SceneManager.LoadScene(buildIndex);
    }
}