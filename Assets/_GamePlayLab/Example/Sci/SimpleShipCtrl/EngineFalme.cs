using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineFalme : MonoBehaviour
{
    public string inputAxis = "Vertical";

    public Vector3 minFalmeScale;
    public Vector3 maxFalmeScale;
    public Transform falme;

    public float minStar=0;
    public float maxStar=20;
    public ParticleSystem star;

    private ParticleSystem.EmissionModule emission;

    // Start is called before the first frame update
    void Start()
    {
        falme.localScale = minFalmeScale;
        emission=star.emission;
        emission.rateOverTime = minStar;
    }

    // Update is called once per frame
    void Update()
    {
        falme.localScale = Vector3.Lerp(minFalmeScale, maxFalmeScale, Mathf.Clamp01(Input.GetAxis(inputAxis)));
        emission.rateOverTime = maxStar * Mathf.Clamp01(Input.GetAxis(inputAxis));
    }
}
