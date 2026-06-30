using UnityEngine;
using Ilumisoft.HealthSystem; // Importamos el sistema de vida de la Asset Store

public class AtaqueEnemigo : MonoBehaviour
{
    [Header("Configuración de Ataque")]
    [SerializeField] private float danoPorContacto = 20f;
    [SerializeField] private float tiempoEntreAtaques = 1f;
    
    [Tooltip("Debe ser igual o un poquito mayor que la 'Distancia Ataque' de tu AiEnemy (Ej: 2.2)")]
    [SerializeField] private float rangoDeAtaque = 2.2f; 

    private Transform jugador;
    private Health healthJugador;
    private float tiempoUltimoAtaque;

    void Start()
    {
        // Buscamos al jugador de forma ultra segura usando tu tag exacto en español
        GameObject playerObj = GameObject.FindGameObjectWithTag("Jugador");
        
        if (playerObj != null)
        {
            jugador = playerObj.transform;
            
            // Buscamos el componente de Ilumisoft de forma elástica (en la raíz, hijos o padres)
            healthJugador = playerObj.GetComponent<Health>();
            if (healthJugador == null) healthJugador = playerObj.GetComponentInChildren<Health>();
            if (healthJugador == null) healthJugador = playerObj.GetComponentInParent<Health>();
        }
        else
        {
            Debug.LogWarning("AtaqueEnemigo: No se encontró ningún GameObject con el Tag 'Jugador'. Asegúrate de que tu personaje lo tenga asignado.");
        }
    }

    void Update()
    {
        // Si no hay jugador, no tiene el componente Health, o el enemigo está muerto, salimos
        if (jugador == null || healthJugador == null) return;

        // Calculamos la distancia matemática real en unidades/metros entre el enemigo y el jugador
        float distanciaAlJugador = Vector3.Distance(transform.position, jugador.position);

        // Si el Zindacoid está dentro de su rango de ataque y el temporizador lo permite
        if (distanciaAlJugador <= rangoDeAtaque)
        {
            if (Time.time >= tiempoUltimoAtaque + tiempoEntreAtaques)
            {
                // Aplicamos el daño directo al sistema de la Asset Store
                healthJugador.ApplyDamage(danoPorContacto);
                tiempoUltimoAtaque = Time.time;
                
                // Mensaje en consola para que veas en tiempo real si funciona sin abrir la UI
                Debug.Log($"¡Zindacoid atacó! Daño realizado: {danoPorContacto}.");
            }
        }
    }
}