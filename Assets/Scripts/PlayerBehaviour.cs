using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerBehaviour : MonoBehaviour
{
    const string DEBUG_TAG = "PenBehaviour: ";

    Vector3 spawnLocation;

    TextMeshPro scoreText;

    int score;

    protected Rigidbody leftBody;
    protected Rigidbody middleBody;
    protected Rigidbody rightBody;

    Rigidbody bodyPartToPlay;

    protected Collider leftCollider;
    protected Collider middleCollider;
    protected Collider rightCollider;

    bool isTargetting;

    LineRenderer targetGizmo;

    protected CameraController myCamera;

    protected GameManager myManager;

    public PlayerBehaviour lastAdversary;

    public float comingInHot;

    public int Score
    {
        get => score; 
        set
        {
            score = Mathf.Clamp(value, 0, value);
            scoreText.SetText(score.ToString());
        }
    }

    public float ComingInHot
    {
        get => comingInHot; 
        set
        {
            comingInHot = Mathf.Clamp(value, 0, value);
        }
    }

    private void OnEnable()
    {
        middleBody = GetComponent<Rigidbody>();
        leftBody = transform.GetChild(0).GetComponent<Rigidbody>();
        rightBody = transform.GetChild(1).GetComponent<Rigidbody>();

        scoreText = transform.GetChild(2).GetComponent<TextMeshPro>();

        middleCollider = middleBody.GetComponent<Collider>();
        leftCollider = leftBody.GetComponent<Collider>();
        rightCollider = rightBody.GetComponent<Collider>();

        myCamera = Camera.main.GetComponent<CameraController>();

        spawnLocation = transform.position;

        scoreText.SetText(Score.ToString());
    }

    public void SetManager(GameManager gameManager)
    {
        myManager = gameManager;
    }

    void Update()
    {
        if (isTargetting && Input.GetMouseButton(0))
        {
            Target();
        }
        else if (isTargetting && Input.GetMouseButtonUp(0))
        {
            FinishTargettingAndPlay(true);
        }

        ComingInHot -= Time.deltaTime;

        scoreText.transform.rotation = myCamera.transform.rotation;
        scoreText.transform.position = transform.position + (Vector3.up * 1.2f);
    }

    public void PlayTurn(LineRenderer targetGizmo)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool isHit = Physics.Raycast(ray, out RaycastHit raycastHit);
        if (isHit)
        {
            Vector3 centerOfBodyPart;
            bodyPartToPlay = ClosestBodyPartTo(raycastHit.point, out centerOfBodyPart);

            this.targetGizmo = targetGizmo;
            targetGizmo.enabled = true;

            StartTargetingFrom(centerOfBodyPart);
        }
    }

    private Rigidbody ClosestBodyPartTo(Vector3 point, out Vector3 centerOfBodyPart)
    {
        float disToLeft = Vector3.Distance(point, leftCollider.bounds.center);
        float disToMiddle = Vector3.Distance(point, middleCollider.bounds.center);
        float disToRight = Vector3.Distance(point, rightCollider.bounds.center);


        float minDist = Mathf.Min(disToLeft, disToMiddle, disToRight);

        if (minDist == disToLeft)
        {
            centerOfBodyPart = leftCollider.bounds.center;
            return leftBody;
        }
        else if (minDist == disToMiddle)
        {
            centerOfBodyPart = middleCollider.bounds.center;
            return middleBody;
        }
        else if (minDist == disToRight)
        {
            centerOfBodyPart = rightCollider.bounds.center;
            return rightBody;
        }
        else
        {
            throw new UnityException(DEBUG_TAG + "Couldn't find a minimum distance to a body part.");
        }
    }

    private void StartTargetingFrom(Vector3 position)
    {
        myCamera.ObservePoint(position);
        targetGizmo.SetPosition(0, position);
        isTargetting = true;
    }

    private void Target()
    {
        Vector3 targetPos = ProjectTargetPoint(Input.mousePosition) + targetGizmo.GetPosition(0);
        targetGizmo.SetPosition(1, targetPos);
    }

    public void FinishTargettingAndPlay(bool actuallyPlayed)
    {
        isTargetting = false;
        myCamera.Reset();

        if (targetGizmo)
        {
            Vector3 force = (targetGizmo.GetPosition(0) - targetGizmo.GetPosition(1));
            bodyPartToPlay.AddForce(force * 60, ForceMode.Impulse);
            targetGizmo.enabled = false;
        }
        targetGizmo = null;

        if(actuallyPlayed)
        {
            ComingInHot = 6;
            lastAdversary = null;
            myManager.TurnPlayed();
        }
    }

    public Vector3 ProjectTargetPoint(Vector2 targetPoint)
    {
        float max = Mathf.Min(Camera.main.pixelWidth / 3, Camera.main.pixelHeight / 3);

        Vector2 centreOfScreen = new Vector2(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2);

        Vector2 targetVector = targetPoint - centreOfScreen;

        targetVector = Vector2.ClampMagnitude(targetVector, max);

        targetVector = targetVector / max; //brings targetVector within range 0-1

        float cameraOffsetAngle = Camera.main.transform.rotation.eulerAngles.y * Mathf.Deg2Rad;

        float directAngle = Mathf.Atan2(targetVector.x, targetVector.y);

        float completePerspectiveAngle = cameraOffsetAngle + directAngle;

                                                              /*****for elastic effect while dragging*****/
        float magnitude = targetVector.magnitude * (1.5f + (0.75f * (1 - targetVector.magnitude))); 
        
        return new Vector3(Mathf.Sin(completePerspectiveAngle),
                    0, Mathf.Cos(completePerspectiveAngle)) * magnitude;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if(collision.transform.CompareTag("Player"))
        //{
        //    Debug.Log(name + " HIT " + collision.transform.name);
        //}
        //else if (collision.transform.parent)
        //{
        //    if (collision.transform.parent.CompareTag("Player"))
        //    {
        //        Debug.Log(name + " HIT " + collision.transform.parent.name + "'s " + collision.rigidbody.name);
        //    }
        //}

        PlayerBehaviour collidedPlayer = collision.transform.GetComponentInParent<PlayerBehaviour>();

        if (!collidedPlayer) return;

        if (comingInHot > 0)
        {
            collidedPlayer.lastAdversary = this;

            Debug.Log(name + " HIT " + collidedPlayer.name);

            collision.rigidbody.AddForce(middleBody.velocity * 4, ForceMode.Impulse);
        }

        if (collidedPlayer.comingInHot > 0)
        {
            lastAdversary = collidedPlayer;

            Debug.Log(collidedPlayer.name + " HIT " + name);

            middleBody.AddForce(middleBody.velocity * 4, ForceMode.Impulse);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Floor"))
        {
            Die();
        }
    }

    public void Killed(PlayerBehaviour deadPlayer)
    {
        if (deadPlayer.Equals(this))
        {
            Debug.Log(name + " COMMITED SUICIDE");
            Score--;
        }
        else
        {
            Debug.Log(name + " KILLED " + deadPlayer.name);
            Score++;
        }
    }

    private void Die()
    {
        if (lastAdversary) lastAdversary.Killed(this);
        else Killed(this);
        myManager.Respawn(this, spawnLocation);
    }

    //My lastAdversary is whoever whoever initiated the collision event I am involved in during his turn
    //My last adversary is whoever I touch if neither of us have a last adversary
}
