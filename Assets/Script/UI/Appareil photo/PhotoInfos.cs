using UnityEngine;
[System.Serializable]
public struct PhotoInfos
{
    [SerializeField] public string imagePath;
    [SerializeField] public string imageTag;
    [SerializeField] public EncyclopedieEntry entry;
    [SerializeField] public bool imageUsedInEncyclopedia;
}
