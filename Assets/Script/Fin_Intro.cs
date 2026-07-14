using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Requerido para acceder al componente Image

public class Fin_Intro : MonoBehaviour
{
    [SerializeField] private string nombreDeLaEscena;
    [SerializeField] private Image logoImage; // Arrastra aquí tu objeto con la imagen
    [SerializeField] private float tiempoDeEspera = 55f;
    [SerializeField] private float velocidadFade = 1f;

    void Start()
    {
        // 1. Aseguramos que empiece invisible (Alpha = 0)
        Color colorInicial = logoImage.color;
        colorInicial.a = 0f;
        logoImage.color = colorInicial;

        StartCoroutine(SecuenciaFade());
    }

    IEnumerator SecuenciaFade()
    {
        yield return new WaitForSeconds(tiempoDeEspera);

        // 2. Aumentar el Alpha progresivamente
        Color colorActual = logoImage.color;
        
        while (colorActual.a < 1f)
        {
            colorActual.a += Time.deltaTime * velocidadFade;
            logoImage.color = colorActual;
            yield return null; // Espera al siguiente frame
        }

        // Asegurar que termine en opaco total
        colorActual.a = 1f;
        logoImage.color = colorActual;

        // Opcional: Esperar un poco antes de cambiar de escena
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene(nombreDeLaEscena);
    }
}