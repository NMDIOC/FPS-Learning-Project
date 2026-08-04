using UnityEngine;
using UnityEngine.SceneManagement;

public class PantallaMuerte : MonoBehaviour
{
    // ✅ El panel ES el mismo objeto donde está el script
    // No necesita referencia externa

    void Start()
    {
        gameObject.SetActive(false);
    }

    public void MostrarPantallaMuerte()
    {
        gameObject.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;

        Time.timeScale = 0f;
    }

    public void Reintentar()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Salir()
    {
        Time.timeScale = 1f;
        Application.Quit();
    }
}