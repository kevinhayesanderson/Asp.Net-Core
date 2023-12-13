using System.Collections;
using System.Dynamic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Entities.Models
{
    public class Entity : DynamicObject, IXmlSerializable, IDictionary<string, object>
    {
        private readonly string _root = "Entity";

        private readonly IDictionary<string, object> _expando;

        public Entity()
        {
            _expando = new ExpandoObject();
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (_expando.TryGetValue(binder.Name, out object value))
            {
                result = value;
                return true;
            }

            return base.TryGetMember(binder, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            _expando[binder.Name] = value;

            return true;
        }

        object IDictionary<string, object>.this[string key] { get => _expando[key]; set => _expando[key] = value; }

        ICollection<string> IDictionary<string, object>.Keys => _expando.Keys;

        ICollection<object> IDictionary<string, object>.Values => _expando.Values;

        int ICollection<KeyValuePair<string, object>>.Count => _expando.Count;

        bool ICollection<KeyValuePair<string, object>>.IsReadOnly => _expando.IsReadOnly;

        void IDictionary<string, object>.Add(string key, object value)
        {
            _expando.Add(key, value);
        }

        void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item)
        {
            _expando.Add(item);
        }

        void ICollection<KeyValuePair<string, object>>.Clear()
        {
            _expando.Clear();
        }

        bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> item)
        {
            return _expando.Contains(item);
        }

        bool IDictionary<string, object>.ContainsKey(string key)
        {
            return _expando.ContainsKey(key);
        }

        void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            _expando.CopyTo(array, arrayIndex);
        }

        IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
        {
            return _expando.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _expando.GetEnumerator();
        }

        XmlSchema? IXmlSerializable.GetSchema()
        {
            throw new NotImplementedException();
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            reader.ReadStartElement(_root);

            while (!reader.Name.Equals(_root))
            {
                string typeContent;
                Type underlyingType;
                var name = reader.Name;

                reader.MoveToAttribute("type");
                typeContent = reader.ReadContentAsString();
                underlyingType = Type.GetType(typeContent);
                reader.MoveToContent();
                _expando[name] = reader.ReadElementContentAs(underlyingType, null);
            }
        }

        bool IDictionary<string, object>.Remove(string key)
        {
            return _expando.Remove(key);
        }

        bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> item)
        {
            return _expando.Remove(item);
        }

        bool IDictionary<string, object>.TryGetValue(string key, out object value)
        {
            return _expando.TryGetValue(key, out value);
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            foreach (var key in _expando.Keys)
            {
                var value = _expando[key];
                WriteLinksToXml(key, value, writer);
            }
        }

        private void WriteLinksToXml(string key, object value, XmlWriter writer)
        {
            writer.WriteStartElement(key);
            writer.WriteString(value.ToString());
            writer.WriteEndElement();
        }
    }
}
