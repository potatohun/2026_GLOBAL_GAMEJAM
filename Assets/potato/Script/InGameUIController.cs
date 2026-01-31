using UnityEngine;

public class InGameUIController : MonoBehaviour
{
    public void OnClickNextButton()
    {
        MaskCreateManager.instance.Next();
    }
}
