# Compte rendu TD6
Emma Glesser 21802242

## SOAP Web Services :
Dossier MathsLibrary

## REST Web Services : 

Dossier MathsLibrary2

- URL basiques à appeler pour les 4 opérations :
	- http://localhost:8733/Design_Time_Addresses/MathsLibraryRest/MathsOperations/Add?a=2&b=10
	- http://localhost:8733/Design_Time_Addresses/MathsLibraryRest/MathsOperations/Sub?a=2&b=10
	- http://localhost:8733/Design_Time_Addresses/MathsLibraryRest/MathsOperations/Mul?a=2&b=10
	- http://localhost:8733/Design_Time_Addresses/MathsLibraryRest/MathsOperations/Div?a=2&b=10
	
- URL à appeler avec une réponse au format XML :
	- http://localhost:8733/Design_Time_Addresses/MathsLibraryRest/MathsOperations/AddXML?a=2&b=10
	- http://localhost:8733/Design_Time_Addresses/MathsLibraryRest/MathsOperations/SubXML?a=2&b=10
	- http://localhost:8733/Design_Time_Addresses/MathsLibraryRest/MathsOperations/MulXML?a=2&b=10
	- http://localhost:8733/Design_Time_Addresses/MathsLibraryRest/MathsOperations/DivXML?a=2&b=10

- URL à appeler avec BodyStyle différent (Bare) :
	- http://localhost:8733/Design_Time_Addresses/MathsLibraryRest/MathsOperations/AddBody?a=2&b=10
	- http://localhost:8733/Design_Time_Addresses/MathsLibraryRest/MathsOperations/SubBody?a=2&b=10
	- http://localhost:8733/Design_Time_Addresses/MathsLibraryRest/MathsOperations/MulBody?a=2&b=10
	- http://localhost:8733/Design_Time_Addresses/MathsLibraryRest/MathsOperations/DivBody?a=2&b=10
	
- URL à appeler utilisant POST -> nécessite Postman :
	- http://localhost:8733/Design_Time_Addresses/MathsLibraryRest/MathsOperations/AddPost
	- http://localhost:8733/Design_Time_Addresses/MathsLibraryRest/MathsOperations/SubPost
	- http://localhost:8733/Design_Time_Addresses/MathsLibraryRest/MathsOperations/MulPost
	- http://localhost:8733/Design_Time_Addresses/MathsLibraryRest/MathsOperations/DivPost
	
	Exemple body en Json à remplir pour envoyer la requête :
		```{ 
			"a" : 5,
			"b" : 2
		}```