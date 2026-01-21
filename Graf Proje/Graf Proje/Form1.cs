using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Runtime.ExceptionServices;

namespace Graf_Proje
{
    public partial class Form1 : Form
    {
        //değişkenler
        Dictionary<string, Makale> TumMakaleler = new Dictionary<string, Makale>();
        List<Makale> CiziliMakaleler = new List<Makale>();
        private ToolTip bilgiKutusu = new ToolTip();
        private Makale sonGosterilenMakale = null;
        Font dugumFont;
        Point baslangicNoktasi;
        float ofsetX = 0;
        float ofsetY = 0;
        float zoom = 1.0f;
        bool surukleniyor = false;
        bool analizMi = false;
        bool yesilcizgi = false;



        //constructor
        public Form1()
        {
            InitializeComponent();
            pnlCiz.Paint += pnlCiz_Paint;
            pnlCiz.MouseWheel += pnlCiz_MouseWheel;
            pnlCiz.Focus();
            dugumFont = new Font("Arial", 11);
        }

        //JSON AYIRMA İŞLEMLERİ

        private Makale MakaleParcala(string Makale)
        {
            Makale makale = new Makale();
            makale.ID = DegerCikar(Makale, "id");
            makale.Doi = DegerCikar(Makale, "doi");
            makale.Baslik = DegerCikar(Makale, "title");
            makale.Yayıncı = DegerCikar(Makale, "venue");
            makale.Yazarlar = ListeDegeriCikar(Makale, "authors");
            makale.Keywords = ListeDegeriCikar(Makale, "keywords");
            makale.AtıfVerilenlerID = ListeDegeriCikar(Makale, "referenced_works");
            string yilStr = DegerCikar(Makale, "year");
            if (int.TryParse(yilStr, out int yil))
            {
                makale.Yil = yil;
            }
            return makale;
        }
        private string IdCikar(string Id)
        {
            int IdIndex = Id.LastIndexOf('/');
            if (IdIndex != -1 && Id.Length > IdIndex + 2)
                return Id = Id.Substring(IdIndex + 2);
            return Id;
        }
        private string DegerCikar(string json, string Key)
        {
            string anahtar = $"\"{Key}\":";
            int baslangicIndex = json.IndexOf(anahtar);

            if (baslangicIndex == -1) return null;

            int keyBitisIndex = baslangicIndex + anahtar.Length;
            int DegerIndex = keyBitisIndex;

            while (DegerIndex < json.Length && char.IsWhiteSpace(json[DegerIndex]))
            {
                DegerIndex++;
            }

            int TırnakIndex = json.IndexOf('\"', DegerIndex);
            if (TırnakIndex == DegerIndex)
            {
                int Baslangic = TırnakIndex + 1;
                int Bitis = json.IndexOf('\"', Baslangic);
                if (Bitis != -1)
                {
                    return json.Substring(Baslangic, Bitis - Baslangic);
                }
            }
            else
            {
                int VirgulIndex = json.IndexOf(',', DegerIndex);
                int ParantezIndex = json.IndexOf('}', DegerIndex);

                int BitisIndex = Math.Min(VirgulIndex != -1 ? VirgulIndex : int.MaxValue,
                                          ParantezIndex != -1 ? ParantezIndex : int.MaxValue);

                if (BitisIndex != int.MaxValue && BitisIndex > DegerIndex)
                {
                    return json.Substring(DegerIndex, BitisIndex - DegerIndex).Trim();
                }
            }
            return null;
        }
        private List<string> ListeDegeriCikar(string json, string Key)
        {
            List<string> liste = new List<string>();
            string anahtar = $"\"{Key}\":";
            int KeyIndex = json.IndexOf(anahtar);
            if (KeyIndex == -1) return liste;

            int ListeBaslangicIndex = json.IndexOf('[', KeyIndex);
            if (ListeBaslangicIndex == -1) return liste;
            int ListeBitisIndex = json.IndexOf(']', ListeBaslangicIndex + 1);
            if (ListeBitisIndex == -1) return liste;

            string Icerik = json.Substring(ListeBaslangicIndex + 1, ListeBitisIndex - ListeBaslangicIndex - 1).Trim();
            if (string.IsNullOrEmpty(Icerik)) return liste;

            string TemizIcerik = Icerik.Replace("\"", "");

            string[] ogeler = TemizIcerik.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string oge in ogeler)
            {
                string temizOge = oge.Trim().Trim('\"');
                if (!string.IsNullOrEmpty(temizOge))
                    liste.Add(temizOge);
            }

