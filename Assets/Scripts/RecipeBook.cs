using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class RecipeBook : MonoBehaviour
{
    [Header("Recipe Book Panel")]
    public GameObject recipePanel;
    public Transform recipeGrid;
    public GameObject recipeRowPrefab;

    [Header("Tarifler")]
    public List<CraftRecipe> recipes;

    void Start()
    {
        recipePanel.SetActive(false);
        PopulateRecipes();
    }

    
    public void OpenRecipeBook()
    {
        recipePanel.SetActive(true);
    }

    public void CloseRecipeBook()
    {
        recipePanel.SetActive(false);
    }

    void PopulateRecipes()
    {
        foreach (CraftRecipe recipe in recipes)
        {
            GameObject row = Instantiate(recipeRowPrefab, recipeGrid);

            Image[] images = row.GetComponentsInChildren<Image>();
            if (images.Length > 0 && recipe.ingredientA != null)
                images[0].sprite = recipe.ingredientA.cleanSprite;
            if (images.Length > 1 && recipe.ingredientB != null)
                images[1].sprite = recipe.ingredientB.cleanSprite;
            if (images.Length > 2)
                images[2].sprite = recipe.resultSprite;

            TextMeshProUGUI[] texts = row.GetComponentsInChildren<TextMeshProUGUI>();
            if (texts.Length > 0)
                texts[0].text = recipe.resultName + " — " + recipe.marketValue + " TL";
        }
    }
}