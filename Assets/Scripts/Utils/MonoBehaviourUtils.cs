using UnityEngine;

public class MonoBehaviourUtils : MonoBehaviour {
    /// <summary>
    /// 굳이 singleton을 쓴 이유:
    /// Instantiate가 MonoBehaviour에서 어떻게 작동하는지 모르는데
    /// 그냥 static method로 만들면 객체마다 다른 MonoBehaviour를
    /// 상속받아서 이상해질 수 있으므로.
    /// </summary>
    // below line이 될지 모르겠지만 일단 이렇게 해보자
    public GameObject __canvas;
    private static MonoBehaviourUtils singleton = null;
    private MonoBehaviourUtils() {
        // below line은 문제가 많음
        singleton = this;
    }
    public static MonoBehaviourUtils getInstance() {
        if (singleton == null)
            singleton = new MonoBehaviourUtils();
        return singleton;
    }

    public static GameObject renderBlockWithPosition(GameObject target, Vector2 position) {
        return getInstance().renderBlockWithPositionAtSingleton(target, position);
    }
    
    private GameObject renderBlockWithPositionAtSingleton(GameObject target, Vector2 position) {
        GameObject ret = Instantiate(target);
        ret.transform.SetParent(__canvas.transform);
        UnityUtils.moveGameObjectToPosition(ret, position);
        return ret;
    }
}
