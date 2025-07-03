using Unity.XR.CoreUtils;
using UnityEngine;

public class Panel : MonoBehaviour
{
    private ProductParent productparent;
    private Transform spawn;

    public void DestroyObj()
    {
        Destroy(gameObject);
    }
    public void DestroyAll()
    {
        Destroy(productparent.gameObject);
    }
    public void SetProductParent(ProductParent productparent)
    {
        this.productparent = productparent;
    }

    public ProductParent GetProductParent()
    {
        return productparent;
    }

    public void setCanvasPosition(Vector3 newPos)
    {
        this.transform.SetParent(spawn);
        this.gameObject.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition3D = newPos;
    }
    public void setCanvasRotation(Vector3 newRotation)
    {
        Debug.Log("Angle " + this.gameObject.transform.GetChild(0).GetComponent<RectTransform>().eulerAngles);
        this.gameObject.transform.GetChild(0).GetComponent<RectTransform>().eulerAngles = newRotation;
    }

    public void OnGrab()
    {
        this.transform.SetParent(productparent.transform);
    }
    public void SetSpawn(Transform spawn){
        this.spawn = spawn;
    }
}
