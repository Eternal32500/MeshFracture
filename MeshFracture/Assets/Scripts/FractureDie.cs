using System.Collections;
using UnityEngine;

public class FractureDie : MonoBehaviour
{
    public float timeToDie = 1f;
    void Start()
    {
        StartCoroutine(DieAfterTime());
    }

    IEnumerator DieAfterTime()
    {
        yield return new WaitForSeconds(timeToDie);
        Destroy(gameObject);
    }
}
