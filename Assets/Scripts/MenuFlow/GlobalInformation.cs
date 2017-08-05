using System.Collections;

public static class GlobalInformation {
    private static Hashtable information = new Hashtable();
    public static void storeKeyValue(object key, object value) {
        information.Add(key, value);
    }
    public static object getValue(object key) {
        return information[key];
    }
    public static bool contains(object key) {
        return information.Contains(key);
    }
}
