using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoulderFall : MonoBehaviour
{
    public float falltime = 1;

    public float speed = 5;

    [SerializeField] private bool grounded = false;

    private ParticleSystem particles;

    private Animator anim;

    [SerializeField] private GameObject dangerSign;

    private GameObject sign_instance;

    [SerializeField] private List<GameObject> SpawnableMonsters;

    [SerializeField] private AudioInterface a;

    void Start()
    {   
        a = this.gameObject.GetComponent<AudioInterface>();
    	anim = this.gameObject.GetComponent<Animator>();
        sign_instance = Instantiate(dangerSign, new Vector3(this.gameObject.GetComponent<Transform>().position.x, -65, 0), new Quaternion(0,0,0,0));
    }

    // Update is called once per frame
    void Update()
    {	
    	if(grounded)
    	{      
    		anim.Play("BoulderCrash");
    	}
        else if(falltime >= 0)
        {
        	falltime -= Time.deltaTime;
        }
        else
        {
        	if(sign_instance != null)
        	{
        		Destroy(sign_instance);
        	}
        	this.gameObject.GetComponent<Transform>().Translate(Vector3.down * Time.deltaTime * speed, Space.World);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {	
    	if (other.gameObject.tag.Equals("Player")) //vai ver se o objeto que entrou na caixa de colisão é o jogador e se o inimigo não está atordoado
		{   
            
            a.PlaySound("blast");
			var hited = other.gameObject.GetComponent<MasterController>();//vai receber a classe de controle geral, que tem em todos os jogadores
			hited.takedamage(20); // chama o método que faz o jogador tomar dano, dentro do código dele
		}
		if(!other.gameObject.tag.Equals("Boss") && !other.gameObject.tag.Equals("hitable"))
		{
			grounded = true;
            a.PlaySound("blast");
			if(Random.Range(0f, 1f) > 0.75f && SpawnableMonsters.Count > 0)
			{    
                
				GameObject monster = Instantiate(SpawnableMonsters[Mathf.RoundToInt(Random.Range(0, SpawnableMonsters.Count))], this.gameObject.GetComponent<Transform>().position + Vector3.up*1.5f, new Quaternion(0,0,0,0));
				monster.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-5, 5), -5), ForceMode2D.Impulse);
			}
		}


    	
    }

    void end()
    {
    	Destroy(this.gameObject);
    }
}
