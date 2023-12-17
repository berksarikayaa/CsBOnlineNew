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
    
    private void Awake()
    {
        //nickname var ise onu getirme iþlemlerinin yapýldýðý bölüm
        if (PlayerPrefs.HasKey("nickname"))
        {
            PhotonNetwork.LocalPlayer.NickName = PlayerPrefs.GetString("nickname");
            PhotonNetwork.NickName = PlayerPrefs.GetString("nickname");
            nickname_input.text = PlayerPrefs.GetString("nickname");
        }

        PhotonNetwork.ConnectUsingSettings(); // varsayýlan ayarlar ile sunucuya baðlanmamýzý saðlar
        //print("Sunucuya baðlanma iþlemi baþladý...");
    }

    public override void OnConnectedToMaster() // Ana servera baðlantý kurulduðunda çalýþýr.
    {
        PhotonNetwork.JoinLobby(); // benim için lobiye baðlantý iþlemlerini baþlatýr.
        //print("lobiye baðlantý iþlemi baþladý...");
    }
    public override void OnJoinedLobby() // Lobby baðlantýsý kurulduysa çalýþacak olan kodlar
    {
        lobiPanel.SetActive(true);
        //print("lobiye baðlanýldý..!!!!");
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
                    //print(roomName_input.text + " odasý oluþturuldu MaxUser: " + roomMaxUser_input.text);
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
    public override void OnJoinedRoom() // bir odaya katýldýðýmýzda çalýþacak olan kodlar.
    {
        //print("odaya katýlma iþlemi BAÞARILI");
        lobiPanel.SetActive(false);
        charSelectPanel.SetActive(true);
    }

    [Header("CharSelect Ayarlar")]
    [SerializeField] Transform lolaSpawnPoint;
    [SerializeField] Transform swatSpawnPoint;
    public void charSelect(string charType)
    {
        if (charType == "Lola")
        {
            GameObject player = PhotonNetwork.Instantiate("LolaPrefab",lolaSpawnPoint.position,Quaternion.identity);

            player.name = PhotonNetwork.NickName;
            charSelectPanel.SetActive(false);
            print("Lola Karakter Oluþturuldu.");
        }

        if (charType == "Swat")
        {
            GameObject player = PhotonNetwork.Instantiate("SwatPrefab", swatSpawnPoint.position, Quaternion.identity);

            player.name = PhotonNetwork.NickName;
            charSelectPanel.SetActive(false);
            print("Swat Karakter Oluþturuldu.");
        }
    }

    public GameObject odaPanelPrefab;
    public Transform roomListPanel;// odalarýn odaListPanel içerisinde child obje olarak oluþmasý için
    // string türde bir key, gameobject türünde bir Value deðeri tutacak. keyi yazdýðým yere value deðerini getirir.
    private Dictionary<string, GameObject> odalar = new Dictionary<string, GameObject>();
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo oda in roomList)
        {
            // oda daha önce oluþturulmamýþ ise burasý çalýþacak
            if (!odalar.ContainsKey(oda.Name))
            {
                print("Oda Oluþturuldu: " + oda.Name + " Odadaki Kiþi Sayýsý: " + oda.PlayerCount);
                GameObject odaPrefab = Instantiate(odaPanelPrefab, roomListPanel);
                odaPrefab.name = oda.Name;

                odaPrefab.GetComponentInChildren<TextMeshProUGUI>().text = $"RoomName: {oda.Name}    User: {oda.PlayerCount}/{oda.MaxPlayers}";

                odalar.Add(oda.Name, odaPrefab);
            }
            // oda daha önce oluþturulmuþ ise burasý çalýþacak
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
