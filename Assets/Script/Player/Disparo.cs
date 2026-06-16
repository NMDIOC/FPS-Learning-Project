using UnityEngine;

public class Disparo : MonoBehaviour
{
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

    [Header("Sonido")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float pitchBase = 1f;
    [SerializeField] private float variacionPitch = 0.07f;
    [SerializeField] private float variacionVolumen = 0.05f;

    // Estado interno
    private GunInfo gunActual;
    private Transform spawnPoint;
    private int gunIndex = 1;
    private int balas;
    private int balasMaximas;
    private float tiempoUltimoDisparo;
    private bool recargando = false;
    private int balasArma1;
    private int balasArma2;

    void Start()
    {
        balasArma1 = gunInfo1.maxAmmo;
        balasArma2 = gunInfo2.maxAmmo;
        StartCoroutine(CambiarArma(1));
    }

    void Update()
    {
        // Cambio de arma
        if (!recargando && Input.GetKeyDown(KeyCode.Alpha1) && gunIndex != 1)
            StartCoroutine(CambiarArma(1));

        if (!recargando && Input.GetKeyDown(KeyCode.Alpha2) && gunIndex != 2)
            StartCoroutine(CambiarArma(2));

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

    System.Collections.IEnumerator CambiarArma(int nuevoIndex)
    {
        recargando = true;

        if (gunIndex == 1) balasArma1 = balas;
        else if (gunIndex == 2) balasArma2 = balas;

        if (animator != null)
        {
            if (nuevoIndex == 1)
                animator.SetTrigger("Change1");
            else if (nuevoIndex == 2)
                animator.SetTrigger("Change2");
        }

        bool esArma1 = nuevoIndex == 1;
        gunActual    = esArma1 ? gunInfo1 : gunInfo2;
        spawnPoint   = esArma1 ? spawnPoint1 : spawnPoint2;
        balasMaximas = gunActual.maxAmmo;
        balas = esArma1 ? balasArma1 : balasArma2;

        yield return new WaitForSeconds(0.25f);

        gunIndex = nuevoIndex;
        modeloArma1.SetActive(esArma1);
        modeloArma2.SetActive(!esArma1);

        recargando = false;
    }

    void Disparar()
    {
        tiempoUltimoDisparo = Time.time;
        balas--;

        ReproducirSonido(gunActual.sonidoDisparo);

        GameObject bala = Instantiate(bulletPrefab, spawnPoint.position, spawnPoint.rotation);
        
        // --- INYECCIÓN DE DAÑO (ÚNICAS LÍNEAS NUEVAS) ---
        if (bala.TryGetComponent<Bala>(out Bala scriptBala))
        {
            scriptBala.SetDano(gunActual.daño);
        }
        // -----------------------------------------------

        Rigidbody rb = bala.GetComponent<Rigidbody>();

        if (rb != null)
            rb.velocity = spawnPoint.forward * velocidad;

        Destroy(bala, 3.5f);
    }

    System.Collections.IEnumerator Recargar()
    {
        recargando = true;

        if (animator != null)
        {
            if (gunIndex == 1)
                animator.SetTrigger("Reload1");
            else if (gunIndex == 2)
                animator.SetTrigger("Reload2");
        }

        ReproducirSonido(gunActual.sonidoRecarga);

        yield return new WaitForSeconds(gunActual.reloadTime);

        balas = balasMaximas;

        if (gunIndex == 1) balasArma1 = balas;
        else if (gunIndex == 2) balasArma2 = balas;

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