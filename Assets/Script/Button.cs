using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButonMute : MonoBehaviour
{
    public static bool mutePermanente = false;

    [Header("Slider de Volumen")]
    [SerializeField] private Slider sliderVolumen;
    
    // Al hacerlo STATIC, el valor sobrevive al cambio de escenas
    // y se puede usar directamente abajo sin buscar componentes.
    private static float volumenActual = 1f;

    [Header("Boton Jugar")]
    [SerializeField] private string nombreEscena;

    void Start()
    {
        if (sliderVolumen != null)
        {
            sliderVolumen.minValue = 0f;
            sliderVolumen.maxValue = 1f;
            
            // Hacemos que el slider se posicione en base al volumen que ya estaba guardado
            sliderVolumen.value = volumenActual / 2f;  
            
            sliderVolumen.onValueChanged.AddListener(OnSliderCambiado);
        }
    }

    public void OnSliderCambiado(float valor)
    {
        volumenActual = valor * 2f;
        ActualizarVolumen();
    }

    public void Silence()
    {
        mutePermanente = !mutePermanente;
        ActualizarVolumen();
    }

    public static void ActualizarVolumen()
    {
        // Trae de forma segura el estado del cursor desde tu script de Movimiento
        bool silencio = mutePermanente || Movimiento.cursorDesbloqueado;
        
        // Al ser volumenActual estático, la asignación es directa, limpia y rápida
        AudioListener.volume = silencio ? 0f : volumenActual;
    }

    // Cambiado a PUBLIC para que el botón de Unity pueda ejecutarlo en el OnClick()
    public void Jugar()
    {
        // CORREGIDO: Usamos SceneManager para cargar la escena de forma correcta
        SceneManager.LoadScene(nombreEscena);
    }
}