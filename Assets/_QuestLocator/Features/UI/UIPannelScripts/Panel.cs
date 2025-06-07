using UnityEngine;

public class Panel : MonoBehaviour
{
    private ProductParent productparent;

    public void DestroyObj()
    {
        Destroy(gameObject);
    }
    public void SetProductParent(ProductParent productparent)
    {
        this.productparent = productparent;
    }

    public ProductParent GetProductParent()
    {
        return productparent;
    }
}
