using UnityEngine;
using System.Collections;

public class DemoCharacterController : MonoBehaviour {

    private Transform target;                        //The target to follow

    public float jumpSpeed = 20f;    //The speed at which the player jumps
    public float fallSpeed = 10f;    //The speed at which the player falls

    public float forwardWalkSpeed = 2f;                //The speed at which the player walks forward
    public float backwardWalkSpeed = 1.4f;            //The speed at which the player walks backward
    public float forwardRunSpeed = 4f;                //The speed at which the player runs forword
    public float backwardRunSpeed = 2.25f;            //The speed at which the player runs backward

    public float slideSpeed = 0.5f;                    //The speed at which the player slides down slopes
    public float slideAngle = 45f;                    //The angle at which the player will start to slide
    public float rotationSpeed = 3f;                //The speed at which the player rotates
    public float jumpDuration = 0.7f;                //The duration of the jump

    public float fallDamageThreshold = 10f;            //How far the player has to fall before losing health

    public LayerMask walkAble;                        //The layers the player can walk on

    public KeyCode forward = KeyCode.W;                //The key to move forward
    public KeyCode backward = KeyCode.S;            //The key to move backward
    public KeyCode left = KeyCode.A;                //The key to rotate left
    public KeyCode right = KeyCode.D;                //The key to rotate right
    public KeyCode leftStrafe = KeyCode.Q;            //The key to strafe left
    public KeyCode rightStrafe = KeyCode.E;            //The key to strafe right
    public KeyCode toggleRunWalk = KeyCode.R;        //The key to toggle run or walk
    public KeyCode jump = KeyCode.Space;            //The key to jump
    public KeyCode leftMouse = KeyCode.Mouse0;        //The left mouse button
    public KeyCode rightMouse = KeyCode.Mouse1;        //The right mouse button
    public KeyCode toggleAutoRun = KeyCode.Plus;    //The button to press for auto run

    [HideInInspector]
    public Vector3 fallStartPosition;                //The position for when the player starts to fall

    private bool autorun;                            //The bool to check wether the player is auto running
    [HideInInspector]
    public bool running = true;                        //The bool to check wether the player is running or not
    [HideInInspector]
    public bool grounded;                            //The bool to check wether the player is on the ground or not
    private bool sliding;                            //The bool to check wether the player is sliding or not
    [HideInInspector]
    public bool jumping;                            //The bool to check wheter the player is jumpign or not
    private float jumpTimer;                        //The timer to check for how long the jump have lasted

    private float movementSpeed;                    //The speed at which the player currently moves

    private CharacterController col;                //The reference to the CharacterController

    [HideInInspector]
    public Vector3 movementInput;                    //3D vector to store the input from the keyboard
    private RaycastHit hit;                            //The position of where the raycasts hit

    void Start() {
        target = transform;
        //Fetch and cache the character controller of the player
        col = GetComponent<CharacterController> ();
        //Raycast 100 units down to find ground - This makes sure that the player doesn't go through the terrain when running
        if (Physics.Raycast (target.position - col.center, -target.up, out hit, 100, walkAble)) {
            //Position the player just above the terrain
            target.position = new Vector3 (hit.point.x, hit.point.y + col.height * 0.5f, hit.point.z);
        }
    }

