using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ScoretText : MonoBehaviour
{
    public TextMeshProUGUI text;
    public Animator animator;
    public void Highlight() {
        animator.SetTrigger("highlight");
    }
    
    public void setScore(int value) 
    {
        text.text = value.ToString();
    }
}
