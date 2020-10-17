using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandlePlayerActive : MonoBehaviour
{
#pragma warning disable CS0649
    [SerializeField] GameObject player;
#pragma warning restore CS0649

    public void EnablePlayer() {
        if (player == null) return;
        player.SetActive(true);
    }
    public void DisablePlayer() {
        if (player == null) return;
        player.SetActive(false);
    }
}
