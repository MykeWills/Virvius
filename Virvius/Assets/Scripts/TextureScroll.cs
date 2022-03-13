using UnityEngine;
public class TextureScroll : MonoBehaviour
{
    public enum Direction { Up, UpRight, Right, DownRight, Down, DownLeft, Left, UpLeft, None };
    public enum Active { On, Off };
    public Direction moveDirection;
    public Active SetScrolling;
    [Range(-1, 1)]
    public float scrollSpeed = 0.01f;
    public int materialIndex = 0;
    private Renderer rend;
    private Vector2 offset = Vector2.zero;
    private float X = 0;
    private float Y = 0;

    public void Start()
    {
        rend = GetComponent<Renderer>();
        offset = rend.materials[materialIndex].mainTextureOffset;
    }
  
    public void Update()
    {
        if (SetScrolling == Active.Off) return;
        switch (moveDirection)
        {
            case Direction.None: X = 0; Y = 0; break;
            case Direction.Up: X = 0; Y = 1; break;
            case Direction.Down: X = 0; Y = -1; break;
            case Direction.Left: X = -1; Y = 0; break;
            case Direction.Right: X = 1; Y = 0; break;
            case Direction.UpLeft: X = -1; Y = 1; break;
            case Direction.UpRight: X = 1; Y = 1; break;
            case Direction.DownLeft: X = -1; Y = -1; break;
            case Direction.DownRight: X = 1; Y = -1; break;
        }
        float time = (Time.deltaTime * scrollSpeed) % 1;
        offset = new Vector2(offset.x + X * time, offset.y + Y * time);
        rend.materials[materialIndex].SetTextureOffset("_MainTex", offset);
    }

}
