<!-- Testing Guide for New Features -->

# 🧪 Test Rehberi - Yeni Özellikler

## Sitenizin Sunucusu Çalışıyor Mu?

✅ `dotnet watch` komutu çalıştırıldı mı?
✅ http://localhost:5000 açılabilir mi?

---

## 🧪 Test 1: Kayıt Ol ve Gizlilik Kabul

### Adım-adım:
1. http://localhost:5000 anasayfaya git
2. Üst sağda "Kayıt Ol" butonuna tıkla
3. **Görmen gereken:**
   - Kayıt formu açılmalı
   - Form alanları: Ad-Soyad, Kullanıcı Adı, E-posta, Şifre, Şifre Onayı
   - **ÖNEMLİ:** "Hüküm ve Koşulları ile Gizlilik Politikasını Kabul Ediyorum" kontrol kutusu

4. **Kontrol kutusu linklerini test et:**
   - "Hüküm ve Koşulları" linkine tıkla → Yeni sekmede açılmalı
   - "Gizlilik Politikasını" linkine tıkla → Yeni sekmede açılmalı

5. **Kontrol kutusuz kayıt testa:** 
   - Formu doldur ama kontrol kutusunu işaretleme
   - "Kayıt Ol" butonuna tıkla
   - **Sonuç:** Form gönderilmemeli, hata mesajı

6. **Kontrol kutusuyla kayıt:**
   - Kontrol kutusunu işaretle ✓
   - "Kayıt Ol" butonuna tıkla
   - **Sonuç:** Başarılı → Giriş sayfasına yönlendir

---

## 🧪 Test 2: Profil Görüntüleme

### Adım-adım:
1. Kayıt olduktan sonra giriş yap
   - Kullanıcı adı ve şifreyi gir
   - Giriş butonuna tıkla

2. **Navbar'ı kontrol et:**
   - Üst sağda "Hoşgeldin, [KullanıcıAdı]!" yazısı görünmeli
   - "Profilim" linki görünmeli
   - "Çıkış" butonu görünmeli

3. "Profilim" linkine tıkla
   - **Görmen gereken:**
     - Profil başlığı: "👤 Profilim"
     - Kullanıcı Adı (👨‍💼)
     - E-posta (✉️)
     - Ad-Soyad (👤)
     - Üye Olduğu Tarih (📅)
     - "✏️ Profili Düzenle" butonu
     - "🚪 Çıkış Yap" butonu

---

## 🧪 Test 3: Profil Düzenleme

### Adım-adım:
1. Profil sayfasında "✏️ Profili Düzenle" butonuna tıkla
   - **Görmen gereken:**
     - Başlık: "✏️ Profili Düzenle"
     - Form alanları: Ad-Soyad, E-posta (devre dışı), Biyografi
     - "💾 Kaydet" butonu
     - "← Geri Dön" butonu

2. **Form doldur:**
   - Ad-Soyad: "Ahmet Yıldız"
   - Biyografi: "Merhaba, ben Ahmet. Yazılım geliştirme üzerine çalışıyorum."

3. "💾 Kaydet" butonuna tıkla
   - **Sonuç:** Profil sayfasına yönlendir
   - Güncellenmiş bilgileri görmeliysin

4. **Değişiklikleri doğrula:**
   - Yeniden "✏️ Profili Düzenle" tıkla
   - Önceki girişler hala orada mı?
   - **Beklenen:** Evet, veriler kaydedilmiş

---

## 🧪 Test 4: Gizlilik Politikası Sayfası

### Adım-adım:
1. **Metod 1: Footer'dan (herhangi bir sayfada)**
   - Sayfanın altına kaydır
   - "Hukuk" bölümünde "Gizlilik Politikası" linkine tıkla

2. **Metod 2: Kayıt sayfasından**
   - Kayıt sayfasındaki kontrol kutusunda linke tıkla

3. **Sayfayı kontrol et:**
   - Başlık: "🔒 Gizlilik Politikası"
   - Bölümler görünmeli:
     - 📋 Giriş
     - 📊 Veri Toplama
     - 🎯 Veri Kullanımı
     - 🍪 Çerezler
     - 🛡️ Veri Güvenliği
     - 👥 Üçüncü Taraf Paylaşımı
     - ✅ Kullanıcı Hakları
     - 📧 İletişim

