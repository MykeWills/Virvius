using UnityEngine;

public class ChunkTrigger : MonoBehaviour
{
    [SerializeField]
    private ChunkSystem chunkSystem;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("LavaChunk"))
        {
            chunkSystem.ChunkActivation(false);
        }
    }
}
