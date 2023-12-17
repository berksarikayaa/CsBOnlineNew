using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime; 

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject lobiPanel;
    [SerializeField] GameObject charSelectPanel;
    [SerializeField] TMP_InputField roomName_input;
    [SerializeField] TMP_InputField roomMaxUser_input;
    [SerializeField] TMP_InputField nickname_input;
    [SerializeField] TMP_Text infoTxt;
    [Header("CharSelect Ayarlar")]
    [SerializeField] Transform lolaSpawnPoint;
    [SerializeField] Transform swatSpawnPoint;
    public GameObject odaPanelPrefab;
    public Transform roomListPanel;
    private Dictionary<string, GameObject> odalar = new Dictionary<string, GameObject>();

    private void Awake()
    {
     
        if (PlayerPrefs.HasKey("nickname"))
        {
            PhotonNetwork.LocalPlayer.NickName = PlayerPrefs.GetString("nickname");
            PhotonNetwork.NickName = PlayerPrefs.GetString("nickname");
            nickname_input.text = PlayerPrefs.GetString("nickname");
        }

        PhotonNetwork.ConnectUsingSettings(); 
    }

    public override void OnConnectedToMaster() 
    {
        PhotonNetwork.JoinLobby(); 
    }
    public override void OnJoinedLobby() 
    {
        lobiPanel.SetActive(true);

    }
    public void createRoom()
    {
        if (nickname_input.text.Length >= 3)
        {
            PlayerPrefs.SetString("nickname", nickname_input.text);

            if (roomName_input.text.Length >= 3)
            {
                if (roomMaxUser_input.text != string.Empty && int.Parse(roomMaxUser_input.text) >= 2)
                {
                    infoTxt.text = "Oda baþarýyla kuruldu..!";

                    PhotonNetwork.CreateRoom(roomName_input.text, new RoomOptions { MaxPlayers = int.Parse(roomMaxUser_input.text), IsVisible = true }, TypedLobby.Default);
 
                }
                else
                {
                    infoTxt.text = "MaxUser BOÞ veya 2'den az olamaz..!";
                }
            }
            else
            {
                infoTxt.text = "RoomName 3 Karakterden kýsa olamaz..!";
            }
        }
        else
        {
            infoTxt.text = "Nickname 3 Karakterden kýsa olamaz..!";
        }
    }
    public override void OnJoinedRoom() 
    {
        lobiPanel.SetActive(false);
        charSelectPanel.SetActive(true);
    }


    public void charSelect(string charType)
    {
        if (charType == "Lola")
        {
            GameObject player = PhotonNetwork.Instantiate("PlayerA",lolaSpawnPoint.position,Quaternion.identity);

            player.name = PhotonNetwork.NickName;
            charSelectPanel.SetActive(false);
        }

        if (charType == "Swat")
        {
            GameObject player = PhotonNetwork.Instantiate("PlayerB", swatSpawnPoint.position, Quaternion.identity);

            player.name = PhotonNetwork.NickName;
            charSelectPanel.SetActive(false);
        }
    }


    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo oda in roomList)
        {
   
            if (!odalar.ContainsKey(oda.Name))
            {
                print("Oda Oluþturuldu: " + oda.Name + " Odadaki Kiþi Sayýsý: " + oda.PlayerCount);
                GameObject odaPrefab = Instantiate(odaPanelPrefab, roomListPanel);
                odaPrefab.name = oda.Name;

                odaPrefab.GetComponentInChildren<TextMeshProUGUI>().text = $"RoomName: {oda.Name}    User: {oda.PlayerCount}/{oda.MaxPlayers}";

                odalar.Add(oda.Name, odaPrefab);
            }

            else
            {
                print("oda daha önce varmýþ   " + oda.Name);
                GameObject odaPrefab = odalar[oda.Name];
                if (oda.PlayerCount > 0)
                {
                    odaPrefab.GetComponentInChildren<TextMeshProUGUI>().text = $"RoomName: {oda.Name}    User: {oda.PlayerCount}/{oda.MaxPlayers}";
                    if (oda.PlayerCount >= oda.MaxPlayers)
                    {
                        odaPrefab.GetComponentInChildren<Button>().interactable = false;
                    }
                    else
                    {
                        odaPrefab.GetComponentInChildren<Button>().interactable = true;
                    }
                }
                else
                {
                    Destroy(odaPrefab);
                    odalar.Remove(oda.Name);
                }
            }
        }
    }
}
