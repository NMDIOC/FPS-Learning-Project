using UnityEngine;

public class AtaqueEnemigo : MonoBehaviour
{
    [Header("Configuración de Ataque")]
    [SerializeField] private float danoPorContacto = 20f;
    [SerializeField] private float tiempoEntreAtaques = 1f; // Espera 1 segundo entre golpes

    private float tiempoUltimoAtaque;

    // Se ejecuta al chocar por primera vez
    void OnCollisionEnter(Collision collision)
    {
        ProcesarChoque(collision.gameObject);
    }

    // Se ejecuta continuamente si el enemigo se queda pegado al jugador
    void OnCollisionStay(Collision collision)
    {
        ProcesarChoque(collision.gameObject);
    }

    private void ProcesarChoque(GameObject objetoColisionado)
    {
        // Comprobamos si ya pasó el tiempo de enfriamiento (cooldown) para volver a atacar
        if (Time.time >= tiempoUltimoAtaque + tiempoEntreAtaques)
        {
            // Intentamos obtener el script de VidaJugador del objeto con el que chocamos
            if (objetoColisionado.TryGetComponent<VidaJugador>(out VidaJugador vidaJugador))
            {
                vidaJugador.RecibirDano(danoPorContacto);
                tiempoUltimoAtaque = Time.time; // Reiniciamos el temporizador
            }
        }
    }
}