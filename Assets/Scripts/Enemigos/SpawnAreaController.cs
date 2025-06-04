using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnAreaController : MonoBehaviour
{
    public Transform centroArea;
    public Vector3 areaSize;

    public List<Vector3> recibirPuntosDeSpawn(int cantNecesaria)
    {
        List<Vector3> puntos = new List<Vector3>();
        for (int i = 0; i < cantNecesaria; i++)
        {
            Vector3 newPosition = new Vector3(Random.Range(-areaSize.x / 2, areaSize.x / 2),
                                        1,
                                        Random.Range(-areaSize.z / 2, areaSize.z / 2));
            newPosition = centroArea.position + newPosition;
            puntos.Add(newPosition);
        }

        return puntos;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(centroArea.position, areaSize);
    }
}
