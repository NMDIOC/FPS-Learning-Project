using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Ilumisoft.HealthSystem.UI
{
    [AddComponentMenu("Health System/UI/Ammobar")]
    public class Ammobar : MonoBehaviour
    {
        [Header("Asignación de Componentes")]
        [SerializeField, Tooltip("Puedes arrastrar el objeto del Jugador aquí directamente.")]
        private Disparo scriptDisparo;

        [SerializeField]
        Canvas canvas;

        [SerializeField]
        Image fillImage;

        [SerializeField, Tooltip("Arrastra aquí el componente Text (o TextMeshPro) para mostrar la munición.")]
        private TMP_Text textoContador;

        [SerializeField, Min(0.1f), Tooltip("Controls how fast changes will be animated in points/second")]
        float changeSpeed = 100;

        float currentValue;

        private void Start()
        {
            // Si no se asignó en el inspector, lo buscamos dinámicamente usando tu tag exacto
            if (scriptDisparo == null)
            {
                var player = GameObject.FindGameObjectWithTag("Jugador");
                if (player != null)
                {
                    scriptDisparo = player.GetComponent<Disparo>();
                }
                else
                {
                    Debug.LogWarning("Ammobar: No se encontró ningún GameObject con el Tag 'Jugador'.");
                }
            }

            if (scriptDisparo != null)
            {
                currentValue = scriptDisparo.CurrentAmmo;
            }
        }

        private void Update()
        {
            if (scriptDisparo == null) return;

            // 1. Actualizar el texto en formato "BalasActuales/BalasMaximas"
            if (textoContador != null)
            {
                textoContador.text = $"{scriptDisparo.CurrentAmmo}/{scriptDisparo.MaxAmmo}";
            }

            // 2. Actualizar la barra visual de forma directa para evitar desfases de frames
            if (fillImage != null)
            {
                float maxAmmo = scriptDisparo.MaxAmmo > 0 ? scriptDisparo.MaxAmmo : 30f;
                fillImage.fillAmount = (float)scriptDisparo.CurrentAmmo / maxAmmo;
            }

            // 3. Mantener la animación suave nativa del paquete
            currentValue = Mathf.MoveTowards(currentValue, scriptDisparo.CurrentAmmo, Time.deltaTime * changeSpeed);
        }
    }
}