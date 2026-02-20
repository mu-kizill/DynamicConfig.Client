Dynamic Configuration System

Bu projede klasik yaklaşımlardaki konfigurasyon değişikliği sonucunda her seferine servisi başlatma zorunda kalınan sorun çözülmeye çalışılmıştır. Bu sistemle servisler çalışırken de konfigurasyonlar güncellenebiliyor.

Proje 3 ana parçadan oluşuyor : 

1 - DynamicConfig.Client : Servislerin kullandığı asıl kütüphane,

2 - ConfigAdmin.Api : Konfigürasyonların tutulduğu ve yönetildiği merkezi backend. Veritabanı olarak MSSQL kullanılmıştır. Tüm kayıtların kaynağı burasıdır.

3 - ConfigAdmin.Mvc : Yönetim paneli. Konfigürasyonların listelenip kontrol edildiği basit ama iş gören bir arayüz sunar.



ÇALIŞMA MANTIĞI : 

ConfigurationReader başlatıldığında, ilgili uygulamaya (ApplicationName) ait aktif konfigürasyon kayıtlarını veritabanından okuyarak belleğe alır.

Sonrasında belirlenen periyotlarda veritabanı kontrol edilir. Yalnızca yeni eklenen veya değişen kayıtlar tespit edilerek cache güncellenir.

Bu sayede uygulama yeniden başlatılmadan konfigürasyon değişiklikleri servise yansır.


 var reader = new ConfigurationReader("SERVICE-A", provider, 5000);
 var siteName = reader.GetValue<string>("SiteName");
 
    
    var reader = new ConfigurationReader("SERVICE-A", provider, 5000);
    var siteName = reader.GetValue<string>("SiteName");


TESTLER : 

DynamicConfig.Client için xUnit ile temel unit testler yazılmıştır.

Test edilen senaryolar:

- string ve int değerlerin doğru parse edilmesi

- Olmayan bir key için uygun exception fırlatılması

- Test ortamında gerçek veritabanına ihtiyaç duyulmadan çalışabilmesi

Testlerde gerçek storage yerine fake provider kullanılmıştır.


ÇALIŞTIRMAK İÇİN :

1- MSSQL üzerinde aşağıdaki tabloyu oluştur: 


    CREATE TABLE Configurations(
    Id INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(100),
    Type NVARCHAR(50),
    Value NVARCHAR(500),
    IsActive BIT,
    ApplicationName NVARCHAR(100),
    ModifiedAt DATETIME  );  




2- Örnek veri ekle:

    INSERT INTO Configurations(Name,Type,Value,IsActive,ApplicationName,ModifiedAt)
    VALUES('SiteName','string','soty.io',1,'SERVICE-A',GETDATE())   
   


3- ConfigAdmin.Api ve ServiceA projelerindeki connection string’leri güncelle.

4- ConfigAdmin.Api ve ConfigAdmin.Mvc projelerini birlikte çalıştır.

5- UI üzerinden kayıt ekle/güncelle/sil.

6- ServiceA tarafında değişikliklerin anında yansıdığını gözlemle.
