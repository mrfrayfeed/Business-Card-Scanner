using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEngine : MonoBehaviour {

    private static Dictionary<Type, object> services = new Dictionary<Type, object> ();

    public static Dictionary<Type, object> Services { get => services; }

    private static UIBase activePanel;
    [SerializeField] private UIBase initialShow;
    private void Start () {
        foreach (var item in Services) {
            if (item.Value is UIBase)
                (item.Value as UIBase).Hide ();
        }
        if (initialShow != null)
            Show (initialShow);
    }
    public static void Set<T> (T service) where T : UIBase {
        if (!services.ContainsKey (typeof (T))) {
            services.Add (typeof (T), service);
        }
    }

    public static T Get<T> () where T : UIBase {
        T ret = null;
        try {
            ret = services[typeof (T)] as T;
        } catch (KeyNotFoundException) {
            Debug.LogError ("Dictionary has not this key");
        }
        return ret;
    }

    public static void Remove<T> () where T : UIBase {
        if (services.ContainsKey (typeof (T))) {
            services.Remove (typeof (T));
        }
    }

    public static void Clear () {
        services.Clear ();
    }

    public static void Show<T> (T panel) where T : UIBase {
        activePanel?.Hide ();
        panel.Show ();
        activePanel = panel;
    }

    public static void Show<T> () where T : UIBase {
        Show (Get<T> ());
    }

}