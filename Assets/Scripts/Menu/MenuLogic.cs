using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuLogic : MonoBehaviour
{
    public Animator anim;
    public float transitionTime = 1f;

    public void LoadScene() {
        if (SceneManager.GetActiveScene().buildIndex == 0) StartCoroutine(LoadLevel(1));
        else StartCoroutine(LoadLevel(0));
    }

    IEnumerator LoadLevel(int levelIndex) {
        anim.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(levelIndex);
    }

}
