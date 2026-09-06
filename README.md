# Project Manager API

REST API для управления проектами и задачами.

## Возможности

- регистрация и авторизация пользователей;
- JWT access и refresh tokens;
- BCrypt-хеширование паролей;
- роли и права доступа;
- CRUD проектов и задач;
- PostgreSQL и Entity Framework Core;
- Swagger-документация.

## Технологии

- C#
- ASP.NET Core
- Entity Framework Core
- PostgreSQL
- JWT
- BCrypt
- Swagger

## Архитектура
```text
Api
Application
Domain
Infrastructure

Api — контроллеры и middleware;

Application — DTO, интерфейсы и сервисы;

Domain — сущности и бизнес-логика;

Infrastructure — EF Core, репозитории и JWT.

