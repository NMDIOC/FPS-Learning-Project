using UnityEngine;

public class Movimiento : MonoBehaviour
{
    [Header("Componentes")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform camara;
    [SerializeField] private CapsuleCollider colisionador;

    [Header("Configuración de Movimiento")]
    [SerializeField] private float velocidadNormal = 5f;
    [SerializeField] private float velocidadAgachado = 2.5f;
    [SerializeField] private float velocidadCarrera = 9f;
    private float velocidadActual;

    [Header("Configuración de Agachado")]
    [SerializeField] private float alturaNormal = 2f;
    [SerializeField] private float alturaAgachado = 1f;
    [SerializeField] private float alturaCamaraNormal = 0.8f;
    [SerializeField] private float alturaCamaraAgachado = 0.3f;
    [SerializeField] private float velocidadTransicion = 10f;

    [Header("Configuración del Ratón")]
    [SerializeField] private float sensibilidadRaton = 2f;
    [SerializeField] private float limiteVerticalMin = -85f;
    [SerializeField] private float limiteVerticalMax = 85f;

    [Header("Sonidos de Pasos — Normal")]
    [SerializeField] private AudioClip sonidoPaso;
    [SerializeField] private AudioClip sonidoPasoAgachado;

    [Header("Sonidos de Pasos — Metal (tag Base)")]
    [SerializeField] private AudioClip sonidoPasoMetal;
    [SerializeField] private AudioClip sonidoPasoMetalAgachado;

    [Header("Configuración de Pasos")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float tiempoEntrePasos = 0.5f;
    [SerializeField] private float tiempoEntrePasosAgachado = 0.7f;
    [SerializeField] private float tiempoEntrePasosCarrera = 0.3f;
    [SerializeField] private float pitchBase = 1f;
    [SerializeField] private float variacionPitch = 0.1f;
    [SerializeField] private float variacionVolumen = 0.05f;
    [SerializeField] private float volumenCarrera = 1.4f;

    private float rotacionX = 0f;
    private Vector3 entradasMovimiento;
    private bool agachado = false;
    private bool corriendo = false;
    private float tiempoSiguientePaso = 0f;
    private bool enSuperficieMetal = false;
    public static bool cursorDesbloqueado = false;
    public bool EstaAgachado  => agachado;
    public bool EstaCorriendo => corriendo;

    void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        if (colisionador == null) colisionador = GetComponent<CapsuleCollider>();

        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        velocidadActual = velocidadNormal;
    }

    void Update()
    {
        float moverX = Input.GetAxisRaw("Horizontal");
        float moverZ = Input.GetAxisRaw("Vertical");
        entradasMovimiento = new Vector3(moverX, 0f, moverZ).normalized;

        float ratonX = Input.GetAxis("Mouse X") * sensibilidadRaton;
        float ratonY = Input.GetAxis("Mouse Y") * sensibilidadRaton;

        if (camara != null)
        camara.localRotation = Quaternion.Euler(rotacionX, 0f, 0f);

        // Agachado — separado del cursor
        if (Input.GetKey(KeyCode.C) && !corriendo)
            agachado = true;
        else if (!Input.GetKey(KeyCode.C) && !ObjetoSobreLaCabeza())
            agachado = false;

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            cursorDesbloqueado = !cursorDesbloqueado;

            if (cursorDesbloqueado)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            // Actualizar volumen respetando el botón mute
            BotonMute.ActualizarVolumen();
        }

        // Bloquear movimiento de cámara cuando el cursor está desbloqueado
        if (!cursorDesbloqueado)
        {
            transform.Rotate(Vector3.up * ratonX);
            rotacionX -= ratonY;
            rotacionX = Mathf.Clamp(rotacionX, limiteVerticalMin, limiteVerticalMax);

            if (camara != null)
                camara.localRotation = Quaternion.Euler(rotacionX, 0f, 0f);
        }

        corriendo = Input.GetKey(KeyCode.LeftShift) && !agachado && entradasMovimiento.magnitude > 0;

        ProcesarAgachado();
        ProcesarPasos();
    }

    void FixedUpdate()
    {
        // Si el cursor está desbloqueado, el personaje no se mueve
        if (cursorDesbloqueado)
        {
            rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
            return;
        }

        Vector3 direccionMovimiento = transform.TransformDirection(entradasMovimiento);
        Vector3 velocidadObjetivo = direccionMovimiento * velocidadActual;
        velocidadObjetivo.y = rb.velocity.y;
        rb.velocity = velocidadObjetivo;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Base"))
            enSuperficieMetal = !enSuperficieMetal;
    }

    void OnTriggerExit(Collider other) { }

    private void ProcesarPasos()
    {
        bool caminando = entradasMovimiento.magnitude > 0;
        if (!caminando || audioSource == null) return;

        float intervalo;
        if (corriendo)     intervalo = tiempoEntrePasosCarrera;
        else if (agachado) intervalo = tiempoEntrePasosAgachado;
        else               intervalo = tiempoEntrePasos;

        if (Time.time >= tiempoSiguientePaso)
        {
            ReproducirPaso();
            tiempoSiguientePaso = Time.time + intervalo;
        }
    }

    private void ReproducirPaso()
    {
        AudioClip clip;

        if (enSuperficieMetal)
            clip = agachado ? sonidoPasoMetalAgachado : sonidoPasoMetal;
        else
            clip = agachado ? sonidoPasoAgachado : sonidoPaso;

        if (clip == null) return;

        audioSource.pitch = pitchBase + Random.Range(-variacionPitch, variacionPitch);

        float volumenBase = corriendo ? volumenCarrera : 1f;
        audioSource.volume = volumenBase + Random.Range(-variacionVolumen, variacionVolumen);

        audioSource.PlayOneShot(clip);
    }

    private void ProcesarAgachado()
    {
        float alturaObjetivo;
        float alturaCamaraObjetivo;

        if (agachado)
        {
            alturaObjetivo       = alturaAgachado;
            alturaCamaraObjetivo = alturaCamaraAgachado;
            velocidadActual      = velocidadAgachado;
        }
        else if (corriendo)
        {
            alturaObjetivo       = alturaNormal;
            alturaCamaraObjetivo = alturaCamaraNormal;
            velocidadActual      = velocidadCarrera;
        }
        else
        {
            alturaObjetivo       = alturaNormal;
            alturaCamaraObjetivo = alturaCamaraNormal;
            velocidadActual      = velocidadNormal;
        }

        colisionador.height = Mathf.Lerp(colisionador.height, alturaObjetivo, Time.deltaTime * velocidadTransicion);

        if (camara != null)
        {
            Vector3 posicionCamaraLocal = camara.localPosition;
            posicionCamaraLocal.y = Mathf.Lerp(posicionCamaraLocal.y, alturaCamaraObjetivo, Time.deltaTime * velocidadTransicion);
            camara.localPosition = posicionCamaraLocal;
        }
    }

    private bool ObjetoSobreLaCabeza()
    {
        float radio = colisionador.radius * 0.9f;
        Vector3 origen = transform.position + Vector3.up * (colisionador.height * 0.5f);
        return Physics.SphereCast(origen, radio, Vector3.up, out RaycastHit hit, 0.5f);
    }
}