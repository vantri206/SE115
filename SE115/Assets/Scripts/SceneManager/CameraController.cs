using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
public class CameraController : MonoBehaviour
{
    [Header("References")]
    public CinemachineCamera vcam;
    public PlayerController player;

    [Header("Settings")]
    public float groundY = 2.0f; 
    public float airY = 0.0f;  

    [Header("Smoothing")]
    public float shiftSpeed = 0.5f;

    public float transitionDuration = 1.0f;

    private CinemachinePositionComposer composer;
    private CinemachineConfiner2D confiner;
    private Collider2D currentArea;

    void Awake()
    {
        composer = vcam.GetComponent<CinemachinePositionComposer>();
        confiner = vcam.GetComponent<CinemachineConfiner2D>();
    }

    void Update()
    {
        float currentOffsetY = composer.TargetOffset.y;
        float newOffsetY = player.CheckOnGround() ? groundY : airY;

        float offsetY = Mathf.Lerp(currentOffsetY, newOffsetY, shiftSpeed * Time.deltaTime);
        composer.TargetOffset.y = offsetY;
    }

    public void SwitchArea(MapArea newArea)
    {
        if (newArea != currentArea)
        {
            StartCoroutine(SwitchAreaCoroutine(newArea));
        }
    }
    IEnumerator SwitchAreaCoroutine(MapArea area)
    {
        confiner.BoundingShape2D = area.areaBounds;
        confiner.InvalidateBoundingShapeCache();
        yield return null;
    }
}