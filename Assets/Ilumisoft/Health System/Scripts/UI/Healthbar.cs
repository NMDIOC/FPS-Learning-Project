using UnityEngine;
using UnityEngine.UI;

namespace Ilumisoft.HealthSystem.UI
{
    [AddComponentMenu("Health System/UI/Healthbar")]
    public class Healthbar : MonoBehaviour
    {
        [field:SerializeField]
        public HealthComponent Health { get; set; }

        [SerializeField]
        Canvas canvas;

        [SerializeField]
        Image fillImage;

        [SerializeField, Tooltip("Whether the healthbar should be hidden when health is empty")]
        bool hideEmpty = false;

        [SerializeField, Tooltip("Makes the healthbar being aligned with the camera")]
        bool alignWithCamera = false;

        [SerializeField, Min(0.1f), Tooltip("Controls how fast changes will be animated in points/second")]
        float changeSpeed = 100;

        float currentValue;

        protected virtual void Reset()
        {
            if (Health == null)
            {
                Health = GetComponentInParent<HealthComponent>();
            }
        }

        private void Start()
        {
            // PROTECCIÓN SÓLIDA: Si es la barra de la UI fija (no se alinea con la cámara), 
            // forzamos la búsqueda del jugador vivo en la escena para evitar errores de asignación de Prefabs.
            if (!alignWithCamera)
            {
                var player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    Health = player.GetComponent<Ilumisoft.HealthSystem.Health>();
                }
            }

            // Si sigue siendo nulo (por ejemplo, en un enemigo), busca en sus componentes padres
            if (Health == null)
            {
                Health = GetComponentInParent<HealthComponent>();
            }

            // Inicializamos el valor solo si encontramos el componente de vida
            if (Health != null)
            {
                currentValue = Health.CurrentHealth;
            }
        }

        private void Update()
        {
            // CONTROL DE SEGURIDAD: Si no hay vida o imagen asignada, salimos para evitar NullReferenceException
            if (Health == null || fillImage == null) return;

            if (alignWithCamera)
            {
                AlignWithCamera();
            }

            // El sistema calcula suavemente el movimiento de la barra hacia la vida real
            currentValue = Mathf.MoveTowards(currentValue, Health.CurrentHealth, Time.deltaTime * changeSpeed);
            
            UpdateFillbar();
            UpdateVisibility();
        }

        private void AlignWithCamera()
        {
            if (Camera.main != null)
            {
                transform.forward = Camera.main.transform.forward;
            }
        }

        void UpdateFillbar()
        {
            if (Health == null || fillImage == null) return;

            // Evitamos división por cero si MaxHealth es configurado erróneamente en 0
            float maxHealth = Health.MaxHealth > 0 ? Health.MaxHealth : 100f;
            float value = Mathf.InverseLerp(0, maxHealth, currentValue);

            fillImage.fillAmount = value;
        }

        void UpdateVisibility()
        {
            if (fillImage == null || canvas == null) return;

            float value = fillImage.fillAmount;

            if (canvas != null)
            {
                // Ocultar si está vacía
                if (Mathf.Approximately(value, 0))
                {
                    if (hideEmpty && canvas.gameObject.activeSelf)
                    {
                        canvas.gameObject.SetActive(false);
                    }
                }
                // Asegurar que el canvas esté activo si la vida es mayor a 0
                else if (value > 0 && canvas.gameObject.activeSelf == false)
                {
                    canvas.gameObject.SetActive(true);
                }
            }
        }
    }
}