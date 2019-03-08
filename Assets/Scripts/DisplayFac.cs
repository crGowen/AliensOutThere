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
            if (Vector3.Distance(GameObject.Find("ViewCam").transform.position, StarUtils.allStars[FactionUtils.allFactions[nClicked].homeStar].gObj.transform.position) > 6.0f)
                GameObject.Find("ViewCam").transform.position = new Vector3(StarUtils.allStars[FactionUtils.allFactions[nClicked].homeStar].gObj.transform.position.x - 4f, StarUtils.allStars[FactionUtils.allFactions[nClicked].homeStar].gObj.transform.position.y + 3f, StarUtils.allStars[FactionUtils.allFactions[nClicked].homeStar].gObj.transform.position.z - 4f);
            GameObject.Find("ViewCam").transform.LookAt(StarUtils.allStars[FactionUtils.allFactions[nClicked].homeStar].gObj.transform);
            Core.focusOn = StarUtils.allStars[FactionUtils.allFactions[nClicked].homeStar].gObj.transform.localPosition;
            FactionUtils.DisplayFacInfo(nClicked);
        }
        else Debug.Log("Button Should be inactive!");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
