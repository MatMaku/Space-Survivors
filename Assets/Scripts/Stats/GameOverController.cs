using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    public static GameOverController Instance { get; private set; }

    [Header("Referencias")]
    [Tooltip("El GameObject del jugador que desactivaremos.")]
    public GameObject player;
    [Tooltip("Prefab de la explosión (debe tener un ParticleSystem configurado para destruirse al acabar).")]
    public GameObject explosionPrefab;
    [Tooltip("Panel o canvas de Game Over (inactivo al inicio).")]
    public GameObject gameOverUI;
    [Tooltip("Panel o canvas de victoria (inactivo al inicio).")]
    public GameObject winUI;
    [Tooltip("Panel o canvas de UI")]
    public GameObject gameUI;

    [Header("Configuración de la explosión")]
    [Tooltip("Tiempo en segundos que durará la explosión antes de mostrar el Game Over.")]
    public float explosionDuration = 2f;

    bool hasTriggered = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        gameOverUI.SetActive(false);
        winUI.SetActive(false);
        gameUI.SetActive(true);
    }

    public void TriggerGameOver()
    {
        if (hasTriggered) return;
        hasTriggered = true;

        if (player != null) player.SetActive(false);

        if (explosionPrefab != null)
        {
            Vector3 spawnPos = player != null
                ? player.transform.position
                : Vector3.zero;
            Instantiate(explosionPrefab, spawnPos, Quaternion.identity);
        }

        StartCoroutine(ShowGameOverAfterDelay());
    }

    public void TriggerWin()
    {
        StartCoroutine(ShowWinAfterDelay());
    }

    IEnumerator ShowWinAfterDelay()
    {
        float timer = 0f;
        while (timer < 4)
        {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        Time.timeScale = 0f;
        gameUI.SetActive(false);
        if (winUI != null)
            winUI.SetActive(true);
    }

    IEnumerator ShowGameOverAfterDelay()
    {
        float timer = 0f;
        while (timer < explosionDuration)
        {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        Time.timeScale = 0f;
        gameUI.SetActive(false);
        if (gameOverUI != null)
            gameOverUI.SetActive(true);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }
}