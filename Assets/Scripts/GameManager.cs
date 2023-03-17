using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    //~ inspector (private)
    [Header("HUDs")]
    [SerializeField][Tooltip("The in-game menu HUD")] private GameObject pauseMenu;
    [SerializeField][Tooltip("The in-game settings HUD")] private GameObject menuSettings;
    [SerializeField][Tooltip("The in-game game over HUD")] private GameObject gameOver;
    //~ inspector (public)
    [Header("Player")]
    [SerializeField][Tooltip("The player in scene")] public PlayerManager Player;

    //~ static (public)
    public static GameManager Instance { get; private set; }

    //~ unity methods (private)
    private void Awake() {
        if(GameManager.Instance is null){
            //~ first start of game
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 1;
            //~ load audio settings
            this.LoadAudioSettings(
                true,
                out bool _,
                out float _,
                out float _,
                out float _
            );
        }
        GameManager.Instance = this;
    }

    //~ public methods
    /// <summary> Toggle <see cref="pauseMenu"/> (in-game HUD) and freeze/unfreeze time </summary>
    public void Menu() {
        if(this.menuSettings.activeSelf){
            GameManager.SavePreferences();
            this.menuSettings.SetActive(false);
        }else{
            GameManager.PauseTime(!this.pauseMenu.activeSelf);
            this.pauseMenu.SetActive(!this.pauseMenu.activeSelf);
        }
    }
    /// <summary> Activates <see cref="gameover"/> (in-game HUD) and freezes time </summary>
    public void GameOver() {
        GameManager.PauseTime(true);
        this.gameOver.SetActive(true);
    }

    //~ static methods (public)
    /// <summary> Reload the active scene </summary>
    public static void ReloadScene() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    /// <summary> Load the given scene </summary>
    /// <param name="sceneName"> The name of the scene to load </param>
    public static void LoadScene(string sceneName) => SceneManager.LoadScene(sceneName);
    /// <summary> Exits the game </summary>
    public static void ExitGame() => Application.Quit();
    /// <summary> Pause/Resume time (set timescale to 0/1) </summary>
    /// <param name="state"> If the game should be paused - resumed on false </param>
    public static void PauseTime(bool state) => Time.timeScale = (state ? 0f : 1f);
    /// <summary> Un-/locks the cursor and hides it </summary>
    /// <param name="state"> If the cursor should be locked - unlocked on false </param>
    public static void CursorLock(bool state) {
        Cursor.lockState = state
            ? CursorLockMode.Locked
            : CursorLockMode.None;
        Cursor.visible = !state;
    }
    /// <summary> Toggles the game between full screen window and windowed mode </summary>
    public static void ToggleFullScreen() {
        Screen.fullScreenMode = Screen.fullScreenMode != FullScreenMode.Windowed
            ? FullScreenMode.Windowed
            : FullScreenMode.FullScreenWindow;
    }
    /// <summary> Loads the audio settings from <see cref="PlayerPrefs"/> </summary>
    /// <param name="resetAudio"> If true overrides the current values in sound manager with the ones loaded </param>
    /// <param name="masterToggle"> The value for the saved "MasterToggle" bool </param>
    /// <param name="masterVolume"> The value for the saved "MasterVolume" float </param>
    /// <param name="musicVolume"> The value for the saved "MusicVolume" float </param>
    /// <param name="sfxVolume"> The value for the saved "SFXVolume" float </param>
    public void LoadAudioSettings(bool resetAudio, out bool masterToggle, out float masterVolume, out float musicVolume, out float sfxVolume){
        masterToggle = PlayerPrefs.GetInt("MasterToggle", 1) == 1;
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 0.5f);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.5f);
        if(resetAudio){
            SoundManager.Instance.ToggleMasterVolume(masterToggle);
            SoundManager.Instance.MasterVolume(musicVolume, sfxVolume, masterVolume);
            SoundManager.Instance.MusicVolume(musicVolume, masterVolume);
            SoundManager.Instance.SFXVolume(sfxVolume, masterVolume);
        }
    }
    /// <summary> Saved the values set in <see cref="PlayerPrefs"/> to disc </summary>
    public static void SavePreferences() => PlayerPrefs.Save();
}
