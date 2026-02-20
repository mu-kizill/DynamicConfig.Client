Dynamic Configuration System

Bu projede klasik yaklaşımlardaki konfigurasyon değişikliği sonucunda her seferine servisi başlatma zorunda kalınan sorun çözülmeye çalışılmıştır. Bu sistemle servisler çalışırken de konfigurasyonlar güncellenebiliyor.

Proje 3 ana parçadan oluşuyor : 

1 - DynamicConfig.Client : Servislerin kullandığı asıl kütüphane,

2 - ConfigAdmin.Api : Konfigürasyonların tutulduğu ve yönetildiği merkezi backend. Veritabanı olarak MSSQL kullanılmıştır. Tüm kayıtların kaynağı burasıdır.

3 - ConfigAdmin.Mvc : Yönetim paneli. Konfigürasyonların listelenip kontrol edildiği basit ama iş gören bir arayüz sunar.



ÇALIŞMA MANTIĞI : 

ConfigurationReader başlatıldığında, ilgili uygulamanın (ApplicationName) sadece aktif konfügürasyon kayıtlarını db'den okuyup önbelleğe alır.

Ardından belirlenen periotlarda veritabanı kontrol edilir yalnızca eklenen ya da değişen kayıtları algılayıp önbelleği buna göre günceller.

Bu sayede uygulama yeniden başlatılmadan kofigürasyon değişiklikleri servise anlık olarak yansır ve kod tarafından yeni değerler kullanılmaya başlanır.

var reader = new ConfigurationReader("SERVICE-A", provider, 5000);

var siteName = reader.GetValue<string>("SiteName");


TESTLER : 

DynamicConfig.Client için xUnit ile temel unit testler yazıldı. string ve int değerlerinin doğru parse edilmesi, olmayan bir key için uygun exception fırlatılması, konfigürasyonların test ortamında DB'ye ihtiyaç duymadan çalıştırılbilmesi gibi senaryolar test edildi.
Testlerde gerçek storage yerine fake provider kullanldı.


ÇALIŞTIRMAK İÇİN :
MSSQL üzerinde Configurations tablosu oluşturulur : 


    CREATE TABLE Configurations(
    Id INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(100),
    Type NVARCHAR(50),
    Value NVARCHAR(500),
    IsActive BIT,
    ApplicationName NVARCHAR(100),
    ModifiedAt DATETIME  );  




Örnek bir kayıt eklemek için :

    INSERT INTO Configurations(Name,Type,Value,IsActive,ApplicationName,ModifiedAt)
    VALUES('SiteName','string','soty.io',1,'SERVICE-A',GETDATE())   
   


ConfigAdmin.Api ve ServiceA kısımlarındaki appsettings.json veritabanı bağlantıları uygun şekilde düzeltilir.

ConfigAdmin.Api ve ConfigAdmin.Mvc projeleri birlikte çalıştırılır.

Arayüz üzerinden kayıt eklenir, update veya delete edilir.

ServiceA tarafında değişikliklerin anında yansıdığı gözlemlenebilir.
