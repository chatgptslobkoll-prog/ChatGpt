# AMONIC Airlines (WPF .NET Framework 4.8)

Стартовый каркас приложения под ваше задание (3 сессии) с подключением к MSSQL через ADO.NET.

## Что уже добавлено

- Базовый WPF-проект (`.NET Framework 4.8`) с формой входа.
- Подключение к SQL Server через `System.Data.SqlClient`.
- Сервис авторизации с проверкой `Email + MD5(Password)`.
- Ограничение входа после 3 неудачных попыток с таймером 10 секунд.
- SQL-скрипт для логирования входов/выходов и фиксации «крашей» пользователя.

## Структура

- `src/Amonic.App/` — приложение WPF.
- `sql/Session3_01_UserActivity.sql` — дополнительная таблица логов активности.

## Важно по БД

В задании указано, что **основную структуру менять нельзя**. Поэтому трекинг реализуется добавлением новой таблицы, без изменения уже существующих таблиц.

## Connection string

`src/Amonic.App/App.config`

```xml
<add name="SessionDb"
     connectionString="Server=localhost\SQLEXPRESS;Database=Session3_01;Trusted_Connection=True;TrustServerCertificate=True;"
     providerName="System.Data.SqlClient" />
```

## Что делать дальше (по сессиям)

1. **Session 1**
   - Импорт `UserData.csv` в `Users` (с MD5).
   - Полное логирование авторизации/выхода.
   - Формы: Admin/User main menu, Add User, Change Role.
2. **Session 2**
   - Импорт изменений расписаний из CSV (`ADD/EDIT`).
   - Поиск/фильтрация/сортировка расписаний.
   - Подтверждение/отмена рейса и редактирование.
3. **Session 3**
   - Поиск рейсов (включая маршруты с пересадкой).
   - Бронирование пассажиров.
   - Оплата и выпуск билетов с уникальным `BookingReference` (6 символов).
