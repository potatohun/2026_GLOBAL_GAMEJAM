using UnityEngine;

public class CutSceneManager : MonoBehaviour
{
    public Animator anim;
    public string anim_string;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void StartCutScene()
    {
        anim.Play(anim_string);
    }

    public void OnEndCutScene()
    {
        GameState.i.SetState(GAME.START);
    }

    
}