    void Update() {
        //Clear the inputs from the keyboard
        movementInput = Vector3.zero;

        //First cast a sphere downwards - If we didn't do so the player would fall through places the player actually would be able to move over
        if (Physics.SphereCast (target.position, col.radius, -target.up, out hit, col.height * 0.4f, walkAble)) {
            //Second cast down a ray to find the right position on the terrain
            if (Physics.Raycast (target.position - Vector3.up * col.height * 0.4f, -target.up, out hit, col.height * 0.2f, walkAble)) {
                //Position our player at the right point on the terrain
                target.position = new Vector3 (hit.point.x, hit.point.y + col.height * 0.5f, hit.point.z);
                //The player is on the ground
                grounded = true;
                //If the player was falling then run the Land function
                if (fallStartPosition != Vector3.zero)
                    Land ();
            }
        }
            //The sphere didn't hit anything
        else {
            //If we're not currently falling and not jumping
            if (fallStartPosition == Vector3.zero && !jumping)
                //Set the start position of the fall equal to the position of where the player started to fall
                fallStartPosition = target.position;
            //We're not on the ground anymore
            grounded = false;
        }

        //Find the input from the keyboard - and also play the animations
        movementInput = CalculateInput ();

        //If the input is forward
        if (movementInput.z > 0) {
            //Set the speed of which the player is moving either to runspeed if running or walkspeed if walking
            movementSpeed = running ? forwardRunSpeed : forwardWalkSpeed;
        }
            //Else if the player is moving backwards
        else {
            //Set the speed of which the player is moving either to backward runspeed if running or backward walkspeed if walking backward
            movementSpeed = running ? backwardRunSpeed : backwardWalkSpeed;
        }

        //If the player is trying to rotate then either set the speed to runspeed if running and walkspeed if walking
        if (movementInput.x != 0)
            movementSpeed = running ? forwardRunSpeed : forwardWalkSpeed;

        //If the player is on the ground
        if (grounded) {
            //Check whether the player should be sliding or not
            sliding = CalculateSlide ();
        }
            //If the player is not on the ground
        else
            //Then move the player downwards based on the fallspeed
            target.Translate (new Vector3 (0, -fallSpeed, 0) * Time.smoothDeltaTime);

        //If the player isn't sliding and is ground or is jumping
        if (!sliding && grounded || jumping) {
            //If the player isn't colliding with anything along the direction the player is trying to move
            if (!CheckCollision ())
                //Then move the player based on the input from the keyboard
                target.Translate (movementInput.normalized * Time.smoothDeltaTime * movementSpeed);

            //If the player presses the jump key and is on the ground
            if (Input.GetKeyDown (jump) && grounded) {
                //Start the jump function (We use StartCoroutine since that's the way to call IEnumerator functions)
                StartCoroutine ("Jump");
            }
        }
        target.rotation = Quaternion.Lerp (target.rotation, Quaternion.Euler (0, Camera.main.transform.eulerAngles.y, 0), Time.deltaTime * 20);
    }

    Vector3 CalculateInput() {

        //Create a temp vector3 to hold our inputs
        Vector3 tempInput = Vector3.zero;

        //If the player presses the button specified for the toggle run / toggle walk
        if (Input.GetKeyDown (toggleRunWalk)) {
            //Set the running boolean either to true or false
            running = !running;
        }

        if (Input.GetKeyDown (toggleAutoRun)) {
            autorun = !autorun;
        }

        //If we're autorunning and pressing a button then break the autorun
        if (autorun && Input.GetKeyDown (forward) || Input.GetKeyDown (backward) || (Input.GetKey (rightMouse) && Input.GetKey (leftMouse))) {
            autorun = false;
        }

        if (autorun)
            tempInput.z = 1f;

        //If the player presses the forward key
        if (Input.GetKey (forward)) {
            //Set the input + 1 for forward
            tempInput.z += 1f;
        }
            //If the player presses the backward key
        else if (Input.GetKey (backward)) {
            //Set the input -1 for backward
            tempInput.z -= 1f;
            //If we aren't jumping
        }
        //If the player presses the right mouse button
        if (Input.GetKey (rightMouse)) {
            //If the player then presses the rotate left key
            if (Input.GetKey (left)) {
                //Set the input x to -1 for moving left
                tempInput.x -= 1f;
            }
                //If the player then presses the rotate right key
            else if (Input.GetKey (right)) {
                //Set the input x to 1 for moving right
                tempInput.x += 1f;
            }
        }
        //If the player presses the left strafe key and we're not currently moving backwards
        if (Input.GetKey (leftStrafe) && !Input.GetKey (backward)) {
            //Set the input x to -1 for moving left
            tempInput.x -= 1f;
        }
        else if (Input.GetKey (rightStrafe) && !Input.GetKey (backward)) {
            //Set the input x to 1 for moving left
            tempInput.x += 1f;
        }

        //If we're not pressing the right mouse button (We check for this because we want to rotate the player to match the mouse when we right click)
        if (!Input.GetKey (rightMouse)) {
            //If the player tries to rotate left
            if (Input.GetKey (left)) {
                //Rotate to the left based on the rotationSpeed
                target.RotateAround (target.position, target.up, -rotationSpeed);
            }
                //If the player tries to rotate right
            else if (Input.GetKey (right)) {
                //Rotate to the right based on the rotationSpeed
                target.RotateAround (target.position, target.up, rotationSpeed);
            }
        }

        if (autorun) {
            return new Vector3 (tempInput.x, 0, 1f);
        }
        //Return the direction that we want to move our player
        return tempInput;
    }

    bool CalculateSlide() {
        //Find the angle of the normal right under us
        float angle = Vector3.Angle (hit.normal, Vector3.up);
        sliding = false;
        //If the angle under us is greater than the angle we've set to be the angle where we begin to slide
        if (angle > slideAngle) {
            //Find the direction of the the slope
            Vector3 groundSlopeDir = Vector3.Cross (Vector3.Cross (hit.normal, Vector3.down), hit.normal);
            //Move the player down the slope
            target.position += groundSlopeDir * fallSpeed * Time.deltaTime * slideSpeed;
        }
            //If the angle of the slope aren't greater than the slideAngle then do nothing
        else {
            sliding = false;
        }
        //Return wether we're sliding or not
        return sliding;
    }

