using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Escena de Juego")]
    [Tooltip("Nombre exacto de la escena de juego en Build Settings")]
    public string gameSceneName = "GameScene";

    [Header("Animaci�n de la Nave")]
    [Tooltip("GameObject de la nave que animaremos")]
    public GameObject ship;
    [Tooltip("Punto A: origen del movimiento")]
    public Transform pointA;
    [Tooltip("Punto B: destino del movimiento")]
    public Transform pointB;

    [Tooltip("Tiempo m�nimo/m�ximo entre vuelos")]
    public float waitTimeMin = 3f;
    public float waitTimeMax = 6f;
    [Tooltip("Duraci�n m�nima/m�xima de cada vuelo")]
    public float moveTimeMin = 2f;
    public float moveTimeMax = 4f;
    [Tooltip("Velocidad de rotaci�n aleatoria (grados/s)")]
    public float rotationSpeedMin = 30f;
    public float rotationSpeedMax = 90f;

    void Start()
    {
        // Arranca con la nave en A y lanza la rutina
        if (ship != null && pointA != null)
            ship.transform.position = pointA.position;
        StartCoroutine(ShipRoutine());
    }

    // Ligar a tu bot�n �Jugar� en el OnClick
    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    IEnumerator ShipRoutine()
    {
        while (true)
        {
            // 1) Espera un intervalo aleatorio
            float wait = Random.Range(waitTimeMin, waitTimeMax);
            yield return new WaitForSeconds(wait);

            // 2) Ejecuta el vuelo y rotaci�n
            yield return StartCoroutine(MoveAndRotateShip());
        }
    }

    IEnumerator MoveAndRotateShip()
    {
        if (ship == null || pointA == null || pointB == null)
            yield break;

        float duration = Random.Range(moveTimeMin, moveTimeMax);
        float elapsed = 0f;

        Vector3 startPos = pointA.position;
        Vector3 endPos = pointB.position;

        // Elige un eje y velocidad aleatoria para girar
        Vector3 rotAxis = Random.onUnitSphere;
        float rotSpeed = Random.Range(rotationSpeedMin, rotationSpeedMax);

        ship.transform.position = startPos;
        ship.transform.rotation = Quaternion.identity;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            // Posici�n lineal
            ship.transform.position = Vector3.Lerp(startPos, endPos, t);
            // Rotaci�n continua
            ship.transform.Rotate(rotAxis, rotSpeed * Time.deltaTime, Space.World);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Asegura posici�n final exacta
        ship.transform.position = endPos;
    }
}