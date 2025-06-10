using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetRotation : MonoBehaviour
{
    public Vector3 rotationSpeed = new Vector3(10f, 5f, 2f);

    void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }

}
