using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeTransfromStart : MonoBehaviour
{
    public Vector3 position;
    public Vector3 rotation;


    // Start is called before the first frame update
    void Start()
    {
        transform.localPosition = position;
        transform.localRotation = Quaternion.Euler(rotation);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
