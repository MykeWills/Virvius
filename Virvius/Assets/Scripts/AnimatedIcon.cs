using UnityEngine;
using UnityEngine.UI;

public class AnimatedIcon : MonoBehaviour
{
    private EnvironmentSystem environmentSystem;
    [SerializeField]
    private float frameRate = 0.06f;
    private int curFrame;
    private float frameTimer;
    [SerializeField]
    private Sprite[] array;
    private Image icon;
    [SerializeField]
    private bool loop = false;
    [SerializeField] private bool overrideIcon = false;
    void Start()
    {
        icon = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        Animate();
    }
    public void Animate()
    {
        if (array == null || !icon.enabled)
            return;
        if (environmentSystem == null) environmentSystem = EnvironmentSystem.environmentSystem;
        if (environmentSystem.headUnderWater && !overrideIcon) icon.enabled = false;
        frameTimer -= Time.unscaledDeltaTime;
        if (frameTimer <= 0)
        {
            frameTimer = frameRate;
            curFrame = (curFrame + 1) % array.Length;
            icon.sprite = array[curFrame];
            if (!loop && curFrame == array.Length - 1) icon.enabled = false;
        }
    }
}
