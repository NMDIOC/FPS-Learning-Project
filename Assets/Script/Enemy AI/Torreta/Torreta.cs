using UnityEngine;
using Ilumisoft.HealthSystem;

public class Torreta : MonoBehaviour
{
    [Header("Detección")]
    [SerializeField] private float rangoDeteccion = 15f;

    [Header("Combate")]
    [SerializeField] private float danoPorSegundo = 20f;
    [SerializeField] private float tiempoEntreDisparos = 0.5f;
    [SerializeField] private Transform puntoDisparo;

    [Header("Rotación")]
    [SerializeField] private float velocidadRotacion = 2f; // ✅ Lento = esquivable

    [Header("Laser")]
    [SerializeField] private LineRenderer lineaLaser;
    [SerializeField] private Material materialLaser;
    [SerializeField] private Color colorLaser = Color.red;

    [Header("Objetivo")]
    [SerializeField] private Transform target; // ✅ Arrastra aquí el hueso/objeto del centro del jugador

    private Transform jugador;
    private Health healthJugador;
    private float tiempoUltimoDisparo;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Jugador");

        if (playerObj != null)
        {
            jugador = playerObj.transform;

            healthJugador = playerObj.GetComponent<Health>();
            if (healthJugador == null) healthJugador = playerObj.GetComponentInChildren<Health>();
            if (healthJugador == null) healthJugador = playerObj.GetComponentInParent<Health>();

            // Si no se asignó target manualmente, apuntar al jugador + 1 metro arriba
            if (target == null)
                target = jugador;
        }
        else
        {
            Debug.LogWarning("Torreta: No se encontró el jugador con tag 'Jugador'");
        }

        ConfigurarLaser();
    }

    void ConfigurarLaser()
    {
        if (lineaLaser == null) return;

        if (materialLaser != null)
            lineaLaser.material = materialLaser;

        lineaLaser.startColor    = colorLaser;
        lineaLaser.endColor      = colorLaser;
        lineaLaser.startWidth    = 0.05f;
        lineaLaser.endWidth      = 0.05f;
        lineaLaser.useWorldSpace = true;
        lineaLaser.positionCount = 2;
        lineaLaser.enabled       = false;
    }

    void Update()
    {
        if (jugador == null || target == null || healthJugador == null) return;

        float distancia = Vector3.Distance(transform.position, jugador.position);

        if (distancia <= rangoDeteccion)
        {
            // ✅ Rotar LENTAMENTE hacia el target — esto lo hace esquivable
            Vector3 direccionObjetivo  = (target.position - puntoDisparo.position).normalized;
            Quaternion rotacionObjetivo = Quaternion.LookRotation(direccionObjetivo);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                rotacionObjetivo,
                Time.deltaTime * velocidadRotacion
            );

            // ✅ Raycast en la dirección ACTUAL del cañón (no hacia el jugador)
            // Si el jugador se esquiva, el rayo no lo sigue instantáneamente
            bool impacta = RaycastCañon(out RaycastHit hit);

            ActualizarLaser(impacta, hit);

            // ✅ Solo daño si el rayo actual toca al jugador
            if (impacta && Time.time >= tiempoUltimoDisparo + tiempoEntreDisparos)
            {
                float dano = danoPorSegundo * tiempoEntreDisparos;
                healthJugador.ApplyDamage(dano);
                tiempoUltimoDisparo = Time.time;
                Debug.Log($"Torreta impactó. Daño: {dano}");
            }
        }
        else
        {
            if (lineaLaser != null)
                lineaLaser.enabled = false;
        }
    }

    bool RaycastCañon(out RaycastHit hit)
    {
        if (puntoDisparo == null)
        {
            hit = default;
            return false;
        }

        // ✅ Rayo en la dirección REAL del cañón, no calculada hacia el jugador
        if (Physics.Raycast(puntoDisparo.position, puntoDisparo.forward, out hit, rangoDeteccion))
        {
            if (hit.collider.CompareTag("Jugador"))
                return true;
        }

        return false;
    }

    void ActualizarLaser(bool impacta, RaycastHit hit)
    {
        if (lineaLaser == null || puntoDisparo == null) return;

        lineaLaser.enabled = true;
        lineaLaser.SetPosition(0, puntoDisparo.position);

        // El laser termina donde choca el rayo del cañón
        if (impacta)
            lineaLaser.SetPosition(1, hit.point);
        else
        {
            // Si no impacta al jugador, el laser choca con lo que haya enfrente
            if (Physics.Raycast(puntoDisparo.position, puntoDisparo.forward, out RaycastHit hitObstaculo, rangoDeteccion))
                lineaLaser.SetPosition(1, hitObstaculo.point);
            else
                lineaLaser.SetPosition(1, puntoDisparo.position + puntoDisparo.forward * rangoDeteccion);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangoDeteccion);
    }
}