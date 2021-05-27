using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public bool practiceRooms;
    public Transform target;
    float trackSpeed = 0.1f;
    Vector3 offset = Vector2.zero;
    TestRoom currTestRoom;
    public float TrackSpeed
    {
        set
        {
            trackSpeed = value;
        }
    }

    public TestRoom CurrTestRoom
    {
        set
        {
            currTestRoom = value;
        }
        get
        {
            return currTestRoom;
        }
    }

    public Vector3 Offset
    {
        set
        {
            offset = value;
        }
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!practiceRooms)
        {
            if (target != null)
            {
                Track();
            }
        }
        else
        {
            if (target != null && currTestRoom == null)
            {
                TrackWithin(new Vector2(-30, 15.37f), new Vector2(60, 15.37f));
            }
            else if(currTestRoom != null)
            {
                Track();
            }
        }
    }

    public void Track()
    {
        Vector3 targetPos = new Vector3(offset.x, transform.position.y, Mathf.Clamp(target.position.z, offset.z-4.18f, offset.z + 4.18f));
        transform.position = Vector3.Lerp(transform.position, targetPos, trackSpeed);
    }

    public void TrackWithin(Vector2 minimums, Vector2 maximums)
    {
        Vector3 targetPos = new Vector3(Mathf.Clamp(target.position.x, offset.x + minimums.x, offset.x + maximums.x), transform.position.y, Mathf.Clamp(target.position.z, offset.z + minimums.y, offset.z + maximums.y));
        transform.position = Vector3.Lerp(transform.position, targetPos, trackSpeed);
    }

    public IEnumerator Shake(float magnitude, float duration)
    {
        Vector3 currPosition = transform.position;
        while (duration >= 0)
        {

            duration -= Time.deltaTime;
            float x = Random.Range(-1, 1);
            float z = Random.Range(-1, 1);

            transform.position = new Vector3(currPosition.x + x * magnitude, currPosition.y, currPosition.z + z * magnitude);
            yield return null;
        }
        transform.position = currPosition;
    }
}
