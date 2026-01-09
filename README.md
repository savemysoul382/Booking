# Booking
Simple booking project

##
- ASP.NET Core 9.0
- Entity Framework Core 9.0
- Swagger
- Заполнены тестовые данные комнат, юзеров и бронирований

### Запустить только Postgres в Докере, проект локально
```
docker-compose up -d
```

### Запустить API и Postgres в Докере
```
docker-compose -f docker-compose.full.yml up -d
```

адрес: `http://localhost:8080` `http://localhost:8080/swagger`