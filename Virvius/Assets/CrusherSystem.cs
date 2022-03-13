using UnityEngine;

public class CrusherSystem : MonoBehaviour
{
    public enum Direction { AbscissaX, OrdinateY, ApplicateZ };
    public Direction moveDirection;
    private Direction currDirection;
    [SerializeField]
    private bool isActive = false;
    [SerializeField]
    private float moveSpeed = 5;
    [SerializeField]
    private float minDistance = 0;  
    [SerializeField]
    private float maxDistance = 0;
    private Vector3 updatedPos = Vector3.zero;
    private Vector3 stoppedPos = Vector3.zero;
    [SerializeField]
    private bool reverse = false;
    [SerializeField]
    private bool hold = false;
    [SerializeField]
    private float holdTimer = 1; 
    private float holdTime = 1;
    private float time = 0;
    private bool isStop = false;

    void Start()
    {
        holdTimer = holdTime;
    }
    void Update()
    {
        time = Time.deltaTime;
        StopMoving();
        Move();
    }
    private void Move()
    {
        if (!isActive) return;
        if (currDirection != moveDirection)
        {
            updatedPos = Vector3.zero;
            stoppedPos = Vector3.zero;
            currDirection = moveDirection;
        }
        switch (moveDirection)
        {
            case Direction.OrdinateY: updatedPos.y = reverse ? maxDistance : minDistance; updatedPos.y = reverse ? minDistance : maxDistance; stoppedPos.y = maxDistance; break;
            case Direction.AbscissaX: updatedPos.x = reverse ? maxDistance : minDistance; updatedPos.x = reverse ? minDistance : maxDistance; stoppedPos.x = maxDistance; break;
            case Direction.ApplicateZ: updatedPos.z = reverse ? maxDistance : minDistance; updatedPos.z = reverse ? minDistance : maxDistance; stoppedPos.z = maxDistance; break;
        }
      
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, updatedPos, time * moveSpeed);
        if(transform.localPosition == updatedPos)
        {
            if (!hold)
            {
                reverse = !reverse;
                return;
            }
            holdTimer -= time;
            holdTimer = Mathf.Clamp(holdTimer, 0, holdTime);
            if(holdTimer == 0)
            {
                holdTimer = holdTime;
                reverse = !reverse;
                return;
            }
        }
    }
    public void ActivateCrush(bool active)
    {
        if (active) StartMoving();
        else StopMoving();
    }
    private void StartMoving()
    {
        if (!isActive)
        {
            isActive = true;
        }
    }
    private void StopMoving()
    {
        if(isStop)
        {
            if (isActive)
            {
                reverse = false;
                isActive = false;
            }
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, stoppedPos, time * moveSpeed);
            if (transform.localPosition == stoppedPos) isStop = false;
        }
    }
}
