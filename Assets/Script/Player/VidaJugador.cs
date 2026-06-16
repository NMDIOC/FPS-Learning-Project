using UnityEngine;

public class VidaJugador : MonoBehaviour
{
    [Header("Configuración de Vida")]
    [SerializeField] private float vidaMaxima = 100f;
    private float vidaActual;

    void Start()
    {
        vidaActual = vidaMaxima;
    }

    public void RecibirDano(float cantidad)
    {
        if (vidaActual <= 0) return;

        vidaActual -= cantidad;
        Debug.Log("El jugador recibió daño. Vida restante: " + vidaActual);

        if (vidaActual <= 0)
        {
            Morir();
        }
    }

    private void Morir()
    {
        Debug.Log("GAME OVER: El jugador ha muerto.");
        // Aquí puedes añadir lógica futura: mostrar pantalla de reinicio, 
        // desactivar el movimiento, o recargar la escena.
        
        // Por ahora, solo desactivamos el objeto del jugador para probar:
        gameObject.SetActive(false); 
    }
}