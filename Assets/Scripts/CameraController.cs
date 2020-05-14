using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject lookAt;
    public GameObject lookTowards;
    [Range(0.5f, 15.0f)]
    public float cameraZoom;
    const float maxOff = 30.5f;
    Vector3 defaultReferencePoint;
    [Range(0.0f, 90.0f)]
    public float fixAngle;

    void Start()
    {
        transform.rotation = Quaternion.identity;
        defaultReferencePoint = transform.position;
    }

    void Update()
    {
        if(lookAt && lookTowards)
            LookAtAndTowards(lookAt.transform.position, lookTowards.transform.position);   
    }

    Vector3 RotateByQuaternion(Vector3 vector, Quaternion quaternion){
        Quaternion inverseQ = Quaternion.Inverse(quaternion);
        Quaternion compVector = new Quaternion(vector.x, vector.y, vector.z, 0);
        Quaternion qNion = quaternion * compVector * inverseQ;
        return new Vector3(qNion.x, qNion.y, qNion.z);
    }

    Vector3 AlignWithReferencePoint(Vector3 referencePoint){
        transform.position = referencePoint;

        Vector3 displacementToFocus = (lookAt.transform.position - transform.position).normalized;

        Quaternion rotationToFocus = Quaternion.FromToRotation(Vector3.forward, displacementToFocus);

        transform.rotation = rotationToFocus;

        return displacementToFocus;
    }

    void FollowLookAtObject(){
        float yOff = (maxOff - cameraZoom) * Mathf.Sin(fixAngle * Mathf.Deg2Rad);
        float zOff = (maxOff - cameraZoom) * Mathf.Cos(fixAngle * Mathf.Deg2Rad);
        transform.position = new Vector3(lookAt.transform.position.x, 
            lookAt.transform.position.y + yOff, lookAt.transform.position.z - zOff);
        transform.rotation = Quaternion.Euler(fixAngle, 0, 0);
    }

    void LookAtAndTowards(Vector3 lookAtPoint, Vector3 lookTowardsPoint){
        Vector3 directionToLookAt = (lookAtPoint - lookTowardsPoint).normalized;
        directionToLookAt = new Vector3(directionToLookAt.x, 0, directionToLookAt.z);
        float xzPlaneOff = (maxOff - cameraZoom) * Mathf.Cos(fixAngle * Mathf.Deg2Rad);
        float yOff = (maxOff - cameraZoom) * Mathf.Sin(fixAngle * Mathf.Deg2Rad);

        transform.position = lookAtPoint + (directionToLookAt * xzPlaneOff) + (Vector3.up * yOff);

        Quaternion rotationToDirection = Quaternion.FromToRotation(Vector3.forward, -directionToLookAt);
        Quaternion yRotation = Quaternion.Euler(0, rotationToDirection.eulerAngles.y, 0);
        Quaternion xRotation = Quaternion.Euler(fixAngle, 0, 0);

        transform.rotation = yRotation * xRotation;
    }   

    void TrackLookAtObject(){
        Vector3 displacementToFocus = AlignWithReferencePoint(defaultReferencePoint);
        transform.position = lookAt.transform.position - (maxOff - cameraZoom) * displacementToFocus;
    }

    public void ObservePoint(Vector3 point){
        enabled = false;

        LookAtAndTowards(point, lookTowards.transform.position);
    }

    public void Reset(){
        enabled = true;
    }

   

    //Generate an algorithm to update the defaultReferencePoint based on the pen's location on the table
    //Generate an algorithm -looselyAlignWithReferencePoint- that will be used in TrackLookAtObject()
    //Write out the algorithm to update the defaultReferencePoint based on the lookTowardsObject
    //When its your turn, the camera focuses on you and once you play, the camera switches to bird's eye view
}
