# Booking
Simple booking project

##
- ASP.NET Core 9.0
- Entity Framework Core 9.0
- Swagger
- Заполнены тестовые данные комнат, юзеров и бронирований

### Запустить только Postgres в Докере, проект локально.
```
docker-compose up -d
```

### Запустить API и Postgres в Докере.
```
docker-compose -f docker-compose.full.yml up -d
```

адрес: `http://localhost:8080` `http://localhost:8080/swagger`

## Современная архитектура и подходы к разработке рассмотрены в соседнем проекте.
https://github.com/savemysoul382/DevQuestions

### Tech
- Scrutor
- FluentValidation

### Architecture
- Monolith
- Modular monolith
- Clean Architecture
- Vertical Slice Architecture

### Patterns and Methodologies
- Result pattern
- Unit of work
- Repository
- Handler и CommandHandler
- IQuery и IQueryHandler
- CQS
