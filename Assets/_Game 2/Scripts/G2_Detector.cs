using SkeletonEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class G2_Detector : MonoBehaviour
{
    private G2_Bot bot;
    private void Awake()
    {
        bot = transform.parent.GetComponent<G2_Bot>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            bot.AddTarget(other.GetComponent<G2_PlayerController>());
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            bot.RemoveTarget(other.GetComponent<G2_PlayerController>());
        }
    }
}
