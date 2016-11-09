using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Classe com Fades de cena
/// </summary>
public class FadingScene : MonoBehaviour {

	public GameObject fadeTexture;//textura (geralmente preta), que vai dar o efeito de fade
	public float fadeSpeed = 0.1f; //velocidade do fade

	private Color color;//recebe a cor do objeto
	private int fadeDir = -1;//direcao do fade
	private bool fadeIn;//fade in

	/// <summary>
	/// Inicializa variaveis de cor e fade texture e ativado
	/// </summary>
	void Start()
	{
		color = fadeTexture.GetComponent<Image>().color;

		fadeTexture.SetActive(true);

		if(fadeDir == -1)
			fadeIn = false;
		else
			fadeIn = true;
		
	}

	/// <summary>
	/// Adiciona ou Subtrai o valor de alpha
	/// </summary>
	void Update()
	{
		if(fadeIn && color.a <= 1)
		{
			if(Time.timeScale == 0)
				Time.timeScale = 1;

			color.a += fadeDir * fadeSpeed * Time.deltaTime;
			fadeTexture.GetComponent<Image>().color = color;

		}
		else if(!fadeIn && color.a >= 0)
		{
			color.a += fadeDir * fadeSpeed * Time.deltaTime;
			
			fadeTexture.GetComponent<Image>().color = color;
			
		}

		if(!fadeIn && color.a < 0)
			fadeTexture.SetActive(false);
			
			
	}

	/// <summary>
	/// Da inicio ao fade
	/// </summary>
	/// <returns>The fade.</returns>
	/// <param name="direction">Direction.</param>
	public float BeginFade(int direction)
	{
		fadeTexture.SetActive(true);
		
		fadeDir = direction;


		if(fadeDir == -1)
		{
			fadeIn = false;
			Color c = fadeTexture.GetComponent<Image>().color;
			c.a = 1;
			fadeTexture.GetComponent<Image>().color = c;			
			
		}
		else
		{
			fadeIn = true;
			Color c = fadeTexture.GetComponent<Image>().color;
			c.a = 0.5f;
			fadeTexture.GetComponent<Image>().color = c;

		}

		return (1.0f);
	}

	/// <summary>
	/// Funcao da unity quando o level e carregado o fadeout e inicializado
	/// </summary>
	void OnLevelWasLoaded(int level)
	{
		BeginFade(-1);
	}
}