            return liste;
        }


        private List<string> JsonObjeleriAyir(string json)
        {
            List<string> bloklar = new List<string>();

            int derinlik = 0;
            int baslangic = 0;
            bool icinde = false;

            for (int i = 0; i < json.Length; i++)
            {
                if (json[i] == '{')
                {
                    if (!icinde)
                    {
                        baslangic = i;
                        icinde = true;
                    }
                    derinlik++;
                }
                else if (json[i] == '}')
                {
                    derinlik--;

                    if (derinlik == 0 && icinde)
                    {
                        bloklar.Add(json.Substring(baslangic, i - baslangic + 1));
                        icinde = false;
                    }
                }
            }

            return bloklar;
        }



        // Değerleri atama ve hesaplama  //

        private void KurulumSonrasiIslemler()
        {
            foreach (var entry in TumMakaleler)
                entry.Value.AtıfSayısı = 0;
            foreach (var entry in TumMakaleler)
            {
                Makale KaynakMakale = entry.Value;
                if(KaynakMakale.AtıfVerenlerID == null) 
                    KaynakMakale.AtıfVerenlerID = new List<string>();

                foreach (string HedefIdRaw in KaynakMakale.AtıfVerilenlerID)
                {
                    string Id = IdCikar(HedefIdRaw);
                    if (TumMakaleler.ContainsKey(Id))
                    {
                        Makale HedefMakale = TumMakaleler[Id];
                        HedefMakale.AtıfSayısı++;
                        HedefMakale.AtıfVerenlerID.Add(KaynakMakale.ID);
                    }
                }
            }
            foreach (var entry in TumMakaleler)
                HIndexHesapla(entry.Value);

        }

        //ALGORİTMALAR//

        //H-Index//
        private void HIndexHesapla(Makale HedefMakale)
        {
            if (HedefMakale.AtıfVerenlerID == null)
                HedefMakale.AtıfVerenlerID = new List<string>();
            List<int> AtıfYapanlarinSkorlari = new List<int>();
            foreach (string atifverenID in HedefMakale.AtıfVerenlerID)
            {
                string Id = IdCikar(atifverenID);
                if (TumMakaleler.ContainsKey(Id))
                {
                    Makale AtifVerenMakale = TumMakaleler[Id];

                    AtıfYapanlarinSkorlari.Add(TumMakaleler[Id].AtıfSayısı);
                }
            }
            AtıfYapanlarinSkorlari.Sort();
            AtıfYapanlarinSkorlari.Reverse();

            int hIndex = 0;
            for (int i = 0; i < AtıfYapanlarinSkorlari.Count; i++)
            {
                if (AtıfYapanlarinSkorlari[i] >= i + 1)
                {
                    hIndex = i + 1;
                }
                else
                {
                    break;
                }
            }
            HedefMakale.HIndex = hIndex;

            if (hIndex > 0)
            {
                List<int> HcoreListesi = AtıfYapanlarinSkorlari.Take(hIndex).ToList();
                int orta = HcoreListesi.Count / 2;

                if (HcoreListesi.Count % 2 != 0)
                {
                    HedefMakale.HMedian = HcoreListesi[orta];
                }
                else
                {
                    HedefMakale.HMedian = (HcoreListesi[orta - 1] + HcoreListesi[orta]) / 2;
                }
            }
            else HedefMakale.HMedian = 0;
        }

