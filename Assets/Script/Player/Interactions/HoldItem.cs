using UnityEngine;

public class HoldItem : MonoBehaviour
{
    GameObject holdItem { get; }
    private bool holdingBaie;
    [SerializeField] private Transform BaieHolder;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Hold(GameObject Baie)
    {
        if (holdingBaie)
        {
            Baie.transform.parent.transform.parent = BaieHolder;
            Baie.transform.parent.transform.position = BaieHolder.transform.position;
            Baie.transform.parent.gameObject.GetComponent<Rigidbody>().useGravity = false;
            Baie.transform.parent.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            Baie.transform.parent.transform.GetChild(1).gameObject.SetActive(false);
            Debug.Log("tiens une baie");
        }
        else
        {
            Baie.transform.parent.transform.parent = null;
            Baie.transform.parent.gameObject.GetComponent<Rigidbody>().useGravity = true;
            Baie.transform.parent.gameObject.GetComponent<Rigidbody>().isKinematic = false;
            Baie.transform.parent.transform.GetChild(1).gameObject.SetActive(true);
            Baie.transform.parent.gameObject.GetComponent<Rigidbody>().AddForce(this.transform.GetChild(0).forward * 300, ForceMode.Force);
            Debug.Log("Lache une baie");
        }
        return;
    }
}
