using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G2_CameraFollow : MonoBehaviour
{
    public static G2_CameraFollow Instance;
    Vector3 offset = new Vector3(0,10,-20);
    private Transform tf;
    private Transform target;
    public Transform TF
    {
        get
        {
            if(tf == null)
            {
                tf = transform;
            }
            return tf;
        }
    }
    private void Awake()
    {
        Instance= this;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Follow(target);
    }
    public void SetTarget( Transform _target)
    {
        target = _target;
    }
    public void Follow(Transform target)
    {
        if (target == null) return;
        TF.position = Vector3.Lerp(TF.position, target.position + offset,0.5f);
    }
}
