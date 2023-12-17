using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraControl : MonoBehaviour
{
    PhotonView pV;

    private void Start()
    {
        pV = GetComponent<PhotonView>();
        if(pV.IsMine)
        {
            this.gameObject.SetActive(true);
        }
        else
        {
            this.gameObject.SetActive(false);
        }
    }
}
