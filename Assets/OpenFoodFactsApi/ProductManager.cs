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
        
        name = productData.Product.ProductName;
        Debug.Log("end");
    }
    public void setProductData(Root productRoot)
    {
        productData = productRoot;
        Debug.Log(productData.Product.ProductName);
        Debug.Log("InstanceCube::");
        myItem = Resources.Load("Cube") as GameObject;
        Debug.Log(myItem);
        GameObject pannel = Instantiate(myItem,transform.position, transform.rotation,  transform);
        
    }
}
