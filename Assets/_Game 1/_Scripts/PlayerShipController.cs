using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerShipController : NetworkBehaviour
{
    public NetworkVariable<int> health = new NetworkVariable<int>();

    [SerializeField]
    int m_maxSpecialPower;

    [SerializeField]
    GameObject m_explosionVfxPrefab;

    [SerializeField]
    float m_hitEffectDuration;

    [Header("AudioClips")]
    [SerializeField]
    AudioClip m_hitClip;

    [SerializeField]
    AudioClip m_shieldClip;

    [Header("ShipSprites")]
    [SerializeField]
    SpriteRenderer m_shipRenderer;


    NetworkVariable<int> m_specials = new NetworkVariable<int>(0);

    bool m_isPlayerDefeated;

    const string k_hitEffect = "_Hit";
}
