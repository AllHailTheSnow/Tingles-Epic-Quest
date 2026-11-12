using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    //Creates a static instance of the class
    private static T instance;
    //Creates a property for the instance of the class to be accessed by other classes
    public static T Instance { get { return instance; } }

    protected virtual void Awake()
    {
        //If the instance is not null and the game object is not null
        if (instance != null && this.gameObject != null)
        {
            //Destroy the game object
            Destroy(this.gameObject);
        }
        //If the instance is null
        else
        {
            //Set the instance to this
            instance = (T)this;
        }

        if (!gameObject.transform.parent)
        {
            //Dont destroy the game object on load of a new scene
            DontDestroyOnLoad(gameObject);
        }
    }
}
