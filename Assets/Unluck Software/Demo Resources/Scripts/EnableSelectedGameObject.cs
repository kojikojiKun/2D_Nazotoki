namespace UnluckSoftware
{
	// Demo script to preview gameobjects and particle systems
	// Please ignore the MinMaxCurve error, it is harmless.
	// !Gizmos must be enabled for this object.

	using UnityEngine;
	using UnityEditor;
	using System.Collections;

	public class EnableSelectedGameObject :MonoBehaviour
	{
		[HideInInspector]
		public Transform prevSelect;
		[HideInInspector]
		public Transform currentSelect;
		[HideInInspector]
		public Transform prevSelectWrong;
		[Header("Gizmos must be enabled for preview to work.")]
		public int particleSystems;
		public bool disableMe;
		public float disableDelay;
		public string ignoreTag = "^^^^";
		public bool disableOnPlay;
		public bool autoSlideShow;
		public float autoSlideShowDelay = 3f;
		public string nextKey = "n";
		public bool randomEnable = false;
		public bool repeat;
		int emitOnEnable = 1;       // Emit particles instantly when enabling the ganeobject
		int counter = 0;
		Transform randomEnablePlaceHolder;
		GameObject prevRandomBird;
		bool autoSlideShowStarted;
		public bool clearBeforeNext;

		void Start()
		{
			if (disableOnPlay)
			{
				gameObject.SetActive(false);
				return;
			}

			//if (randomEnable)
			//	{
			DisableAllChildren();
			randomEnablePlaceHolder = new GameObject("randomEnablePlaceHolder").transform;
			//}

			if (autoSlideShow)
			{
				DisableAllChildren();
			}
		}

		private void Update()
		{
			if (autoSlideShowStarted) return;
			if (Input.GetKeyUp("n"))
			{
				if (autoSlideShow)
				{
					if (randomEnable) InvokeRepeating("RandomModel", 0, autoSlideShowDelay);
					else InvokeRepeating("NextModel", 0, autoSlideShowDelay);
					autoSlideShowStarted = true;
				} else
				{
					if (randomEnable) RandomModel();
					else NextModel();
				}
			}
			if (Input.GetKeyUp("p"))
			{
				PauseParticles();
			}
		}

		void RandomModel()
		{
			if (transform.childCount == 0 && !repeat) return;
			if (transform.childCount == 0 && repeat)
			{
				while (randomEnablePlaceHolder.childCount > 0)
				{
					randomEnablePlaceHolder.GetChild(0).parent = transform;
				}
			}
			if (prevRandomBird != null)
			{
				StopParticles();
				DisableGameObject(prevRandomBird);
				prevRandomBird.transform.SetParent(randomEnablePlaceHolder.transform);
			}
			if (transform.childCount == 0) return;
			int randomBird = Random.Range(0, transform.childCount);
			prevRandomBird = transform.GetChild(randomBird).gameObject;
			prevRandomBird.SetActive(true);
			currentSelect = prevRandomBird.transform;
		}

		void NextModel()
		{
			StopParticles();
			if (currentSelect.transform.parent = transform)
			{
				if (!repeat) currentSelect.transform.SetParent(randomEnablePlaceHolder.transform);
				DisableGameObject(currentSelect.gameObject);


			}
			if (transform.childCount == 0) return;
			transform.GetChild(counter % transform.childCount).gameObject.SetActive(true);
			prevSelect = currentSelect;
			currentSelect = transform.GetChild(counter % transform.childCount);
			if (repeat) counter++;
		}

		void StopParticles()
		{
			if (!currentSelect) currentSelect = prevSelect;
			ParticleSystem pss;
			pss = currentSelect.GetComponent<ParticleSystem>();
			if (!pss) return;
			pss.Stop();
			if (clearBeforeNext) pss.Clear();

		}

		void PauseParticles()
		{
			if (!currentSelect) currentSelect = prevSelect;
			ParticleSystem pss;
			pss = currentSelect.GetComponent<ParticleSystem>();
			if (!pss) return;
			if (pss.isPaused) pss.Play();
			else pss.Pause();
		}



		void DisableAllChildren()
		{
			//Debug.Log("Disable All");
			for (int i = 0; i < transform.childCount; i++)
			{
				if (transform.GetChild(i).transform.parent = transform)
				{
					transform.GetChild(i).gameObject.SetActive(false);
				}
			}
		}

		void DisableGameObject(GameObject go)
		{
			//Debug.Log("DisableGameObject");
			//yield return new WaitForSeconds(disableDelay);
			go.SetActive(false);
			//Debug.Log("DisableGameObject " + go);

		}



#if UNITY_EDITOR
		void OnDrawGizmos()
		{
			if (disableMe) return;
			//if (Application.isPlaying) return;
			if (Selection.activeTransform == null) return;
			Transform s = Selection.activeTransform;
			if (s == prevSelect) return;
			if (s == null) return;
			if (s == prevSelect) return;
			if (s == prevSelectWrong) return;
			if (s.parent == null) return;
			if (s.name.Contains(ignoreTag)) return;
			if (s.parent != transform) return;
			if (prevSelect != null)
			{
				prevSelect.gameObject.SetActive(false);
				s.gameObject.SetActive(true);
				ParticleSystem pss = s.GetComponent<ParticleSystem>();
				if (pss)
				{
					pss.time -= pss.time;
					pss.Stop();
				}
			}
			prevSelect = s;
			CountParticles_Editor();
			PlayParticles_Editor();
		}

		void PlayParticles_Editor()
		{
			if (!prevSelect) return;
			ParticleSystem pss;
			pss = prevSelect.GetComponent<ParticleSystem>();
			if (!pss) return;
			pss.Clear();
			pss.Play();
			pss.Emit(emitOnEnable);
		}

		void CountParticles_Editor()
		{
			ParticleSystem[] obj = transform.GetComponentsInChildren<ParticleSystem>(true);
			particleSystems = 0;
			for (int i = 0; i < obj.Length; i++)
			{
				if (obj[i].transform.parent != null)
				{
					ParticleSystem p = obj[i].transform.parent.GetComponent<ParticleSystem>();
					if (p == null)
					{
						particleSystems++;
					}
				}
			}
		}
#endif
	}
}
