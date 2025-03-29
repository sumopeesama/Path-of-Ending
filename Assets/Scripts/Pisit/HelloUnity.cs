
using System.Collections.Generic;
using UnityEngine;

namespace Pisit
{
    public class HelloUnity : MonoBehaviour
    {
        [SerializeField] string playerName = "Default";
        [SerializeField] int playerAge = 7;
        [SerializeField] float playerHp = 100.0f;
        [SerializeField] List<string> myNames;

        public string PlayerName => playerName;
        public int PlayerAge => playerAge;
        public float PlayerHp => playerHp;

        void Awake()
        {
            Debug.Log("Hello Unity - Awake"); // C# Style
            myNames = new List<string>();
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            Debug.Log("Hello Unity - Start"); // C# Style
                                              //book.rentToday(); // Java Style
            myNames.Add("P1");
            myNames.Add("P2");
            myNames.Add("P3");
            myNames.Add("P4");
            myNames.Add("P5");
            Debug.Log("List size = " + myNames.Count);
        }

        // Update is called once per frame
        void Update()
        {
            //Debug.Log("Hello Unity " + (i++) ); // C# Style
            //playerHp = playerHp + 10;
            if(Input.GetKey( KeyCode.A) )
            {
                Debug.Log("Go Left");
                Debug.Log(myNames[0]);
            }
            if (Input.GetKey(KeyCode.D))
            {
                Debug.Log("Go Right");
                Debug.Log(myNames[4]);
            }
        }

        private void OnEnable()
        {
            Debug.Log("Hello Unity - Enable");
        }

        private void OnDisable()
        {
            Debug.Log("Hello Unity - Disable");
        }
        private void OnDestroy()
        {
            Debug.Log("Hello Unity - Destory");
        }

    }
}