        //K-Core//
        private void KCoreHesapla(int k)
        {
            analizMi = true;
            Dictionary<string, HashSet<string>> graf = new Dictionary<string, HashSet<string>>();
            foreach (var id in TumMakaleler.Keys)
            {
                string kısaId = IdCikar(id);
                graf[kısaId] = new HashSet<string>();
            }


            foreach (var makale in TumMakaleler.Values)
            {
                if (makale.AtıfVerenlerID == null) continue;
                string u = IdCikar(makale.ID);
                if (!graf.ContainsKey(u)) continue;
                foreach (var hedefId in makale.AtıfVerenlerID)
                {
                    string v = IdCikar(hedefId);
                    if (!graf.ContainsKey(v) || v == u) continue;
                    graf[u].Add(v);
                    graf[v].Add(u);
                }
            }
            bool silindiMi;
            do
            {
                silindiMi = false;
                List<string> silinecekler = new List<string>();
                foreach (var id in graf.Keys.ToList())
                {
                    if (graf[id].Count < k)
                        silinecekler.Add(id);
                }
                foreach (var sil in silinecekler)
                {
                    foreach (var komsu in graf[sil])
                        graf[komsu].Remove(sil);
                    graf.Remove(sil);
                    silindiMi = true;
                }
            } while (silindiMi);


            if (graf.Count == 0)
            {
                MessageBox.Show($"K-Core analizi sonucunda k={k} için çekirdek bulunamadı !");
                return;
            }


            int i = 0;
            foreach (var makale in CiziliMakaleler)
            {
                string ID = IdCikar(makale.ID);
                if (graf.ContainsKey(ID))
                {
                    makale.CizimRengi = Color.Blue;
                    i++;
                }
                else
                    makale.CizimRengi = Color.FromArgb(50, Color.Gray);
            }


            pnlCiz.Invalidate();
            MessageBox.Show($"K-Core analizi tamamlandı !\nK={k} için çekirdekte {graf.Count} makale bulundu.\n" +
                $"Mavi Düğümler: Kalanlar \n" +
                $"Gri Düğümler: Elenenler");
        }



        //Betwenness//
        private void BetwennessHesapla()
        {
            analizMi= true;
            Dictionary<string,HashSet<string>> graf = new Dictionary<string, HashSet<string>>();

            foreach(var id in TumMakaleler.Keys)
                graf[id] = new HashSet<string>();
            
            foreach(var m in TumMakaleler.Values)
            {
                if(m.AtıfVerenlerID == null) continue;
                string u= IdCikar(m.ID);
                if (!graf.ContainsKey(u)) continue;
                foreach (var hedefId in m.AtıfVerenlerID)
                {
                    string V = IdCikar(hedefId);
                    if(!graf.ContainsKey(V)|| V==u) continue;
                    graf[u].Add(V);
                    graf[V].Add(u);
                }
            }
            foreach (var m in TumMakaleler.Values) m.BetwennesSkor = 0;

            foreach(var s in graf.Keys)
            {
                Stack<string> S = new Stack<string>();
                Dictionary<string, List<string>> P = new Dictionary<string, List<string>>();
                Dictionary<string, double> sigma = new Dictionary<string,double>();
                Dictionary<string, int> d = new Dictionary<string, int>();

                foreach(var v in graf.Keys)
                {
                    P[v] = new List<string>();
                    sigma[v] = 0;
                    d[v] = -1;
                }
                sigma[s] = 1;
                d[s] = 0;
                Queue<string> Q = new Queue<string>();
                Q.Enqueue(s);

                while(Q.Count > 0)
                {
                    string v = Q.Dequeue();
                    S.Push(v);
                    foreach(var w in graf[v])
                    {
                        if(d[w] < 0)
                        {
                            d[w] = d[v] + 1;
                            Q.Enqueue(w);
                        }
                        if(d[w] == d[v] + 1)
                        {
                            sigma[w] += sigma[v];
                            P[w].Add(v);
                        }
                    }
                }
                Dictionary<string, double> delta = new Dictionary<string, double>();
                foreach (var V in graf.Keys) delta[V] = 0;

                while (S.Count > 0)
                {
                    string w = S.Pop();
                    foreach (var v in P[w])
                    {
                        if(sigma[w]!=0)
                        delta[v] += (sigma[v] /sigma[w]) * (1 + delta[w]);
                    }
                    if (w != s)
                    {
                        TumMakaleler[w].BetwennesSkor += delta[w];
                    }
                }
            }
           
            foreach(var m in TumMakaleler.Values)m.BetwennesSkor /= 2.0;
            
        }


