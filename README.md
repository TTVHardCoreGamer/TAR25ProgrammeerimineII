1. Milline on DbContext-i ülesanne?
DbContext võimaldab rakendusel andmebaasiga suhelda, andmeid lugeda, lisada, muuta ja kustutada.

2. Miks ei ole hea paigutada kõiki andmebaasipäringuid otse kasutajaliidese koodi?
See muudab koodi keerulisemaks, raskemini hooldatavaks ja raskemini testitavaks.

3. Millisesse kihti paigutaksid andmebaasiga suhtlemise MVC- või VIPER-arhitektuuris? Põhjenda oma vastust.
MVC-arhitektuuris paigutaksin selle Model-kihti ja VIPER-arhitektuuris Interactor-kihti. Nii on andmebaasiga suhtlemine kasutajaliidesest eraldatud ning kood on paremini organiseeritud.