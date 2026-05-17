using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class AlbumUI : SerializedMonoBehaviour
{
    [Header("Scripts")]
    [SerializeField] private ChangeEntryPhoto changeEntryPhoto;
    [SerializeField] private SortAlbum sortAlbum;
    [SerializeField] private DialogueTelephoneManager dialogueTelephoneManager;

    [Header("Canvas / Panels")]
    [SerializeField] private UI_Notifications _uiNotifications;
    [SerializeField] private GameObject panelAlbum, panelPhoto;
    
    [Header("Buttons")]
    [SerializeField] private GameObject PanelPhoto_Photo;
    [SerializeField] private GameObject PanelPhoto_ButtonDelete;
    [SerializeField] private GameObject PanelPhoto_ButtonReplace;
    [SerializeField] private GameObject PanelPhoto_ButtonCarnet;
    [SerializeField] private Toggle SelectionMultipleToggle;
    [SerializeField] private GameObject SelectionMultipleOptions;

    private GameObject PhotoZoomedOn;
    [HideInInspector] public List<PhotoInfos> ListSelectedPhotos = new List<PhotoInfos>();
    private bool selectionMultipleOn = false;

    private void OnEnable()
    {
        _uiNotifications.ChangeAlbum(false);
    }

    private void OnDisable()
    {
        QuitPhoto();
    }

    public void QuitAlbum()
    {
        OpenUI openUI = FindFirstObjectByType<OpenUI>();
        if (openUI != null)
        {
            openUI.QuitUI(new PlayerInteractions());
        }
        QuitPhoto();
    }

    public void SelectionMultiple()
    {
        ListSelectedPhotos.Clear();
        selectionMultipleOn = !selectionMultipleOn;
        foreach (GameObject photo in sortAlbum.AlbumDisplayedPhotos)
        {
            GameObject frame = photo.transform.GetChild(1).gameObject;
            frame.GetComponent<PhotoFrame>().canBeSelected = selectionMultipleOn;
            frame.transform.GetChild(2).gameObject.SetActive(selectionMultipleOn);
            if(!selectionMultipleOn)
                frame.transform.GetChild(2).GetComponent<Toggle>().isOn = false;
        }
        SelectionMultipleOptions.SetActive(selectionMultipleOn);
        SelectionMultipleToggle.isOn  = selectionMultipleOn;
    }
    
    public void QuitPhoto()
    {
        if (selectionMultipleOn)
        {
            SelectionMultiple();
        }
        panelAlbum.SetActive(true);
        panelPhoto.SetActive(false);
    }
    
    public void ShowPhoto (Sprite photoSprite, PhotoInfos photoInfos)
    {
        // Change Panel
        panelPhoto.SetActive(true);
        // verifier si la photo est dans l'encyclop�die
        if (photoInfos.imageUsedInNotes)
        {
            // Ne pas activer le bouton supprimer
            PanelPhoto_ButtonDelete.SetActive(false);

            // Ne pas activer le bouton remplacer
            PanelPhoto_ButtonReplace.SetActive(false);
            
            // Activer le bouton Carnet
            PanelPhoto_ButtonCarnet.SetActive(true);
        }
        else
        {
            // Activer le bouton supprimer
            PanelPhoto_ButtonDelete.SetActive(true);

            // Activer le bouton remplacer
            PanelPhoto_ButtonReplace.SetActive(true);
            
            // ne pas activer le bouton Carnet
            PanelPhoto_ButtonCarnet.SetActive(false);
        }

        PanelPhoto_Photo.GetComponent<Image>().sprite = photoSprite;
        // Turn off album Panel
        panelAlbum.SetActive(false);
    }


    public void DeletePhoto()
    {
        foreach (PhotoInfos photo in ListSelectedPhotos)
        {
            SaveSystem.DeletePicture(photo);
        }
        ListSelectedPhotos.Clear();
        if(panelPhoto.activeSelf)
            QuitPhoto();
        sortAlbum.LoadUI();
    }

    public void FavUnfavPhoto()
    {
        foreach (PhotoInfos photo in ListSelectedPhotos)
        {
            SaveSystem.ModifyPicture(photo, true);
        }
        ListSelectedPhotos.Clear();
        if(panelAlbum.activeSelf)
            sortAlbum.LoadUI();
    }
    
    public void ChangeUsedInNotes()
    {
        return;
        
        foreach (PhotoInfos photo in ListSelectedPhotos)
        {
            SaveSystem.ModifyPicture(photo, false, true);
        }
        ListSelectedPhotos.Clear();
        sortAlbum.LoadUI();
    }

    public void SendPhoto(GameObject photo)
    {
        Message conv = new Message();
        conv.sender = "Noor";
        conv.message = null;
        conv.messageFont = null;
        conv.sprite = photo.GetComponent<Image>().sprite;
        conv.infos = ListSelectedPhotos[0];
        
        //Debug.Log($"Conv.sprite is null: {conv.sprite == null}");
        dialogueTelephoneManager.messagesToInstantiate.Add(conv);
        
        DialogueManager dialogueManager = FindFirstObjectByType<DialogueManager>();
        
        bool foundADialogue = false;
        Dialogue dialogueFound = null;
        foreach (Dialogue dialogue in dialogueManager.dialoguesWhenImageWithConditions)
        {
            if (dialogue.conditions.imageTag != null && conv.infos.imageTag != null)
            {
                if (dialogue.conditions.imageTag == conv.infos.imageTag)
                {
                    foundADialogue = true;
                    dialogueFound = dialogue;
                }
            }
        }
        if (!foundADialogue)
        {
            dialogueFound = dialogueManager.dialoguesWhenImageWithoutConditions[(int) Random.Range(0, dialogueManager.dialoguesWhenImageWithoutConditions.Count -1)];
        }
        else{dialogueManager.dialoguesWhenImageWithConditions.Remove(dialogueFound);}
        if(dialogueFound.conversation != null)
            dialogueManager.StartConv(dialogueFound);
        
        OpenUI openUI = FindFirstObjectByType<OpenUI>();
        openUI.OpenTelephone(new PlayerInteractions());
    }

    public void On_ChangeEncyclopediaPhotoClicked(GameObject photo)
    {
        /*
        foreach (GameObject picture in ListInEncyclopedia)
        {
            if (albumDictionary[picture].imageTag == albumDictionary[photo].imageTag)
            {
                ListInEncyclopedia.Remove(picture);
                // albumDictionary[picture].imageUsedInEncyclopedia = false;
                break;
            }
        }
        ListInEncyclopedia.Add(photo);
        ChangeEntryPhoto.AddPhotoToEntries(albumDictionary[photo]);
        // albumDictionary[photo].imageUsedInEncyclopedia = true;
        */
    }
}
