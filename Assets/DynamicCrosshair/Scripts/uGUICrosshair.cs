using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class uGUICrosshair : MonoBehaviour {

    [HideInInspector]
    public List<RectTransform> lines;        //List of all the lines

    public float rotationOffset;            //How much the crosshair should be rotated in the beginning
    public float spread;                    //See the documentation on how to implement this one!
    public float size;                        //Size of the crosshair
    public int textureCount;                //How many lines to be used
    public Sprite middleTexture;            //Sprite used for the middle dot
    public Sprite lineTexture;                //Sprite used for the lines
    public Color color;                        //Crolor of the crosshair

    public bool followMouse;                //Whether the crosshair should follow the position of the mouse or not
    public bool showCursor;                    //Show or hide the cursor
    public bool useMiddleTexture;            //Whether to use a middle texture or not
    public bool rotateOverTime;                //Rotates the crosshair over time
    public bool rotateRight;                //Direction of the rotation

    public float minSpread;                    //How little the crosshair can spread
    public float maxSpread;                    //How much the crosshair can spread
    public float spreadSpeed;                //How fast the crosshair spreads
    public float contractionSpeed;        //How fast the crosshair contracts

    public float rotationSpeed;                //How fast the crosshair rotates

    public RectTransform pivot;                //The pivot (middle texture)

    public GameObject linePrefab;            //Prefab used to create the lines

    public Transform container;                //The container that holds the lines

    public bool forwardObject;
    public bool focusOnTarget;
    public Transform muzzle;
    public float offset;

    public LayerMask mask;

    // Use this for initialization
    void Start () {

        //Create a new empty list of lines
        lines = new List<RectTransform> ();

        //Create the lines
        for (int i = 0; i < textureCount; i++) {
            GameObject temp = Instantiate (linePrefab) as GameObject;
            temp.transform.SetParent (container);
            temp.transform.localPosition = Vector3.zero;
            temp.GetComponent<Image> ().sprite = lineTexture;
            temp.transform.RotateAround (pivot.position, -pivot.forward, (360f / textureCount) * i);
            lines.Add (temp.GetComponent<RectTransform> ());
        }

        //Make sure the lines are at min distance
        for (int i = 0; i < lines.Count; i++) {
            lines [i].localPosition += lines [i].up * minSpread;
            lines [i].localPosition = new Vector3 ((float)System.Math.Round (lines [i].localPosition.x, 2), (float)System.Math.Round (lines [i].localPosition.y, 2));
        }

        //If the middle texture shouldn't be shown then hide it
        if (!useMiddleTexture) {
            HideShowMiddleTexture (false);
        }

        //If the cursor shouldn't be shown then hide it
        if (!showCursor) {
            Cursor.visible = false;
        }

        ChangeCrosshairColor (color);

        ChangeMiddleTextureSprite (middleTexture);

        OffsetCrosshairRotation (rotationOffset);

        ChangeCrosshairSize (size);
    }

    public void ChangeLineTextureSprite(Sprite sprite) {
        for (int i = 0; i < lines.Count; i++) {
            lines [i].GetComponent<Image> ().sprite = sprite;
        }
    }

    public void ChangeMiddleTextureSprite(Sprite sprite) {
        //Set the middle dot texture
        pivot.GetComponent<Image> ().sprite = sprite;
    }

    public void ChangeCrosshairColor(Color color) {
        //Set the color of all the sprites in the childrens
        foreach (Image img in transform.GetComponentsInChildren<Image> ()) {
            img.color = color;
        }
    }

    public void HideShowMiddleTexture(bool state) {
        pivot.GetComponent<Image> ().enabled = state;
    }

    public void OffsetCrosshairRotation(float offset) {
        //Run through all the lines and set the rotation offset
        for (int i = 0; i < lines.Count; i++) {
            lines [i].RotateAround (pivot.position, -pivot.forward, offset);
        }
    }

    //Set the scale factor of the canvas to the size specified
    //We do this control the overall size of the crosshair
    public void ChangeCrosshairSize(float size) {
        GetComponent<Canvas> ().scaleFactor = size;
    }

    // Update is called once per frame
    void Update () {

        //NOTE!
        //This should be replaced like stated in the documentation
        if (Input.GetMouseButton (0)) {
            spread = Mathf.Clamp (spread += Time.deltaTime * spreadSpeed, minSpread, maxSpread);
        }
        else if (spread > minSpread) {
            spread = Mathf.Clamp (spread -= Time.deltaTime * contractionSpeed, minSpread, maxSpread);
        }

        for (int i = 0; i < lines.Count; i++) {
            //Position the line
            lines [i].localPosition = lines [i].up * spread;
            //To make the lines align properly we round the position with 2 decimals
            lines [i].localPosition = new Vector3 ((float)System.Math.Round (lines [i].localPosition.x, 2), (float)System.Math.Round (lines [i].localPosition.y, 2));
        }

        //If follow mouse is set true
        //Make the pivot follow the location of the mouse
        if (followMouse) {
            pivot.position = Input.mousePosition;
        }
        else if (forwardObject) {
            if (focusOnTarget) {
                RaycastHit hit;
                if (Physics.Raycast (muzzle.position, muzzle.forward, out hit, offset, mask)) {
                    pivot.position = Camera.main.WorldToScreenPoint (hit.transform.position);
                }
                else {
                    pivot.position = Camera.main.WorldToScreenPoint (muzzle.forward * offset);
                }
            }
            else {
                pivot.position = Camera.main.WorldToScreenPoint (muzzle.forward * offset);
            }
        }

        //Rotate the lines over time.
        if (rotateOverTime) {
            //Run through all the lines.
            for (int i = 0; i < lines.Count; i++) {
                //Rotate the line in the specified direction
                //We use the pivot as the point to rotate around from
                if (rotateRight) {
                    lines [i].RotateAround (pivot.position, -pivot.forward, rotationSpeed);
                }
                else {
                    lines [i].RotateAround (pivot.position, pivot.forward, rotationSpeed);
                }
            }
        }
    }
}
