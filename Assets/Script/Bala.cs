using UnityEngine;
using Ilumisoft.HealthSystem; // Importamos el sistema de vida de la Asset Store

public class Bala : MonoBehaviour
{
    private float danoBala;

    [SerializeField] GameObject EfectoDeExplosion;

    public void SetDano(float cantidad)
    {
        danoBala = cantidad;
    }

    void OnCollisionEnter(Collision collision)
    {
        // Buscamos el componente Health de Ilumisoft en el objeto que golpeamos
        if (collision.gameObject.TryGetComponent<Health>(out Health healthEnemigo))
        {
            // Aplicamos el daño usando la función nativa del paquete
            healthEnemigo.ApplyDamage(danoBala);
        }

        Instantiate(EfectoDeExplosion, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}