using UnityEngine;

public class PlaceTiledMeshes : MonoBehaviour
{
    public GameObject model;
    public float width = 6f;
    public bool xAxis = true;
    public float height = 6f;

    void Start()
    {
        Vector3 realScale = transform.rotation *transform.localScale;
        realScale.x = Mathf.Abs(realScale.x);
        realScale.y = Mathf.Abs(realScale.y);
        realScale.z = Mathf.Abs(realScale.z);

        float scale = xAxis ? realScale.x : realScale.z;
        int count = Mathf.RoundToInt(scale / width);
        float newWidth = scale / count;
        float edge = (xAxis ? transform.position.x : transform.position.z) - scale / 2;
        float start = edge + newWidth / 2;
        float yScale = model.transform.localScale.y * realScale.y / height;
        Vector3 startVec = xAxis ? new Vector3(start, transform.position.y - realScale.y / 2, transform.position.z) : new Vector3(transform.position.x, transform.position.y, start);
        Vector3 stepVec = xAxis ? new Vector3(newWidth, 0, 0) : new Vector3(0, 0, newWidth);
        for (int i = 0; i < count; i++)
        {
            Vector3 pos = startVec + stepVec * i;
            GameObject obj = Instantiate(model, pos, xAxis ? Quaternion.Euler(0, -90, 0) : Quaternion.identity);
            Vector3 scaleVec = new Vector3(model.transform.localScale.x, yScale, model.transform.localScale.z);
            obj.transform.localScale = scaleVec;
            obj.transform.SetParent(transform);
        }
    }
}