using UnityEngine;
using TMPro;

public class Portal : MonoBehaviour
{
    [Header("Escena")]
    [SerializeField] private string nombreEscena;

    [Header("UI")]
    [SerializeField] private string nombreTexto = "TextoPortal"; // El nombre del objeto en la jerarquía
    [SerializeField] private string mensajePortal = "Presiona E para entrar";

    private TextMeshProUGUI texto;
    private bool jugadorDentro = false;

    void Start()
    {
        // Buscar el texto por nombre en toda la escena
        GameObject go = GameObject.Find(nombreTexto);
        if (go != null)
            texto = go.GetComponent<TextMeshProUGUI>();

        if (texto != null) texto.enabled = false;
    }

    void Update()
    {
        if (jugadorDentro && Input.GetKeyDown(KeyCode.E))
            UnityEngine.SceneManagement.SceneManager.LoadScene(nombreEscena);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Jugador"))
        {
            Debug.Log("Jugador dentro");
            jugadorDentro = true;
            if (texto != null)
            {
                texto.text = mensajePortal;
                texto.enabled = true;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Jugador"))
        {
            Debug.Log("Jugador fuera");
            jugadorDentro = false;
            if (texto != null) texto.enabled = false;
        }
    }
}