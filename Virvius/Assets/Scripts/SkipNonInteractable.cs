using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Rewired;
public class SkipNonInteractable : MonoBehaviour, ISelectHandler
{
    private Selectable m_Selectable;
    private Player inputPlayer;
    private IEnumerator routine = null;
    private WaitForEndOfFrame endOfFrame = new WaitForEndOfFrame();
    private AudioSystem audioSystem;
    void Awake()
    {
        m_Selectable = GetComponent<Selectable>();
        inputPlayer = ReInput.players.GetPlayer(0);
    }
   public void SelectCurrentSelected()
    {
        m_Selectable.Select();
    }
    public void OnSelect(BaseEventData evData)
    {
        // Don't apply skipping unless we are not interactable.
        if (m_Selectable.interactable) return;

        // Check if the user navigated to this selectable.
        if (inputPlayer.GetAxis("LSHUI") < 0)
        {
            Selectable select = m_Selectable.FindSelectableOnLeft();
            //if (select == null || !select.gameObject.activeInHierarchy)
            //    select = m_Selectable.FindSelectableOnRight();
            if (routine != null) StopCoroutine(routine);
            routine = DelaySelect(select);
            StartCoroutine(routine);
        }
        else if (inputPlayer.GetAxis("LSHUI") > 0)
        {
            Selectable select = m_Selectable.FindSelectableOnRight();
            //if (select == null || !select.gameObject.activeInHierarchy)
            //    select = m_Selectable.FindSelectableOnLeft();
            if (routine != null) StopCoroutine(routine);
            routine = DelaySelect(select);
            StartCoroutine(routine);
        }
        else if (inputPlayer.GetAxis("LSVUI") < 0)
        {
            Selectable select = m_Selectable.FindSelectableOnDown();
            //if (select == null || !select.gameObject.activeInHierarchy)
            //    select = m_Selectable.FindSelectableOnUp();
            if (routine != null) StopCoroutine(routine);
            routine = DelaySelect(select);
            StartCoroutine(routine);
        }
        else if (inputPlayer.GetAxis("LSVUI") > 0)
        {
            Selectable select = m_Selectable.FindSelectableOnUp();
            //if (select == null || !select.gameObject.activeInHierarchy)
            //    select = m_Selectable.FindSelectableOnDown();
            if (routine != null) StopCoroutine(routine);
            routine = DelaySelect(select);
            StartCoroutine(routine);
        }
    }
    private IEnumerator DelaySelect(Selectable select)
    {
        yield return endOfFrame;

        if (select != null || !select.gameObject.activeInHierarchy)
            select.Select();
        else
            Debug.LogWarning("Please make sure your explicit navigation is configured correctly.");
        yield break;
    }
    public void OnSelectable(AudioClip clip)
    {
        if (audioSystem == null) audioSystem = AudioSystem.audioSystem;
        if (GetComponent<Button>() != null)
        {
            if (GetComponent<Button>().interactable)
                audioSystem.PlayAudioSource(clip, 1, 1, 128);
        }
        else
            audioSystem.PlayAudioSource(clip, 1, 1, 128);
    }
}
