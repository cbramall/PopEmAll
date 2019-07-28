using UnityEngine;
using System.Collections;
using System;
using Random = UnityEngine.Random;

/// Makes objects float up & down while gently spinning.
public class Floater : MonoBehaviour
{
    // User Inputs
    public float degreesPerSecond = 15.0f;
    public float degreePerSecondMin = 5f;
    public float degreePerSecondMax = 25f;
    public float amplitude = 0.5f;
    public float amplitudeMin = 0.25f;
    public float amplitudeMax = 1f;
    public float yFrequency = 1f;
    public float frequencyMin = 0.05f;
    public float frequencyMax = 0.1f;
    public float xFrequency = 1f;
    public bool IsPopped = false;

    public EventHandler HasPopped;

    [SerializeField]
    private Animator animator = null;

    // Position Storage Variables
    Vector3 posOffset = new Vector3();
    Vector3 tempPos = new Vector3();

    // Use this for initialization
    void Start()
    {
        // Store the starting position & rotation of the object
        posOffset = transform.position;
        amplitude = Random.Range(amplitudeMin, amplitudeMax);
        yFrequency = Random.Range(frequencyMin, frequencyMax);
        xFrequency = Random.Range(frequencyMin, frequencyMax/2);
        degreesPerSecond = Random.Range(degreePerSecondMin, degreePerSecondMax);
    }

    private void OnEnable()
    {
        animator.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        // Spin object around Y-Axis
        transform.Rotate(new Vector3(0f, Time.deltaTime * degreesPerSecond, 0f), Space.World);

        // Float up/down with a Sin()
        tempPos = posOffset;
        tempPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * yFrequency) * amplitude;
        tempPos.x += Mathf.Sin(Time.fixedUnscaledTime * Mathf.PI * xFrequency);
        transform.position = tempPos;
    }

    public void PopMe()
    {
        IsPopped = true;
        Invoke(nameof(TurnMeOffLater), 3f);
    }

    private void TurnMeOffLater()
    {
        gameObject.SetActive(false);
        HasPopped.Invoke(this, null);
    }
}