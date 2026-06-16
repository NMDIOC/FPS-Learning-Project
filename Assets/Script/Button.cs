using UnityEngine;
using UnityEngine.UI;

public class BotonMute : MonoBehaviour
{
    public static bool mutePermanente = false;

    [Header("Slider de Volumen")]
    [SerializeField] private Slider sliderVolumen;
    // 0.5 = volumen normal (1f), 1 = doble volumen (2f), 0 = silencio
    private float volumenActual = 1f;

    void Start()
    {
        if (sliderVolumen != null)
        {
            sliderVolumen.minValue = 0f;
            sliderVolumen.maxValue = 1f;
            sliderVolumen.value = 0.5f;  // Empieza en el centro (volumen normal)
            sliderVolumen.onValueChanged.AddListener(OnSliderCambiado);
        }
    }

    // Llamado automáticamente cuando el slider cambia
    public void OnSliderCambiado(float valor)
    {
        // 0.0 → 0f (silencio)
        // 0.5 → 1f (normal)
        // 1.0 → 2f (doble)
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
        bool silencio = mutePermanente || Movimiento.cursorDesbloqueado;
        // Si está muteado volumen 0, si no aplica el volumen del slider
        AudioListener.volume = silencio ? 0f : FindObjectOfType<BotonMute>()?.volumenActual ?? 1f;
    }
}