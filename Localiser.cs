using System.Xml;
using System.Linq;
public class Localiser 
{
    private Dictionary<string, XmlDocument> l10ns = new Dictionary<string, XmlDocument>();

    public Localiser()
    {
        foreach (string file in Directory.GetFiles(".", "BotStrings.*.xml", SearchOption.TopDirectoryOnly))
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(file);
            l10ns.Add(doc.SelectNodes("/localisation")?[0]?.Attributes?["locale"]?.Value ?? file.Substring(11, file.Length - 15), doc);
        }
        
    }

    public string GetString(string locale, string key)
    {
        if (!l10ns.ContainsKey(locale)) return key;
        var doc = l10ns[locale];
        var str = doc.SelectNodes($"//string[@key='{key}']")?[0]?.InnerText ?? key;
        return str;

    }

    public string FormatString(string locale, string key, params string[] inserts)
    {
        var str = GetString(locale, key);
        return String.Format(str, inserts);
    }
}