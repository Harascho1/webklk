### appsetting.json i appsetings.Development.json
``` json
"ConnectionStrings": {
    "IspitCS": "Server=(localdb)\\IndexNPR;Database=IndexNPR"
}
```

### Napravi i startuj bazu
``` bash
sqllocaldb create IndexNPR -s
```
Nazovi bazu isto kao i sto si stavio u conn stringu

### Migracije
``` bash
dotnet ef migrations add v1
dotnet ef database update
```
Ovo radi svaki put kad promenis nesto u folderu Models!

### Brisanje Migracija
``` bash
dotnet ef database drop
dotnet ef migrations remove
```
Nekad nece da ti da da obrises migracije ako su vec stavljene u bazu, tako da ces morati da obrises tu bazu

### Run
``` bash
dotnet watch run
```
