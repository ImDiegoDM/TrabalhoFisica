using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum MenuState{DEFAULT, MENU, CREDITOS, SAIR, JOGAR}
public enum MoveAirplane {DEFAULT, LEFT, RIGHT, RIGHTLEFT, LEFTRIGHT}

public class MenuController : MonoBehaviour {
    
    public GameObject menuPanel;
    public GameObject creditosPanel;
    public GameObject selecaoPanel;
    public GameObject sairPanel;
   // public GameObject logo;

    private Animation menuAnimation;
    private Animation creditosAnimation;
    private Animation selecaoAnimation;
    private Animation sairAnimation;

    public GameObject btnJogar;
    public GameObject btnJCreditos;
    public GameObject btnSair;

    private MenuState menuState;

    //Aviao
    private List<GameObject> airplaneList;
    private int indexAirplane;
    private MoveAirplane moveAirplane;
    public float speed;
    private Vector3 middlePosition;
    private Vector3 startPosition;
    private Vector3 finalPosition;
    private Vector3 backCameraPosition;
    public Vector3 offSetRotation;
    public Vector3 offSetPosition;



    #region UnityMethods
    void Start()
    {

        menuAnimation = menuPanel.GetComponent<Animation>();
        creditosAnimation = creditosPanel.GetComponent<Animation>();
        selecaoAnimation = selecaoPanel.GetComponent<Animation>();
        sairAnimation = sairPanel.GetComponent<Animation>();
        menuState = MenuState.DEFAULT;

		middlePosition = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width/2+ offSetPosition.x, Screen.height/2 + offSetPosition.y, offSetPosition.z));
        startPosition = Camera.main.ScreenToWorldPoint(new Vector3(-500, Screen.height/2 + offSetPosition.y, offSetPosition.z));
        finalPosition = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width + 200, Screen.height/2 + offSetPosition.y , offSetPosition.z));
        backCameraPosition = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width + 200, Screen.height/2, -500));


        indexAirplane = 0;

        moveAirplane = MoveAirplane.DEFAULT;

        airplaneList = new List<GameObject>();

        for (int i = 0; i < Persistent.singleton.airplaneList.Count; i++)
        {
            airplaneList.Add((GameObject)Instantiate(Persistent.singleton.airplaneList[i], backCameraPosition, Quaternion.identity));
            airplaneList[i].transform.Rotate(offSetRotation);
        }


    }
    void Update()
    {
        
        #region Controle de Animacao
        if(!menuAnimation.IsPlaying("Menu_Exit_2") && menuState == MenuState.CREDITOS)
        {
            creditosAnimation.CrossFade("Creditos_Enter_2");

            menuPanel.SetActive(false);
            creditosPanel.SetActive(true);
           
           
            menuState = MenuState.DEFAULT;
        }
        if(!menuAnimation.IsPlaying("Menu_Exit_2") && menuState == MenuState.JOGAR)
        {
            selecaoAnimation.CrossFade("Selecao_Enter");
            // Debug.Log("Selecao_Enter");
            menuPanel.SetActive(false);
            selecaoPanel.SetActive(true);

            menuState = MenuState.DEFAULT;

            airplaneList[indexAirplane].transform.position = startPosition;
            moveAirplane = MoveAirplane.LEFTRIGHT;

        }
        if(!menuAnimation.IsPlaying("Menu_Exit_2") && menuState == MenuState.SAIR)
        {
            sairAnimation.CrossFade("Sair_Enter");
            menuPanel.SetActive(false);
            sairPanel.SetActive(true);

            menuState = MenuState.DEFAULT;

        }
        if(creditosPanel.activeSelf)
        {
            if(!creditosAnimation.IsPlaying("Creditos_Exit_2") && menuState == MenuState.MENU)
            {
                menuAnimation.CrossFade("Menu_Enter_2");

                menuPanel.SetActive(true);
                creditosPanel.SetActive(false);
                menuState = MenuState.DEFAULT;

            }
        }
        if(sairPanel.activeSelf)
        {
            if(!sairAnimation.IsPlaying("Sair_Exit") && menuState == MenuState.MENU)
            {
                menuAnimation.CrossFade("Menu_Enter_2");

                menuPanel.SetActive(true);
                sairPanel.SetActive(false);
                menuState = MenuState.DEFAULT;

            }
        }
        if(selecaoPanel.activeSelf)
        {
            if(!selecaoAnimation.IsPlaying("Selecao_Exit_2") && menuState == MenuState.MENU)
            {
                menuAnimation.CrossFade("Menu_Enter_2");

                menuPanel.SetActive(true);
                selecaoPanel.SetActive(false);
                menuState = MenuState.DEFAULT;
                moveAirplane = MoveAirplane.DEFAULT;

            }
            else if(selecaoAnimation.IsPlaying("Selecao_Exit_2") && menuState == MenuState.MENU)
            {
                if(moveAirplane == MoveAirplane.LEFT)
                {
                    airplaneList[indexAirplane].transform.position = Vector3.MoveTowards(airplaneList[indexAirplane].transform.position,
                        startPosition, speed * Time.deltaTime);


                }

               
            }
        }
        #endregion

        #region Controle de Escolha_de_Avioes
        if(moveAirplane == MoveAirplane.RIGHT  && menuState != MenuState.MENU)
        {
            airplaneList[indexAirplane].transform.position = Vector3.MoveTowards(airplaneList[indexAirplane].transform.position,
                finalPosition, speed * Time.deltaTime);

            if(airplaneList[indexAirplane].transform.position == finalPosition)
            {
                moveAirplane = MoveAirplane.RIGHTLEFT;

                indexAirplane ++;

                if(indexAirplane >= airplaneList.Count)
                    indexAirplane = 0;

                airplaneList[indexAirplane].transform.position = startPosition;
            }


        }

        if(moveAirplane == MoveAirplane.RIGHTLEFT  && menuState != MenuState.MENU)
        {
            airplaneList[indexAirplane].transform.position = Vector3.MoveTowards(airplaneList[indexAirplane].transform.position,
                middlePosition, speed * Time.deltaTime);

            if(airplaneList[indexAirplane].transform.position == middlePosition)
                moveAirplane = MoveAirplane.DEFAULT;
        }

        if(moveAirplane == MoveAirplane.LEFT  && menuState != MenuState.MENU)
        {
            airplaneList[indexAirplane].transform.position = Vector3.MoveTowards(airplaneList[indexAirplane].transform.position,
                startPosition, speed * Time.deltaTime);

            if(airplaneList[indexAirplane].transform.position == startPosition)
            {
                moveAirplane = MoveAirplane.LEFTRIGHT;

                indexAirplane --;

                if(indexAirplane < 0)
                    indexAirplane = airplaneList.Count - 1;

                airplaneList[indexAirplane].transform.position = finalPosition;
            }


        }

        if(moveAirplane == MoveAirplane.LEFTRIGHT  && menuState != MenuState.MENU)
        {
            airplaneList[indexAirplane].transform.position = Vector3.MoveTowards(airplaneList[indexAirplane].transform.position,
                middlePosition, speed * Time.deltaTime);

            if(airplaneList[indexAirplane].transform.position == middlePosition)
                moveAirplane = MoveAirplane.DEFAULT;
        }
        #endregion

    }
    #endregion

    #region Menu
    void EnterMenu()
    {
        
        menuAnimation.CrossFade("Menu_Enter_2");
    }

    void ExitMenu()
    {
        menuAnimation.CrossFade("Menu_Exit_2");
       
    }
    #endregion

    #region Btns
    public void BtnSairSim()
    {
        Application.Quit();

    }
    public void BtnSairNao()
    {
        sairPanel.SetActive(false);
        SoundClick();

    }
    public void BtnSair()
    {
        sairPanel.SetActive(true);
        SoundClick();

    }

    public void BtnCreditos()
    {
        menuState = MenuState.CREDITOS;
        ExitMenu();
        SoundClick();

    }

    public void BtnSelecionar()
    {
        SoundClick();
       
        Persistent.singleton.indexAirplaneSelect = indexAirplane;
       
        Persistent.singleton.CallLevel();
        Persistent.singleton.level = 2;

    }
    public void BtnJogar()
    {
        menuState = MenuState.JOGAR;
        ExitMenu();
        SoundClick();

    }


    public void BtnVoltar()
    {
        SoundClick();

        menuState = MenuState.MENU;
        if(creditosPanel.activeSelf)
            creditosAnimation.CrossFade("Creditos_Exit_2");

        if(selecaoPanel.activeSelf)
        {
            selecaoAnimation.CrossFade("Selecao_Exit_2");
            moveAirplane = MoveAirplane.LEFT;
            //Debug.Log("Selecao_Exit");

        }
    }

    public void AvancarAirplane()
    {
        
        
        moveAirplane = MoveAirplane.RIGHT;

    }

    public void AnteriorAirplane()
    {
        

        moveAirplane = MoveAirplane.LEFT;
       
    }
    #endregion

    void SoundClick()
    {
        if (Persistent.singleton.gameMusics.ContainsKey("click"))
        {
            Persistent.singleton.audioSourceEffects.clip = Persistent.singleton.gameEffects["click"];
            Persistent.singleton.audioSourceEffects.Play();
            Persistent.singleton.audioSourceEffects.loop = true;

        }
    }
}
