using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnAreaController : MonoBehaviour
{
    public Vector3 centroArea;
    public Vector3 areaSize;
    public Vector3 distaciaCamara;
    public bool pedirPosiciones;

    private void Awake()
    {
        Vector3 centro = Camera.main.transform.position + distaciaCamara;
        centro.y = 0;
        centroArea = centro;
    }

    private void Update()
    {
        Vector3 centro = Camera.main.transform.position + distaciaCamara;
        centro.y = 0;
        centroArea = centro;
        if (Input.GetKeyDown(KeyCode.P) && pedirPosiciones)
        {
            List<Vector3> pos = recibirPuntosDeSpawn(5);
            for (int i = 0; i < pos.Count; i++)
            {
                Debug.Log(pos[i]);
            }
        }
    }

    public List<Vector3> recibirPuntosDeSpawn(int cantNecesaria)
    {
        List<Vector3> puntos = new List<Vector3>();
        for (int i = 0; i < cantNecesaria; i++)
        {
            Vector3 newPosition = new Vector3(Random.Range(-areaSize.x / 2, areaSize.x / 2),
                                        0,
                                        Random.Range(-areaSize.z / 2, areaSize.z / 2));
            newPosition = centroArea + newPosition;
            newPosition.y = 0;
            puntos.Add(newPosition);
        }

        return puntos;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(centroArea, areaSize);
    }
}
