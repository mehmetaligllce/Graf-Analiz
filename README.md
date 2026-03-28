# Akademik Makale Graf Analizi (C#)

Akademik makaleler arasındaki atıf ilişkilerini analiz eden ve graf yapısı üzerinde görselleştiren bir uygulama. Makaleler düğüm, atıflar ise kenar olarak modellenmiştir.

---

## Özellikler

- JSON verisinden makale bilgilerini okuma ve parsing  
- Makaleler arası atıf ilişkilerinden graf oluşturma  
- H-Index ve H-Median hesaplama  
- K-Core analizi  
- Betweenness centrality hesaplama  
- İnteraktif graf görselleştirme (zoom, pan, node seçimi)  
- Makale detaylarını dinamik olarak görüntüleme  
- Graf genişletme (node üzerinden bağlantıları açma)  

---

## Kullanılan Teknolojiler

- C#  
- .NET WinForms  
- GDI+ (grafik çizimi)  
- JSON veri işleme (manuel parsing)  

---

## Mimari

- Veri işleme:
  - JSON → makale nesneleri → graf oluşturma  

- Graf yapısı:
  - Düğümler: Makaleler  
  - Kenarlar: Atıf ilişkileri  

- Katmanlar:
  - Veri parsing  
  - Algoritmalar (H-index, K-core, Betweenness)  
  - Görselleştirme ve kullanıcı etkileşimi  

---

## Öne Çıkan Kısımlar

- JSON verisi manuel olarak parse edilerek nesnelere dönüştürülür  
- Atıf ilişkilerinden çift yönlü graf oluşturulur  
- H-Index ve H-Median hesaplamaları yapılır  
- K-Core algoritması ile graf çekirdeği bulunur  
- Betweenness centrality ile en merkezi düğümler belirlenir  
- Graf üzerinde zoom, pan ve node etkileşimi sağlanır  
- Düğümler ve kenarlar dinamik olarak çizilir  

---

## Kazanımlar

- Graf teorisi ve algoritmaları  
- Merkeziyet ölçümleri (Betweenness, K-Core)  
- Veri parsing ve işleme  
- İnteraktif grafik uygulaması geliştirme  
- Algoritma + görselleştirme entegrasyonu  
