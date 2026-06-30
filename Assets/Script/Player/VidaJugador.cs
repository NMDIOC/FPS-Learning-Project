using UnityEngine;
using Ilumisoft.HealthSystem;

public class VidaJugador : MonoBehaviour
{
    private Health componenteVidaAssetStore;

    [Header("Componentes a apagar al morir")]
    [SerializeField] private MonoBehaviour scriptMovimiento; // Arrastra aquí tu script de movimiento
    [SerializeField] private Disparo scriptDisparo;         // Arrastra aquí tu script de disparo
    [SerializeField] private GameObject graficosJugador;    // El objeto hijo que contiene el modelo 3D/mallas

    void Start()
    {
        componenteVidaAssetStore = GetComponent<Health>();

        if (componenteVidaAssetStore != null)
        {
            componenteVidaAssetStore.OnHealthEmpty += Morir;
        }
        else
        {
            Debug.LogError("¡Falta el componente 'Health' de Ilumisoft en el Jugador!");
        }
    }

    private void Morir()
    {
        if (componenteVidaAssetStore != null)
        {
            componenteVidaAssetStore.OnHealthEmpty -= Morir;
        }

        Debug.Log("GAME OVER: El jugador ha muerto.");
        
        // En lugar de apagar el GameObject completo, apagamos sus funciones:
        if (scriptMovimiento != null) scriptMovimiento.enabled = false;
        if (scriptDisparo != null) scriptDisparo.enabled = false;
        if (graficosJugador != null) graficosJugador.SetActive(false);
    }
}