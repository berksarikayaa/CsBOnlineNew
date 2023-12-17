using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class odaPanelManager : MonoBehaviour
{
    public Button odaKatilBTN;
    public TMP_InputField nickname_input;
    public TMP_Text infoTxt;

    private void Start()
    {
        nickname_input = GameObject.Find("nickname_input").GetComponent<TMP_InputField>();
        
        infoTxt = GameObject.Find("infoTxt").GetComponent<TMP_Text>();
    }
    public void odaKatil()
    {   
        PlayerPrefs.SetString("nickname",nickname_input.text);
        if (PhotonNetwork.JoinRoom(this.gameObject.name))
        {
            print($"{this.gameObject.name} odasýna katýlma baþarýlý!!!");
        }
        else
        {
            print($"{this.gameObject.name} odasýna katýlma BAÞARISIZ!!!");
        }
    }
}
