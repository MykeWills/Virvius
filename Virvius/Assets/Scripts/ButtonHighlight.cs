using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class ButtonHighlight : MonoBehaviour
{
    private Image highlightImage;
    private AudioSystem audioSystem;
    private GameObject button;
    [SerializeField]
    private AudioClip selectFx;
    private bool highlight = false;
    private float fillInterval = 0;
    private bool selected = false;
    [SerializeField]
    private Image.FillMethod fillMethod;
    void Start()
    {
        audioSystem = AudioSystem.audioSystem;
        button = gameObject;
        for(int o = 0; o < transform.childCount; o++)
        {
            if (transform.GetChild(o).name == "Highlight")
            {
                highlightImage = transform.GetChild(o).GetComponent<Image>();
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject != button && highlight) Activate(false);
        FillImage(fillMethod);
        PulseAlpha();
    }
    private void FillImage(Image.FillMethod fillMethod)
    {
        if (!selected || highlight) return;
        
        if (highlightImage.fillMethod != fillMethod)
            highlightImage.fillMethod = fillMethod;
        fillInterval += Time.unscaledDeltaTime * 2;
        fillInterval = Mathf.Clamp01(fillInterval);
        highlightImage.fillAmount = Mathf.Lerp(0, 1, fillInterval);
        if (fillInterval == 1)
        {
            highlight = true;
            fillInterval = 0;
            
        }
    }
    private void PulseAlpha()
    {
       
        if (!highlight) return;
        
        highlightImage.color = new Color(highlightImage.color.r, highlightImage.color.g, highlightImage.color.b, Mathf.Sin(Time.unscaledTime * 7f) * 0.5f + 0.5f);
    }
    public void Activate(bool active)
    {
        if (highlightImage == null) return;
        if (active) 
        {
            if (EventSystem.current.currentSelectedGameObject != button)
            {
                if(!EventSystem.current.alreadySelecting)
                    EventSystem.current.SetSelectedGameObject(button);
            }
            audioSystem.PlayAudioSource(selectFx, 1, 1, 128); 
        }
        fillInterval = 0;
        highlightImage.fillAmount = 0;
        highlightImage.color = new Color(highlightImage.color.r, highlightImage.color.g, highlightImage.color.b, 1);
        selected = active;
        highlight = false;
       
    }
}
