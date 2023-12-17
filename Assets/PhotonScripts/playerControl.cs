using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Chat;
using Photon.Realtime;
using Photon.Pun;
using TMPro;

public class playerControl : MonoBehaviourPunCallbacks, IPunObservable
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

        if (pV.IsMine)
        {
            nicknameTxt.text = PhotonNetwork.NickName;
            //setNickname(PhotonNetwork.NickName);
            pV.RPC("setNickname",RpcTarget.AllBuffered,PhotonNetwork.NickName);
            
        }
    }

    void Movement()
    {

    }
    
    
    
    [PunRPC]
    void setNickname(string nickname)
    {
        if(nicknameTxt != null)
        {
            this.gameObject.name = nickname + "_Player";
            nicknameTxt.text = nickname;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(PhotonNetwork.NickName);
        }
        else
        {
            nicknameTxt.text = (string)stream.ReceiveNext();
        }
    }

    public override void OnPlayerEnteredRoom(Player enterPlayer)
    {
        if (pV.IsMine)
        {
            pV.RPC("setNickname", RpcTarget.AllBuffered, PhotonNetwork.NickName);
        }
    }

    public override void OnPlayerLeftRoom(Player leftPlayer)
    {
        pV.RPC("setNickname", RpcTarget.AllBuffered, PhotonNetwork.NickName);
    }

}
