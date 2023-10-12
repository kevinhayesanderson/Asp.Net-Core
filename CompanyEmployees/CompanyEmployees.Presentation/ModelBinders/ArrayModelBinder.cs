using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel;
using System.Reflection;

namespace CompanyEmployees.Presentation.ModelBinders
{
    public class ArrayModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (!bindingContext.ModelMetadata.IsEnumerableType)
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return Task.CompletedTask;
            }

            //// we extract the value (a comma-separated string of GUIDs) with the ValueProvider.GetValue() expression.
            string providedValue = bindingContext.ValueProvider
                .GetValue(bindingContext.ModelName)
                .ToString();
            if (string.IsNullOrEmpty(providedValue))
            {
                //// we return null as a result because we have a null check in our action in the controller.
                bindingContext.Result = ModelBindingResult.Success(null);
                return Task.CompletedTask;
            }

            //// we inspected what is the nested type of the IEnumerable parameter and
            //// then created a converter for that exact type, thus making this binder generic.
            Type genericType = bindingContext.ModelType.GetTypeInfo().GenericTypeArguments[0];

            //// we create a converter to a GUID type
            TypeConverter converter = TypeDescriptor.GetConverter(genericType);

            object?[] objectArray = providedValue.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                                            .Select(x => converter.ConvertFromString(x.Trim()))
                                            .ToArray();

            Array guidArray = Array.CreateInstance(genericType, objectArray.Length);
            objectArray.CopyTo(guidArray, 0);
            bindingContext.Model = guidArray;

            bindingContext.Result = ModelBindingResult.Success(bindingContext.Model);
            return Task.CompletedTask;
        }
    }
}