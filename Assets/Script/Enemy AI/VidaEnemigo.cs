using UnityEngine;
using UnityEngine.AI;
using Ilumisoft.HealthSystem; // Importamos el sistema de vida de la Asset Store

public class VidaEnemigo : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [Header("Componentes a Desactivar al Morir")]
    [SerializeField] private AiEnemy scriptIA;
    [SerializeField] private NavMeshAgent agente;
    [SerializeField] private Collider colisionadorPrincipal;

    // Componente de la Asset Store
    private Health componenteVidaAssetStore;

    void Start()
    {
        if (scriptIA == null) scriptIA = GetComponent<AiEnemy>();
        if (agente == null) agente = GetComponent<NavMeshAgent>();
        if (colisionadorPrincipal == null) colisionadorPrincipal = GetComponent<Collider>();

        // Buscamos el script Health en este enemigo
        componenteVidaAssetStore = GetComponent<Health>();

        if (componenteVidaAssetStore != null)
        {
            // CORRECCIÓN: Usamos += para suscribirnos a la acción de C#
            componenteVidaAssetStore.OnHealthEmpty += Morir;
        }
        else
        {
            Debug.LogError("¡Falta el componente 'Health' de Ilumisoft en el objeto " + gameObject.name + "!");
        }
    }

    private void Morir()
    {
        animator.SetTrigger("Die");
        // CORRECCIÓN: Usamos -= para desuscribirnos de la acción de C#
        if (componenteVidaAssetStore != null)
        {
            componenteVidaAssetStore.OnHealthEmpty -= Morir;
        }

        if (scriptIA != null) scriptIA.enabled = false;
        if (agente != null) agente.enabled = false;
        if (colisionadorPrincipal != null) colisionadorPrincipal.enabled = false;

        Debug.Log(gameObject.name + " ha muerto a través del Health System.");
        Destroy(gameObject, 5f);
    }
}