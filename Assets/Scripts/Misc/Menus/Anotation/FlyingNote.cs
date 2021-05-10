using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FlyingNote : MonoBehaviour
{	
	[SerializeField] private Collider2D thiscol;

	[SerializeField] private GameObject border;

	[SerializeField] private GameObject Line;

	private float loadTime = 1;

	private Camera mainc;

	private SpriteRenderer borderspr; 

 public bool isDrag;

 public bool isSelected;

 public bool isLoading;

 public bool Locked;

	private Vector2 lastMousePos;

	[SerializeField] private GameObject Ligation;

	void OnEnable()
	{
		GameEvents.StartListening("ClosedTable", Vanish);
	}
	void OnDisable()
	{
		GameEvents.StopListening("ClosedTable", Vanish);
	}
	void Start()
	{
		mainc = Camera.main;
		borderspr = border.GetComponent<SpriteRenderer>();
	}
	void FixedUpdate()
	{
		Vector2 mousep = mainc.ScreenToWorldPoint(Input.mousePosition);
		if(!Locked)
		{

			if(Input.GetMouseButton(0))
			{
				Collider2D[] cols = Physics2D.OverlapPointAll(mousep, Physics2D.DefaultRaycastLayers, -1, 1);
				if(cols.Length > 0)
				{	

					if(cols[0] == thiscol)
					{	

						isDrag = true;
						isSelected = true;
						if(Vector2.Distance(mousep, lastMousePos) < 0.1f)
						{
							CountDown();
						}
						else
						{
							isLoading = false;
						}
					}
					else
					{	
						isSelected = false;
						isLoading = false;
					}

				}
				else
				{	

					isSelected = false;
					isLoading = false;
				}


				
			}
			else
			{
				isDrag = false;
				isLoading = false;
				border.SetActive(false);
			}

			if(!isLoading)
			{	
				loadTime = 1;
				if(isDrag)
				{	
					border.SetActive(true);
					borderspr.color = Color.yellow;
					this.transform.position = mousep;
				}
				else if(isSelected)
				{
					border.SetActive(true);
					borderspr.color = Color.green;
				}		
			}
		}
		else
		{	
			Collider2D[] cols = Physics2D.OverlapPointAll(mousep, Physics2D.DefaultRaycastLayers, -1, 1);
			if(isSelected)
			{
				
				if(Input.GetMouseButton(0))
				{
					if(cols.Length > 0)
					{
						if(cols[0] != thiscol)
						{	
							Lock(cols[0].gameObject);
						}

					}
					else
					{
						Unlock();
					}
				}
			}
			else
			{
				if(Input.GetMouseButton(0))
				{
					if(cols.Length > 0)
					{
						if(cols[0] == thiscol && Vector2.Distance(mousep, lastMousePos) < 0.1f)
						{	
							CountUp();
						}
						else
						{	
							borderspr.color = Color.blue;
							loadTime = 1;
						}

					}
				}
				else
				{	
					borderspr.color = Color.blue;
					loadTime = 1;
				}
			}
			
		}
		lastMousePos = mousep;
	}

	private void CountDown()
	{
		isLoading = true;
		loadTime -= Time.fixedDeltaTime;
		if(loadTime <= 0f)
		{
			Load();
		}
		borderspr.color = Color.Lerp(borderspr.color, Color.blue, (1 - loadTime));
	}
	private void CountUp()
	{
		loadTime -= Time.fixedDeltaTime;
		if(loadTime <= 0f)
		{
			Unlock();
		}
		borderspr.color = Color.Lerp(borderspr.color, Color.red, (1 - loadTime));
	}
	private void Load()
	{
		isSelected = true;
		isLoading = false;
		isDrag = false;
		Locked = true;
		border.SetActive(true);
		borderspr.color = Color.blue;

		Line = Instantiate(Ligation, this.gameObject.transform.parent);

		Line.GetComponent<NoteLigator>().Ob1 = this.gameObject;

	}

	public void Lock(GameObject otherCol)
	{	
		if(otherCol.GetComponent<PhisicNote>().isText != this.gameObject.GetComponent<PhisicNote>().isText)
		{
			otherCol.GetComponent<FlyingNote>().GetLocked();
			otherCol.GetComponent<FlyingNote>().Line = Line;
			Line.GetComponent<NoteLigator>().Ob2 = otherCol;
			isSelected = false;
			borderspr.color = Color.blue;
			loadTime = 1;		
		}

	}

	public void GetLocked()
	{	
		isSelected = false;
		isLoading = false;
		isDrag = false;
		Locked = true;
		border.SetActive(true);
		borderspr.color = Color.blue;
	}

	public void Unlock()
	{	
		loadTime = 1;
		NoteLigator LNote= Line.GetComponent<NoteLigator>();
		isSelected = false;
		if(LNote.Ob2)
		{
			LNote.Ob2.GetComponent<FlyingNote>().Locked = false;
		}
		
		if(LNote.Ob1)
		{
			LNote.Ob1.GetComponent<FlyingNote>().Locked = false;
		}

		Destroy(Line);

	}

	void Vanish()
	{
		Destroy(this.gameObject);
	}

}
