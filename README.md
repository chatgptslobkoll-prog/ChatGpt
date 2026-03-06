# AMONIC Airlines (WPF .NET Framework 4.8)

Проект реализует рабочий каркас по 3 сессиям AMONIC Airlines на WPF (.NET Framework 4.8) с SQL Server.

## Что уже работает

- Авторизация через таблицу `Users` (plain text пароль, как в текущей БД).
- Разделение по ролям после входа (Admin/User).
- Логирование входа/выхода в `UserActivityLogs`.
- Админ-экран:
  - загрузка списка пользователей из БД,
  - фильтр по офисам,
  - блокировка/разблокировка,
  - смена роли,
  - добавление пользователя.
- Пользовательский экран:
  - приветствие,
  - суммарное время за 30 дней,
  - количество сбоев,
  - журнал активностей.
- Управление расписаниями:
  - фильтры/сортировка,
  - изменение цены/даты/времени,
  - подтверждение/отмена,
  - импорт CSV (`ADD`/`EDIT`) с подсчётом результатов.
- Поиск рейсов и бронирование:
  - поиск one-way/round-trip,
  - выбор класса обслуживания,
  - проверка доступных мест,
  - ввод пассажиров,
  - выпуск билетов и генерация уникального `BookingReference`.

## Структура

- `src/Amonic.App/Views/` — окна WPF (14 окон).
- `src/Amonic.App/Services/AppRepository.cs` — SQL-операции для всех экранов.
- `src/Amonic.App/Models/` — модели таблиц БД.
- `src/Amonic.App/ViewModels/AppViewModels.cs` — DTO для UI.
- `sql/Session3_01_UserActivity.sql` — скрипт таблицы активности.

## Connection string

`src/Amonic.App/App.config`

```xml
<add name="SessionDb"
     connectionString="Server=KAB17-11\SQLEXPRESS;Database=Session3_01;Trusted_Connection=True;TrustServerCertificate=True;Connect Timeout=5;"
     providerName="System.Data.SqlClient" />
```
