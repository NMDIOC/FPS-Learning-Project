using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AiEnemy : MonoBehaviour
{
    // 1. Añadimos 'Atacando' al Enum para tener un control absoluto de los comportamientos
    private enum EstadoIA { Patrullando, Persiguiendo, Atacando, Muerto }

    [Header("Componentes")]
    [SerializeField] private NavMeshAgent agente;
    [SerializeField] private Animator animator;

    [Header("Configuración de Patrulla")]
    [SerializeField] private Transform puntoA;
    [SerializeField] private Transform puntoB;
    [SerializeField] private float distanciaMinimaPunto = 1f;

    [Header("Detección del Jugador")]
    [SerializeField] private Transform jugador;
    [SerializeField] private float rangoVision = 15f;
    [SerializeField] private float anguloVision = 60f;
    [SerializeField] private LayerMask capaObstaculos;

    [Header("Configuración de Ataque")]
    [SerializeField] private float distanciaAtaque = 2f; 

    [Header("Ajustes de Muerte (Caída al suelo)")]
    [SerializeField] private float velocidadCaida = 2.5f; 
    [SerializeField] private LayerMask capaSuelo; 

    private EstadoIA estadoActual = EstadoIA.Patrullando;
    private Transform puntoObjetivoActual;
    private bool haGritado = false; 
    private Vector3 posicionSueloObjetivo; 
    private bool sueloDetectado = false;

    // 2. OPTIMIZACIÓN DE RENDIMIENTO: Convertimos los textos de animación en IDs numéricos únicos
    private static readonly int HashWalk = Animator.StringToHash("Walk");
    private static readonly int HashAttack = Animator.StringToHash("attack");
    private static readonly int HashPush = Animator.StringToHash("Push");
    private static readonly int HashDie = Animator.StringToHash("Die");

    void Start()
    {
        if (agente == null) agente = GetComponent<NavMeshAgent>();
        if (animator == null) animator = GetComponent<Animator>();

        if (jugador == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Jugador");
            if (playerObj != null) jugador = playerObj.transform;
        }

        puntoObjetivoActual = puntoA;
        if (puntoObjetivoActual != null && agente != null && agente.isActiveAndEnabled && agente.isOnNavMesh) 
        {
            agente.SetDestination(puntoObjetivoActual.position);
            if (animator != null) animator.SetTrigger(HashWalk);
        }
    }

    void Update()
    {
        // Si está muerto, procesamos su caída y detenemos todo lo demás de inmediato
        if (estadoActual == EstadoIA.Muerto)
        {
            ProcesarCaidaMuerte();
            return;
        }

        // Validación de seguridad para el NavMeshAgent
        if (agente == null || !agente.enabled || !agente.isOnNavMesh) return;

        // 3. CENTRALIZACIÓN: Usamos un único Switch para controlar toda la lógica del enemigo
        switch (estadoActual)
        {
            case EstadoIA.Patrullando:
                LogicaPatrulla();
                // Transición: Si patrulla y ve al jugador, cambia a perseguir
                if (VariableVerJugador())
                {
                    CambiarEstado(EstadoIA.Persiguiendo);
                }
                break;

            case EstadoIA.Persiguiendo:
                LogicaPersecucion();
                break;

            case EstadoIA.Atacando:
                LogicaAtaque();
                break;
        }
    }

    // 4. MEJORA DE ARQUITECTURA: Método dedicado a cambiar de estado de forma segura
    private void CambiarEstado(EstadoIA nuevoEstado)
    {
        estadoActual = nuevoEstado;

        // Acciones que se ejecutan una única vez AL ENTRAR al nuevo estado
        if (nuevoEstado == EstadoIA.Atacando)
        {
            agente.isStopped = true; // Frenamos el agente para que golpee desde su lugar
            if (animator != null) animator.SetBool(HashPush, true);
        }
        else if (nuevoEstado == EstadoIA.Persiguiendo)
        {
            agente.isStopped = false; // Reanudamos el movimiento
            if (animator != null) animator.SetBool(HashPush, false);
        }
    }

    private void LogicaPatrulla()
    {
        if (puntoA == null || puntoB == null) return;

        if (!agente.pathPending && agente.remainingDistance <= distanciaMinimaPunto)
        {
            puntoObjetivoActual = (puntoObjetivoActual == puntoA) ? puntoB : puntoA;
            agente.SetDestination(puntoObjetivoActual.position);
        }
    }

    private void LogicaPersecucion()
    {
        if (jugador == null) return;

        agente.SetDestination(jugador.position);

        // Grito de alerta la primera vez que ve al jugador
        if (!haGritado)
        {
            if (animator != null) animator.SetTrigger(HashAttack);
            haGritado = true;
        }

        // Transición: Si se acerca lo suficiente, cambia al estado Atacando
        float distanciaAlJugador = Vector3.Distance(transform.position, jugador.position);
        if (distanciaAlJugador <= distanciaAtaque)
        {
            CambiarEstado(EstadoIA.Atacando);
        }
    }

    private void LogicaAtaque()
    {
        if (jugador == null) return;

        // MEJORA VISUAL: Forzamos al Zindacoid a mirar al jugador mientras lo empuja/ataca
        Vector3 direccionAlJugador = (jugador.position - transform.position).normalized;
        direccionAlJugador.y = 0; // Evita rotaciones extrañas si el mapa tiene desniveles
        if (direccionAlJugador != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direccionAlJugador);
        }

        // Transición: Si el jugador logra alejarse, el enemigo vuelve a perseguirlo
        float distanciaAlJugador = Vector3.Distance(transform.position, jugador.position);
        if (distanciaAlJugador > distanciaAtaque)
        {
            CambiarEstado(EstadoIA.Persiguiendo);
        }
    }

    private void ProcesarCaidaMuerte()
    {
        if (!sueloDetectado) return;

        if (transform.position.y > posicionSueloObjetivo.y)
        {
            transform.position = Vector3.MoveTowards(
                transform.position, 
                new Vector3(transform.position.x, posicionSueloObjetivo.y, transform.position.z), 
                velocidadCaida * Time.deltaTime
            );
        }
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

    public void Morir()
    {
        if (estadoActual == EstadoIA.Muerto) return;

        estadoActual = EstadoIA.Muerto;

        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, out RaycastHit hit, 10f, capaSuelo))
        {
            posicionSueloObjetivo = hit.point;
            sueloDetectado = true;
        }
        else
        {
            posicionSueloObjetivo = transform.position;
            sueloDetectado = true;
        }

        if (animator != null)
        {
            animator.SetBool(HashPush, false);
            animator.SetTrigger(HashDie);
        }

        if (agente != null)
        {
            agente.isStopped = true;
            agente.enabled = false;
        }

        if (TryGetComponent<Collider>(out Collider col))
        {
            col.enabled = false; 
        }
    }
}