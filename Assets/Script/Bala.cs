using UnityEngine;

public class Bala : MonoBehaviour
{
    private float danoBala;

    // Método para inyectar el daño desde el script de disparo
    public void SetDano(float cantidad)
    {
        danoBala = cantidad;
    }

    void OnCollisionEnter(Collision collision)
    {
        // Si choca con el enemigo, le aplica el daño
        if (collision.gameObject.TryGetComponent<VidaEnemigo>(out VidaEnemigo vidaEnemigo))
        {
            vidaEnemigo.RecibirDano(danoBala);
        }

        // Se destruye al impactar con cualquier cosa
        Destroy(gameObject);
    }
}