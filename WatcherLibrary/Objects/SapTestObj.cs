using System.Xml.Serialization;

namespace WatcherLibrary.Objects
{
    [XmlRoot(ElementName = "SapTestObj", Namespace = "http://admevis")]
    public class SapTestObj
    {
        [XmlElement(ElementName = "IdNo")]
        public string IdNo { get; set; }
        [XmlElement(ElementName = "FileName")]
        public string FileName { get; set; }
        [XmlElement(ElementName = "Status")]
        public string Status { get; set; }
        [XmlElement(ElementName = "Note")]
        public string Note { get; set; }
        [XmlAttribute(AttributeName = "ns0", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns0 { get; set; }
    }
}
