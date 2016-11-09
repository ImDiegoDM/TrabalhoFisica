using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Persistent : MonoBehaviour {

    public static Persistent singleton;

    public int level;
    [HideInInspector]
    public bool callSelecaoAviao;
    public List<GameObject> airplaneList;
    public int indexAirplaneSelect;

    //Fade
    private FadingScene fade;

    /// <summary>
    /// Contem o nome do audio e a clip para ser adicionados no dicionario
    /// </summary>
    [System.Serializable]
    public class AudioObject
    {
        public string audioName;
        public AudioClip audioClip;
    }

    public List<AudioObject> musicsList = new List<AudioObject>();//lista com as musicas
    public List<AudioObject> effectsList = new List<AudioObject>();//ista com os efeitos


    public Dictionary<string, AudioClip> gameEffects = new Dictionary<string, AudioClip>();//dicionario com os efeitos
    public Dictionary<string, AudioClip> gameMusics = new Dictionary<string, AudioClip>();//dicionario com as musicas

    public AudioSource audioSourceMusic;//componente de audio para musica
    public AudioSource audioSourceEffects;//componente de audio para efeitos

    #region Unity Methods
    void Awake()
    {
        singleton = this;
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        //settings = new Settings();

        foreach (var item in musicsList)
        {
            gameMusics.Add(item.audioName, item.audioClip);
        }

        foreach (var item in effectsList)
        {
            gameEffects.Add(item.audioName, item.audioClip);
        }

        int number = Random.Range(1, 4);
        if (gameMusics.ContainsKey("MainMusic"+" "+number))
        {
            //Debug.Log("Main: ENTER");

            audioSourceMusic.clip = gameMusics["MainMusic"+" "+number];
            audioSourceMusic.Play();
            audioSourceMusic.loop = true;

        } 

    }

    void FixedUpdate()
    {
        if(!audioSourceMusic.isPlaying)
        {
            //if (level == 1 || level == 2)//Menu
            {
                int number = Random.Range(1, 4);
                if (gameMusics.ContainsKey("MainMusic"+" "+number))
                {
                    //Debug.Log("Main: ENTER");

                    audioSourceMusic.clip = gameMusics["MainMusic"+" "+number];
                    audioSourceMusic.Play();
                    audioSourceMusic.loop = true;

                }
            }
        } 
    }
        


    /// <summary>
    /// Quando um level e carregado escolhe uma musica pra ser tocada
    /// </summary>
    /// <param name="level">Level.</param>
    void OnLevelWasLoaded(int level)
    {
        // audioSourceMusic.volume = settings.music;
        // audioSourceEffects.volume = settings.songsEffect;
        Debug.Log("level: "+level);
        this.level = level;

        if (level == 1)//Menu
        {
            
            Persistent[] persistente = GameObject.FindObjectsOfType<Persistent>();

            for (int i = 1; i < persistente.Length; i++)
            {
                Destroy(persistente[i].gameObject);
            }
        }

        if (level == 1  && callSelecaoAviao)//Menu
        {
            Invoke("WaitLoad", 0.5f);

        }

        if(!audioSourceMusic.isPlaying)
        {
            if (level == 1 || level == 2)//Menu
            {
                int number = Random.Range(1, 4);
                if (gameMusics.ContainsKey("MainMusic"+" "+number))
                {
                    //Debug.Log("Main: ENTER");

                    audioSourceMusic.clip = gameMusics["MainMusic"+" "+number];
                    audioSourceMusic.Play();
                    audioSourceMusic.loop = true;

                }
            }
        }
       
    }
    #endregion

    public void CallLevel()
    {
        fade = GameObject.FindObjectOfType<FadingScene>();
        fade.BeginFade(1);
        Invoke("WaitFade", 1.5f);
    }

    private void WaitFade()
    {
		SceneManager.LoadScene(level);
		if(level==SceneManager.GetSceneByName("Fase 1").buildIndex){
		}
    }

    private void WaitLoad()
    {
        MenuController menuController = GameObject.FindObjectOfType<MenuController>();

        menuController.BtnJogar();
        callSelecaoAviao = false;
    }
}
