using System.Collections;
using System.Collections.Generic;
using UnityEditor.iOS;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuLogic : MonoBehaviour
{
    public Animator anim;
    public float transitionTime = 1f;

    public Toggle joyFloating;
    public Toggle joyFixed;

    [Header("GameObjects_Scene0")]
    public GameObject settingsPanel;

    [Header("GameObjects_Scene1")]
    public GameObject mainCanvas;
    public Button infoReturnButton;

    public void LoadScene() {
        if (SceneManager.GetActiveScene().buildIndex == 0) StartCoroutine(LoadLevel(1));
        else StartCoroutine(LoadLevel(0));
    }

    IEnumerator LoadLevel(int levelIndex) {
        anim.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

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
    }

    public void SavePrefs() {
        // Joystick...
        if (joyFloating == null) return;
        if (joyFloating.isOn) PlayerPrefs.SetInt("joystick", 0);
        else PlayerPrefs.SetInt("joystick", 1);
    }

    public void TimeToDefault() {
        Time.timeScale = 1;
    }

    private void OnEnable() {
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
            else if (SceneManager.GetActiveScene().buildIndex == 1) {
                if (infoReturnButton.transform.root.gameObject.activeSelf) {
                    infoReturnButton.onClick.Invoke();
                } 
                else {
                    infoReturnButton.transform.root.gameObject.SetActive(true);
                    mainCanvas.SetActive(false);
                    Time.timeScale = 0;
                }
            }
        }
    }

    #region Scene_One

    public void ClickSound(Image button) {
        if (button.sprite.name.Contains("no")) {
            button.sprite = Resources.Load<Sprite>("Sprites/settings sound icon");
        }
        else { 
            button.sprite = Resources.Load<Sprite>("Sprites/settings no sound icon");
        }
    }
    public void ClickMusic(Image button) {
        if (button.sprite.name.Contains("no")) {
            button.sprite = Resources.Load<Sprite>("Sprites/settings music icon");
        }
        else {
            button.sprite = Resources.Load<Sprite>("Sprites/settings no music icon");
        } 
    }

    #endregion

}
