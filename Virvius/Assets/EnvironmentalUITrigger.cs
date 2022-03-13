using UnityEngine;

public class EnvironmentalUITrigger : MonoBehaviour
{
    public enum Environment { rain, heavyRain, heat };
    [SerializeField]
    private Environment environment;
    private Environment curEnvironment;
    private PlayerSystem playerSystem;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (playerSystem == null) playerSystem = PlayerSystem.playerSystem;
            curEnvironment = environment;
            switch (curEnvironment)
            {
                case Environment.rain: playerSystem.StartFadeEnvironmentUI(true, 0, new Color(1f, 1f, 1f, 1)); break;
                case Environment.heavyRain: playerSystem.StartFadeEnvironmentUI(true, 1, new Color(1f, 1f, 1f, 1)); break;
                case Environment.heat: playerSystem.StartFadeEnvironmentUI(true, 2, new Color(1f, 0.6f, 0.6f, 1)); break;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (playerSystem == null) playerSystem = PlayerSystem.playerSystem;
            switch (curEnvironment)
            {
                case Environment.rain: playerSystem.StartFadeEnvironmentUI(false, 0, new Color(1f, 1f, 1f, 1)); break;
                case Environment.heavyRain: playerSystem.StartFadeEnvironmentUI(false, 1, new Color(1f, 1f, 1f, 1)); break;
                case Environment.heat: playerSystem.StartFadeEnvironmentUI(false, 2, new Color(1f, 0.6f, 0.6f, 1)); break;
            }
        }
    }
}
