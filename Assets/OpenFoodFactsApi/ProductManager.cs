using UnityEngine;

public class ProductScript : MonoBehaviour
{
    public Root productData;
    //public GameObject ingredientUI;
    private GameObject myItem;

    void Start()
    {
        //myItem = Instantiate(Resources.Load("Cube")) as GameObject;

        //Debug.Log(myItem);
    }
    public void createUI()
    {
        myItem = Resources.Load("Cube") as GameObject;
        GameObject newObject = Instantiate(myItem, transform);
        name = productData.Product.ProductName;
        Debug.Log("end");
    }
    public void setProductData(Root productRoot)
    {
        this.productData = productRoot;
    }
}
