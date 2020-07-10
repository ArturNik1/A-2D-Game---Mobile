using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonController : EnemyController
{
    public override void Move()
    {
        base.Move();
        //print(transform.rotation + " : " + transform.rotation.eulerAngles);
        //transform.RotateAround(transform.position, Vector3.back, 1);
        //transform.Rotate(0, 0, Time.fixedDeltaTime * 100, Space.World);
        //transform.Rotate(Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(new Vector3(0.2f, -0.6f, 0f) * Time.fixedDeltaTime, Vector3.back), 0.1f).eulerAngles, Space.World);
    }
}
