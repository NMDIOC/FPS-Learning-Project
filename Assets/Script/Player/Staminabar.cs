using UnityEngine;
using UnityEngine.UI;

namespace Ilumisoft.HealthSystem.UI
{
    [AddComponentMenu("Health System/UI/Staminabar")]
    public class Staminabar : MonoBehaviour
    {
        [Header("Asignación de Componentes")]
        [SerializeField, Tooltip("Puedes arrastrar el objeto del Jugador directamente aquí si lo deseas.")]
        private Movimiento scriptMovimiento;

        [SerializeField]
        Canvas canvas;

        [SerializeField]
        Image fillImage;

        [SerializeField, Min(0.1f), Tooltip("Controls how fast changes will be animated in points/second")]
        float changeSpeed = 100;

        float currentValue;

        private void Start()
        {
            // Si no arrastraste el script manualmente en el inspector, lo buscamos con tu tag exacto
            if (scriptMovimiento == null)
            {
                var player = GameObject.FindGameObjectWithTag("Jugador"); // Corregido a tu tag en español
                if (player != null)
                {
                    scriptMovimiento = player.GetComponent<Movimiento>();
                }
                else
                {
                    Debug.LogWarning("Staminabar: No se encontró ningún GameObject con el Tag 'Jugador'. Asegúrate de asignarlo en el inspector.");
                }
            }

            if (scriptMovimiento != null)
            {
                currentValue = scriptMovimiento.CurrentStamina;
            }
        }

        private void Update()
        {
            if (scriptMovimiento == null || fillImage == null) return;

            // Forzamos el fillAmount directo para evitar que se quede congelada al inicio
            float maxStamina = scriptMovimiento.MaxStamina > 0 ? scriptMovimiento.MaxStamina : 100f;
            fillImage.fillAmount = scriptMovimiento.CurrentStamina / maxStamina;

            // Transición suave para efectos visuales estéticos
            currentValue = Mathf.MoveTowards(currentValue, scriptMovimiento.CurrentStamina, Time.deltaTime * changeSpeed);
        }
    }
}