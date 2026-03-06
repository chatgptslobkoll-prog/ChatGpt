# AMONIC Airlines (WPF .NET Framework 4.8)

Проект переведен в формат «почти финального каркаса» по вашему ТЗ: добавлен полный набор окон (14 штук) для всех 3 сессий.

## Реализовано сейчас

- WPF-проект (`.NET Framework 4.8`) + решение Visual Studio.
- Подключение к MS SQL через ADO.NET (`System.Data.SqlClient`).
- Авторизация (`Email + MD5(password)`), проверка `Active`, блокировка на 10 секунд после 3 ошибок.
- Дополнительная таблица `UserActivityLogs` (в отдельном SQL-скрипте) для фиксации входов/выходов/сбоев.
- Созданы окна по этапам задания:
  1. `LoginWindow`
  2. `AdminMainWindow`
  3. `AddUserWindow`
  4. `ChangeRoleWindow`
  5. `UserMainWindow`
  6. `ManageSchedulesWindow`
  7. `EditScheduleWindow`
  8. `ImportSchedulesWindow`
  9. `ImportResultWindow`
  10. `FlightSearchWindow`
  11. `BookingConfirmationWindow`
  12. `PaymentWindow`
  13. `TicketSummaryWindow`
  14. `TestingChecklistWindow`

## Структура

- `src/Amonic.App/` — WPF приложение.
- `sql/Session3_01_UserActivity.sql` — скрипт дополнительной таблицы активности.

## Connection string

`src/Amonic.App/App.config`

```xml
<add name="SessionDb"
     connectionString="Server=KAB17-11\SQLEXPRESS;Database=Session3_01;Trusted_Connection=True;TrustServerCertificate=True;Connect Timeout=5;"
     providerName="System.Data.SqlClient" />
```

## Следующий шаг

Дальше можно последовательно подключить бизнес-логику/SQL к каждому из окон (CRUD пользователей, импорт расписаний, поиск с пересадками, выпуск билетов и уникальный Booking Reference).

## Модели БД (без ADO.NET)

Добавлены POCO-модели таблиц `Session3_01` в `src/Amonic.App/Models/`:
`Aircraft`, `Airport`, `CabinType`, `Country`, `Office`, `Role`, `Route`, `Schedule`, `Ticket`, `User`, `UserActivityLog`.



Примечание: ошибки подключения к SQL Server в авторизации обрабатываются отдельно и не учитываются как неверные попытки ввода пароля.
