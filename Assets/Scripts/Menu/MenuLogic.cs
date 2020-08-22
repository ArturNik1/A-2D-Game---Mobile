using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuLogic : MonoBehaviour
{
    public Animator anim;
    public float transitionTime = 1f;

    public Toggle joyFloating;
    public Toggle joyFixed;
    public Slider musicSlider;
    public Slider soundSlider;

    [Header("GameObjects_Scene0")]
    public GameObject settingsPanel;

    [Header("GameObjects_Scene1")]
    public GameObject mainCanvas;
    public Button infoReturnButton;
    public PlayerController player;
    public GameObject debug;

    public void LoadScene() {
        if (SceneManager.GetActiveScene().buildIndex == 0) StartCoroutine(LoadLevel(1));
        else StartCoroutine(LoadLevel(0));
    }

    IEnumerator LoadLevel(int levelIndex) {
        anim.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        if (AudioManager.instance.IsPlaying("TypeWriter01")) AudioManager.instance.StopPlaying("TypeWriter01");

        SceneManager.LoadScene(levelIndex);
    }

    void LoadPrefs() {
        if (SceneManager.GetActiveScene().buildIndex != 0) {
            PlayerController pController = GameObject.Find("Player").GetComponent<PlayerController>();

            // Joystick...
            if (PlayerPrefs.GetInt("joystick") == 0) {
                pController.floatingJoystick.gameObject.SetActive(true);
                pController.fixedJoystick.gameObject.SetActive(false);
                pController.joystick = pController.floatingJoystick;
            } else {
                pController.floatingJoystick.gameObject.SetActive(false);
                pController.fixedJoystick.gameObject.SetActive(true);
                pController.joystick = pController.fixedJoystick;
            }

            return;
        }

        // Joystick...
        GameObject.Find("Canvas").transform.Find("Settings Panel").gameObject.SetActive(true);
        if (PlayerPrefs.GetInt("joystick") == 1) {
            joyFixed.isOn = true;
        }
        GameObject.Find("Canvas").transform.Find("Settings Panel").gameObject.SetActive(false);

        // Sound...
        musicSlider.value = PlayerPrefs.GetFloat("music");
        soundSlider.value = PlayerPrefs.GetFloat("sound");
        AudioManager.instance.GetAudioSource("Theme").volume = musicSlider.value;
        AudioListener.volume = soundSlider.value;
    }

    public void SavePrefs() {
        // Joystick...
        if (joyFloating == null) return;
        if (joyFloating.isOn) PlayerPrefs.SetInt("joystick", 0);
        else PlayerPrefs.SetInt("joystick", 1);

        // Sound...
        PlayerPrefs.SetFloat("music", musicSlider.value);
        PlayerPrefs.SetFloat("sound", soundSlider.value);
    }

    public void TimeToDefault() {
        Time.timeScale = 1;
    }

    private void OnEnable() {

        if (SceneManager.GetActiveScene().buildIndex == 0) {
            musicSlider.onValueChanged.AddListener(delegate { AudioManager.instance.ChangeMusicVolume(musicSlider); });
            soundSlider.onValueChanged.AddListener(delegate { AudioManager.instance.ChangeSoundVolume(soundSlider); });
        }
        
        LoadPrefs();
    }

    private void OnDisable() {
        SavePrefs();
    }

    private void Update()
    {
        // Handle Back Button.
        if (Input.GetKeyDown(KeyCode.Escape)) { 
            if (SceneManager.GetActiveScene().buildIndex == 0) { 
                if (settingsPanel.activeSelf) {
                    SavePrefs();
                    settingsPanel.SetActive(false);
                }
            }
            else if (SceneManager.GetActiveScene().buildIndex == 1 && player.isAlive) {
                if (infoReturnButton.transform.root.gameObject.activeSelf) {
                    infoReturnButton.onClick.Invoke();
                } 
                else {
                    infoReturnButton.transform.root.gameObject.SetActive(true);
                    mainCanvas.SetActive(false);

                    Text leftText = infoReturnButton.transform.parent.transform.Find("Left Text").GetComponent<Text>();
                    SetGamePauseText(leftText);

                    Time.timeScale = 0;
                }
            }
        }

        if (Debug.isDebugBuild && SceneManager.GetActiveScene().buildIndex == 1) {
            debug.SetActive(true);
        }

    }

    void SetGamePauseText(Text leftText) {
        leftText.text = "HEALTH: " + player.gameObject.GetComponent<PlayerInfo>().health + "/" + player.gameObject.GetComponent<PlayerInfo>().maxHealth + "\n\n";

        leftText.text += "DAMAGE: " + (int)ProjectileController.damageAmount + "\n\n";

        if (ItemManager.instance.AmountInList(ItemInformation.ItemType.ExtraAttackSpeed) == -1)
            leftText.text += "ATTACK SPEED: %100\n\n";
        else
            leftText.text += "ATTACK SPEED: %" + (100 + 50 * ItemManager.instance.AmountInList(ItemInformation.ItemType.ExtraAttackSpeed)) + "\n\n";

        if (ItemManager.instance.AmountInList(ItemInformation.ItemType.ExtraMovementSpeed) == -1)
            leftText.text += "SPEED: %100\n\n";
        else
            leftText.text += "SPEED: %" + (100 + 7.5 * ItemManager.instance.AmountInList(ItemInformation.ItemType.ExtraMovementSpeed)) + "\n\n";
    }

    #region Scene_One

    public void ClickSound(Image button) {
        if (button.sprite.name.Contains("no")) {
            button.sprite = Resources.Load<Sprite>("Sprites/settings sound icon");
            AudioManager.instance.UnMuteSound();
        }
        else { 
            button.sprite = Resources.Load<Sprite>("Sprites/settings no sound icon");
            AudioManager.instance.MuteSound();
        }
    }
    public void ClickMusic(Image button) {
        if (button.sprite.name.Contains("no")) {
            button.sprite = Resources.Load<Sprite>("Sprites/settings music icon");
            AudioManager.instance.UnMuteMusic();
        }
        else {
            button.sprite = Resources.Load<Sprite>("Sprites/settings no music icon");
            AudioManager.instance.MuteMusic();
        }
    }

    public void ReturnToMenu() {
        Time.timeScale = 1;
        if (AudioManager.instance.IsPlaying("TypeWriter01")) AudioManager.instance.StopPlaying("TypeWriter01");
        StartCoroutine(LoadLevel(0));
    }

    #endregion

}