    bool CheckCollision() {
        bool colliding = false;
        //The position of the lower part of the player.
        Vector3 p1 = target.position - Vector3.up * col.height * 0.25f;
        //The position of the upper part of the player
        Vector3 p2 = p1 + Vector3.up * col.height * 0.5f;
        //Cast character controller shape in front of the player to see if we collide with any obstacles
        //But only do it in the direction the player is moving
        if (movementInput.z > 0) {    //If we're moving forward
            if (Physics.CapsuleCast (p1, p2, col.radius, target.forward, out hit, col.radius * 0.25f, walkAble)) {
                //Check to see wether the angle of the colliding normal is greater than the slideAngle
                float angle = Vector3.Angle (hit.normal, Vector3.up);
                //If it's steeper than the slide then we are colliding
                if (angle > slideAngle)
                    colliding = true;
            }
            else
                colliding = false;
        }
        else if (movementInput.z < 0) {    //If we'ere moving backwards
            if (Physics.CapsuleCast (p1, p2, col.radius, -target.forward, out hit, col.radius * 0.25f, walkAble)) {
                //Check to see wether the angle of the colliding normal is greater than the slideAngle
                float angle = Vector3.Angle (hit.normal, Vector3.up);
                //If it's steeper than the slide then we are colliding
                if (angle > slideAngle)
                    colliding = true;
            }
            else
                colliding = false;
        }
        else if (movementInput.x > 0) {    //If we're moving right
            if (Physics.CapsuleCast (p1, p2, col.radius, target.right, out hit, col.radius * 0.25f, walkAble)) {
                //Check to see wether the angle of the colliding normal is greater than the slideAngle
                float angle = Vector3.Angle (hit.normal, Vector3.up);
                //If it's steeper than the slide then we are colliding
                if (angle > slideAngle)
                    colliding = true;
            }
            else
                colliding = false;
        }
        else if (movementInput.x < 0) {    //If we're moving left
            if (Physics.CapsuleCast (p1, p2, col.radius, -target.right, out hit, col.radius * 0.25f, walkAble)) {
                //Check to see wether the angle of the colliding normal is greater than the slideAngle
                float angle = Vector3.Angle (hit.normal, Vector3.up);
                //If it's steeper than the slide then we are colliding
                if (angle > slideAngle)
                    colliding = true;
            }
            else {
                colliding = false;
            }
        }
        //If we're colliding with anything it will return true and we can't move that direction
        return colliding;
    }

    void Land() {
        //If the distance the player has fallen is greater than the fallDamageThreshold then remove the some health
        if (Vector3.Distance (fallStartPosition, target.position) > fallDamageThreshold) {
            /* Here you could remove player health by a percentage - but only if they fall more than the fallDamageThreshold value.
             * This is an example:
             * vitals.health *= 1 - (Vector3.Distance(fallStartPosition, target.position) - fallDamageThreshold) / 100
             * That will remove a percentage of the player health based on how far the player has fallen.
             */
            Debug.Log ("OUCH! Fell : " + Vector3.Distance (fallStartPosition, target.position) + " meters!");
        }
        //Set the position of where the fall started to zero
        fallStartPosition = Vector3.zero;
    }

    //We use IEnumerator so that we can use the yield statement
    IEnumerator Jump() {
        //Set jumping equal to true
        jumping = true;
        //While we're jumping
        while (jumping) {
            //Shoot a sphere upwards to check if we're hitting anything above us. If we hit something then break the jump.
            if (Physics.SphereCast (target.position, col.radius, target.up, out hit, col.height * 0.5f, walkAble)) {
                jumping = false;
            }
            //Wait for the next frame (else the code will run a fraction of a second and the jump won't be animated that way
            yield return new WaitForEndOfFrame ();
            jumpTimer += Time.deltaTime;
            //Lerp-move the target upwards over time
            target.Translate (Vector3.Lerp (new Vector3 (0f, jumpSpeed, 0f), Vector3.zero, jumpTimer / jumpDuration) * Time.deltaTime);
            //If the jump have lasting longer than the jump is set the last at maximum
            if (jumpTimer > jumpDuration) {
                //Stop the while loop by setting jump equal to false
                jumping = false;
                //Set the jumpTimer equal to zero so that it's ready for the next jump
                jumpTimer = 0;
                //Stop the Jump function
                StopCoroutine ("Jump");
            }
        }
    }
}