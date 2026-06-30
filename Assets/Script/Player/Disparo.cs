using UnityEngine;

public class Disparo : MonoBehaviour
{
    // ENUM: Control de armas limpio sin usar números sueltos
    private enum TipoArma { Ninguna, Arma1, Arma2 }

    [Header("Armas en Escena")]
    [SerializeField] private GameObject modeloArma1;
    [SerializeField] private GameObject modeloArma2;
    [SerializeField] private Transform spawnPoint1;
    [SerializeField] private Transform spawnPoint2;
    [SerializeField] private Animator animator;

    [Header("Info Armas")]
    [SerializeField] private GunInfo gunInfo1;
    [SerializeField] private GunInfo gunInfo2;

    [Header("Bala")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float velocidad = 150f;
    [SerializeField] private float rangoMaximoRaycast = 100f;

    [Header("Sonido")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float pitchBase = 1f;
    [SerializeField] private float variacionPitch = 0.07f;
    [SerializeField] private float variacionVolumen = 0.05f;

    public int CurrentAmmo => balas;
    public int MaxAmmo => balasMaximas;

    // Estado interno
    private TipoArma armaActualID = TipoArma.Ninguna;
    private GunInfo gunActual;
    private Transform spawnPoint;
    
    private int balas;
    private int balasMaximas;
    private float tiempoUltimoDisparo;
    private bool recargando = false;
    private int balasArma1;
    private int balasArma2;

    // OPTIMIZACIÓN: Convertimos los triggers del Animator a Hashes numéricos para evitar lag por strings
    private static readonly int HashChange1 = Animator.StringToHash("Change1");
    private static readonly int HashChange2 = Animator.StringToHash("Change2");
    private static readonly int HashReload1 = Animator.StringToHash("Reload1");
    private static readonly int HashReload2 = Animator.StringToHash("Reload2");

    void Start()
    {
        balasArma1 = gunInfo1.maxAmmo;
        balasArma2 = gunInfo2.maxAmmo;
        
        // Iniciamos equipando la primera arma usando el Enum
        StartCoroutine(CambiarArma(TipoArma.Arma1));
    }

    void Update()
    {
        // SWITCH: Cambio de armas optimizado
        if (!recargando)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) && armaActualID != TipoArma.Arma1)
                StartCoroutine(CambiarArma(TipoArma.Arma1));

            if (Input.GetKeyDown(KeyCode.Alpha2) && armaActualID != TipoArma.Arma2)
                StartCoroutine(CambiarArma(TipoArma.Arma2));
        }

        if (Movimiento.cursorDesbloqueado) return;

        // Recarga
        if (!recargando && (Input.GetKeyDown(KeyCode.R) || balas == 0))
        {
            StartCoroutine(Recargar());
            return;
        }

        // Disparo
        if (!recargando && balas > 0 && Input.GetMouseButton(0) && Time.time >= tiempoUltimoDisparo + gunActual.fireRate)
            Disparar();
    }

    System.Collections.IEnumerator CambiarArma(TipoArma nuevaArma)
    {
        recargando = true;

        // Guardamos las balas del arma anterior usando Switch
        switch (armaActualID)
        {
            case TipoArma.Arma1: balasArma1 = balas; break;
            case TipoArma.Arma2: balasArma2 = balas; break;
        }

        // Activamos la animación correspondiente con los Hashes optimizados
        if (animator != null)
        {
            switch (nuevaArma)
            {
                case TipoArma.Arma1: animator.SetTrigger(HashChange1); break;
                case TipoArma.Arma2: animator.SetTrigger(HashChange2); break;
            }
        }

        bool esArma1 = (nuevaArma == TipoArma.Arma1);
        gunActual    = esArma1 ? gunInfo1 : gunInfo2;
        spawnPoint   = esArma1 ? spawnPoint1 : spawnPoint2;
        balasMaximas = gunActual.maxAmmo;
        balas        = esArma1 ? balasArma1 : balasArma2;

        yield return new WaitForSeconds(0.25f);

        armaActualID = nuevaArma;
        modeloArma1.SetActive(esArma1);
        modeloArma2.SetActive(!esArma1);

        recargando = false;
    }

    void Disparar()
    {
        tiempoUltimoDisparo = Time.time;
        balas--;

        ReproducirSonido(gunActual.sonidoDisparo);

        // 1. INSTANCIAR LA BALA FÍSICA (Tu lógica original)
        GameObject bala = Instantiate(bulletPrefab, spawnPoint.position, spawnPoint.rotation);
        
        // --- INYECCIÓN DE DAÑO ORIGINAL ---
        if (bala.TryGetComponent<Bala>(out Bala scriptBala))
        {
            scriptBala.SetDano(gunActual.daño);
        }
        // ----------------------------------

        Rigidbody rb = bala.GetComponent<Rigidbody>();

        if (rb != null)
        {
            // 2. RAYCAST GLOBAL DESDE EL SPAWNPOINT (Sin capas molestas)
            // Lanzamos el rayo hacia adelante desde la posición del cañón de tu arma actual
            if (Physics.Raycast(spawnPoint.position, spawnPoint.forward, out RaycastHit hit, rangoMaximoRaycast))
            {
                // Si el rayo choca con algo (suelo, pared o enemigo), calculamos la dirección hacia ese punto exacto
                Vector3 direccionHaciaImpacto = (hit.point - spawnPoint.position).normalized;
                rb.velocity = direccionHaciaImpacto * velocidad;
            }
            else
            {
                // Si no choca con nada en su rango máximo, la bala viaja recto de manera convencional
                rb.velocity = spawnPoint.forward * velocidad;
            }
        }

        Destroy(bala, 3.5f);
    }

    System.Collections.IEnumerator Recargar()
    {
        recargando = true;

        // SWITCH: Animación de recarga según el arma equipada
        if (animator != null)
        {
            switch (armaActualID)
            {
                case TipoArma.Arma1: animator.SetTrigger(HashReload1); break;
                case TipoArma.Arma2: animator.SetTrigger(HashReload2); break;
            }
        }

        ReproducirSonido(gunActual.sonidoRecarga);

        yield return new WaitForSeconds(gunActual.reloadTime);

        balas = balasMaximas;

        switch (armaActualID)
        {
            case TipoArma.Arma1: balasArma1 = balas; break;
            case TipoArma.Arma2: balasArma2 = balas; break;
        }

        recargando = false;
    }

    void ReproducirSonido(AudioClip clip)
    {
        if (audioSource == null || clip == null) return;

        audioSource.pitch  = pitchBase + Random.Range(-variacionPitch, variacionPitch);
        audioSource.volume = 1f        + Random.Range(-variacionVolumen, variacionVolumen);
        audioSource.PlayOneShot(clip);
    }
}