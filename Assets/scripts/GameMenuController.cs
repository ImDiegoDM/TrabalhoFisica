using UnityEngine;
using System.Collections;

public class GameMenuController : MonoBehaviour {

    public GameObject pausePanel;

    private bool pause;

    #region Unity Methods
    void Start()
    {
        pause = false;
    }

    void FixedUpdate()
    {
        if(Input.GetButtonDown("Pause"))
        {
            pause = !pause;

            if(pause)
                pausePanel.SetActive(true);
            else
                pausePanel.SetActive(false);
        }
    }
    #endregion

    #region Botoes
    public void BtnContinuar()
    {
        pause = false;
        pausePanel.SetActive(false);
    }

    public void BtnReiniciar()
    {
    }

    public void BtnSelecaoAviao()
    {
        Persistent.singleton.callSelecaoAviao = true;
        Persistent.singleton.level = 1;
        Persistent.singleton.CallLevel();
        //Application.LoadLevel("Menu");

    }

    public void BtnMenuPrincipal()
    {
        Persistent.singleton.level = 1;
        Persistent.singleton.CallLevel();
        //Application.LoadLevel("Menu");
    }
    #endregion
}
