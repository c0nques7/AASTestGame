using UnityEngine;
using System.Collections;

public class GunExample : MonoBehaviour {

    [SerializeField]
    private int SpreadAmount;
    public int spreadAmount {
        get {
            return SpreadAmount;
        }
        set {
            SpreadAmount = value;
        }
    }

    [SerializeField]
    private int MaxBulletCount;
    public int maxBulletCount {
        get {
            return MaxBulletCount;
        }
        set {
            MaxBulletCount = value;
        }
    }

    private int CurrentBulletCount;
    public int currentBulletCount {
        get {
            return CurrentBulletCount;
        }
        set {
            CurrentBulletCount = value;
            if (CurrentBulletCount == 0) {
                StartCoroutine (Reload ());
            }
        }
    }

    [SerializeField]
    private float ShootDelay;
    public float shootDelay {
        get {
            return ShootDelay;
        }
        set {
            ShootDelay = value;
        }
    }

    private float ReloadTime;
    public float reloadTime {
        get {
            return ReloadTime;
        }
        set {
            ReloadTime = value;
        }
    }

    public bool automatic;

    public bool shooting { get; set; }

    public Transform muzzle;

    public Transform hitMarker;

    public AudioClip reloadSound;
    public AudioClip shootSound;
    public AudioClip outOfBullets;

    public GameObject impactParticle;

    private Quaternion originalRotation { get; set; }

    private bool reloading { get; set; }

    void Start() {
        originalRotation = transform.localRotation;
        currentBulletCount = maxBulletCount;
    }


    // Update is called once per frame
    void Update () {
        if (!shooting) {
            if (Input.GetMouseButton (0) && automatic) {
                StartCoroutine (Shoot ());
            }
            else if (Input.GetMouseButtonDown (0)) {
                StartCoroutine (Shoot ());
            }
        }
    }

    IEnumerator Shoot() {
        shooting = true;
        if (reloading) {
            Camera.main.GetComponent<AudioSource> ().PlayOneShot (outOfBullets);
            yield return new WaitForSeconds (shootDelay);
        }
        else {
            yield return new WaitForSeconds (shootDelay);

            muzzle.localEulerAngles = SprayDirection ();

            RaycastHit hit;
            if (Physics.Raycast (muzzle.position, muzzle.forward, out hit)) {
                Instantiate (hitMarker, hit.point + hit.normal * 0.01f, Quaternion.FromToRotation (Vector3.forward, hit.normal));
                Instantiate (impactParticle, hit.point + hit.normal * 0.01f, Quaternion.FromToRotation (Vector3.forward, hit.normal));
            }

            Camera.main.GetComponent<AudioSource> ().PlayOneShot (shootSound);

            CrosshairController.Instance.Spread (spreadAmount);

            currentBulletCount--;

        }
        shooting = false;

    }

    Vector3 SprayDirection() {
        Vector2 random = Random.insideUnitSphere;
        return new Vector3 (random.x, random.y) * CrosshairController.Instance.currentSpread * 0.15f;
    }

    IEnumerator Reload() {
        reloading = true;
        Camera.main.GetComponent<AudioSource> ().PlayOneShot (reloadSound);
        yield return new WaitForSeconds (reloadSound.length);
        currentBulletCount = maxBulletCount;
        reloading = false;
    }
}
