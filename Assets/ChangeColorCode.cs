using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Cambia el color del modelo de Spider-Man de forma aleatoria cada vez que
/// se presiona el boton "Change Color".
///
/// Donde va:   en el ImageTarget (igual que en el tutorial del profe).
/// Model:      arrastrar spiderman_2 (el padre de Cuerpo_Pies_Cabeza y
///             Cara_Piernas_Manos) desde la Hierarchy.
/// Boton:      On Click () -> ImageTarget -> ChangeColorCode.ChangeColor_BTN()
///
/// Como funciona el color:
///   El color elegido se MULTIPLICA con la textura original del traje
///   (roja y azul). Por eso los colores puros y saturados (verde, cian,
///   morado) se notan mucho, y los parecidos al rojo (amarillo, naranja)
///   casi no. El blanco deja la textura tal cual: es el traje original.
/// </summary>
public class ChangeColorCode : MonoBehaviour
{
    // Objeto padre del modelo. El script tomara TODAS sus piezas hijas,
    // asi el cambio de color se aplica parejo a todo el cuerpo.
    public GameObject model;

    // Lista de colores posibles.
    // [ColorUsage(false, true)] habilita el control "Intensity" en el
    // selector de color del Inspector (colores HDR). Subir la intensidad
    // a 1 o 1.5 hace que el color se vea mas brillante sobre la textura.
    // El Element 0 debe quedarse en blanco: es el traje original.
    [ColorUsage(false, true)]
    public Color[] colores = new Color[]
    {
        Color.white,                          // 0: original (FFFFFF)
        new Color(0f,   1f,   0f),            // 1: verde intenso (00FF00)
        new Color(0f,   1f,   1f),            // 2: cian / turquesa (00FFFF)
        new Color(0.6f, 0f,   1f),            // 3: morado (9900FF)
        new Color(0.23f, 0.23f, 0.23f)        // 4: negro simbionte (3A3A3A)
    };

    // Renderers de las piezas del modelo (se llenan en Start).
    private Renderer[] piezas;

    // Posicion en la lista del color que tiene puesto el modelo ahora.
    // Arranca en 0 porque el modelo inicia con el traje original.
    private int colorActual = 0;

    // Start is called before the first frame update
    void Start()
    {
        // Busca todos los Renderer dentro de spiderman_2.
        // En este modelo son dos: Cuerpo_Pies_Cabeza y Cara_Piernas_Manos.
        piezas = model.GetComponentsInChildren<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Metodo publico que se conecta al OnClick del boton.
    public void ChangeColor_BTN()
    {
        // Se necesitan al menos 2 colores para poder cambiar.
        if (colores.Length < 2) return;

        // Elige una posicion al azar de la lista.
        // Si sale el mismo color que ya esta puesto, vuelve a elegir,
        // para que cada clic siempre produzca un cambio visible.
        // Un color SI puede volver a salir mas adelante, solo no dos veces seguidas.
        int nuevoIndice;
        do
        {
            nuevoIndice = Random.Range(0, colores.Length); // de 0 a Length-1
        }
        while (nuevoIndice == colorActual);

        colorActual = nuevoIndice;
        Color nuevo = colores[colorActual];

        // Aplica el color a cada pieza del modelo.
        foreach (Renderer pieza in piezas)
        {
            // pieza.material (y NO sharedMaterial) crea una copia temporal
            // del material al ejecutar. Asi traje_prueba y traje_prueba2
            // en la carpeta Assets nunca se modifican.
            Material mat = pieza.material;

            // El modelo usa el shader glTF/PbrMetallicRoughness, que nombra
            // su color base distinto al shader Standard del ejemplo del profe.
            // Se revisa cual propiedad existe y se usa esa.
            if (mat.HasProperty("baseColorFactor"))
                mat.SetColor("baseColorFactor", nuevo);   // shader glTF
            else if (mat.HasProperty("_BaseColor"))
                mat.SetColor("_BaseColor", nuevo);        // shaders URP
            else
                mat.color = nuevo;                        // shader Standard (_Color)
        }
    }
}