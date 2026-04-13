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
        holdingBaie = !holdingBaie;
        if (holdingBaie)
        {
            Baie.transform.parent = BaieHolder;
            Baie.transform.position = BaieHolder.transform.position;
            Baie.gameObject.GetComponent<Rigidbody>().useGravity = false;
            Baie.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            Baie.transform.GetChild(1).gameObject.SetActive(false);

            Baie.GetComponent<Berry>().IsHold = true;
            Baie.GetComponent<Berry>().CurrentLife = Baie.GetComponent<Berry>().LifeTime;
            Baie.GetComponentInParent<IInteractable>().Priority = 1000000;
            //Debug.Log("tiens une baie");
        }
        else
        {
            Baie.transform.parent = null;
            Baie.gameObject.GetComponent<Rigidbody>().useGravity = true;
            Baie.gameObject.GetComponent<Rigidbody>().isKinematic = false;
            Baie.transform.GetChild(1).gameObject.SetActive(true);
            Baie.gameObject.GetComponent<Rigidbody>().AddForce(this.transform.forward * 300, ForceMode.Force);
            
            Baie.GetComponent<Berry>().IsHold = false;
            Baie.GetComponentInParent<IInteractable>().Priority = 2;
            //Debug.Log("Lache une baie");
        }
        return;
    }
}
