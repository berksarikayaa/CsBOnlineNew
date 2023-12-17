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
        //nickname var ise onu getirme i�lemlerinin yap�ld��� b�l�m
        if (PlayerPrefs.HasKey("nickname"))
        {
            PhotonNetwork.LocalPlayer.NickName = PlayerPrefs.GetString("nickname");
            PhotonNetwork.NickName = PlayerPrefs.GetString("nickname");
            nickname_input.text = PlayerPrefs.GetString("nickname");
        }

        PhotonNetwork.ConnectUsingSettings(); // varsay�lan ayarlar ile sunucuya ba�lanmam�z� sa�lar
        //print("Sunucuya ba�lanma i�lemi ba�lad�...");
    }

    public override void OnConnectedToMaster() // Ana servera ba�lant� kuruldu�unda �al���r.
    {
        PhotonNetwork.JoinLobby(); // benim i�in lobiye ba�lant� i�lemlerini ba�lat�r.
        //print("lobiye ba�lant� i�lemi ba�lad�...");
    }
    public override void OnJoinedLobby() // Lobby ba�lant�s� kurulduysa �al��acak olan kodlar
    {
        lobiPanel.SetActive(true);
        //print("lobiye ba�lan�ld�..!!!!");
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
                    infoTxt.text = "Oda ba�ar�yla kuruldu..!";

                    PhotonNetwork.CreateRoom(roomName_input.text, new RoomOptions { MaxPlayers = int.Parse(roomMaxUser_input.text), IsVisible = true }, TypedLobby.Default);
                    //print(roomName_input.text + " odas� olu�turuldu MaxUser: " + roomMaxUser_input.text);
                }
                else
                {
                    infoTxt.text = "MaxUser BO� veya 2'den az olamaz..!";
                }
            }
            else
            {
                infoTxt.text = "RoomName 3 Karakterden k�sa olamaz..!";
            }
        }
        else
        {
            infoTxt.text = "Nickname 3 Karakterden k�sa olamaz..!";
        }
    }
    public override void OnJoinedRoom() // bir odaya kat�ld���m�zda �al��acak olan kodlar.
    {
        //print("odaya kat�lma i�lemi BA�ARILI");
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
            print("Lola Karakter Olu�turuldu.");
        }

        if (charType == "Swat")
        {
            GameObject player = PhotonNetwork.Instantiate("SwatPrefab", swatSpawnPoint.position, Quaternion.identity);

            player.name = PhotonNetwork.NickName;
            charSelectPanel.SetActive(false);
            print("Swat Karakter Olu�turuldu.");
        }
    }

    public GameObject odaPanelPrefab;
    public Transform roomListPanel;// odalar�n odaListPanel i�erisinde child obje olarak olu�mas� i�in
    // string t�rde bir key, gameobject t�r�nde bir Value de�eri tutacak. keyi yazd���m yere value de�erini getirir.
    private Dictionary<string, GameObject> odalar = new Dictionary<string, GameObject>();
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo oda in roomList)
        {
            // oda daha �nce olu�turulmam�� ise buras� �al��acak
            if (!odalar.ContainsKey(oda.Name))
            {
                print("Oda Olu�turuldu: " + oda.Name + " Odadaki Ki�i Say�s�: " + oda.PlayerCount);
                GameObject odaPrefab = Instantiate(odaPanelPrefab, roomListPanel);
                odaPrefab.name = oda.Name;

                odaPrefab.GetComponentInChildren<TextMeshProUGUI>().text = $"RoomName: {oda.Name}    User: {oda.PlayerCount}/{oda.MaxPlayers}";

                odalar.Add(oda.Name, odaPrefab);
            }
            // oda daha �nce olu�turulmu� ise buras� �al��acak
            else
            {
                print("oda daha �nce varm��   " + oda.Name);
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