        //Çizimler//
        private void GrafiGenislet(Makale yeniMerkez)
        {
            HIndexHesapla(yeniMerkez);

            var yeniHCoreListesi = TumMakaleler.Values.Where(m => yeniMerkez.AtıfVerenlerID.Contains(m.ID))
                .OrderByDescending(m => m.AtıfSayısı).Take(yeniMerkez.HIndex).ToList();

            if (yeniHCoreListesi.Count == 0)
            {
                MessageBox.Show("Bu makalenin atıf yapanı yok genişlemiyor !");
                return;
            }

            int yaricap = 130;
            double aci = 2 * Math.PI / yeniHCoreListesi.Count;

            Random rnd = new Random();
            double baslangicAcisi = rnd.Next(0, 360);

            for (int i = 0; i < yeniHCoreListesi.Count; i++)
            {
                Makale yeniMakale = yeniHCoreListesi[i];
                if (CiziliMakaleler.Contains(yeniMakale)) continue;

                bool yerbulundu = false;
                int deneme = 0;
                float adayx = 0, adayy = 0;

                while (!yerbulundu && deneme < 50)
                {
                    double radyan = baslangicAcisi + (aci * i) + (deneme * 0.5);
                    float guncelyaricap = yaricap + (deneme * 10);

                    adayx = yeniMerkez.X + (float)(guncelyaricap * Math.Cos(radyan));
                    adayy = yeniMerkez.Y + (float)(guncelyaricap * Math.Sin(radyan));
                    if (!CarpismaVarMi(adayx, adayy)) yerbulundu = true;
                    deneme++;
                }


                yeniMakale.X = adayx;
                yeniMakale.Y = adayy;
                CiziliMakaleler.Add(yeniMakale);




            }
            pnlCiz.Invalidate();
        }

        private void Istatistik()
        {
            int toplamMakale = TumMakaleler.Count;
            int toplamAtif = 0;

            Makale encokAlan = null;
            int MaxAlan = -1;

            Makale encokVeren = null;
            int MaxVeren = -1;

            foreach (var m in TumMakaleler.Values)
            {
                int verilenAtif = (m.AtıfVerilenlerID != null) ? m.AtıfVerilenlerID.Count : 0;
                toplamAtif += verilenAtif;

                if (verilenAtif > MaxVeren)
                {
                    MaxVeren = verilenAtif;
                    encokVeren = m;
                }
                if (m.AtıfSayısı > MaxAlan)
                {
                    MaxAlan = m.AtıfSayısı;
                    encokAlan = m;
                }
            }
            string Istatistik = $"--GRAF İSTATİSTİK--\n" +
                                $"Toplam Makale: {toplamMakale}\n" +
                                $"Toplam Atıf: {toplamAtif}\n" +
                                $"En Çok Atıf Alan Makale ID: {IdCikar(encokAlan.ID)} \nAtıf Sayısı: {encokAlan.AtıfSayısı}\n" +
                                $"En Çok Atıf Veren Makale ID: {IdCikar(encokVeren.ID)}\nVerilen Atıf Sayısı: {encokVeren.AtıfVerilenlerID.Count}\n";

            if (label3 != null)
            {
                label3.Text = Istatistik;
            }


        }

