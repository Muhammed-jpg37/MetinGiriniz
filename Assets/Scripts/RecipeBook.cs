using UnityEngine;

public class RecipeBook : MonoBehaviour
{
    public GameObject recipeImage;

    public void ToggleRecipeBook()
    {
        recipeImage.SetActive(!recipeImage.activeSelf);
    }
}