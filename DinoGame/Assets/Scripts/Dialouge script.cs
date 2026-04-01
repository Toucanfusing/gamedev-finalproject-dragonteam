using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class Dialougescript : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public string[] lines;
    public float textSpeed;
    private int index;
    private InputSystem_Actions ctrl;


    
    
    
    
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textComponent.text = string.Empty;
        StartDialouge();
        ctrl = new InputSystem_Actions();
        ctrl.Enable();
        ctrl.Player.click.started += NextLine;
    }

    // Update is called once per frame
    void Update()
    {
    }
    
    
    void StartDialouge()
    {
    index = 0;
    StartCoroutine(TypeLine());
    }

     
    IEnumerator TypeLine()
    {
        foreach(char c in lines[index].ToCharArray())  
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed); 
        }
        
    }
    void NextLine(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (index < lines.Length - 1)
        {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}