        private void DugumCiz(Graphics g, Makale makale, Brush renk)
        {
            float ekranX = (makale.X * zoom) + ofsetX;
            float ekranY = (makale.Y * zoom) + ofsetY;
            int cap = (int)(40 * zoom);

            float x = ekranX - cap / 2;
            float y = ekranY - cap / 2;

            g.FillEllipse(renk, x, y, cap, cap);
            g.DrawEllipse(Pens.Black, x, y, cap, cap);

            string Yazi = makale.AtıfSayısı.ToString();
            Font font = dugumFont;

            SizeF boyut = g.MeasureString(Yazi, font);


            g.DrawString(Yazi, font, Brushes.Black, ekranX - boyut.Width / 2, ekranY - boyut.Height / 2);

        }
        private void OkCiz(Graphics g, Makale baslangic, Makale hedef, Color renk)
        {

            float x1 = (baslangic.X * zoom) + ofsetX;
            float y1 = (baslangic.Y * zoom) + ofsetY;
            float x2 = (hedef.X * zoom) + ofsetX;
            float y2 = (hedef.Y * zoom) + ofsetY;

            float yaricap = 20 * zoom;
            float dx = x2 - x1;
            float dy = y2 - y1;
            float mesafe = (float)Math.Sqrt(dx * dx + dy * dy);

            if (mesafe > 0)
            {
                x2 = x2 - (yaricap * (dx / mesafe));
                y2 = y2 - (yaricap * (dy / mesafe));
                x1 = x1 - (yaricap * (dx / mesafe));
                y1 = y1 - (yaricap * (dy / mesafe));
            }

            Pen kalem = new Pen(renk, 1.2f * zoom);
            kalem.CustomEndCap = new System.Drawing.Drawing2D.AdjustableArrowCap(4, 4);
            g.DrawLine(kalem, x1, y1, x2, y2);
        }
        private bool CarpismaVarMi(float x, float y)
        {
            int Mesafe = 60;

            foreach (Makale m in CiziliMakaleler)
            {
                float mesafe = (float)Math.Sqrt(Math.Pow(x - m.X, 2) + Math.Pow(y - m.Y, 2));

                if (mesafe < Mesafe)
                    return true;

            }
            return false;
        }
        private void SahneyiCiz(Graphics g)
        {

            g.Clear(Color.White);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            var SiraliMakaleler = CiziliMakaleler.OrderBy(m =>long.TryParse(new string(IdCikar(m.ID).Where(char.IsDigit).ToArray()),out var v) ? v : long.MaxValue).ToList();
            {
                if(yesilcizgi)
                for (int i = 0; i < SiraliMakaleler.Count - 1; i++)
                {
                    Makale m1 = SiraliMakaleler[i];
                    Makale m2 = SiraliMakaleler[i + 1];
                    float x1 = (m1.X * zoom) + ofsetX;
                    float y1 = (m1.Y * zoom) + ofsetY;
                    float x2 = (m2.X * zoom) + ofsetX;
                    float y2 = (m2.Y * zoom) + ofsetY;

                    using (Pen yesilboya = new Pen(Color.LightGreen, 2 * zoom))
                    {
                        yesilboya.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                        g.DrawLine(yesilboya, x1, y1, x2, y2);
                    }

                }

                foreach (var m in CiziliMakaleler)
                {
                    foreach (string Id in m.AtıfVerenlerID)
                    {
                        string hedefId = IdCikar(Id);
                        if (TumMakaleler.ContainsKey(hedefId))
                        {
                            Makale kaynak = TumMakaleler[hedefId];
                            if (CiziliMakaleler.Contains(kaynak))
                                if (analizMi)
                                {
                                    float x1 = (kaynak.X * zoom) + ofsetX;
                                    float y1 = (kaynak.Y * zoom) + ofsetY;
                                    float x2 = (m.X * zoom) + ofsetX;
                                    float y2 = (m.Y * zoom) + ofsetY;

                                    using (Pen p = new Pen(Color.Black, 2 * zoom))
                                    {
                                        g.DrawLine(p, x1, y1, x2, y2);
                                    }
                                }
                                else
                                {
                                    OkCiz(g, kaynak, m, Color.Black);
                                }

                        }
                    }
                }
                foreach (var m in CiziliMakaleler)
                {
                    Brush firca;
                    if (m.CizimRengi != Color.LightGray)
                        firca = new SolidBrush(m.CizimRengi);
                    else
                    {
                        if (m == CiziliMakaleler.FirstOrDefault())
                            firca = Brushes.OrangeRed;
                        else if (m.GenisletildiMi)
                            firca = Brushes.Orange;
                        else
                            firca = Brushes.LightGray;

                    }

                    DugumCiz(g, m, firca);
                }
            }
        }


        //Butonlar//


        private void jsonOku(object sender, EventArgs e)
        {
            analizMi = true;
            yesilcizgi = true;
            TumMakaleler.Clear();

            try
            {
                string jsondata = File.ReadAllText("ornekdata.json").Trim();

                if (jsondata.StartsWith("[")) jsondata = jsondata.Substring(1);
                if (jsondata.EndsWith("]")) jsondata = jsondata.Substring(0, jsondata.Length - 1);

                var makalelerJson = JsonObjeleriAyir(jsondata);

                foreach (var makalejson in makalelerJson)
                {
                    Makale makale = MakaleParcala(makalejson);
                    if (makale != null && !string.IsNullOrEmpty(makale.ID))
                    {
                        string kısaId = IdCikar(makale.ID);
                        if (!TumMakaleler.ContainsKey(kısaId))
                            TumMakaleler.Add(kısaId, makale);
                    }
                }

                KurulumSonrasiIslemler();
                Istatistik();

                var enYuksekH = TumMakaleler.Values.OrderByDescending(m => m.HIndex).FirstOrDefault();
                if (enYuksekH != null)
                {
                    MessageBox.Show($"İşlem Tamam!\nToplam Makale: {TumMakaleler.Count}\n" +
                                    $"En Yüksek H-Index: {enYuksekH.HIndex} (ID: {IdCikar(enYuksekH.ID)})" +
                                    $"Medyan : {enYuksekH.HMedian}");
                }

            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("ornekdata.json dosyası bulunamadı");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}");
            }
            CiziliMakaleler.Clear();
            yesilcizgi = true;
            analizMi = false;

