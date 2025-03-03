# TsmartTask

Merhaba,

Postman üzerinden api endPointleri test ve kontrol edebilirsiniz. 

Task projesinin swagger olarak canlıda ki versiyonu = https://tsmarttask.bariscakdi.com.tr/swagger/index.html

--Login olup token oluşturmak için; https://tsmarttask.bariscakdi.com.tr/api/Auth/login  // endpointi kullanınız.
{
  "username": "Tsmart",
  "password": "task"
}

-- Bütün ürünleri görmek için kullanılacak endpoint ; https://tsmarttask.bariscakdi.com.tr/api/Product/GetAll // login kısmından aldığınız
	token şifresi ile 'Authorization' kısmına gelip oradan 'Auth Type' bölümünü 'Bearer Token' kısmına getirerek aldığınız token
	ile işlem yapabilirsiniz.


-- Bu kısımda seçilen id üzerinden ürünün bütün içeriğine ulaşabileceğiniz endpoint;  https://tsmarttask.bariscakdi.com.tr/api/Product/GetById/{id} // Token id'niz ile
	'Bearer Token' kısmına kopyalayıp, sonrasında verilen endpoint'in sonuna istediğiniz id numarasını yazıp çalıştırabilirsiniz.


-- Bu kısımda kullanacağınız endpoint ile ürün ekleme işlemi gerçekleştirebilirsiniz ; https://tsmarttask.bariscakdi.com.tr/api/Product/AddProduct // Token id'nizi yazdıktan sonra
	'Body' kısmından 'raw' seçerek 'JSON' formatında verileri doldurup ekleme gerçekleştirilebilir. 
	{
		"name": "Yeni ürün adı",
		"price": 123,
		"stock": 10
	}

-- Bu endpoint ile seçtiğimiz id'deki ürünün güncelleme işlemleri yapılabilir ; https://tsmarttask.bariscakdi.com.tr/api/Product/UpdateProduct/{id} // Token id'sinden sonra 
	'Body' kısmında ki 'raw' seçeneği ile 'JSON' formatında eklemede kullanılan verilerden hepsi yada herhangi birisini yazarak güncelleme yapabilirsiniz.
	{
		"price": 200
	}

-- Bu endpoint silme işlemi için kullanılmaktadır ; https://tsmarttask.bariscakdi.com.tr/api/Product/DeleteProduct/{id} // Token'imizi yazdıktan sonra silmek istediğiniz id'li 
	ürünü endpointin sonuna yazıp gönderdiğimiz zaman 'softdelete' işlemi gerçekleşmektedir. Veri tabanında mevcut olup ürün listelemede gözükmemektedir.


-- Bu endpoint'in işlevi ise silinmiş ürünlerin tamamını göstermek için kullanılmaktadır ; https://tsmarttask.bariscakdi.com.tr/api/Product/Get/deleted 


-- Burada ki endpoint silinmiş olan ürünü tekrar listeye eklenmesini sağlamaktadır ; https://tsmarttask.bariscakdi.com.tr/api/Product/PatchRestore/{id} // Endpoint sonuna aktif hale 
	gelmesini istediğiniz ürünün id'sini yazıp göndermeniz yeterlidir.

