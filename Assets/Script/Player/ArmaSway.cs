using UnityEngine;

public class ArmaSway : MonoBehaviour
{
    [Header("Sway (retraso de cámara)")]
    [SerializeField] private float intensidadSway = 0.05f;
    [SerializeField] private float suavidadSway = 6f;

    [Header("Balanceo al caminar")]
    [SerializeField] private float intensidadBalanceo = 0.015f;
    [SerializeField] private float velocidadBalanceo = 10f;
    [SerializeField] private float suavidadBalanceo = 8f;

    [Header("Inclinación al mirar")]
    [SerializeField] private float intensidadInclinacion = 2f;
    [SerializeField] private float suavidadInclinacion = 6f;

    private Vector3 posicionOriginal;
    private Quaternion rotacionOriginal;
    private float timerBalanceo;

    void Start()
    {
        posicionOriginal = transform.localPosition;
        rotacionOriginal = transform.localRotation;
    }

    void Update()
    {
        if (Movimiento.cursorDesbloqueado) return;
        
        AplicarSway();
        AplicarBalanceo();
        AplicarInclinacion();
    }

    void AplicarSway()
    {
        // Leer movimiento del ratón
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // El arma se desplaza levemente al opuesto del movimiento del ratón
        Vector3 swayObjetivo = new Vector3(
            -mouseX * intensidadSway,
            -mouseY * intensidadSway,
            0f
        );

        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            posicionOriginal + swayObjetivo,
            Time.deltaTime * suavidadSway
        );
    }

    void AplicarBalanceo()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        bool caminando = moveX != 0 || moveZ != 0;

        if (caminando)
        {
            timerBalanceo += Time.deltaTime * velocidadBalanceo;

            // Movimiento de bob: seno para Y, coseno para X
            float bobX = Mathf.Cos(timerBalanceo * 0.5f) * intensidadBalanceo;
            float bobY = Mathf.Abs(Mathf.Sin(timerBalanceo)) * intensidadBalanceo;

            Vector3 bobObjetivo = posicionOriginal + new Vector3(bobX, bobY, 0f);
            transform.localPosition = Vector3.Lerp(transform.localPosition, bobObjetivo, Time.deltaTime * suavidadBalanceo);
        }
        else
        {
            // Volver suavemente al origen cuando está quieto
            timerBalanceo = 0f;
            transform.localPosition = Vector3.Lerp(transform.localPosition, posicionOriginal, Time.deltaTime * suavidadBalanceo);
        }
    }

    void AplicarInclinacion()
    {
        float mouseX = Input.GetAxis("Mouse X");

        // Inclinar el arma levemente en Z al girar
        Quaternion inclinacionObjetivo = rotacionOriginal * Quaternion.Euler(0f, 0f, -mouseX * intensidadInclinacion);

        transform.localRotation = Quaternion.Lerp(
            transform.localRotation,
            inclinacionObjetivo,
            Time.deltaTime * suavidadInclinacion
        );
    }
}