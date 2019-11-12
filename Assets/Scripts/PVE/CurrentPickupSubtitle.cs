using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Current the subtitle of current pick up item
/// </summary>
public class CurrentPickupSubtitle : MonoBehaviour
{

    Animator anim;
    Queue contentList = new Queue();
    struct InfoBlock
    {
        public string category;
        public string count;
    }
    Text text;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if(contentList.Count != 0 && anim.GetCurrentAnimatorStateInfo(0).IsName("Default"))
        {
            ShowPickUpContent();
        }
    }

    public void AddContent(string category, string count)
    {
        InfoBlock infoBlock = new InfoBlock();
        infoBlock.category = category;
        infoBlock.count = count;
        contentList.Enqueue(infoBlock);
    }

    void ShowPickUpContent()
    {
        InfoBlock tmp = (InfoBlock)contentList.Dequeue();
        text.text = "Get " + tmp.category + " X " + tmp.count;
        anim.SetTrigger("AppearContent");
    }
}
