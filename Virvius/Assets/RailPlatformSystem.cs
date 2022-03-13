using UnityEngine;

public class RailPlatformSystem : MonoBehaviour
{
    [SerializeField]
    private Transform[] railPositions;
    [SerializeField]
    private Transform[] rails;
    [SerializeField]
    private float moveSpeed = 5f;
    [SerializeField]
    private int[] interval;

    // Update is called once per frame
    void FixedUpdate()
    {
        for (int i = 0; i < rails.Length; i++) 
        { 
            rails[i].transform.position = Vector3.MoveTowards(rails[i].transform.position, railPositions[interval[i]].transform.position, Time.deltaTime * moveSpeed);
            if (rails[i].transform.position == railPositions[interval[i]].transform.position)
            {
                interval[i]++;
                if (interval[i] > railPositions.Length - 1)
                {
                    interval[i] = 1;
                    rails[i].transform.position = railPositions[0].transform.position;
                }
            }

        }
    }
}