            int kolon = (int)Math.Ceiling(Math.Sqrt(TumMakaleler.Count));
            int aralikX = 300;
            int aralikY = 250;

            int i = 0;
            foreach (var m in TumMakaleler.Values
                     .OrderBy(x => long.TryParse(
                         new string(IdCikar(x.ID).Where(char.IsDigit).ToArray()),
                         out var v) ? v : long.MaxValue))
            {
                int satir = i / kolon;
                int sutun = i % kolon;

                m.X = sutun * aralikX;
                m.Y = satir * aralikY;
                m.CizimRengi = Color.LightGray;

                CiziliMakaleler.Add(m);
                i++;
            }

            ofsetX = pnlCiz.Width / 2-200;
            ofsetY = pnlCiz.Height / 2-200;
            zoom = 0.6f;
            pnlCiz.Invalidate();

        }

        private void btnCiz_Click(object sender, EventArgs e)
        {
            yesilcizgi= false;
            analizMi = false;
            string girilenID = aranacakID.Text.Trim();
            foreach (var m in TumMakaleler.Values)
            {
                m.GenisletildiMi = false;
                m.CizimRengi = Color.LightGray;
            }
            if (!TumMakaleler.ContainsKey(girilenID))
            {
                MessageBox.Show("Hata : Bu id ile Makale bulunamadı.");
                return;
            }
            Makale merkezMakale = TumMakaleler[girilenID];
            HIndexHesapla(merkezMakale);


            ofsetX = 0;
            ofsetY = 0;

            merkezMakale.X = pnlCiz.Width / 2;
            merkezMakale.Y = pnlCiz.Height / 2;

            CiziliMakaleler.Clear();
            CiziliMakaleler.Add(merkezMakale);

            var hCoreListesi = TumMakaleler.Values.Where(m => merkezMakale.AtıfVerenlerID.Contains(m.ID)).
                OrderByDescending(m => m.AtıfSayısı).Take(merkezMakale.HIndex).ToList();


            if (hCoreListesi.Count > 0)
            {
                int yaricap = 150;
                double aci = 2 * Math.PI / hCoreListesi.Count;

                for (int i = 0; i < hCoreListesi.Count; i++)
                {
                    Makale yeniMakale = hCoreListesi[i];
                    double radyan = i * aci;
                    yeniMakale.X = merkezMakale.X + (float)(yaricap * Math.Cos(radyan));
                    yeniMakale.Y = merkezMakale.Y + (float)(yaricap * Math.Sin(radyan));
                    CiziliMakaleler.Add(yeniMakale);
                }

            }
            pnlCiz.Invalidate();
        }


        private void Analiz_Click(object sender, EventArgs e)
        {
            yesilcizgi= false;
            if (TumMakaleler.Count ==0)
            {
                MessageBox.Show("Önce JSON verisini okuyun !");
                return;
            }
            Cursor.Current = Cursors.WaitCursor;

            try
            {
                BetwennessHesapla();

                var enMerkeziler=TumMakaleler.Values
                    .OrderByDescending(m => m.BetwennesSkor).Take(5).ToList();
                string mesaj = "--Betwennes Analizi Tamamlandı--\n\n En Merkezi 5 Makale\n";
                foreach (var m in enMerkeziler)
                {
                    mesaj += $"ID: {IdCikar(m.ID)} - Betwennes Skor: {m.BetwennesSkor:F2}\n";
                }
                MessageBox.Show(mesaj);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }




        private void pnlCiz_Paint(object sender, PaintEventArgs e)
        {
            SahneyiCiz(e.Graphics);
        }



        private void KCore_Click(object sender, EventArgs e)
        {
            yesilcizgi= false;
            analizMi = false;
            if (TumMakaleler.Count == 0)
            {
                MessageBox.Show("Önce JSON verisini okuyun !");
                return;
            }

            if(int.TryParse(KDegeri.Text,out int k))
            {
                if(k<0)
                {
                    MessageBox.Show("K değeri pozitif bir tam sayı olmalıdır !");
                    return;
                }
                KCoreHesapla(k);
            }
            else
                MessageBox.Show("Geçersiz K değeri !");
        }



        private void Reset_Click(object sender, EventArgs e)
        {
            zoom = 1f;
            ofsetX = ofsetY = 0;
            pnlCiz.Invalidate();
        }

       
        
        // Mouse hareketleri // 



        private void pnlCiz_MouseMove(object sender, MouseEventArgs e)
        {
            if (!pnlCiz.ClientRectangle.Contains(e.Location))
                return;
            if (e.Button == MouseButtons.Right || e.Button == MouseButtons.Left)
            {
                float dunyaX = (e.X - ofsetX) / zoom;
                float dunyaY = (e.Y - ofsetY) / zoom;

                float farkX = dunyaX - baslangicNoktasi.X;
                float farkY = dunyaY - baslangicNoktasi.Y;

                if (!surukleniyor)
                {
                    if (Math.Abs(farkX) + Math.Abs(farkY) > 3)
                    {
                        surukleniyor = true;
                        pnlCiz.Cursor = Cursors.SizeAll;
                    }
                }
                if (surukleniyor)
                {
                    ofsetX += farkX * zoom;
                    ofsetY += farkY * zoom;
                    baslangicNoktasi = new Point((int)dunyaX, (int)dunyaY);
                    pnlCiz.Invalidate();
                    return;
                }
            }
            foreach (Makale makale in CiziliMakaleler.ToList())
            {
                float dunyaX = (e.X - ofsetX) / zoom;
                float dunyaY = (e.Y - ofsetY) / zoom;

                float dx = dunyaX - makale.X;
                float dy = dunyaY - makale.Y;

                if (dx * dx + dy * dy <= 22 * 22)
                {
                    string Baslik = makale.Baslik;
                    string Yazarlar = string.Join(", ", makale.Yazarlar);
                    int bosluk = Baslik.IndexOf(' ',
                                 Baslik.IndexOf(' ',
                                 Baslik.IndexOf(' ',
                                 Baslik.IndexOf(' ') + 1) + 1) + 1);
                    if (bosluk != -1)
                        Baslik = Baslik.Substring(0, bosluk) + "\n  " + Baslik.Substring(bosluk + 1);
                    int bosluk2 = Yazarlar.IndexOf(' ',
                                 Yazarlar.IndexOf(' ',
                                 Yazarlar.IndexOf(' ',
                                 Yazarlar.IndexOf(' ') + 1) + 1) + 1);
                    if (bosluk2 != -1)
                        Yazarlar = Yazarlar.Substring(0, bosluk2) + "\n  " + Yazarlar.Substring(bosluk2 + 1);

                    string bilgi = $"Başlık: {Baslik}\n" +
                                    $"Yazarlar: {Yazarlar}\n" +
                                    $"Yıl: {makale.Yil} \n" +
                                    $"ID: {makale.ID}\n" +
                                    $"Atıf Sayısı:{makale.AtıfSayısı} \n" +
                                    $"H-Index: {makale.HIndex}\n" +
                                    $"Median: {makale.HMedian}";
                    label4.Text = bilgi;
                    return;
                }
            }

            Cursor = Cursors.Default;


        }

        private void pnlCiz_MouseClick(object sender, MouseEventArgs e)
        {
            if (surukleniyor)
            {
                surukleniyor = false;
                return;
            }
            if (e.Button != MouseButtons.Left) return;



            foreach (Makale makale in CiziliMakaleler.ToList())
            {
                float dunyaX = (e.X - ofsetX) / zoom;
                float dunyaY = (e.Y - ofsetY) / zoom;

                float dx = dunyaX - makale.X;
                float dy = dunyaY - makale.Y;

                if (dx * dx + dy * dy <= 22 * 22)
                {

                    if (makale.GenisletildiMi)
                    {
                        return;
                    }
                    makale.GenisletildiMi = true;
                    GrafiGenislet(makale);
                    return;
                }
            }

        }
        private void pnlCiz_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
                zoom *= 1.1f;
            else
                zoom /= 1.1f;

            if (zoom < 0.3f) zoom = 0.3f;
            if (zoom > 3.0f) zoom = 3.0f;

            pnlCiz.Invalidate();
        }
        private void pnlCiz_MouseUp(object sender, MouseEventArgs e)
        {
            surukleniyor = false;
            pnlCiz.Cursor = Cursors.Default;
        }
        private void pnlCiz_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right || e.Button == MouseButtons.Left)
            {
                surukleniyor = false;
                baslangicNoktasi = new Point(
                (int)((e.X - ofsetX) / zoom),
                (int)((e.Y - ofsetY) / zoom));
            }
        }
    }
}
