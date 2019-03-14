using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private const float KEY_SPEED = 7f;
    private const float MOUSE_SPEED = 1.5f;
    private const float SCROLL_SPEED = 7f;
    private const float ROTATION_SPEED = 20f;

    // If the user is holding SHIFT, some actions should be accelerated, or if holding CTRL, decelerated
    private static float GetMultiplier()
    {
        if (Input.GetKey(KeyCode.LeftControl))
            return 0.2f;
        else if (Input.GetKey(KeyCode.LeftShift))
            return 4.0f;
        else
            return 1.0f;
    }

    // Scrolling forward/backward, moving axis is the same as pressing W or S
    public static void Scroll()
    {
        GameObject.Find("ViewCam").transform.Translate(Input.GetAxis("Mouse ScrollWheel") * SCROLL_SPEED * Vector3.forward);
    }

    // Click and drag to rotate view
    public static void Rotation()
    {
        GameObject.Find("ViewCam").transform.Rotate(new Vector3(Input.GetAxis("Mouse Y") * MOUSE_SPEED, -Input.GetAxis("Mouse X") * MOUSE_SPEED, 0));
    }

    // Press Q or E to roll the view
    public static void RollClockwise()
    {
        GameObject.Find("ViewCam").transform.Rotate(new Vector3(0f, 0f, 1f * ROTATION_SPEED * Time.deltaTime * GetMultiplier()));
    }

    // Press Q or E to roll the view
    public static void RollCounterClockwise()
    {
        GameObject.Find("ViewCam").transform.Rotate(new Vector3(0f, 0f, -1f * ROTATION_SPEED * Time.deltaTime * GetMultiplier()));
    }

    // Press W, move forward
    public static void MoveForward()
    {
        GameObject.Find("ViewCam").transform.Translate(Time.deltaTime * KEY_SPEED * GetMultiplier() * Vector3.forward);
    }

    // WASD, come on you know what this does...
    public static void MoveLeft()
    {
        GameObject.Find("ViewCam").transform.Translate(Time.deltaTime * KEY_SPEED * GetMultiplier() * Vector3.left);
    }

    // It's W. A. S. D.
    public static void MoveRight()
    {
        GameObject.Find("ViewCam").transform.Translate(Time.deltaTime * KEY_SPEED * GetMultiplier() * Vector3.right);
    }

    // WASD
    public static void MoveBack()
    {
        GameObject.Find("ViewCam").transform.Translate(Time.deltaTime * KEY_SPEED * GetMultiplier() * Vector3.back);
    }

    // Go "up" (relative to camera) by pressing X
    public static void MoveUp()
    {
        GameObject.Find("ViewCam").transform.Translate(Time.deltaTime * KEY_SPEED * GetMultiplier() * Vector3.up);
    }

    // Go "down" (relative to camera) by pressing Z
    public static void MoveDown()
    {
        GameObject.Find("ViewCam").transform.Translate(Time.deltaTime * KEY_SPEED * GetMultiplier() * Vector3.down);
    }
}
