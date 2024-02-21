using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SwitchBetween : MonoBehaviour
{
    [SerializeField] int index;
    [SerializeField] float delay = .50f;
    [SerializeField] List<Sprite> alternates;
    [SerializeField] Image img;
    [SerializeField] SpriteRenderer sr;

    private void Start() {
        StartAlternate();
    }

    public void StartAlternate()
    {
        StartCoroutine(StartingAlternate());
        IEnumerator StartingAlternate()
        {
            while(true)
            {
                if(img == null)
                {
                    sr.sprite = alternates[index];
                }
                else
                {
                    img.sprite = alternates[index];
                }
                index++;
                if(index >= alternates.Count)
                {
                    index = 0;
                }
                yield return new WaitForSeconds(delay);
            }
        }
    }

    public void StopAlternate()
    {
        StopCoroutine("StartingAlternate");
    }
}
