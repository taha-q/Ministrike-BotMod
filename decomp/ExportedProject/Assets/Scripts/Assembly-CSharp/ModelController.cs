using UnityEngine;

public class ModelController : MonoBehaviour
{
	public Model model;

	public Model[] models;

	public Model GetModel()
	{
		return model;
	}

	public void SetModel(Model.ModelType mt)
	{
		Model[] array = models;
		foreach (Model model in array)
		{
			model.gameObject.SetActive(model.type == mt);
			if (model.type == mt)
			{
				this.model = model;
			}
		}
	}
}
