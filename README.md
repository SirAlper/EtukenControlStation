# 🦅 Savaşan İHA Taktik Yer İstasyonu (GCS) ve Simülatör

Bu proje, TEKNOFEST Savaşan İHA ve benzeri otonom İHA yarışmaları için geliştirilmiş profesyonel bir **Yer İstasyonu Arayüzü (C# WPF)** içerir. 

Proje yekpare (monolitik) bir yapıdan ziyade, modüler mimariler kullanılarak tasarlanmıştır. Bu sayede projedeki harita, video kaydedici, asenkron haberleşme veya simülatör sistemlerini **kendi projenize kolayca kopyalayıp entegre edebilirsiniz.**

---

## ✨ Öne Çıkan Özellikler

- 📡 **Asenkron Telemetri (Fire-and-Forget):** Seri porttan gelen yüksek frekanslı JSON verisini arayüzü kasmadan (UI donması yaşamadan) işler ve aynı anda Hakem Sunucusuna milisaniyelik HTTP POST işlemleriyle fırlatır.
- 🗺️ **Çoklu Hedefli Taktik Harita (Leaflet.js):** WebView2 altyapısıyla çalışan, kendi uçağınızı ve rakip İHA'ları farklı renklerde/ikonlarda gösteren, uçakların arkasında **zaman ayarlı eriyen duman izleri (snail trail)** bırakan siberpunk temalı harita.
- 🎥 **Ultra-Düşük Gecikmeli FPV & DVR:** UDP üzerinden sıfır gecikmeli canlı video yayını alır ve arka planda paket kaybı yaşamadan `.ts` (Transport Stream) formatında kaydeder. Kayıt bittiğinde videoyu otomatik olarak açar.
- ✈️ **Fizik Motorlu SITL Simülatörü:** Sadece sabit veriler değil; kendi rotasında ilerleyen, rüzgar/türbülans etkisiyle titreyen ve **rakip İHA'ları** da simüle eden Python tabanlı test altyapısı.
- 📐 **Canlı HUD:** İHA'nın anlık pitch, roll ve yaw (dikilme, yatış, yönelme) açılarına tepki veren gerçekçi havacılık göstergesi.

---

## 🛠️ Kullanılan Teknolojiler
- **Kullanıcı Arayüzü:** C# / WPF (.NET Core/5/6+)
- **Mimari:** MVVM (CommunityToolkit.Mvvm)
- **Video Çözücü:** LibVLCSharp
- **Harita Motoru:** Microsoft.Web.WebView2 & HTML/JS (Leaflet.js)


---

## 🏗️ Sistem Mimarisi ve Veri Akışı (Geliştiriciler İçin)

Proje, kodun sürdürülebilirliğini, test edilebilirliğini artırmak ve UI donmalarını engellemek için **MVVM (Model-View-ViewModel)** tasarım deseniyle inşa edilmiştir.

### 📂 Temel Dizin Yapısı
- **`/Models`**: Veri yapılarını tutar (Örn: `IhaTelemetri.cs`, `RakipIha.cs`). Sadece özellikleri (property) içerir, mantık barındırmaz. Gelen JSON verileri doğrudan bu sınıflara dönüştürülür.
- **`/ViewModels`**: Uygulamanın beynidir (Örn: `DashboardViewModel.cs`). Arayüzün (View) ihtiyaç duyduğu verileri işler, servislerle haberleşir. XAML (Arayüz) tarafını hiç bilmez, sadece veri sunar.
- **`/Views`**: Kullanıcı arayüzüdür (Örn: `DashboardView.xaml`). Sadece görselliği ve WebView2 (C# -> JS) köprü fonksiyonlarını barındırır.
- **`/Services`**: Dış dünyayla iletişim kuran bağımsız sınıflardır (Örn: `RfService.cs` seri portu okur, `TeknofestApiService.cs` HTTP POST atar).

### 🔄 Veri Akış Döngüsü (Telemetri)
1. **Veri Üretimi:** Donanım (RF Modül) veya Python Simülatörü seri port üzerinden saniyede 2+ kez JSON string yollar.
2. **Dinleme:** `RfService` arka planda (Thread) bu metni okur ve `DataReceived` olayı ile ViewModel'e fırlatır.
3. **Anlamlandırma:** `DashboardViewModel`, gelen metni `IhaTelemetri` nesnesine dönüştürür.
4. **Dağıtım (Eşzamanlı İşlemler):**
   - **XAML (UI):** İrtifa, Hız, Batarya gibi veriler `ObservableProperty` sayesinde XAML'da **otomatik** güncellenir.
   - **Harita:** ViewModel, View içindeki harita köprüsünü `RequestMapUpdate` delegesi ile tetikler. View, bu veriyi `ExecuteScriptAsync` ile `map.html` içindeki JS fonksiyonlarına iletir.
   - **API:** Aynı anda `TeknofestApiService`, arayüzü bekletmeden veriyi Hakem Sunucusuna fırlatır.

---

## 💻 Kurulum ve Çalıştırma

### 1. C# Yer İstasyonu (GCS)
1. Repoyu klonlayın ve `ControlStation.sln` dosyasını Visual Studio ile açın.
2. NuGet paketlerini (LibVLCSharp, WebView2, CommunityToolkit.Mvvm) geri yükleyin (Restore).
3. Derleyip çalıştırın.

---

# Kaynaklar

**WPF öğrenmek ve MVVM oğrenmek için:**

	 1. [Microsoft Learn WPF](https://learn.microsoft.com/tr-tr/dotnet/desktop/wpf/overview/)
	 2. [WPF Tutorial](https://wpf-tutorial.com/)
	 3. [Model-View-ViewModel Toolkit](https://learn.microsoft.com/tr-tr/dotnet/communitytoolkit/mvvm/)
	 4. Model-View-VeiwModel 
