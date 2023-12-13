using Contracts;
using Entities.Models;
using System.Dynamic;
using System.Reflection;

namespace Service.DataShaping
{
    public class DataShaper<T> : IDataShaper<T> where T : class
    {
        public DataShaper()
        {
            Properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        }

        public PropertyInfo[] Properties { get; set; }

        public IEnumerable<Entity> ShapeData(IEnumerable<T> entities, string fieldsString)
        {
            var requiredProperties = GetRequiredProperties(fieldsString);
            return FetchData(entities, requiredProperties);
        }

        public Entity ShapeData(T entity, string fieldsString)
        {
            var requiredProperties = GetRequiredProperties(fieldsString);
            return DataShaper<T>.FetchDataForEntity(entity, requiredProperties);
        }

        private List<Entity> FetchData(IEnumerable<T> entities, IEnumerable<PropertyInfo> requiredProperties)
        {
            var shapedData = new List<Entity>();
            foreach (var entity in entities)
            {
                var shapedObject = DataShaper<T>.FetchDataForEntity(entity, requiredProperties);
                shapedData.Add(shapedObject);
            }
            return shapedData;
        }

        private static Entity FetchDataForEntity(T entity, IEnumerable<PropertyInfo> requiredProperties)
        {
            var shapedObject = new Entity();
            foreach (var property in requiredProperties)
            {
                var objectPropertyValue = property.GetValue(entity);
                _ = shapedObject.TryAdd(property.Name, objectPropertyValue);
            }
            return shapedObject;
        }

        private List<PropertyInfo> GetRequiredProperties(string fieldsString)
        {
            var requiredProperties = new List<PropertyInfo>();
            if (!string.IsNullOrEmpty(fieldsString))
            {
                var fields = fieldsString.Split(',', StringSplitOptions.RemoveEmptyEntries);

                foreach (var field in fields)
                {
                    var property = Array.Find(Properties, pi => pi.Name.Equals(field.Trim(), StringComparison.InvariantCultureIgnoreCase));
                    if (property != null)
                        requiredProperties.Add(property);
                }
            }
            else
            {
                requiredProperties = Properties.ToList();
            }

            return requiredProperties;
        }
    }
}