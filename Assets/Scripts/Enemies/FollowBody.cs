using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowBody : MonoBehaviour
{
    Transform body;

    // Start is called before the first frame update
    void Start()
    {
        //body = transform.parent.Find("Body");
        body = transform.parent.Find("BodyHolder").GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = body.position;
    }
}
