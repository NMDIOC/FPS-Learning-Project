using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AiEnemy : MonoBehaviour
{
    private enum EstadoIA { Patrullando, Persiguiendo }

    [Header("Componentes")]
    [SerializeField] private NavMeshAgent agente;

    [Header("Configuración de Patrulla")]
    [SerializeField] private Transform puntoA;
    [SerializeField] private Transform puntoB;
    [SerializeField] private float distanciaMinimaPunto = 1f;

    [Header("Detección del Jugador")]
    [SerializeField] private Transform jugador;
    [SerializeField] private float rangoVision = 15f;
    [SerializeField] private float anguloVision = 60f;
    [SerializeField] private LayerMask capaObstaculos;

    private EstadoIA estadoActual = EstadoIA.Patrullando;
    private Transform puntoObjetivoActual;

    void Start()
    {
        if (agente == null) agente = GetComponent<NavMeshAgent>();

        if (jugador == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) jugador = playerObj.transform;
        }

        puntoObjetivoActual = puntoA;
        if (puntoObjetivoActual != null && agente != null && agente.isActiveAndEnabled && agente.isOnNavMesh) 
            agente.SetDestination(puntoObjetivoActual.position);
    }

    void Update()
    {
        if (agente == null || !agente.enabled || !agente.isOnNavMesh) return;

        if (estadoActual == EstadoIA.Patrullando && VariableVerJugador())
        {
            estadoActual = EstadoIA.Persiguiendo;
        }
    }

    void FixedUpdate()
    {
        if (agente == null || !agente.enabled || !agente.isOnNavMesh) return;
        
        switch (estadoActual)
        {
            case EstadoIA.Patrullando: LogicaPatrulla(); break;
            case EstadoIA.Persiguiendo: LogicaPersecucion(); break;
        }
    }

    private void LogicaPatrulla()
    {
        if (agente == null || !agente.enabled || !agente.isOnNavMesh) return;
        if (puntoA == null || puntoB == null) return;

        if (!agente.pathPending && agente.remainingDistance <= distanciaMinimaPunto)
        {
            puntoObjetivoActual = (puntoObjetivoActual == puntoA) ? puntoB : puntoA;
            agente.SetDestination(puntoObjetivoActual.position);
        }
    }

    private void LogicaPersecucion()
    {
        if (agente == null || !agente.enabled || !agente.isOnNavMesh) return;
        if (jugador == null) return;

        agente.SetDestination(jugador.position);
    }

    private bool VariableVerJugador()
    {
        if (jugador == null) return false;

        float distanciaAlJugador = Vector3.Distance(transform.position, jugador.position);
        if (distanciaAlJugador > rangoVision) return false;

        Vector3 direccionAlJugador = (jugador.position - transform.position).normalized;
        float angulo = Vector3.Angle(transform.forward, direccionAlJugador);

        if (angulo < anguloVision)
        {
            Vector3 origenRayo = transform.position + Vector3.up * 1f;
            Vector3 destinoRayo = jugador.position + Vector3.up * 1f;
            Vector3 direccionRayo = destinoRayo - origenRayo;

            if (!Physics.Raycast(origenRayo, direccionRayo, distanciaAlJugador, capaObstaculos))
            {
                return true;
            }
        }
        return false;
    }
}