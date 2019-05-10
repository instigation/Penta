using System.Collections;
using UnityEngine;

public static class GlobalInformation {
    public static void setInt(string key, int value)
    {
        ;
        PlayerPrefs.SetString(key, LocalCryptography.encrypt(value.ToString()));
    }
    public static void setString(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
    }
    public static void setBool(string key, bool value)
    {
        PlayerPrefs.SetString(key, value ? LocalCryptography.encrypt("true") : LocalCryptography.encrypt("false"));
    }
    public static int getInt(string key)
    {
        return System.Convert.ToInt32(LocalCryptography.decrypt(PlayerPrefs.GetString(key)));
    }
    public static string getString(string key)
    {
        return PlayerPrefs.GetString(key);
    }
    public static bool getBool(string key)
    {
        string ret = LocalCryptography.decrypt(PlayerPrefs.GetString(key));
        if (string.Compare(ret, "true", System.StringComparison.Ordinal) == 0)
        {
            return true;
        }
        else if (string.Compare(ret, "false", System.StringComparison.Ordinal) == 0)
        {
            return false;
        }
        else
        {
            // data is somehow modified
            return false;
        }
    }
    public static bool hasKey(string key)
    {
        return PlayerPrefs.HasKey(key);
    }
    public static int getOrInitInt(string key, int value)
    {
        if (hasKey(key))
            return getInt(key);
        else
        {
            setInt(key, value);
            return value;
        }
    }
    public static string getOrInitString(string key, string value)
    {
        if (hasKey(key))
            return getString(key);
        else
        {
            setString(key, value);
            return value;
        }
    }
    public static bool getOrInitBool(string key, bool value)
    {
        if (hasKey(key))
            return getBool(key);
        else
        {
            setBool(key, value);
            return value;
        }
    }
}
