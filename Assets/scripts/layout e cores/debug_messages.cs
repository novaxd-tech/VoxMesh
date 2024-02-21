using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class debug_messages : MonoBehaviour
{
    public TextMeshProUGUI mesh_chamado;
    public TextMeshProUGUI mesh_escutando;
    public TextMeshProUGUI input_recebido;

    private debug_messages instance;
    private void Start()
    {
        instance = this;
    }

    public void meshChamado(bool value)
    {
        mesh_chamado.text = value == true ? "MESH_CHAMADO: ✓" : "MESH_CHAMADO: ";
    }

    public void meshEscutando(bool value) 
    {
        mesh_escutando.text = value == true ? "MESH_ESCUTANDO: ✓" : "MESH_ESCUTANDO: ";
    }

    public void voiceInput(string text)
    {
        input_recebido.text = $"INPUT_RECEBIDO:\n\n{text}"; 
    }

}
