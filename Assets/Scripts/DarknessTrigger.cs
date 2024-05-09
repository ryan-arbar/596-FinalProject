using UnityEngine;
using System.Collections;

public class DarknessTrigger : MonoBehaviour
{
    public Light directionalLight;
    public float fadeDuration = 2.0f;

    private void Start()
    {
        if (directionalLight == null)
        {
            GameObject lightGameObject = GameObject.FindGameObjectWithTag("Sun");
            if (lightGameObject != null)
            {
                directionalLight = lightGameObject.GetComponent<Light>();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(FadeToDarkness());
        }
    }

    // Fades out both the directional light and ambient light intensity
    IEnumerator FadeToDarkness()
    {
        float startIntensity = directionalLight.intensity;
        float startAmbientIntensity = RenderSettings.ambientIntensity;
        float elapsed = 0.0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / fadeDuration;
            directionalLight.intensity = Mathf.Lerp(startIntensity, 0f, progress);
            RenderSettings.ambientIntensity = Mathf.Lerp(startAmbientIntensity, 0f, progress);

            yield return null;
        }

        directionalLight.intensity = 0f;  // Ensure the light is at 0
        RenderSettings.ambientIntensity = 0f;  // Ensure ambient light is at 0
    }
}