4. **Butonları test et:**
   - "← Kayıt Ol" butonu → Kayıt sayfasına git
   - "Anasayfaya Dön" butonu → Anasayfaya git

---

## 🧪 Test 5: Hüküm ve Koşullar Sayfası

### Adım-adım:
1. **Metod 1: Footer'dan**
   - Sayfanın altına kaydır
   - "Hukuk" bölümünde "Hüküm ve Koşullar" linkine tıkla

2. **Metod 2: Kayıt sayfasından**
   - Kayıt sayfasındaki kontrol kutusunda linke tıkla

3. **Sayfayı kontrol et:**
   - Başlık: "⚖️ Hüküm ve Koşullar"
   - Bölümler görünmeli:
     - 📋 Anlaşma
     - 🔐 Kullanıcı Hesabı
     - ⛔ Yasak Etkinlikler
     - 📄 İçerik Sahipliği
     - 👤 Kullanıcı İçeriği
     - 🔗 Dış Bağlantılar
     - ⚠️ Sorumluluk Reddi
     - 🛡️ Sorumluluk Sınırlaması

4. **Butonları test et:**
   - "← Kayıt Ol" butonu → Kayıt sayfasına git
   - "Anasayfaya Dön" butonu → Anasayfaya git

---

## 🧪 Test 6: Oturum Kapatma

### Adım-adım:
1. Profil sayfasında "🚪 Çıkış Yap" butonuna tıkla
   - **Sonuç:** Anasayfaya yönlendir
   - "Profilim" linki kaybolmalı
   - "Giriş" ve "Kayıt Ol" butonları görülmeli

---

## 🧪 Test 7: Responsive Tasarım

### Her sayfada test et:
1. **Bilgisayar ekranında:**
   - Tüm içerik iyi görünmeli
   - Linkler tıklanabilir

2. **Mobil simülasyonda (F12 açıp cihaz simüle et):**
   - Contentler yukarıdan aşağıya düzgün akmalı
   - Butonlar ve linkler tıklanabilir boyutlarda
   - Kayıt formu açılabilir

---

## 🧪 Test 8: Animasyonlar

### Her yeni sayfada kontrol et:
- [ ] Sayfalar fadeInUp animasyonu ile yüklenmeli
- [ ] Kartlar yumuşak geçiş ile görünmeli
- [ ] Linkler hover edilince rengi değişmeli
- [ ] Butonlar hover edilince efekt yapmalı

---

## ✅ Son Kontrol Listesi

- [ ] Kayıt formu gizlilik kontrol kutusunu gerektiriyor
- [ ] Legal linkler çalışıyor (gizlilik + hüküm)
- [ ] Profil sayfası giriş yapmış kullanıcılara açık
- [ ] Profil düzenleme çalışıyor ve veriler kaydediliyor
- [ ] Footer'da hukuk linkileri var
- [ ] Navbar'da "Profilim" linki görülüyor (giriş yapanlar)
- [ ] Çıkış yap butonları çalışıyor
- [ ] Hata yok (Console'da kırmızı hatalar yok)
- [ ] Responsive tasarım çalışıyor
- [ ] Tüm animasyonlar çalışıyor

---

## 🐛 Sorun Çıkarsa

1. **"Profilim linki görülmüyor"**
   - ✓ Giriş yap mı? (giriş yapanlar için gösterilir)
   - ✓ dotnet watch yeniden başlatıldı mı?

2. **"Profil sayfası açılmıyor"**
   - ✓ Giriş yap mı? (Authorize gerekli)
   - ✓ URL doğru mu? `/Profile/Index`

3. **"Kayıt kontrol kutusu çalışmıyor"**
   - ✓ Register.cshtml güncellenmiş mi?
   - ✓ dotnet watch çalışıyor mu?

4. **"Hukuk sayfaları açılmıyor"**
   - ✓ LegalController.cs var mı?
   - ✓ Views/Legal/ klasörü var mı?
   - ✓ URL doğru mu? `/Legal/PrivacyPolicy`, `/Legal/TermsOfService`

---

**Bütün testler başarılı olduktan sonra: Tebrikler! 🎉**
