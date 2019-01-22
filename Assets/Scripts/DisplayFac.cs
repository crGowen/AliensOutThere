using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DisplayFac : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ClickFunc(int nClicked)
    {
        if (nClicked<FactionUtils.allFactions.Count)
        {
            GameObject.Find("ViewCam").transform.localPosition = new Vector3(3.0f, 7.0f, 3.0f);
            GameObject.Find("ViewCam").transform.LookAt(StarUtils.allStars[FactionUtils.allFactions[nClicked].controlledSystems[0]].gObj.transform);
            Core.focusOn = StarUtils.allStars[FactionUtils.allFactions[nClicked].controlledSystems[0]].gObj.transform.localPosition;
        }
        else Debug.Log("Button Should be inactive!");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
