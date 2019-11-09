using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Firebase.Unity.Editor;
using UnityEngine.Assertions;


using System.Threading.Tasks;

public class Spinner : MonoBehaviour
{
    private DatabaseReference reference;
    private FirebaseAuth auth;
    private DataSnapshot Snapshot;
    public RectTransform Progress;
    public float RotationSpeed = 100f;
    private List<string> messagesList = new List<string>();

    public GameObject messages;

    IEnumerator Start()
    {
        // Set up the Editor before calling into the realtime database.
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://charmander-d429e.firebaseio.com/");
        // Get the root reference location of the database.
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        // Check Firebase dependencies
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(fixTask =>
        {
            Assert.IsNull(fixTask.Exception);
            auth = FirebaseAuth.DefaultInstance;

            //Debug.Log(auth.CurrentUser);
            auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(authTask =>
            {
                Assert.IsNull(authTask.Exception);
            });
        });
        /* 
        if (messages != null)
        {
            messagesList.Add("Generando diálogo ingenioso ...");
            messagesList.Add("Localizando los gigapixeles requeridos para renderizar...");
            messagesList.Add("Programando el condesandor de flujo...");
            messagesList.Add("Poniendo a girar al hámster...");
            messagesList.Add("Echando carbón al servidor...");
            messagesList.Add("Creando campo de inversión de bucle de tiempo...");
            messagesList.Add("Los programadores están pelando platanos...");
            messagesList.Add("El servidor funciona con un limón y dos electrodos... por favor espera");
            messagesList.Add("Ingresa la raiz cuadrada de 8684386 para continuar...");
            messagesList.Add("Trabajando... nah sólo bromeaba...");
            messagesList.Add("Trabajando. A diferencia de ti :)");
            messagesList.Add("Negociando la contraseña del WiFi..");
            messagesList.Add("Escaneando su memoria para obtener datos de su tarjeta de crédito...");
            messagesList.Add("Formateando tu teléfono...");
        }
        */
        yield return StartCoroutine(GetTipsList());
        yield return StartCoroutine(ShowMessages());
    }


    private void Update()
    {
        Progress.Rotate(0f, 0f, RotationSpeed * Time.deltaTime);
    }

    //Show the messages picking one random from the list
    private IEnumerator ShowMessages()
    {
        while (this.isActiveAndEnabled)
        {
            int number = Random.Range(0, messagesList.Count);
            messages.GetComponent<TextMeshProUGUI>().text = messagesList[number];
            messagesList.RemoveAt(number);
            yield return new WaitForSeconds(1.5f);
        }

    }
    
    //Gets the messages to be shown in a list from Firebase
    private IEnumerator GetTipsList()
    {
        yield return new YieldTask(reference.Child("Tips").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                Snapshot = task.Result;
                foreach (DataSnapshot data in Snapshot.Children)
                {
                    messagesList.Add(data.Value.ToString());
                }
            }
        }));
    }

    //Method to wait for a task to complete
    public class YieldTask : CustomYieldInstruction
    {
        public YieldTask(Task task)
        {
            Task = task;
        }
        public override bool keepWaiting => !Task.IsCompleted;

        public Task Task { get; }
    }
}
