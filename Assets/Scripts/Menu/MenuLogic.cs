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

    void SavePrefs() {
        // Joystick...
        if (joyFloating == null) return;
        if (joyFloating.isOn) PlayerPrefs.SetInt("joystick", 0);
        else PlayerPrefs.SetInt("joystick", 1);
    }

    private void OnEnable() {
        LoadPrefs();
    }

    private void OnDisable() {
        SavePrefs();
    }

}
