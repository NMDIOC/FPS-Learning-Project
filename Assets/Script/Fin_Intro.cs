using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Fin_Intro : MonoBehaviour
{
    [SerializeField] private string nombreDeLaEscena;

    void Start()
    {
        StartCoroutine(EsperarYEjecutar());
    }

    IEnumerator EsperarYEjecutar()
    {
        yield return new WaitForSeconds(60f);

        SceneManager.LoadScene(nombreDeLaEscena);
    }
}
