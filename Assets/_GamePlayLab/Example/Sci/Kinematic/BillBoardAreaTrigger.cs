using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BillBoardAreaTrigger : MonoBehaviour
{
    public string showInfo;

    private Text billboardText;

    // Start is called before the first frame update
    void Start()
    {
        billboardText = GameObject.Find("/------UI------/AreaShowBoardUI/BillBoard/Text").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;

        billboardText.text = showInfo;
        billboardText.transform.parent.gameObject.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        billboardText.transform.parent.gameObject.SetActive(false);
    }
}
