using System.Collections;

public static class GlobalInformation {
    private static Hashtable information = new Hashtable();
    public static void storeKeyValue(object key, object value) {
        if (contains(key))
            information.Remove(key);
        information.Add(key, value);
    }
    public static object getValue(object key) {
        return information[key];
    }
    public static bool contains(object key) {
        return information.Contains(key);
    }
    public static object getOrInitValue(object key, object initialValue)
    {
        if (contains(key))
            return getValue(key);
        else
        {
            storeKeyValue(key, initialValue);
            return initialValue;
        }
    }
}
