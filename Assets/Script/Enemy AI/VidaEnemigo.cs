using UnityEngine;
using UnityEngine.AI;

public class VidaEnemigo : MonoBehaviour
{
    [Header("Configuración de Vida")]
    [SerializeField] private float vidaMaxima = 100f;
    private float vidaActual;

    [Header("Componentes a Desactivar al Morir")]
    [SerializeField] private AiEnemy scriptIA;
    [SerializeField] private NavMeshAgent agente;
    [SerializeField] private Collider colisionadorPrincipal;

    void Start()
    {
        vidaActual = vidaMaxima;

        if (scriptIA == null) scriptIA = GetComponent<AiEnemy>();
        if (agente == null) agente = GetComponent<NavMeshAgent>();
        if (colisionadorPrincipal == null) colisionadorPrincipal = GetComponent<Collider>();
    }

    public void RecibirDano(float cantidad)
    {
        if (vidaActual <= 0) return;

        vidaActual -= cantidad;

        if (vidaActual <= 0) Morir();
    }

    private void Morir()
    {
        if (scriptIA != null) scriptIA.enabled = false;
        if (agente != null) agente.enabled = false;
        if (colisionadorPrincipal != null) colisionadorPrincipal.enabled = false;

        Debug.Log(gameObject.name + " ha muerto.");
        Destroy(gameObject, 5f);
    }
}