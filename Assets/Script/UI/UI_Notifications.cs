using UnityEngine;
using UnityEngine.UI;

public class UI_Notifications : MonoBehaviour
{
    [Header("GameObjects")]
    [SerializeField] private GameObject UI_Carnet;
    [SerializeField] private GameObject UI_Album;
    [SerializeField] private GameObject UI_Message;
    
    [Header("Sprites")]
    [SerializeField] private Sprite CarnetNoWarn;
    [SerializeField] private Sprite CarnetWarn;
    [SerializeField] private Sprite AlbumNoWarn;
    [SerializeField] private Sprite AlbumWarn;
    [SerializeField] private Sprite MessageNoWarn;
    [SerializeField] private Sprite MessageWarn;

    public void ChangeCarnet(bool addNotif)
    {
        if (addNotif)
        { UI_Carnet.GetComponent<Image>().sprite = CarnetWarn; }
        else 
        { UI_Carnet.GetComponent<Image>().sprite = CarnetNoWarn; }
    }
    
    public void ChangeAlbum(bool addNotif)
    {
        if (addNotif)
        { UI_Album.GetComponent<Image>().sprite = AlbumWarn; }
        else 
        { UI_Album.GetComponent<Image>().sprite = AlbumNoWarn; }
    }
    
    public void ChangeMessage(bool addNotif)
    {
        if (addNotif)
        { UI_Message.GetComponent<Image>().sprite = MessageWarn; }
        else 
        { UI_Message.GetComponent<Image>().sprite = MessageNoWarn; }
    }
}
