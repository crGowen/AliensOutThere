using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    private const float KEY_SPEED = 7f;
    private const float MOUSE_SPEED = 1.5f;
    private const float SCROLL_SPEED = 7f;
    private const float ROTATION_SPEED = 20f;

    // If the user is holding SHIFT, some actions should be accelerated, or if holding CTRL, decelerated
    private static float GetMultiplier()
    {
        if (Input.GetKey(KeyCode.LeftShift))
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

    public static void CheckInputs()
    {
        // Input functions here... in future projects let's put all this stuff in a dedicated file for neatness. Lessons learned.
        if (Input.GetKeyDown(KeyCode.Period))
        {
            Core.IncreaseWarp();
        }

        if (Input.GetKeyDown(KeyCode.Comma))
        {
            Core.DecreaseWarp();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            GameObject.Find("ViewCam").transform.localPosition = new Vector3(-10.0f, 16.0f, -42.0f);
            GameObject.Find("ViewCam").transform.LookAt(new Vector3(0.0f, 0.0f, 0.0f));
        }

        if (Input.GetKey(KeyCode.W))
            MoveForward();

        if (Input.GetKey(KeyCode.A))
            MoveLeft();

        if (Input.GetKey(KeyCode.S))
            MoveBack();

        if (Input.GetKey(KeyCode.D))
            MoveRight();

        if (Input.GetKey(KeyCode.Z))
            MoveDown();

        if (Input.GetKey(KeyCode.X))
            MoveUp();

        if (Input.GetKey(KeyCode.Q))
            RollClockwise();

        if (Input.GetKey(KeyCode.E))
            RollCounterClockwise();

        if (Input.GetMouseButton(0))
            Rotation();

        if (Input.GetKeyDown(KeyCode.M))
            Core.MuteAll();

        if (Input.GetKeyDown(KeyCode.F) && Core.facViewable != 99)
            DisplayFac.ViewFacSt(Core.facViewable);

        // function to check if a star was clicked on, it's in StarUtils because it's actually a fairly long function involving raycasts, and to avoid having to specify "StarUtils." for variables
        // however, just look most other things in this area, should be wrapped in a faction in a dedicated controls file for future projects.
        Core.CheckForClick();

        Scroll();
    }
}
