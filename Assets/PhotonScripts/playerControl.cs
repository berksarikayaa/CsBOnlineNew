using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Chat;
using Photon.Realtime;
using Photon.Pun;
using TMPro;

public class playerControl : MonoBehaviourPunCallbacks
{
    Rigidbody rb;
    public PhotonView pV;
    public TMP_Text nicknameTxt;
    [Header("Movement Settings")]
    public bool groundControl = false;
    public float playerSpeed = 10f;
    public float jumpPower = 10f;
    public float rotationSpeed = 10f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pV = GetComponent<PhotonView>();
        nicknameTxt = GetComponentInChildren<TextMeshProUGUI>();
    }
    void Movement()
    {

    }
}