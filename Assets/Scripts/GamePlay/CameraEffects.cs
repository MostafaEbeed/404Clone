using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class CameraEffects : MonoBehaviour, IPlayerStateListener
{
    [SerializeField] float shakeIntensity;
    [SerializeField] float shakeTime;
    [SerializeField] AnimationCurve shakeCurve;

    CinemachineCamera virtualCamera;
    private float timer;
    private CinemachineBasicMultiChannelPerlin basicNoise;


    [Header("FOV Controls")]
    [SerializeField] private float fovChangeSpeedUp = 7;
    [SerializeField] private float fovChangeSpeedDown =2;


    private void Awake()
    {
        virtualCamera = FindObjectOfType<CinemachineCamera>();

        StopShake();

        Health.OnDamaged += ShakeCameraOnDamaged;
    }

    private void OnDestroy()
    {
        StopShake();
        Health.OnDamaged -= ShakeCameraOnDamaged;
    }

    public void ShakeCameraOnDamaged()
    {
        Debug.Log("Shaking camera");
        
        basicNoise = virtualCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();

        shakeIntensity = shakeCurve.Evaluate(1);

        basicNoise.AmplitudeGain = shakeIntensity;

        timer = shakeTime;
    }

    void StopShake()
    {
        if(virtualCamera)
        {
            basicNoise = virtualCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();
            basicNoise.AmplitudeGain = 0;
        }
        timer = 0;
    }


    private void Update()
    {
        if(timer > 0)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                StopShake();
            }
        }
    }

    [ContextMenu("Test FOV")]
    private void TextFOV()
    {
        ChangeCamFOV(2f);
    }

    public void ChangeCamFOV(float duration)
    {
        StopCoroutine(ChangeFOV(duration));
        StartCoroutine(ChangeFOV(duration));
    }
    
    public void ChangeCamFOV()
    {
        StopCoroutine(ChangeFOV(fovChangeSpeedDown));
        StartCoroutine(ChangeFOV(fovChangeSpeedUp));
    }


    private IEnumerator ChangeFOV(float duration)
    {
        float elapsedTime = 0f;

        // Increase FOV
        while (elapsedTime < duration * 0.85f)
        {
            elapsedTime += Time.deltaTime;
            virtualCamera.Lens.FieldOfView = Mathf.Lerp(virtualCamera.Lens.FieldOfView, 6.5f, (elapsedTime / duration) * fovChangeSpeedUp);
            yield return null;
        }

        elapsedTime = 0f;
        duration *= 0.15f;

        // Decrease FOV back to original
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            virtualCamera.Lens.FieldOfView = Mathf.Lerp(virtualCamera.Lens.FieldOfView, 5.5f, (elapsedTime / duration) * fovChangeSpeedDown);
            yield return null;
        }
    }

    public void OnPlayerStateChange(GameManager.PlayerState playerState)
    {
        if (playerState == GameManager.PlayerState.Boosted)
        {
            ChangeCamFOV();
        }
    }
}
