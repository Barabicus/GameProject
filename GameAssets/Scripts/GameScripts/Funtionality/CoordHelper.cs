using UnityEngine;
using System.Collections;

public static class CoordHelper {

    public static Rect TransformToScreenRect(GameObject gameObj, int w, int h)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(gameObj.transform.position);
        return ScreenRect(screenPos.x, screenPos.y, w, h);
    }

    public static Rect TransformToScreenRect(Transform trans, int w, int h)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(trans.position);
        return ScreenRect(screenPos.x, screenPos.y, w, h);
    }

    public static Vector3 RectCoords(Vector3 coords)
    {
        return new Vector3(coords.x, Screen.height - coords.y, coords.z);
    }

    public static Rect ScreenRect(int x, int y, int w, int h)
    {
        return new Rect(x, Screen.height - y, w, h);
    }

    public static Rect ScreenRect(float x, float y, float w, float h)
    {
        return new Rect(x, Screen.height - y, w, h);
    }

    public static Rect NonNegativeRect(Rect r)
    {
        return new Rect(r.width < 0 ? r.x + r.width : r.x, r.height < 0 ? r.y + r.height : r.y, Mathf.Abs(r.width), Mathf.Abs(r.height));
    }

    public static Rect NonNegativeRectd(Rect r)
    {
        return new Rect(r.x, r.y, r.width, r.height);
    }


}
