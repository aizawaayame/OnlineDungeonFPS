using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UILoadingInGame : MonoBehaviour
    {
        
        public Slider progressBar;
        // Start is called before the first frame update
        IEnumerator Start()
        {
            yield return new WaitForSeconds(1f);

            for (float i = 0; i < 100;)
            {
                i += Random.Range(0.1f, 1.5f);
                progressBar.value = i;
                yield return new WaitForEndOfFrame();
            }
            this.gameObject.SetActive(false); 
            yield return null;
        }
    }
}
