using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;
using Unity.VisualScripting;
using System;

public class speechRecognition : MonoBehaviour
{
    private string[] nome_ia = new string[1];
    private KeywordRecognizer keyword_recognizer;
    private DictationRecognizer dictation_recognizer;
    
    public static List<string> respostas = new List<string>();

    public TextMeshProUGUI text;
    public tts text_to_speech;
 
    [SerializeField]
    TMP_InputField campo_nome;

    [SerializeField]
    bool meshChamado;

    debug_messages dbg;

    

    void Start()
    {
        //addNewName(campo_nome.text);
        //createsNewKeywordRecognizer();
        dbg = FindObjectOfType<debug_messages>();
    }

    private void OnDestroy()
    {
        if (dictation_recognizer == null) return;
        dictation_recognizer.Stop();
        dictation_recognizer.Dispose();
        
    }

    public void addResponse(string response)
    {
        respostas.Add(response);
    }

    private void meshCalled(PhraseRecognizedEventArgs args)
    {

        //tts resposta
        System.Random rnd = new System.Random();
        print(respostas.Count);
        string resposta = respostas[rnd.Next(respostas.Count)];
        text_to_speech.fala(resposta);

        keyword_recognizer.Stop();
        keyword_recognizer.Dispose();
        meshChamado = true;
        dbg.meshChamado(true);

        StartCoroutine("desativaMesh");
        
        // Inicia o dictation recognizer e responde uma das respostas
        System.Random num_aleatorio = new System.Random();
        int index = num_aleatorio.Next(respostas.Count);
        if(index > 0) text_to_speech.fala(respostas[index]);
        

        PhraseRecognitionSystem.Shutdown();
        
        dictation_recognizer = new DictationRecognizer();
        dictation_recognizer.DictationResult += OnDictationResult;
        dictation_recognizer.DictationHypothesis += OnDictationHypothesis;
        dictation_recognizer.DictationComplete += OnDictationComplete;
        dictation_recognizer.DictationError += OnDictationError;
        dictation_recognizer.Start();
        dbg.meshEscutando(true);
    }

    

    private void OnDictationComplete(DictationCompletionCause cause)
    {
        if (cause != DictationCompletionCause.Complete)
        {
            Debug.LogError("Dictation completed unsuccessfully: " + cause.ToString());
        }
        else
        {
            Debug.Log("dictation completed successfully!");
        }
    }

    private void OnDictationError(string error, int hresult)
    {
        Debug.LogError("Dictation error: " + error + "\nHRESULT: " + hresult);
    }

    private void OnDictationHypothesis(string text)
    {
        //Debug.Log("dictation hypothesis: " + text);
    }

    private void OnDictationResult(string voice_command, ConfidenceLevel confidence)
    {
        if (confidence == ConfidenceLevel.High || confidence == ConfidenceLevel.Medium)
        {
            if (!string.IsNullOrEmpty(MyListener.id))
            {
                netHandler.sendMessage($"plea,{voice_command}");
                dbg.voiceInput(voice_command);
                disposeDictationRecognizer();
            }
        }
            
    }

    private void disposeDictationRecognizer()
    {
        meshChamado = false;
        dictation_recognizer.Stop();
        dictation_recognizer.Dispose();
        dbg.meshChamado(false);
        dbg.meshEscutando(false);
        dbg.voiceInput("");
        createsNewKeywordRecognizer();
        
    }
    
    public void addNewName(string new_name)
    {
        //reseta o array com o nome
        nome_ia = new string[1];
        nome_ia[0] = new_name;
        createsNewKeywordRecognizer();
    }

    private void createsNewKeywordRecognizer()
    {
        if(dictation_recognizer != null)
        {
            keyword_recognizer.Stop();
            keyword_recognizer.Dispose();
        }

        if (string.IsNullOrEmpty(nome_ia[0]))
        {
            Debug.LogError("N�o foi poss�vel criar o objeto KeywordRecognizer: Nome indefinido");
            return;
        }

        keyword_recognizer = new KeywordRecognizer(nome_ia);
        keyword_recognizer.OnPhraseRecognized += meshCalled;
        keyword_recognizer.Start(); 
    }

    private IEnumerator desativaMesh()
    {
        yield return new WaitForSeconds(5);
        disposeDictationRecognizer();
    }
}
