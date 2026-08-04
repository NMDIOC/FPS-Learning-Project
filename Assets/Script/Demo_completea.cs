using UnityEngine;

public class Demo_completa : MonoBehaviour
{
    [SerializeField] private GameObject objetoFinal;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        int final = PlayerPrefs.GetInt("Final", 0);
        Debug.Log("Final guardado: " + final);

        if (objetoFinal != null)
            objetoFinal.SetActive(final == 1);
    }
}