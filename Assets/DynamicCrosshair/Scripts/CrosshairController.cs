using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class CrosshairController : MonoBehaviour {

    [SerializeField]
    private bool FollowMouse;
    public bool followMouse {
        get {
            return FollowMouse;
        }
        set {
            FollowMouse = value;
            if (value) {
                crosshairTransform.anchorMax = Vector2.zero;
                crosshairTransform.anchorMin = Vector2.zero;
            }
            else {
                middleTexture.transform.position = crosshairTransform.anchoredPosition = Vector3.zero;
                crosshairTransform.anchorMax = Vector2.one * 0.5f;
                crosshairTransform.anchorMin = Vector2.one * 0.5f;
            }
        }
    }

    public bool rotateOverTime;
    public bool rotateRight;
    public bool showCursor;

    [SerializeField]
    private bool UseMiddleTexture;
    public bool useMiddleTexture {
        get {
            return UseMiddleTexture;
        }
        set {
            UseMiddleTexture = value;
            middleTexture.gameObject.SetActive(value);
        }
    }

    [SerializeField]
    private Color CrosshairColor;
    public Color crosshairColor {
        get {
            return CrosshairColor;
        }
        set {
            CrosshairColor = value;
            foreach (RectTransform trans in segments) {
                trans.GetComponent<Image> ().color = CrosshairColor;
            }
        }
    }

    public GameObject segmentPrefab;

    [SerializeField]
    private float CrosshairRotationOffset;
    public float crosshairRotationOffset {
        get {
            return CrosshairRotationOffset;
        }
        set {
            CrosshairRotationOffset = value;
            foreach (RectTransform segment in segments) {
                segment.RotateAround (CrosshairTransform.position, segment.forward, ((360 / segmentCount) + CrosshairRotationOffset));
            }
        }
    }

    private RectTransform CrosshairTransform;
    public RectTransform crosshairTransform {
        get {
            if (CrosshairTransform == null) {
                CrosshairTransform = transform.FindChild ("Crosshair") as RectTransform;
            }
            return CrosshairTransform;
        }
        set {
            CrosshairTransform = value;
        }
    }

    private GameObject MiddleTexture;
    public GameObject middleTexture {
        get {
            if(MiddleTexture == null) {
                MiddleTexture = transform.FindChild("MiddleTexture").gameObject;
            }
            return MiddleTexture;
        }
        set {
            MiddleTexture = value;
        }
    }

    [SerializeField]
    private float RotationSpeed;
    public float rotationSpeed {
        get {
            return RotationSpeed;
        }
        set {
            RotationSpeed = value;
        }
    }

    private List<RectTransform> Segments;
    public List<RectTransform> segments {
        get {
            return Segments;
        }
        set {
            if (Segments == null) {
                Segments = new List<RectTransform> ();
            }
            Segments = value;
        }
    }

    [SerializeField]
    private int SegmentCount;
    public int segmentCount {
        get {
            return SegmentCount;
        }
        set {
            while (crosshairTransform.childCount != 0) {
                DestroyImmediate (crosshairTransform.GetChild (0).gameObject);
            }

            SegmentCount = value;

            for (int i = 0; i < segmentCount; i++) {
                RectTransform trans = (Instantiate (segmentPrefab) as GameObject).GetComponent<RectTransform> ();
                trans.SetParent (crosshairTransform);
                trans.localPosition = Vector3.zero;
                trans.eulerAngles = i * ((Vector3.forward * 360) / SegmentCount);
                trans.localScale = Vector3.one;
                segments.Add (trans);
            }
            crosshairColor = CrosshairColor;
            crosshairRotationOffset = CrosshairRotationOffset;
        }
    }

    private float CurrentSpread;
    public float currentSpread {
        get {
            return CurrentSpread;
        }
        set {
            CurrentSpread = value;
            if (CurrentSpread < minSpread) {
                CurrentSpread = minSpread;
            }
            if (CurrentSpread > maxSpread) {
                currentSpread = maxSpread;
            }

            foreach (RectTransform segment in segments) {
                segment.localPosition = segment.up * currentSpread;
                segment.localPosition = new Vector3 ((float)System.Math.Round ((segment.up * currentSpread).x, 1), (float)System.Math.Round ((segment.up * currentSpread).y, 1));
            }
        }
    }

    [SerializeField]
    private float MinSpread;
    public float minSpread {
        get {
            return MinSpread;
        }
        set {
            minSpread = value;
        }
    }

    [SerializeField]
    private float MaxSpread;
    public float maxSpread {
        get {
            return MaxSpread;
        }
        set {
            MaxSpread = value;
        }
    }

    [SerializeField]
    private float SpreadSpeed;
    public float spreadSpeed {
        get {
            return SpreadSpeed;
        }
        set {
            SpreadSpeed = value;
        }
    }

    [SerializeField]
    private float CrosshairSize;
    public float crosshairSize {
        get {
            return CrosshairSize;
        }
        set {
            CrosshairSize = value;
            crosshairTransform.transform.localScale = Vector3.one * CrosshairSize;
        }
    }

    private static CrosshairController instance;
    public static CrosshairController Instance {
        get {
            if (instance == null) {
                instance = FindObjectOfType<CrosshairController> ();
                if (instance == null) {
                    GameObject obj = new GameObject ();
                    obj.hideFlags = HideFlags.HideAndDontSave;
                    instance = obj.AddComponent<CrosshairController> ();
                }
            }
            return instance;
        }
    }

    // Use this for initialization
    void Start () {
        Cursor.visible = showCursor;
        Rebuild ();
    }

    void LateUpdate () {
        /*
        If follow mouse is true
        Set the position of the mouse to the position of the mouse
         */
        if (followMouse) {
            middleTexture.transform.position = crosshairTransform.anchoredPosition = new Vector2 ((float)Math.Round (Input.mousePosition.x, 2), (float)Math.Round (Input.mousePosition.y, 2));
        }

        /*
        If rotate over time is true
        Rotate each of the line segments over time
         */
        if (rotateOverTime) {
            Rotate ();
        }

        /*
        If the current spread is greater than the min spread decrease the spread
         */
        if (currentSpread > minSpread) {
            currentSpread -= Time.deltaTime * spreadSpeed;
        }
    }

    /*
    Rebuild the crosshair
     */
    public void Rebuild() {
        segments = new List<RectTransform> ();
        segmentCount = SegmentCount;
        currentSpread = minSpread;
    }

    /*
    Increase the spread. Should be called from a gun script. See GunExample.cs for an example use.
     */
    public void Spread(float SpreadAmount) {
        currentSpread += Time.deltaTime * spreadSpeed * SpreadAmount;
    }

    /*
    Rotate each of the line segments
     */
    public void Rotate() {
        foreach (RectTransform segment in segments) {
            segment.RotateAround (crosshairTransform.position, rotateRight ? -segment.forward : segment.forward, rotationSpeed * Time.deltaTime);
        }
    }

    /*
    Called whenever a change happens to the script in the editor.
     */
    public void OnValidate() {
        crosshairSize = CrosshairSize;
        followMouse = FollowMouse;
        useMiddleTexture = UseMiddleTexture;
    }
}
