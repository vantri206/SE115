using System.Collections;
using UnityEngine;

public class SatyrBeamShooter : EnemyWeapon
{
    [Header("Beam Settings")]
    public GameObject beamProjectiles; 
    public float beamGrowTime = 0.5f;
    public float beamMaxLength = 20f;

    private SpriteRenderer beamSprite;

    private void Awake()
    {
        if (beamProjectiles != null)
        {
            if(beamSprite == null)
                beamSprite = beamProjectiles.GetComponent<SpriteRenderer>();

            beamProjectiles.SetActive(false);
        }
    }

    public override void PerformAttack()
    {
        if (beamProjectiles)
        {
            beamProjectiles.SetActive(true);
            StopAllCoroutines();
            StartCoroutine(GrowBeam());
        }
    }

    public override void FinishAttack()
    {
        if (beamProjectiles)
        {
            beamProjectiles.SetActive(false);
        }
    }

    private IEnumerator GrowBeam()
    {
        beamProjectiles.transform.localScale = Vector3.one;
        beamSprite.size = new Vector2(0, beamSprite.size.y);

        float t = 0;
        while (t < beamGrowTime)
        {
            float currentLen = Mathf.Lerp(0, beamMaxLength, t / beamGrowTime);
            beamSprite.size = new Vector2(currentLen, beamSprite.size.y);

            t += Time.deltaTime;
            yield return null;
        }
        beamSprite.size = new Vector2(beamMaxLength, beamSprite.size.y);
    }
}