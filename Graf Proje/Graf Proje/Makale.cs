using System;
using System.Collections.Generic;
using System.Drawing;

namespace Graf_Proje
{
    public class Makale
    {
        public List<string> Yazarlar { get; set; }
        public List<string> AtıfVerilenlerID { get; set; } = new List<string>();
        public List<string> AtıfVerenlerID { get; set; } = new List<string>();
        public List<string> Keywords { get; set; }
        public string ID { get; set; }
        public string Doi { get; set; }
        public string Baslik { get; set; }
        public string Yayıncı { get; set; }
        public int Yil { get; set; }
        public int AtıfSayısı { get; set; }
        public int HIndex { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public double HMedian { get; set; }
        public double BetwennesSkor { get; set; } = 0;
        public bool GenisletildiMi { get; set; } = false;
        public Color CizimRengi { get; set; } = Color.LightGray;
        public Makale()
        {
            Yazarlar = new List<string>();
            AtıfVerilenlerID = new List<string>();
            Keywords = new List<string>();
        }

    }
}
