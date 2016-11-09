using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Fade do logo da puc
/// </summary>
public class LogoPucFading : MonoBehaviour {

	public Image imagefading;//componente de imagem do fade
	public float speedIn;//velocidade do fade in
	public float speedOut;//velocidade do fade out
	public float timeWait;//tempo de espera quando chega no alpha = 1
	public string sceneName;//nome da cena chamada

	private float speed;//velocidade do fade	
	private Color color;//cor
	private int multip;//multiplicador de cor
	private bool waitTime;//controle do tempo de espera


	/// <summary>
	/// Inicizaliza a variavel de cor e o multiplicador
	/// </summary>
	void Start()
	{
		color = imagefading.color;
		color.a = 0;
		multip = 1;
		waitTime = false;

		speed = speedIn;

		imagefading.color = color;
		
	}
	
	/// <summary>
	/// Atualiza a variavel de cor e seta a cor do objeto.
	/// </summary>
	void Update()
	{
		
		color.a += multip * speed * Time.deltaTime;
		
		if(color.a >= 1 && !waitTime)
			StartCoroutine(WaitTimeFade());
		else if(color.a <= 0)
			Application.LoadLevel(sceneName);
		
		imagefading.color = color;
		

	}

	IEnumerator WaitTimeFade()
	{
		waitTime = true;
		yield return new WaitForSeconds(timeWait);
		multip = -1;
		color.a = 1;
		speed = speedOut;
		
		imagefading.color = color;
		
	}
}
