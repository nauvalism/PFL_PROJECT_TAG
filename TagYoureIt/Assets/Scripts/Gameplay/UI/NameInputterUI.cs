using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NameInputterUI : MonoBehaviour
{
    [SerializeField] TMP_InputField nameInputter;
    [SerializeField] TextMeshProUGUI inputterPlaceholder;
    [SerializeField] Button submitBtn;
    [SerializeField] CanvasGroup nameInputterCG;


    public void SetPlayer()
    {
        if(nameInputter.text.Length == 0)
        {
            nameInputter.text = "";
            inputterPlaceholder.text = "Cannot be Empty";
            return;
        }
        else if(nameInputter.text.Length < 5)
        {
            nameInputter.text = "";
            inputterPlaceholder.text = "Must more than 5 characters";
            return;
            
        }
        
        StartCoroutine(Setup());
        IEnumerator Setup()
        {
            LeanTween.value(nameInputterCG.gameObject, 1.0f, .0f, .250f).setOnUpdate((float f)=>{
                nameInputterCG.alpha = f;
                nameInputterCG.interactable = false;
                nameInputterCG.blocksRaycasts = false;
            });
            yield return new WaitForSeconds(.250f);
            nameInputter.interactable = false;
            submitBtn.interactable = false;
            string content = nameInputter.text;
            yield return GameplayController.instance.SetLocalIDPlayer(content);
            yield return new WaitForEndOfFrame();
            PhotonController.instance.TryingToConnect();
        }

        
    }
}
