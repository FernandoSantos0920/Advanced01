using UnityEngine;

[CreateAssetMenu(fileName = "RoboData", menuName = "Guerra de Robos/Robo Data")]
public class RoboData : ScriptableObject
{
    [Header("Identificação")]
    public string roboName;

    [Header("Movimentação")]
    public float velocidadeInicial = 2f;
    public float velocidadeSprint = 5.335f;

    [Header("Atributos")]
    public float tamanho = 1f;
    public float forcaEmpurrao = 10f;

    [Header("Cores")]
    public Color corPrincipal = Color.white;
    public Color corSecundaria = Color.gray;
}