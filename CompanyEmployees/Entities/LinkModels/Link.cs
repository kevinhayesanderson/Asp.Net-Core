namespace Entities.LinkModels
{
    public class Link
    {
        public Link()
        {
        }

        public Link(string href, string rel, string method)
        {
            Href = href;
            Rel = rel;
            Method = method;
        }

        public string? Href { get; }
        public string? Rel { get; }
        public string? Method { get; }
    }
}