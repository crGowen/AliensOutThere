using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private static float speed = 7f;
    private static float mspeed = 1.5f;
    private static float sspeed = 7f;
    private static float rspeed = 20f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private static float GetMultiplier()
    {
        if (Input.GetKey(KeyCode.LeftControl))
            return 0.2f;
        else if (Input.GetKey(KeyCode.LeftShift))
            return 4.0f;
        else
            return 1.0f;
    }

    public static void Scroll()
    {
        GameObject.Find("ViewCam").transform.Translate(Input.GetAxis("Mouse ScrollWheel") * sspeed * Vector3.forward);
    }

    public static void Rotation()
    {
        GameObject.Find("ViewCam").transform.Rotate(new Vector3(Input.GetAxis("Mouse Y") * mspeed, -Input.GetAxis("Mouse X") * mspeed, 0));
    }

    public static void RollClockwise()
    {
        GameObject.Find("ViewCam").transform.Rotate(new Vector3(0f, 0f, 1f * rspeed * Time.deltaTime * GetMultiplier()));
    }

    public static void RollCounterClockwise()
    {
        GameObject.Find("ViewCam").transform.Rotate(new Vector3(0f, 0f, -1f * rspeed * Time.deltaTime * GetMultiplier()));
    }

    public static void MoveForward()
    {
        GameObject.Find("ViewCam").transform.Translate(Time.deltaTime * speed * GetMultiplier() * Vector3.forward);
    }

    public static void MoveLeft()
    {
        GameObject.Find("ViewCam").transform.Translate(Time.deltaTime * speed * GetMultiplier() * Vector3.left);
    }

    public static void MoveRight()
    {
        GameObject.Find("ViewCam").transform.Translate(Time.deltaTime * speed * GetMultiplier() * Vector3.right);
    }

    public static void MoveBack()
    {
        GameObject.Find("ViewCam").transform.Translate(Time.deltaTime * speed * GetMultiplier() * Vector3.back);
    }

    public static void MoveUp()
    {
        GameObject.Find("ViewCam").transform.Translate(Time.deltaTime * speed * GetMultiplier() * Vector3.up);
    }

    public static void MoveDown()
    {
        GameObject.Find("ViewCam").transform.Translate(Time.deltaTime * speed * GetMultiplier() * Vector3.down);
    }
}
