using UnityEngine;

public class Mira : MonoBehaviour
{
    [SerializeField] private Color color = Color.white;
    [SerializeField] private float tamano = 10f;
    [SerializeField] private float grosor = 2f;
    [SerializeField] private float espacio = 4f;  // hueco en el centro

    private Texture2D pixel;

    void Start()
    {
        pixel = new Texture2D(1, 1);
        pixel.SetPixel(0, 0, Color.white);
        pixel.Apply();
    }

    void OnGUI()
    {
        GUI.color = color;

        float cx = Screen.width  / 2f;
        float cy = Screen.height / 2f;

        // Línea izquierda
        GUI.DrawTexture(new Rect(cx - tamano - espacio, cy - grosor / 2f, tamano, grosor), pixel);
        // Línea derecha
        GUI.DrawTexture(new Rect(cx + espacio,          cy - grosor / 2f, tamano, grosor), pixel);
        // Línea arriba
        GUI.DrawTexture(new Rect(cx - grosor / 2f, cy - tamano - espacio, grosor, tamano), pixel);
        // Línea abajo
        GUI.DrawTexture(new Rect(cx - grosor / 2f, cy + espacio,          grosor, tamano), pixel);
    }
}